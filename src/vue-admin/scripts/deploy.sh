#!/bin/bash

# 通用部署脚本
# Usage: ./deploy.sh [dev|staging|prod]

set -euo pipefail

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 日志函数
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# 检查必要的工具
check_prerequisites() {
    local tools=("docker" "docker-compose" "git" "curl")
    
    for tool in "${tools[@]}"; do
        if ! command -v "$tool" &> /dev/null; then
            log_error "$tool 未安装，请先安装"
            exit 1
        fi
    done
    
    log_success "所有必要工具已安装"
}

# 获取环境参数
get_environment() {
    local env=${1:-}
    
    case $env in
        dev|development)
            echo "development"
            ;;
        staging|stage)
            echo "staging"
            ;;
        prod|production)
            echo "production"
            ;;
        *)
            log_error "无效的环境参数: $env"
            log_info "支持的环境: dev, staging, prod"
            exit 1
            ;;
    esac
}

# 加载环境变量
load_env_vars() {
    local environment=$1
    local env_file=".env.${environment}"
    
    if [[ -f $env_file ]]; then
        log_info "加载环境变量: $env_file"
        export $(grep -v '^#' "$env_file" | xargs)
    else
        log_warning "环境变量文件不存在: $env_file"
    fi
    
    # 设置构建变量
    export BUILD_VERSION=$(git rev-parse --short HEAD)
    export BUILD_TIME=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
    export ENVIRONMENT=$environment
    
    log_info "构建版本: $BUILD_VERSION"
    log_info "构建时间: $BUILD_TIME"
}

# 构建应用
build_app() {
    log_info "构建前端应用..."
    
    # 安装依赖
    if [[ -f "pnpm-lock.yaml" ]]; then
        pnpm install --frozen-lockfile
    else
        npm ci
    fi
    
    # 生成API客户端
    if [[ -f "nswag.json" ]]; then
        log_info "生成API客户端..."
        npm run generate-api || log_warning "API客户端生成失败，继续构建"
    fi
    
    # 构建应用
    npm run build
    
    log_success "应用构建完成"
}

# Docker 构建
build_docker() {
    local environment=$1
    local image_name="vue-admin:${environment}-${BUILD_VERSION}"
    
    log_info "构建Docker镜像: $image_name"
    
    docker build \
        --build-arg BUILD_VERSION="$BUILD_VERSION" \
        --build-arg BUILD_TIME="$BUILD_TIME" \
        --tag "$image_name" \
        --tag "vue-admin:${environment}-latest" \
        .
    
    log_success "Docker镜像构建完成: $image_name"
}

# 部署服务
deploy_services() {
    local environment=$1
    local compose_file="docker-compose.${environment}.yml"
    
    # 使用默认配置文件如果环境特定文件不存在
    if [[ ! -f $compose_file ]]; then
        compose_file="docker-compose.yml"
        log_warning "使用默认docker-compose.yml文件"
    fi
    
    log_info "部署服务: $compose_file"
    
    # 停止现有服务
    docker-compose -f "$compose_file" down --remove-orphans
    
    # 拉取最新镜像
    docker-compose -f "$compose_file" pull
    
    # 启动服务
    docker-compose -f "$compose_file" up -d
    
    log_success "服务部署完成"
}

# 健康检查
health_check() {
    local environment=$1
    local health_url="${HEALTH_CHECK_URL:-http://localhost:3000/health}"
    local max_attempts=30
    local attempt=1
    
    log_info "执行健康检查: $health_url"
    
    while [[ $attempt -le $max_attempts ]]; do
        if curl -f -s "$health_url" > /dev/null; then
            log_success "健康检查通过 (尝试 $attempt/$max_attempts)"
            return 0
        fi
        
        log_warning "健康检查失败，等待重试 ($attempt/$max_attempts)"
        sleep 10
        ((attempt++))
    done
    
    log_error "健康检查失败，服务可能未正常启动"
    return 1
}

# 部署后处理
post_deploy() {
    local environment=$1
    
    log_info "执行部署后处理..."
    
    # 清理旧的Docker镜像
    docker image prune -f --filter "label=stage=builder"
    
    # 发送部署通知
    if [[ -n "${WEBHOOK_URL:-}" ]]; then
        curl -X POST "$WEBHOOK_URL" \
            -H "Content-Type: application/json" \
            -d "{
                \"environment\": \"$environment\",
                \"version\": \"$BUILD_VERSION\",
                \"status\": \"success\",
                \"timestamp\": \"$BUILD_TIME\"
            }" || log_warning "发送部署通知失败"
    fi
    
    log_success "部署完成!"
}

# 主函数
main() {
    local environment
    
    # 检查参数
    if [[ $# -eq 0 ]]; then
        log_error "请指定部署环境"
        log_info "用法: $0 [dev|staging|prod]"
        exit 1
    fi
    
    environment=$(get_environment "$1")
    
    log_info "开始部署到 $environment 环境"
    
    # 执行部署步骤
    check_prerequisites
    load_env_vars "$environment"
    build_app
    build_docker "$environment"
    deploy_services "$environment"
    
    # 健康检查
    if health_check "$environment"; then
        post_deploy "$environment"
    else
        log_error "部署失败，请检查服务状态"
        exit 1
    fi
}

# 信号处理
trap 'log_error "部署被中断"; exit 1' INT TERM

# 执行主函数
main "$@"
# 部署指南

本文档详细介绍了Vue Admin项目的部署流程和配置。

## 概述

项目支持多种部署方式：
- 🐳 Docker容器化部署
- ☸️ Kubernetes集群部署  
- 🔄 CI/CD自动化部署
- 📊 完整的监控和日志收集

## 快速开始

### 本地开发部署

```bash
# 1. 克隆项目
git clone https://github.com/your-repo/vue-admin.git
cd vue-admin

# 2. 安装依赖
npm install

# 3. 启动开发服务器
npm run dev

# 4. 使用Docker Compose
npm run docker:dev
```

### 生产环境部署

```bash
# 1. 构建生产版本
npm run build:prod

# 2. 使用部署脚本
npm run deploy:prod

# 或者直接使用Docker Compose
npm run docker:prod
```

## 环境配置

### 环境变量文件

项目支持多环境配置：

- `.env.development` - 开发环境
- `.env.staging` - 预发布环境
- `.env.production` - 生产环境

### 关键环境变量

```bash
# 应用配置
VITE_APP_TITLE=Vue Admin
VITE_API_BASE_URL=https://api.example.com
VITE_SENTRY_DSN=https://your-sentry-dsn

# 数据库配置
POSTGRES_DB=vue_admin
POSTGRES_USER=admin
POSTGRES_PASSWORD=secure_password

# 监控配置
GRAFANA_PASSWORD=admin_password
HEALTH_CHECK_URL=https://your-domain.com/health
```

## Docker 部署

### 单容器部署

```bash
# 构建镜像
docker build -t vue-admin .

# 运行容器
docker run -p 3000:80 vue-admin
```

### Docker Compose 部署

```bash
# 开发环境
docker-compose up -d

# 生产环境  
docker-compose -f docker-compose.prod.yml up -d
```

### 服务包含

- **vue-admin**: 前端应用
- **postgres**: PostgreSQL数据库
- **redis**: Redis缓存
- **traefik**: 反向代理和SSL
- **prometheus**: 指标收集
- **grafana**: 监控面板
- **loki**: 日志收集

## CI/CD 流水线

### GitHub Actions

项目包含完整的CI/CD流水线：

```yaml
# .github/workflows/ci-cd.yml
名称: CI/CD Pipeline

触发条件:
- 推送到 main/dev 分支
- Pull Request 到 main 分支

流水线阶段:
1. 代码质量检查 (ESLint, Prettier, Tests)
2. 安全扫描 (Snyk)
3. 构建应用 (多环境)
4. Docker 镜像构建和推送
5. 自动部署
6. 健康检查
7. 通知
```

### 流水线配置

需要配置的Secrets：

```bash
# Docker Hub
DOCKER_USERNAME
DOCKER_PASSWORD

# 部署配置
DEPLOY_WEBHOOK_DEV
DEPLOY_WEBHOOK_PROD
DEPLOY_TOKEN

# 监控配置
SENTRY_DSN
CODECOV_TOKEN
SNYK_TOKEN

# 通知配置
SLACK_WEBHOOK_URL
```

## Kubernetes 部署

### 准备工作

```bash
# 1. 创建命名空间
kubectl create namespace vue-admin

# 2. 创建配置映射
kubectl create configmap vue-admin-config \
  --from-env-file=.env.production \
  -n vue-admin

# 3. 创建密钥
kubectl create secret generic vue-admin-secrets \
  --from-literal=database-password=your-password \
  -n vue-admin
```

### 部署清单

```yaml
# deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: vue-admin
  namespace: vue-admin
spec:
  replicas: 3
  selector:
    matchLabels:
      app: vue-admin
  template:
    metadata:
      labels:
        app: vue-admin
    spec:
      containers:
      - name: vue-admin
        image: vue-admin:latest
        ports:
        - containerPort: 80
        envFrom:
        - configMapRef:
            name: vue-admin-config
        - secretRef:
            name: vue-admin-secrets
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
```

## 监控和日志

### Prometheus 监控

监控指标包括：
- 应用性能指标
- HTTP请求指标
- 错误率统计
- 资源使用情况

### Grafana 面板

预配置的监控面板：
- 应用概览面板
- 性能监控面板
- 错误监控面板
- 基础设施面板

### 日志收集

使用Loki + Promtail收集日志：
- 应用日志
- Nginx访问日志
- 系统日志
- 错误日志

## 安全配置

### SSL/TLS

```bash
# 使用Let's Encrypt自动获取SSL证书
# 在docker-compose.yml中配置Traefik

labels:
  - "traefik.http.routers.vue-admin.tls=true"
  - "traefik.http.routers.vue-admin.tls.certresolver=letsencrypt"
```

### 安全头

Nginx配置了完整的安全头：
- X-Frame-Options
- X-Content-Type-Options
- X-XSS-Protection
- Content-Security-Policy
- Referrer-Policy

### 访问控制

- 基于角色的访问控制(RBAC)
- JWT Token认证
- API限流
- 请求日志记录

## 性能优化

### 前端优化

- 代码分割和懒加载
- 资源压缩和缓存
- CDN加速
- 图片优化

### 服务器优化

```nginx
# Nginx配置优化
gzip on;
gzip_types text/plain text/css application/javascript;
expires 1y; # 静态资源缓存

# HTTP/2支持
listen 443 ssl http2;
```

### 数据库优化

- 连接池配置
- 索引优化
- 查询优化
- 定期备份

## 故障排查

### 常见问题

1. **容器启动失败**
   ```bash
   # 查看日志
   docker logs vue-admin
   
   # 检查配置
   docker inspect vue-admin
   ```

2. **API连接失败**
   ```bash
   # 测试API连接
   curl -f $VITE_API_BASE_URL/health
   
   # 检查网络连接
   docker network ls
   ```

3. **健康检查失败**
   ```bash
   # 手动健康检查
   curl -f http://localhost:3000/health
   
   # 查看容器状态
   docker ps -a
   ```

### 日志查看

```bash
# 应用日志
docker logs vue-admin

# Nginx日志
docker exec vue-admin tail -f /var/log/nginx/access.log

# 系统监控
docker stats vue-admin
```

## 备份和恢复

### 数据库备份

```bash
# 自动备份脚本
#!/bin/bash
docker exec postgres pg_dump -U $POSTGRES_USER $POSTGRES_DB > backup_$(date +%Y%m%d_%H%M%S).sql
```

### 配置备份

```bash
# 备份配置文件
tar -czf config_backup_$(date +%Y%m%d).tar.gz .env.* docker-compose*.yml
```

### 恢复流程

```bash
# 1. 停止服务
docker-compose down

# 2. 恢复数据库
docker exec postgres psql -U $POSTGRES_USER $POSTGRES_DB < backup.sql

# 3. 恢复配置
tar -xzf config_backup.tar.gz

# 4. 重启服务
docker-compose up -d
```

## 扩展和维护

### 水平扩展

```bash
# 增加副本数量
docker-compose up -d --scale vue-admin=3

# Kubernetes水平扩展
kubectl scale deployment vue-admin --replicas=5 -n vue-admin
```

### 滚动更新

```bash
# Docker Compose滚动更新
docker-compose up -d --force-recreate --no-deps vue-admin

# Kubernetes滚动更新
kubectl rollout restart deployment/vue-admin -n vue-admin
```

### 维护窗口

建议的维护任务：
- 每周检查日志和监控
- 每月更新依赖和镜像
- 每季度进行安全扫描
- 每年进行灾难恢复测试

## 联系支持

如需技术支持，请联系：
- 📧 Email: support@example.com
- 💬 Slack: #vue-admin-support
- 🐛 GitHub Issues: [项目Issues页面]
# 多阶段构建 Dockerfile
FROM node:18-alpine as builder

# 设置工作目录
WORKDIR /app

# 复制 package.json 和 pnpm-lock.yaml
COPY package*.json pnpm-lock.yaml ./

# 安装 pnpm
RUN npm install -g pnpm

# 安装依赖
RUN pnpm install --frozen-lockfile

# 复制源代码
COPY . .

# 构建参数
ARG BUILD_VERSION=unknown
ARG BUILD_TIME=unknown

# 设置环境变量
ENV VITE_BUILD_VERSION=${BUILD_VERSION}
ENV VITE_BUILD_TIME=${BUILD_TIME}

# 构建应用
RUN pnpm run build

# 生产镜像
FROM nginx:alpine

# 安装必要的工具
RUN apk add --no-cache \
    curl \
    jq \
    && rm -rf /var/cache/apk/*

# 创建非 root 用户
RUN addgroup -g 1001 -S nodejs \
    && adduser -S -u 1001 -G nodejs nodejs

# 复制构建产物
COPY --from=builder /app/dist /usr/share/nginx/html

# 复制 nginx 配置
COPY docker/nginx.conf /etc/nginx/nginx.conf
COPY docker/default.conf /etc/nginx/conf.d/default.conf

# 创建日志目录
RUN mkdir -p /var/log/nginx \
    && chown -R nodejs:nodejs /var/log/nginx \
    && chown -R nodejs:nodejs /usr/share/nginx/html \
    && chown -R nodejs:nodejs /etc/nginx/conf.d

# 健康检查
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# 标签
LABEL maintainer="FlyFramework Team" \
      version=${BUILD_VERSION} \
      build-time=${BUILD_TIME} \
      description="Vue Admin Dashboard"

# 切换到非 root 用户
USER nodejs

# 暴露端口
EXPOSE 80

# 启动命令
CMD ["nginx", "-g", "daemon off;"]
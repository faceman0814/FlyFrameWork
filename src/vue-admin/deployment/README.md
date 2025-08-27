# 部署脚本集合

## Kubernetes 部署
kubectl/
├── namespace.yaml
├── configmap.yaml  
├── secret.yaml
├── deployment.yaml
├── service.yaml
├── ingress.yaml
└── hpa.yaml

## Helm Chart
helm/
├── Chart.yaml
├── values.yaml
├── values-dev.yaml
├── values-staging.yaml
├── values-prod.yaml
└── templates/
    ├── deployment.yaml
    ├── service.yaml
    ├── ingress.yaml
    ├── configmap.yaml
    └── secret.yaml

## Docker Compose 变体
docker-compose.dev.yml      # 开发环境
docker-compose.staging.yml  # 预发布环境
docker-compose.prod.yml     # 生产环境

## 部署脚本
deploy.sh                   # 通用部署脚本
deploy-dev.sh              # 开发环境部署
deploy-staging.sh          # 预发布环境部署
deploy-prod.sh             # 生产环境部署
rollback.sh                # 回滚脚本
health-check.sh            # 健康检查脚本

## 环境配置
.env.development           # 开发环境变量
.env.staging              # 预发布环境变量
.env.production           # 生产环境变量

## 监控配置
monitoring/
├── prometheus/
│   └── prometheus.yml
├── grafana/
│   ├── dashboards/
│   └── datasources/
├── loki/
│   └── loki.yml
└── promtail/
    └── promtail.yml
# Vue3 Admin 管理系统

基于 Vue3 + TypeScript + Element Plus + Vite 构建的现代化后台管理系统。

## 🚀 功能特性

- ⚡ Vue3 + Vite4 + TypeScript
- 🎨 Element Plus 组件库
- 🌍 国际化支持（中英文）
- 🔐 完整的权限系统
- 📱 响应式布局
- 🎯 动态路由
- 🏷️ 标签页导航
- 📊 数据可视化
- 🛠️ 代码规范检查

## 📦 技术栈

- **框架**: Vue 3.3+
- **构建工具**: Vite 4+
- **语言**: TypeScript
- **UI 组件库**: Element Plus
- **状态管理**: Pinia
- **路由**: Vue Router 4
- **国际化**: Vue I18n
- **HTTP 客户端**: Axios
- **CSS 预处理**: Sass
- **代码规范**: ESLint + Prettier

## 🛠️ 开发

### 环境要求

- Node.js >= 16
- pnpm >= 7

### 安装依赖

```bash
pnpm install
```

### 启动开发服务器

```bash
pnpm dev
```

### 构建生产版本

```bash
pnpm build
```

### 代码检查

```bash
pnpm lint
```

### 代码格式化

```bash
pnpm format
```

## 📁 项目结构

```
src/
├── api/                 # API 接口
├── assets/             # 静态资源
├── components/         # 全局组件
├── hooks/              # 组合式函数
├── layout/             # 布局组件
├── locales/            # 国际化文件
├── router/             # 路由配置
├── stores/             # 状态管理
├── styles/             # 全局样式
├── utils/              # 工具函数
├── views/              # 页面组件
├── App.vue            # 根组件
└── main.ts            # 入口文件
```

## 🎨 页面预览

### 登录页面

- 响应式设计
- 表单验证
- 记住密码功能

### 仪表盘

- 数据统计卡片
- 快捷操作面板
- 实时时间显示

### 用户管理

- 用户列表展示
- 添加/编辑/删除用户
- 状态管理
- 分页功能

### 角色管理

- 角色列表管理
- 权限分配
- 角色权限树形选择

## 🔧 配置说明

### 环境变量

```bash
# 开发环境
VITE_API_BASE_URL=http://localhost:3000

# 生产环境
VITE_API_BASE_URL=https://flyframework.faceman.cn
```

### 代理配置

在 `vite.config.ts` 中配置开发环境代理：

```typescript
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:3000',
      changeOrigin: true,
        // 不重写路径，因为我们需要保持 /api 前缀
        // rewrite: (path: string) => path.replace(/^\/api/, '')
    }
  }
}
```

## 🌐 国际化

支持中英文切换，语言文件位于 `src/locales/` 目录：

- `zh-cn.json` - 中文
- `en.json` - 英文

## 🔐 权限系统

- 基于角色的权限控制 (RBAC)
- 动态路由生成
- 按钮级权限控制
- 路由守卫

## 📱 响应式支持

- 桌面端 (≥1200px)
- 平板端 (768px-1199px)
- 移动端 (<768px)

## 🤝 贡献指南

1. Fork 项目
2. 创建特性分支: `git checkout -b feature/amazing-feature`
3. 提交更改: `git commit -m 'Add some amazing feature'`
4. 推送分支: `git push origin feature/amazing-feature`
5. 提交 Pull Request

## 📄 许可证

[MIT License](LICENSE)

## 🙏 致谢

- [Vue.js](https://vuejs.org/)
- [Element Plus](https://element-plus.org/)
- [Vite](https://vitejs.dev/)
- [TypeScript](https://www.typescriptlang.org/)

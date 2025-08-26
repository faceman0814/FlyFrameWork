# NSwag 联动 .NET 后端配合前端使用 - 完整教程

## 🎯 项目概述

本教程展示了如何使用 NSwag 实现 .NET 后端 API 与 Vue 3 前端的完美配合，提供类型安全的 API 调用体验。

## 📁 项目结构

```
vue-admin/
├── src/
│   ├── api/
│   │   ├── base.ts          # API 基础类（已配置）
│   │   ├── service.ts       # API 服务封装
│   │   ├── generated.ts     # NSwag 生成的客户端代码
│   │   ├── index.ts         # API 入口文件
│   │   └── types.ts         # 临时类型定义
│   └── views/
│       └── system/
│           └── user/
│               ├── NSwagDemo.vue     # 完整示例组件
│               └── UserManagementExample.vue
├── docs/
│   ├── NSWAG_TUTORIAL.md           # 详细教程
│   ├── NSWAG_COMPLETE_GUIDE.md     # 完整指南
│   └── backend-swagger-setup.cs    # 后端配置示例
├── nswag.json               # NSwag 配置文件
├── .env.development         # 开发环境变量
└── .env.production         # 生产环境变量
```

## 🚀 快速开始

### 1. 确保后端 API 运行

```bash
# 验证后端 API 可访问
npm run api:check
```

### 2. 生成前端 API 客户端

```bash
# 生成 TypeScript 客户端代码
npm run generate-api
```

### 3. 在组件中使用 API

```typescript
import { apiService } from '@/api/service'

// 获取用户列表
const users = await apiService.getUsers()

// 创建用户
await apiService.createOrUpdateUser(userData)
```

## 📋 配置文件说明

### nswag.json 配置

```json
{
  "runtime": "Net80",
  "documentGenerator": {
    "fromDocument": {
      "url": "http://localhost:6298/swagger/v1/swagger.json"
    }
  },
  "codeGenerators": {
    "openApiToTypeScriptClient": {
      "className": "ApiClient",
      "exceptionClass": "ApiException",
      "output": "src/api/generated.ts"
    }
  }
}
```

### 环境变量

```bash
# .env.development
VITE_API_BASE_URL=http://localhost:6298

# .env.production
VITE_API_BASE_URL=/api
```

## 🛠️ API 服务使用

### 基本用法

```typescript
import { apiService, UserDto } from '@/api/service'

// 用户管理
const users = await apiService.getUsers('搜索关键字', 'CreationTime desc', 10, 0)
const user = await apiService.getUser('用户ID')
const userData = new UserDto({ userName: 'test', email: 'test@example.com' })
await apiService.createOrUpdateUser(userData)

// 角色管理
const roles = await apiService.getRoles()
await apiService.assignRoles(['用户ID'], ['角色ID'])

// 认证
const loginResult = await apiService.login('username', 'password')
```

### 错误处理

API 服务已内置统一错误处理：

- 401: 自动跳转登录页
- 403: 显示权限不足提示
- 404/500: 显示相应错误信息
- 网络错误: 显示通用错误提示

## 🎨 Vue 组件示例

完整的用户管理组件示例位于：`src/views/system/user/NSwagDemo.vue`

### 特性展示

- ✅ 用户列表查询（分页、搜索、排序）
- ✅ 用户创建和编辑
- ✅ 角色分配
- ✅ 类型安全的 API 调用
- ✅ 统一错误处理
- ✅ Loading 状态管理

### 关键代码片段

```vue
<script setup lang="ts">
import { apiService, UserDto } from '@/api/service'
import type { UserListDto } from '@/api/service'

// 获取用户列表
const loadUsers = async () => {
  try {
    loading.value = true
    const response = await apiService.getUsers(
      searchForm.filterText,
      searchForm.sorting,
      pageSize.value,
      (currentPage.value - 1) * pageSize.value
    )
  
    if (response.data?.datas) {
      userList.value = response.data.datas.items || []
      totalCount.value = response.data.datas.totalCount || 0
    }
  } catch (error) {
    ElMessage.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

// 保存用户
const handleSaveUser = async () => {
  const userData = new UserDto({
    fullName: userForm.fullName,
    userName: userForm.userName,
    email: userForm.email,
    isActive: userForm.isActive
  })
  
  await apiService.createOrUpdateUser(userData)
  ElMessage.success('保存成功')
}
</script>
```

## 🔄 开发工作流

### 日常开发

```bash
# 1. 启动后端项目 (确保 Swagger 可访问)
dotnet run

# 2. 当后端 API 有变更时，重新生成客户端代码
npm run generate-api

# 3. 启动前端开发服务器
npm run dev

# 4. 或者一次性执行（生成 API + 启动开发服务器）
npm run dev:with-api
```

### 构建部署

```bash
# 生成 API 客户端并构建生产版本
npm run build:with-api
```

## 🎯 核心优势

### 1. 类型安全

- 🔒 编译期类型检查
- 🎯 IntelliSense 智能提示
- 🛡️ 避免运行时类型错误

### 2. 自动同步

- 🔄 后端 API 变更自动同步
- 📝 自动生成文档
- ⚡ 减少前后端沟通成本

### 3. 开发效率

- 🚀 自动生成客户端代码
- 🛠️ 统一的错误处理
- 📦 模块化的 API 服务

### 4. 维护性

- 🏗️ 结构化的代码组织
- 🔧 易于扩展和维护
- 📊 清晰的数据流

## 📚 相关文档

- [详细教程](./docs/NSWAG_TUTORIAL.md)
- [完整指南](./docs/NSWAG_COMPLETE_GUIDE.md)
- [NSwag 官方文档](https://github.com/RicoSuter/NSwag)
- [OpenAPI 规范](https://swagger.io/specification/)

## ⚠️ 注意事项

1. **版本兼容性**: 确保 NSwag 版本与 .NET 版本兼容
2. **网络访问**: 生成时需要能访问后端 Swagger 文档
3. **类型定义**: 使用生成的类而不是普通对象字面量
4. **错误处理**: 利用内置的错误处理机制
5. **环境配置**: 正确配置不同环境的 API 基础 URL

## 🎉 总结

通过 NSwag，我们实现了：

- ✅ 前后端类型安全的 API 通信
- ✅ 自动化的代码生成和同步
- ✅ 统一的错误处理和状态管理
- ✅ 优秀的开发者体验

这套方案极大地提高了开发效率，减少了维护成本，是现代 Web 应用开发的最佳实践之一。

---

**开发团队**: FlyFrameWork
**最后更新**: 2025年8月26日

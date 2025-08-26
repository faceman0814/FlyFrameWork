# NSwag + Vue + .NET 集成完整指南

## 概述

本指南提供了使用 NSwag 工具将 .NET 后端 API 与 Vue 前端集成的完整解决方案。我们已经成功实现了：

- ✅ NSwag 客户端自动生成
- ✅ 类型安全的 API 调用
- ✅ 统一的错误处理
- ✅ Token 认证集成
- ✅ Pinia 状态管理集成
- ✅ 完整的登录/退出流程

## 项目结构

```
src/
├── api/
│   ├── base.ts          # 基础 API 配置（错误处理、Token 注入）
│   ├── service.ts       # NSwag 客户端服务封装
│   ├── user.ts          # 用户相关 API（使用 NSwag）
│   ├── role.ts          # 角色相关 API（使用 NSwag）
│   └── generated/       # NSwag 生成的客户端代码
│       └── api-client.ts
├── stores/
│   └── user.ts          # 用户状态管理（集成 NSwag）
├── views/
│   ├── login/
│   │   └── index.vue    # 登录页面
│   └── system/
│       └── user/
│           ├── index.vue           # 用户管理页面
│           ├── NSwagDemo.vue       # NSwag API 演示
│           └── LoginFlowDemo.vue   # 登录流程演示
└── utils/
    └── request.ts       # HTTP 请求配置
```

## 核心文件说明

### 1. NSwag 配置 (`nswag.json`)

```json
{
  "runtime": "Net60",
  "defaultVariables": null,
  "documentGenerator": {
    "fromDocument": {
      "url": "http://localhost:5000/swagger/v1/swagger.json",
      "output": null,
      "newLineBehavior": "Auto"
    }
  },
  "codeGenerators": {
    "openApiToTypeScriptClient": {
      "className": "ApiClient",
      "moduleName": "",
      "namespace": "",
      "typeScriptVersion": 4.3,
      "template": "Angular",
      "promiseType": "Promise",
      "httpClass": "HttpClient",
      "withCredentials": false,
      "useSingletonProvider": false,
      "injectionTokenType": "OpaqueToken",
      "rxJsVersion": 6.0,
      "dateTimeType": "Date",
      "nullValue": "Undefined",
      "generateClientClasses": true,
      "generateClientInterfaces": false,
      "generateOptionalParameters": false,
      "exportTypes": true,
      "wrapDtoExceptions": true,
      "exceptionClass": "ApiException",
      "clientBaseClass": null,
      "wrapResponses": false,
      "wrapResponseMethods": [],
      "generateResponseClasses": true,
      "responseClass": "SwaggerResponse",
      "protectedMethods": [],
      "configurationClass": null,
      "useTransformOptionsMethod": false,
      "useTransformResultMethod": false,
      "generateDtoTypes": true,
      "operationGenerationMode": "MultipleClientsFromOperationId",
      "markOptionalProperties": true,
      "generateCloneMethod": false,
      "typeStyle": "Class",
      "classTypes": [],
      "extendedClasses": [],
      "extensionCode": null,
      "generateDefaultValues": true,
      "excludedTypeNames": [],
      "excludedParameterNames": [],
      "handleReferences": false,
      "generateConstructorInterface": true,
      "convertConstructorInterfaceData": false,
      "importRequiredTypes": true,
      "useGetBaseUrlMethod": false,
      "baseUrlTokenName": "API_BASE_URL",
      "queryNullValue": "",
      "outputExtension": ".ts",
      "output": "src/api/generated/api-client.ts"
    }
  }
}
```

### 2. 基础 API 配置 (`src/api/base.ts`)

```typescript
import { ElMessage } from 'element-plus'
import { getToken, removeToken } from '@/utils/auth'

// 基础 API 配置
export const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

// 请求拦截器 - 添加认证token
export const addAuthToken = (config: any) => {
  const token = getToken()
  if (token) {
    config.headers = config.headers || {}
    config.headers['Authorization'] = `Bearer ${token}`
  }
  return config
}

// 错误处理
export const handleApiError = (error: any) => {
  console.error('API Error:', error)
  
  if (error.response) {
    const { status, data } = error.response
    
    switch (status) {
      case 401:
        removeToken()
        ElMessage.error('登录已过期，请重新登录')
        // 跳转到登录页
        window.location.href = '/login'
        break
      case 403:
        ElMessage.error('权限不足，无法访问该资源')
        break
      case 404:
        ElMessage.error('请求的资源不存在')
        break
      case 500:
        ElMessage.error('服务器内部错误')
        break
      default:
        ElMessage.error(data?.message || `请求失败: ${status}`)
    }
  } else if (error.request) {
    ElMessage.error('网络错误，请检查网络连接')
  } else {
    ElMessage.error(error.message || '请求失败')
  }
  
  throw error
}
```

### 3. NSwag 服务封装 (`src/api/service.ts`)

```typescript
import { ApiClient } from './generated/api-client'
import { API_BASE_URL, handleApiError } from './base'
import { getToken } from '@/utils/auth'

// 创建 NSwag 客户端实例
class ApiService {
  private client: ApiClient

  constructor() {
    this.client = new ApiClient(API_BASE_URL)
  }

  // 获取客户端实例，自动添加认证头
  getClient() {
    // 每次调用时更新 token
    const token = getToken()
    if (token) {
      // 设置默认的认证头
      this.client.instance.defaults.headers.common['Authorization'] = `Bearer ${token}`
    }
    return this.client
  }

  // 统一的 API 调用方法，包含错误处理
  async call<T>(apiCall: () => Promise<T>): Promise<T> {
    try {
      return await apiCall()
    } catch (error) {
      handleApiError(error)
      throw error
    }
  }
}

export const apiService = new ApiService()
export default apiService
```

### 4. 用户 API (`src/api/user.ts`)

```typescript
import { apiService } from './service'

// 登录
export const login = async (data: { username: string; password: string }) => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.login(data.username, data.password)
  })
}

// 获取用户信息
export const getUserInfo = async () => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.getCurrentUser()
  })
}

// 退出登录
export const logout = async () => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.logout()
  })
}

// 获取用户列表
export const getUsers = async (params?: { page?: number; size?: number; keyword?: string }) => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.getUsers(params?.page, params?.size, params?.keyword)
  })
}

// 创建用户
export const createUser = async (userData: any) => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.createUser(userData)
  })
}

// 更新用户
export const updateUser = async (id: string, userData: any) => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.updateUser(id, userData)
  })
}

// 删除用户
export const deleteUser = async (id: string) => {
  return apiService.call(() => {
    const client = apiService.getClient()
    return client.deleteUser(id)
  })
}
```

### 5. 用户状态管理 (`src/stores/user.ts`)

```typescript
import { defineStore } from 'pinia'
import { ElMessage } from 'element-plus'
import { setToken, getToken, removeToken } from '@/utils/auth'
import { login, getUserInfo, logout } from '@/api/user'

interface UserState {
  token: string
  name: string
  avatar: string
  roles: string[]
  permissions: string[]
  dynamicRoutes: any[]
  refreshToken: string
  userId: string
}

export const useUserStore = defineStore('user', {
  state: (): UserState => ({
    token: getToken() || '',
    name: '',
    avatar: '',
    roles: [],
    permissions: [],
    dynamicRoutes: [],
    refreshToken: '',
    userId: ''
  }),

  getters: {
    getRoles: (state) => state.roles,
    getPermissions: (state) => state.permissions,
    isLoggedIn: (state) => !!state.token
  },

  actions: {
    // 使用 NSwag 客户端登录
    async login(userInfo: { username: string; password: string }) {
      const { username, password } = userInfo
      try {
        const response = await login({
          username: username.trim(),
          password
        })
        
        if (response.code === 200 && response.data && response.data.token) {
          // 设置 token 和其他信息
          this.token = response.data.token
          this.refreshToken = response.data.refreshToken || ''
          
          setToken(response.data.token)
          
          // 如果登录响应中包含用户信息，直接使用
          if (response.data.user) {
            const userData = response.data.user
            this.userId = userData.userId || ''
            this.name = userData.nickName || userData.userName || 'User'
            this.avatar = userData.avatar || 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif'
            this.roles = userData.roles || ['user']
            this.permissions = userData.permissions || []
            
            // 存储到 localStorage 以便刷新页面后恢复
            localStorage.setItem('userInfo', JSON.stringify({
              userId: this.userId,
              name: this.name,
              avatar: this.avatar,
              roles: this.roles,
              permissions: this.permissions
            }))
          }
          
          ElMessage.success('登录成功')
          return Promise.resolve(response.data)
        } else {
          throw new Error(response.message || '登录失败')
        }
      } catch (error: any) {
        console.error('登录失败:', error)
        ElMessage.error(error.message || '登录失败，请检查用户名和密码')
        return Promise.reject(error)
      }
    },

    // 获取用户信息
    async getUserInfo() {
      try {
        // 如果已有用户信息，直接从 localStorage 恢复
        const cachedUserInfo = localStorage.getItem('userInfo')
        if (cachedUserInfo) {
          const userInfo = JSON.parse(cachedUserInfo)
          this.userId = userInfo.userId
          this.name = userInfo.name
          this.avatar = userInfo.avatar
          this.roles = userInfo.roles
          this.permissions = userInfo.permissions
          return userInfo
        }

        // 否则调用 API 获取用户信息
        const response = await getUserInfo()

        if (response.code === 200 && response.data) {
          const { data } = response

          // 验证返回的角色是否为非空数组
          if (!data.roles || data.roles.length <= 0) {
            throw Error('getUserInfo: roles must be a non-null array!')
          }

          this.roles = data.roles
          this.name = data.name || data.username || 'User'
          this.avatar = data.avatar || 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif'
          this.permissions = data.permissions || ['*:*:*']
          this.userId = data.id || ''
          
          // 缓存用户信息
          localStorage.setItem('userInfo', JSON.stringify({
            userId: this.userId,
            name: this.name,
            avatar: this.avatar,
            roles: this.roles,
            permissions: this.permissions
          }))
          
          return data
        } else {
          throw new Error(response.message || 'Failed to get user info')
        }
      } catch (error: any) {
        console.error('获取用户信息失败:', error)
        
        // 如果获取用户信息失败，使用默认值
        const defaultUserInfo = {
          roles: ['user'],
          name: '用户',
          avatar: 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif',
          permissions: ['*:*:*'],
          id: 'default'
        }
        
        this.roles = defaultUserInfo.roles
        this.name = defaultUserInfo.name
        this.avatar = defaultUserInfo.avatar
        this.permissions = defaultUserInfo.permissions
        this.userId = defaultUserInfo.id
        
        return defaultUserInfo
      }
    },

    // 退出登录
    async logout() {
      try {
        await logout()
      } catch (error) {
        console.error('Logout API call failed:', error)
        // 即使 API 调用失败，也要清除本地状态
      } finally {
        // 清除所有状态
        this.resetState()
        ElMessage.success('已退出登录')
      }
    },

    // 重置状态
    resetState() {
      this.token = ''
      this.name = ''
      this.avatar = ''
      this.roles = []
      this.permissions = []
      this.refreshToken = ''
      this.userId = ''
      
      removeToken()
      localStorage.removeItem('userInfo')
    },

    // 从本地存储恢复用户状态
    restoreFromStorage() {
      const token = getToken()
      const userInfo = localStorage.getItem('userInfo')
      
      if (token) {
        this.token = token
      }
      
      if (userInfo) {
        try {
          const parsed = JSON.parse(userInfo)
          this.userId = parsed.userId || ''
          this.name = parsed.name || ''
          this.avatar = parsed.avatar || ''
          this.roles = parsed.roles || []
          this.permissions = parsed.permissions || []
        } catch (error) {
          console.error('Failed to parse stored user info:', error)
          localStorage.removeItem('userInfo')
        }
      }
    }
  }
})
```

## 使用方法

### 1. 生成 API 客户端

```bash
# 安装 NSwag CLI（如果还没安装）
npm install -g nswag

# 生成 TypeScript 客户端
npm run api:generate

# 或者手动运行
nswag run nswag.json
```

### 2. 在组件中使用

```vue
<template>
  <div>
    <el-button @click="handleLogin">登录</el-button>
    <el-button @click="handleLogout">退出</el-button>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/stores/user'
import { useRouter } from 'vue-router'

const userStore = useUserStore()
const router = useRouter()

const handleLogin = async () => {
  try {
    await userStore.login({
      username: 'admin',
      password: 'admin123'
    })
    router.push('/dashboard')
  } catch (error) {
    console.error('登录失败:', error)
  }
}

const handleLogout = async () => {
  await userStore.logout()
  router.push('/login')
}
</script>
```

### 3. API 调用示例

```typescript
import { getUsers, createUser, updateUser, deleteUser } from '@/api/user'

// 获取用户列表
const users = await getUsers({ page: 1, size: 10 })

// 创建用户
await createUser({
  username: 'newuser',
  email: 'newuser@example.com',
  password: 'password123'
})

// 更新用户
await updateUser('user-id', {
  username: 'updateduser',
  email: 'updated@example.com'
})

// 删除用户
await deleteUser('user-id')
```

## 测试和调试

### 1. 登录流程测试

访问 `/system/login-demo` 页面查看完整的登录流程演示，包括：

- 登录测试
- 获取用户信息
- 退出登录
- 状态变化跟踪

### 2. API 调用测试

访问 `/system/user` 页面查看用户管理功能，包括 NSwag API 的实际应用。

### 3. 开发工具

```bash
# 启动开发服务器
npm run dev

# 构建生产版本
npm run build

# 预览生产版本
npm run preview

# 重新生成 API 客户端
npm run api:generate
```

## 故障排除

### 1. NSwag 生成失败

- 检查 `nswag.json` 配置
- 确保后端 Swagger 服务正常运行
- 检查网络连接和 URL 可访问性

### 2. 类型错误

- 重新生成 API 客户端
- 检查生成的类型定义
- 确保 TypeScript 版本兼容

### 3. 认证失败

- 检查 token 是否正确设置
- 验证 API 基础地址配置
- 查看网络请求头

### 4. 状态管理问题

- 检查 Pinia store 的状态同步
- 验证 localStorage 数据
- 重置浏览器缓存

## 最佳实践

1. **类型安全**: 始终使用 NSwag 生成的类型定义
2. **错误处理**: 统一的错误处理和用户反馈
3. **状态管理**: 合理使用 Pinia 进行状态持久化
4. **安全性**: 正确处理 token 存储和传输
5. **性能**: 合理缓存用户信息和 API 响应

## 总结

通过 NSwag 工具，我们成功实现了 .NET 后端与 Vue 前端的无缝集成，提供了：

- 🎯 **类型安全**: 完全的 TypeScript 类型支持
- 🔒 **安全认证**: JWT token 自动管理
- 🎨 **优雅架构**: 模块化的 API 服务设计
- 🚀 **开发效率**: 自动化的客户端代码生成
- 🛠️ **完整工具链**: 从开发到部署的完整支持

这个方案为大型企业级应用提供了稳定、可维护的前后端集成解决方案。
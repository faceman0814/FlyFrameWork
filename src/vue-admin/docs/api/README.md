# API 文档

## API 架构

本项目采用分层的 API 架构，提供类型安全的 API 调用。

### 文件结构

```
src/api/
├── base.ts              # 基础配置和拦截器
├── generated.ts         # NSwag生成的API客户端
├── index.ts             # API导出入口
├── role.ts              # 角色相关API
├── service-proxies.ts   # 服务代理
├── service.ts           # 服务基类
├── types.ts             # 类型定义
└── user.ts              # 用户相关API
```

## 基础配置

### HTTP 客户端配置

```typescript
// src/api/base.ts
import axios from 'axios'
import { ElMessage } from 'element-plus'

const apiClient = axios.create({
  baseURL: '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截器
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// 响应拦截器
apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    ElMessage.error(error.message)
    return Promise.reject(error)
  }
)
```

## API 服务

### 用户服务

```typescript
// src/api/user.ts
export interface User {
  id: string
  username: string
  email: string
  roles: string[]
  createdAt: string
  updatedAt: string
}

export interface CreateUserRequest {
  username: string
  email: string
  password: string
  roles?: string[]
}

export interface UpdateUserRequest {
  username?: string
  email?: string
  roles?: string[]
}

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  user: User
}

export class UserService {
  // 用户登录
  static async login(request: LoginRequest): Promise<LoginResponse> {
    return apiClient.post('/auth/login', request)
  }

  // 获取当前用户信息
  static async getCurrentUser(): Promise<User> {
    return apiClient.get('/users/me')
  }

  // 获取用户列表
  static async getUsers(page = 1, pageSize = 20): Promise<{
    items: User[]
    total: number
    page: number
    pageSize: number
  }> {
    return apiClient.get('/users', {
      params: { page, pageSize }
    })
  }

  // 创建用户
  static async createUser(request: CreateUserRequest): Promise<User> {
    return apiClient.post('/users', request)
  }

  // 更新用户
  static async updateUser(id: string, request: UpdateUserRequest): Promise<User> {
    return apiClient.put(`/users/${id}`, request)
  }

  // 删除用户
  static async deleteUser(id: string): Promise<void> {
    return apiClient.delete(`/users/${id}`)
  }
}
```

### 角色服务

```typescript
// src/api/role.ts
export interface Role {
  id: string
  name: string
  description: string
  permissions: Permission[]
  createdAt: string
  updatedAt: string
}

export interface Permission {
  id: string
  name: string
  resource: string
  action: string
}

export interface CreateRoleRequest {
  name: string
  description?: string
  permissions: string[]
}

export class RoleService {
  // 获取角色列表
  static async getRoles(): Promise<Role[]> {
    return apiClient.get('/roles')
  }

  // 创建角色
  static async createRole(request: CreateRoleRequest): Promise<Role> {
    return apiClient.post('/roles', request)
  }

  // 更新角色
  static async updateRole(id: string, request: Partial<CreateRoleRequest>): Promise<Role> {
    return apiClient.put(`/roles/${id}`, request)
  }

  // 删除角色
  static async deleteRole(id: string): Promise<void> {
    return apiClient.delete(`/roles/${id}`)
  }

  // 获取权限列表
  static async getPermissions(): Promise<Permission[]> {
    return apiClient.get('/permissions')
  }
}
```

## NSwag 集成

### 配置文件

```json
// nswag.json
{
  "runtime": "Net60",
  "defaultVariables": null,
  "documentGenerator": {
    "fromDocument": {
      "url": "http://localhost:6298/swagger/v1/swagger.json",
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
      "enumStyle": "Enum",
      "useLeafType": false,
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
      "useAbortSignal": false,
      "inlineNamedDictionaries": false,
      "inlineNamedAny": false,
      "includeHttpContext": false,
      "templateDirectory": null,
      "typeNameGeneratorType": null,
      "propertyNameGeneratorType": null,
      "enumNameGeneratorType": null,
      "serviceHost": null,
      "serviceSchemes": null,
      "output": "src/api/generated.ts"
    }
  }
}
```

### 使用生成的客户端

```typescript
// src/api/service.ts
import { ApiClient } from './generated'

export class ApiService {
  private client: ApiClient

  constructor() {
    this.client = new ApiClient('http://localhost:6298')
  }

  // 获取客户端实例
  getClient(): ApiClient {
    return this.client
  }
}

// 使用示例
const apiService = new ApiService()
const client = apiService.getClient()

// 调用API
const users = await client.getUsers()
```

## 错误处理

### 错误类型定义

```typescript
export interface ApiError {
  code: string
  message: string
  details?: any
}

export class ApiException extends Error {
  constructor(
    public code: string,
    message: string,
    public details?: any
  ) {
    super(message)
  }
}
```

### 统一错误处理

```typescript
// 响应拦截器中的错误处理
apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (error.response) {
      const { status, data } = error.response
      
      switch (status) {
        case 401:
          // 未授权，跳转到登录页
          router.push('/login')
          break
        case 403:
          ElMessage.error('无权限访问')
          break
        case 404:
          ElMessage.error('资源不存在')
          break
        case 500:
          ElMessage.error('服务器错误')
          break
        default:
          ElMessage.error(data?.message || '请求失败')
      }
    }
    
    return Promise.reject(new ApiException(
      error.code,
      error.message,
      error.response?.data
    ))
  }
)
```

## 最佳实践

1. **类型安全**：使用 TypeScript 定义所有 API 接口
2. **错误处理**：统一处理 HTTP 错误和业务错误
3. **请求取消**：支持请求取消以避免竞争条件
4. **缓存策略**：实现合适的缓存机制
5. **重试机制**：对失败的请求实现重试
6. **Loading 状态**：提供请求状态指示
7. **参数验证**：在发送请求前验证参数
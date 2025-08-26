# NSwag 完整操作步骤指南

## 🎯 概述

NSwag 是一个强大的工具，可以根据 OpenAPI/Swagger 规范自动生成 TypeScript 客户端代码，实现前后端类型安全的 API 调用。

## 📋 前置条件

1. **.NET 后端项目**已配置 Swagger/OpenAPI
2. **Vue 3 + TypeScript** 前端项目
3. **Node.js** 和 **npm** 环境

## 🚀 快速开始

### 步骤 1: 后端配置

确保您的 .NET 项目已经配置了 Swagger：

```csharp
// Program.cs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    
    // 添加 JWT 认证支持
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

// 启用 Swagger
app.UseSwagger();
app.UseSwaggerUI();
```

### 步骤 2: 验证 Swagger 文档

启动后端项目，访问：
```
http://localhost:6298/swagger/v1/swagger.json
```

确保能看到完整的 API 文档。

### 步骤 3: 生成前端 API 客户端

在前端项目根目录执行：

```bash
# 生成 API 客户端代码
npm run generate-api

# 或者直接使用 nswag
npx nswag run nswag.json
```

这将在 `src/api/generated.ts` 生成客户端代码。

### 步骤 4: 使用生成的客户端

```typescript
// 在 Vue 组件中使用
import { UserClient, type UserDto } from '@/api/generated'

const userClient = new UserClient('/api')

// 获取用户列表
const users = await userClient.getUsers()

// 创建用户
const newUser = await userClient.createUser({
  userName: 'testuser',
  email: 'test@example.com',
  password: 'password123'
})
```

## 🛠️ 详细配置说明

### NSwag 配置文件 (nswag.json)

```json
{
  "runtime": "Net60",
  "documentGenerator": {
    "fromDocument": {
      "url": "http://localhost:6298/swagger/v1/swagger.json"
    }
  },
  "codeGenerators": {
    "openApiToTypeScriptClient": {
      "className": "ApiClient",
      "clientBaseClass": "ApiClientBase",
      "useTransformOptionsMethod": true,
      "useTransformResultMethod": true,
      "output": "src/api/generated.ts"
    }
  }
}
```

**关键配置项解释：**
- `clientBaseClass`: 指定基类，用于统一处理认证和错误
- `useTransformOptionsMethod`: 启用请求拦截（添加 token 等）
- `useTransformResultMethod`: 启用响应拦截（错误处理等）

### 基础类配置 (src/api/base.ts)

```typescript
export class ApiClientBase {
  protected transformOptions = (options: RequestInit): Promise<RequestInit> => {
    const token = getToken()
    if (token) {
      options.headers = {
        ...options.headers,
        Authorization: `Bearer ${token}`
      }
    }
    return Promise.resolve(options)
  }

  protected transformResult = (url: string, response: Response, processor: any) => {
    return processor(response).catch(async (error: any) => {
      if (response.status === 401) {
        // 处理未授权
        localStorage.removeItem('token')
        window.location.href = '/login'
      }
      throw error
    })
  }
}
```

## 📝 实际使用示例

### 在 Vue 组件中使用

```vue
<template>
  <div>
    <el-button @click="loadUsers">获取用户</el-button>
    <el-table :data="users" v-loading="loading">
      <el-table-column prop="userName" label="用户名" />
      <el-table-column prop="email" label="邮箱" />
    </el-table>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { UserClient, type UserDto } from '@/api/generated'

const users = ref<UserDto[]>([])
const loading = ref(false)
const userClient = new UserClient('/api')

const loadUsers = async () => {
  try {
    loading.value = true
    const result = await userClient.getUsers()
    users.value = result
  } catch (error) {
    console.error('获取用户失败:', error)
  } finally {
    loading.value = false
  }
}
</script>
```

## 🔄 开发工作流程

### 1. 日常开发流程

```bash
# 1. 启动后端项目
dotnet run

# 2. 更新前端 API 客户端（当后端 API 变更时）
npm run generate-api

# 3. 启动前端开发服务器
npm run dev
```

### 2. 自动化脚本

我们已经在 `package.json` 中添加了便捷脚本：

```bash
# 生成 API 并启动开发服务器
npm run dev:with-api

# 生成 API 并构建生产版本
npm run build:with-api

# 检查后端 API 是否可用
npm run api:check
```

## ⚠️ 常见问题与解决方案

### 问题 1: 无法访问 Swagger 文档
**错误**: `Failed to fetch swagger.json`

**解决方案**:
1. 检查后端项目是否正在运行
2. 确认端口号是否正确 (默认 6298)
3. 检查 CORS 配置

### 问题 2: 生成的代码有编译错误
**解决方案**:
1. 检查 `nswag.json` 配置
2. 确保 `ApiClientBase` 类存在
3. 重新生成代码：`npm run generate-api`

### 问题 3: 认证 token 未生效
**解决方案**:
1. 检查 `transformOptions` 方法实现
2. 确认 token 存储和获取逻辑
3. 验证后端 JWT 配置

## 🎛️ 高级配置

### 多环境配置

```bash
# .env.development
VITE_API_BASE_URL=http://localhost:6298/api

# .env.production  
VITE_API_BASE_URL=/api
```

### 自定义错误处理

```typescript
export class CustomApiException extends Error {
  constructor(
    public status: number,
    public response: string,
    public headers: Record<string, any>
  ) {
    super(`API Error ${status}`)
  }
}
```

## 🔧 调试技巧

1. **启用详细日志**：在 `nswag.json` 中添加调试选项
2. **检查网络请求**：使用浏览器开发者工具
3. **验证生成的代码**：检查 `src/api/generated.ts` 文件

## 📚 最佳实践

1. **类型安全**：充分利用 TypeScript 类型检查
2. **错误处理**：在基类中实现统一错误处理
3. **缓存策略**：对不变数据实现适当缓存
4. **版本控制**：不要提交生成的文件到 Git
5. **文档同步**：保持前后端 API 文档同步

## 🚀 部署注意事项

1. **生产环境**：确保 API 基础 URL 正确配置
2. **构建脚本**：使用 `npm run build:with-api`
3. **CI/CD**：在构建流程中包含 API 生成步骤

通过以上配置，您就可以享受到 NSwag 带来的类型安全、自动化的前后端协作体验了！
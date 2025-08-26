# NSwag 使用教程

## 工作流程

### 1. 启动后端项目
确保您的 .NET 项目正在运行，并且 Swagger 文档可以通过以下地址访问：
```
http://localhost:6298/swagger/v1/swagger.json
```

### 2. 生成前端 API 客户端
在前端项目根目录运行以下命令：

```bash
# 使用 package.json 中的脚本
npm run generate-api

# 或直接使用 nswag 命令
npx nswag run nswag.json
```

### 3. 使用生成的客户端

生成的代码会包含：
- 类型定义（DTOs）
- API 客户端类
- 异常类型

#### 基本使用示例：

```typescript
import { UserClient, UserDto, CreateUserDto } from '@/api/generated'

// 创建客户端实例
const userClient = new UserClient('/api')

// 获取用户列表
const users = await userClient.getUsers(1, 10, 'search', true)

// 获取单个用户
const user = await userClient.getUser(1)

// 创建用户
const newUser: CreateUserDto = {
  userName: 'testuser',
  email: 'test@example.com',
  password: 'password123'
}
const createdUser = await userClient.createUser(newUser)
```

## Vue 组件中的使用示例

### 在 Vue 3 + Composition API 中使用：

```typescript
<template>
  <div>
    <el-table :data="users" v-loading="loading">
      <el-table-column prop="id" label="ID" />
      <el-table-column prop="userName" label="用户名" />
      <el-table-column prop="email" label="邮箱" />
      <el-table-column prop="isActive" label="状态">
        <template #default="scope">
          <el-tag :type="scope.row.isActive ? 'success' : 'danger'">
            {{ scope.row.isActive ? '激活' : '禁用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template #default="scope">
          <el-button @click="editUser(scope.row)">编辑</el-button>
          <el-button type="danger" @click="deleteUser(scope.row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    
    <el-pagination
      v-model:current-page="queryParams.page"
      v-model:page-size="queryParams.pageSize"
      :total="total"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { UserClient, UserDto, UserQueryDto } from '@/api/generated'
import { ElMessage, ElMessageBox } from 'element-plus'

// 数据
const users = ref<UserDto[]>([])
const total = ref(0)
const loading = ref(false)

// 查询参数
const queryParams = reactive<UserQueryDto>({
  page: 1,
  pageSize: 10,
  search: '',
  isActive: undefined
})

// API 客户端
const userClient = new UserClient('/api')

// 获取用户列表
const getUserList = async () => {
  try {
    loading.value = true
    const result = await userClient.getUsers(
      queryParams.page,
      queryParams.pageSize,
      queryParams.search,
      queryParams.isActive
    )
    users.value = result.items
    total.value = result.total
  } catch (error) {
    ElMessage.error('获取用户列表失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

// 删除用户
const deleteUser = async (id: number) => {
  try {
    await ElMessageBox.confirm('确认删除该用户？', '提示')
    await userClient.deleteUser(id)
    ElMessage.success('删除成功')
    await getUserList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
      console.error(error)
    }
  }
}

// 编辑用户
const editUser = (user: UserDto) => {
  // 实现编辑逻辑
  console.log('编辑用户', user)
}

// 分页处理
const handleSizeChange = (size: number) => {
  queryParams.pageSize = size
  getUserList()
}

const handleCurrentChange = (page: number) => {
  queryParams.page = page
  getUserList()
}

// 页面加载时获取数据
onMounted(() => {
  getUserList()
})
</script>
```

## 错误处理

生成的客户端会抛出 `ApiException` 类型的异常，您可以在基础类中统一处理：

```typescript
import { ApiException } from '@/api/generated'

try {
  const result = await userClient.getUsers(1, 10)
  // 处理成功结果
} catch (error) {
  if (error instanceof ApiException) {
    // 处理 API 异常
    switch (error.status) {
      case 401:
        // 未授权，跳转登录
        break
      case 403:
        // 权限不足
        break
      case 404:
        // 资源不存在
        break
      default:
        // 其他错误
        console.error('API Error:', error.message)
    }
  } else {
    // 处理其他类型的错误
    console.error('Unexpected error:', error)
  }
}
```

## 配置说明

### nswag.json 配置解释：

- `clientBaseClass`: 指定生成的客户端继承的基类
- `useTransformOptionsMethod`: 启用请求转换方法（用于添加认证头等）
- `useTransformResultMethod`: 启用响应转换方法（用于统一错误处理）
- `useAbortSignal`: 支持请求取消
- `exceptionClass`: 自定义异常类名称

### 环境变量配置：

在 `.env.development` 和 `.env.production` 中配置不同环境的 API 基础 URL。

## 开发建议

1. **类型安全**：充分利用 TypeScript 的类型检查，避免运行时错误
2. **错误处理**：在基础类中实现统一的错误处理逻辑
3. **缓存策略**：对于不经常变化的数据，考虑实现缓存机制
4. **请求取消**：对于长时间运行的请求，使用 AbortSignal 支持取消
5. **环境隔离**：使用不同的环境变量文件管理不同环境的配置

## 自动化工作流

可以在 `package.json` 中添加更多脚本来自动化开发流程：

```json
{
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "generate-api": "nswag run nswag.json",
    "dev:with-api": "npm run generate-api && npm run dev",
    "build:with-api": "npm run generate-api && npm run build"
  }
}
```
<template>
  <div class="user-management">
    <!-- 搜索栏 -->
    <el-card class="search-card">
      <el-form :model="searchForm" inline>
        <el-form-item label="搜索">
          <el-input 
            v-model="searchForm.search" 
            placeholder="请输入用户名或邮箱"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="searchForm.isActive" placeholder="请选择状态" clearable>
            <el-option label="激活" :value="true" />
            <el-option label="禁用" :value="false" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleAdd">新增用户</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 用户列表 -->
    <el-card class="table-card">
      <el-table :data="userList" v-loading="loading" border stripe>
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="userName" label="用户名" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'">
              {{ scope.row.isActive ? '激活' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button type="primary" size="small" @click="handleEdit(scope.row)">
              编辑
            </el-button>
            <el-button 
              type="danger" 
              size="small" 
              @click="handleDelete(scope.row.id)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="queryParams.page"
          v-model:page-size="queryParams.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 用户编辑对话框 -->
    <el-dialog 
      v-model="dialogVisible" 
      :title="isEdit ? '编辑用户' : '新增用户'"
      width="500px"
    >
      <el-form 
        ref="formRef" 
        :model="userForm" 
        :rules="formRules" 
        label-width="80px"
      >
        <el-form-item label="用户名" prop="userName">
          <el-input v-model="userForm.userName" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="userForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!isEdit">
          <el-input 
            v-model="userForm.password" 
            type="password" 
            placeholder="请输入密码" 
          />
        </el-form-item>
        <el-form-item label="状态" prop="isActive" v-if="isEdit">
          <el-switch v-model="userForm.isActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { apiService } from '@/api'
import type { UserDto, CreateUserDto, UpdateUserDto, UserQueryDto } from '@/api/types'

// 数据定义
const userList = ref<UserDto[]>([])
const total = ref(0)
const loading = ref(false)
const submitLoading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance>()

// 搜索表单
const searchForm = reactive({
  search: '',
  isActive: undefined as boolean | undefined
})

// 查询参数
const queryParams = reactive<UserQueryDto>({
  page: 1,
  pageSize: 10,
  search: '',
  isActive: undefined
})

// 用户表单
const userForm = reactive({
  id: 0,
  userName: '',
  email: '',
  password: '',
  isActive: true
})

// 表单验证规则
const formRules: FormRules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 2, max: 50, message: '用户名长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度至少 6 个字符', trigger: 'blur' }
  ]
}

// 获取用户列表
const getUserList = async () => {
  try {
    loading.value = true
    const result = await apiService.user.getUsers(queryParams)
    userList.value = result.items
    total.value = result.total
  } catch (error: any) {
    ElMessage.error('获取用户列表失败：' + (error.message || '未知错误'))
    console.error(error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  queryParams.page = 1
  queryParams.search = searchForm.search
  queryParams.isActive = searchForm.isActive
  getUserList()
}

// 重置搜索
const handleReset = () => {
  searchForm.search = ''
  searchForm.isActive = undefined
  queryParams.page = 1
  queryParams.search = ''
  queryParams.isActive = undefined
  getUserList()
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

// 新增用户
const handleAdd = () => {
  isEdit.value = false
  dialogVisible.value = true
  resetForm()
}

// 编辑用户
const handleEdit = (user: UserDto) => {
  isEdit.value = true
  dialogVisible.value = true
  userForm.id = user.id
  userForm.userName = user.userName
  userForm.email = user.email
  userForm.isActive = user.isActive
  userForm.password = '' // 编辑时密码为空
}

// 删除用户
const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确认删除该用户？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await apiService.user.deleteUser(id)
    ElMessage.success('删除成功')
    await getUserList()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败：' + (error.message || '未知错误'))
      console.error(error)
    }
  }
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  const valid = await formRef.value.validate()
  if (!valid) return
  
  try {
    submitLoading.value = true
    
    if (isEdit.value) {
      // 更新用户
      const updateDto: UpdateUserDto = {
        userName: userForm.userName,
        email: userForm.email,
        isActive: userForm.isActive
      }
      await apiService.user.updateUser(userForm.id, updateDto)
      ElMessage.success('更新成功')
    } else {
      // 创建用户
      const createDto: CreateUserDto = {
        userName: userForm.userName,
        email: userForm.email,
        password: userForm.password
      }
      await apiService.user.createUser(createDto)
      ElMessage.success('创建成功')
    }
    
    dialogVisible.value = false
    await getUserList()
  } catch (error: any) {
    ElMessage.error((isEdit.value ? '更新' : '创建') + '失败：' + (error.message || '未知错误'))
    console.error(error)
  } finally {
    submitLoading.value = false
  }
}

// 重置表单
const resetForm = () => {
  userForm.id = 0
  userForm.userName = ''
  userForm.email = ''
  userForm.password = ''
  userForm.isActive = true
  formRef.value?.clearValidate()
}

// 格式化日期
const formatDate = (date: Date) => {
  return new Date(date).toLocaleString('zh-CN')
}

// 页面加载时获取数据
onMounted(() => {
  getUserList()
})
</script>

<style scoped>
.user-management {
  padding: 20px;
}

.search-card {
  margin-bottom: 20px;
}

.table-card {
  .pagination-wrapper {
    margin-top: 20px;
    text-align: right;
  }
}
</style>
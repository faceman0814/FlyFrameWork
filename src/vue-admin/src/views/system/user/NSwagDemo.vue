<template>
  <div class="user-management-demo">
    <el-card class="demo-card">
      <template #header>
        <div class="demo-header">
          <span>用户管理示例 - 使用 NSwag 生成的 API 客户端</span>
          <el-button type="primary" @click="loadUsers">刷新数据</el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <el-form :model="searchForm" inline class="search-form">
        <el-form-item label="搜索关键字">
          <el-input 
            v-model="searchForm.filterText" 
            placeholder="请输入用户名或邮箱"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="showAddDialog">新增用户</el-button>
        </el-form-item>
      </el-form>

      <!-- 用户列表 -->
      <el-table :data="userList" v-loading="loading" border stripe>
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="fullName" label="姓名" />
        <el-table-column prop="userName" label="用户名" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="phoneNumber" label="手机号" />
        <el-table-column label="角色">
          <template #default="scope">
            <el-tag 
              v-for="role in scope.row.roleName" 
              :key="role" 
              size="small"
              style="margin-right: 5px;"
            >
              {{ role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'">
              {{ scope.row.isActive ? '激活' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="creationTime" label="创建时间" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.creationTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button type="primary" size="small" @click="editUser(scope.row)">
              编辑
            </el-button>
            <el-button 
              type="warning" 
              size="small" 
              @click="showAssignRoles(scope.row)"
            >
              分配角色
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="totalCount"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 用户编辑对话框 -->
    <el-dialog 
      v-model="userDialogVisible" 
      :title="isEditMode ? '编辑用户' : '新增用户'"
      width="600px"
    >
      <el-form 
        ref="userFormRef" 
        :model="userForm" 
        :rules="userFormRules" 
        label-width="100px"
      >
        <el-form-item label="姓名" prop="fullName">
          <el-input v-model="userForm.fullName" />
        </el-form-item>
        <el-form-item label="用户名" prop="userName">
          <el-input v-model="userForm.userName" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="userForm.email" />
        </el-form-item>
        <el-form-item label="手机号" prop="phoneNumber">
          <el-input v-model="userForm.phoneNumber" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!isEditMode">
          <el-input v-model="userForm.password" type="password" />
        </el-form-item>
        <el-form-item label="状态" prop="isActive">
          <el-switch v-model="userForm.isActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="userDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSaveUser" :loading="saving">
          确定
        </el-button>
      </template>
    </el-dialog>

    <!-- 角色分配对话框 -->
    <el-dialog v-model="roleDialogVisible" title="分配角色" width="500px">
      <div>
        <p>为用户 <strong>{{ selectedUser?.fullName }}</strong> 分配角色：</p>
        <el-checkbox-group v-model="selectedRoleIds">
          <el-checkbox 
            v-for="role in roleList" 
            :key="role.key" 
            :value="role.key"
          >
            {{ role.value }}
          </el-checkbox>
        </el-checkbox-group>
      </div>
      <template #footer>
        <el-button @click="roleDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleAssignRoles" :loading="assigning">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, type Ref } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { apiService, UserDto } from '@/api/service'
import type { UserListDto } from '@/api/service'

// 响应式数据
const loading = ref(false)
const saving = ref(false)
const assigning = ref(false)
const userList: Ref<UserListDto[]> = ref([])
const roleList = ref<Array<{ key: string; value: string }>>([])
const totalCount = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)

// 表单相关
const userDialogVisible = ref(false)
const roleDialogVisible = ref(false)
const isEditMode = ref(false)
const userFormRef = ref<FormInstance>()
const selectedUser: Ref<UserListDto | null> = ref(null)
const selectedRoleIds = ref<string[]>([])

// 搜索表单
const searchForm = reactive({
  filterText: '',
  sorting: 'CreationTime desc'
})

// 用户表单
const userForm = reactive({
  id: '',
  fullName: '',
  userName: '',
  email: '',
  phoneNumber: '',
  password: '',
  isActive: true
})

// 表单验证规则
const userFormRules: FormRules = {
  fullName: [
    { required: true, message: '请输入姓名', trigger: 'blur' }
  ],
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度至少6位', trigger: 'blur' }
  ]
}

// 方法
const loadUsers = async () => {
  try {
    loading.value = true
    const response = await apiService.getUsers(
      searchForm.filterText || undefined,
      searchForm.sorting || undefined,
      pageSize.value,
      (currentPage.value - 1) * pageSize.value
    )
    
    if (response.data?.datas) {
      userList.value = response.data.datas.items || []
      totalCount.value = response.data.datas.totalCount || 0
    }
  } catch (error: any) {
    console.error('获取用户列表失败:', error)
    ElMessage.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

const loadRoles = async () => {
  try {
    const response = await apiService.getRoleDropDownList()
    if (response.data) {
      roleList.value = response.data.map(role => ({
        key: role.key || '',
        value: role.value || ''
      }))
    }
  } catch (error: any) {
    console.error('获取角色列表失败:', error)
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadUsers()
}

const handleReset = () => {
  searchForm.filterText = ''
  searchForm.sorting = 'CreationTime desc'
  currentPage.value = 1
  loadUsers()
}

const handleSizeChange = (size: number) => {
  pageSize.value = size
  currentPage.value = 1
  loadUsers()
}

const handleCurrentChange = (page: number) => {
  currentPage.value = page
  loadUsers()
}

const showAddDialog = () => {
  isEditMode.value = false
  userDialogVisible.value = true
  resetUserForm()
}

const editUser = async (user: UserListDto) => {
  try {
    const response = await apiService.getUser(user.id!)
    if (response.data) {
      isEditMode.value = true
      userDialogVisible.value = true
      
      // 填充表单数据
      const userData = response.data
      userForm.id = userData.id || ''
      userForm.fullName = userData.fullName || ''
      userForm.userName = userData.userName || ''
      userForm.email = userData.email || ''
      userForm.phoneNumber = userData.phoneNumber || ''
      userForm.isActive = userData.isActive || false
    }
  } catch (error: any) {
    console.error('获取用户详情失败:', error)
    ElMessage.error('获取用户详情失败')
  }
}

const handleSaveUser = async () => {
  if (!userFormRef.value) return
  
  const valid = await userFormRef.value.validate()
  if (!valid) return
  
  try {
    saving.value = true
    
    const userData = new UserDto({
      id: userForm.id || undefined,
      fullName: userForm.fullName,
      userName: userForm.userName,
      email: userForm.email,
      phoneNumber: userForm.phoneNumber,
      password: userForm.password,
      isActive: userForm.isActive,
      creationTime: new Date()
    })
    
    await apiService.createOrUpdateUser(userData)
    
    ElMessage.success(isEditMode.value ? '更新成功' : '创建成功')
    userDialogVisible.value = false
    await loadUsers()
  } catch (error: any) {
    console.error('保存用户失败:', error)
    ElMessage.error('保存用户失败')
  } finally {
    saving.value = false
  }
}

const showAssignRoles = (user: UserListDto) => {
  selectedUser.value = user
  selectedRoleIds.value = []
  roleDialogVisible.value = true
}

const handleAssignRoles = async () => {
  if (!selectedUser.value?.id) return
  
  try {
    assigning.value = true
    
    await apiService.assignRoles([selectedUser.value.id], selectedRoleIds.value)
    
    ElMessage.success('角色分配成功')
    roleDialogVisible.value = false
    await loadUsers()
  } catch (error: any) {
    console.error('分配角色失败:', error)
    ElMessage.error('分配角色失败')
  } finally {
    assigning.value = false
  }
}

const resetUserForm = () => {
  userForm.id = ''
  userForm.fullName = ''
  userForm.userName = ''
  userForm.email = ''
  userForm.phoneNumber = ''
  userForm.password = ''
  userForm.isActive = true
  
  userFormRef.value?.clearValidate()
}

const formatDate = (date: Date | string) => {
  if (!date) return ''
  return new Date(date).toLocaleString('zh-CN')
}

// 页面加载时初始化数据
onMounted(() => {
  loadUsers()
  loadRoles()
})
</script>

<style scoped>
.user-management-demo {
  padding: 20px;
}

.demo-card {
  margin-bottom: 20px;
}

.demo-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-form {
  margin-bottom: 20px;
  padding: 20px;
  background-color: #f5f5f5;
  border-radius: 4px;
}

.pagination-wrapper {
  margin-top: 20px;
  text-align: right;
}
</style>
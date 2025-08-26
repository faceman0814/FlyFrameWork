<template>
  <div class="app-container">
    <!-- 搜索表单 -->
    <el-form :model="queryParams" ref="queryRef" :inline="true" class="search-form">
      <el-form-item :label="$t('user.username')" prop="username">
        <el-input
          v-model="queryParams.username"
          :placeholder="$t('user.username')"
          clearable
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item :label="$t('user.name')" prop="name">
        <el-input
          v-model="queryParams.name"
          :placeholder="$t('user.name')"
          clearable
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item :label="$t('user.status')" prop="status">
        <el-select v-model="queryParams.status" :placeholder="$t('user.status')" clearable>
          <el-option :label="$t('user.active')" value="1" />
          <el-option :label="$t('user.inactive')" value="0" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">{{ $t('common.search') }}</el-button>
        <el-button @click="resetQuery">{{ $t('common.reset') }}</el-button>
      </el-form-item>
    </el-form>

    <!-- 操作按钮 -->
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button
          type="primary"
          plain
          @click="handleAdd"
        >
          <el-icon><Plus /></el-icon>
          {{ $t('user.add') }}
        </el-button>
      </el-col>
    </el-row>

    <!-- 用户表格 -->
    <el-table v-loading="loading" :data="userList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="$t('user.username')" align="center" prop="username" />
      <el-table-column :label="$t('user.name')" align="center" prop="name" />
      <el-table-column :label="$t('user.email')" align="center" prop="email" />
      <el-table-column :label="$t('user.phone')" align="center" prop="phone" />
      <el-table-column :label="$t('user.status')" align="center">
        <template #default="scope">
          <el-tag :type="scope.row.status === '1' ? 'success' : 'danger'">
            {{ scope.row.status === '1' ? $t('user.active') : $t('user.inactive') }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('user.createTime')" align="center" prop="createTime" width="180" />
      <el-table-column :label="$t('user.actions')" align="center" width="160">
        <template #default="scope">
          <el-button
            type="primary"
            text
            @click="handleEdit(scope.row)"
          >
            <el-icon><Edit /></el-icon>
            {{ $t('common.edit') }}
          </el-button>
          <el-button
            type="danger"
            text
            @click="handleDelete(scope.row)"
          >
            <el-icon><Delete /></el-icon>
            {{ $t('common.delete') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="getList"
    />

    <!-- 添加或修改用户对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="userRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item :label="$t('user.username')" prop="username">
          <el-input v-model="form.username" :placeholder="$t('user.username')" />
        </el-form-item>
        <el-form-item :label="$t('user.name')" prop="name">
          <el-input v-model="form.name" :placeholder="$t('user.name')" />
        </el-form-item>
        <el-form-item :label="$t('user.email')" prop="email">
          <el-input v-model="form.email" :placeholder="$t('user.email')" />
        </el-form-item>
        <el-form-item :label="$t('user.phone')" prop="phone">
          <el-input v-model="form.phone" :placeholder="$t('user.phone')" />
        </el-form-item>
        <el-form-item v-if="!form.id" :label="$t('login.password')" prop="password">
          <el-input v-model="form.password" type="password" :placeholder="$t('login.password')" />
        </el-form-item>
        <el-form-item :label="$t('user.status')" prop="status">
          <el-radio-group v-model="form.status">
            <el-radio label="1">{{ $t('user.active') }}</el-radio>
            <el-radio label="0">{{ $t('user.inactive') }}</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ $t('common.confirm') }}</el-button>
          <el-button @click="cancel">{{ $t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Edit, Delete } from '@element-plus/icons-vue'
import { getUserList, createUser, updateUser, deleteUser } from '@/api/user'
import Pagination from '@/components/Pagination/index.vue'
import type { FormInstance } from 'element-plus'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  username: '',
  name: '',
  status: ''
})

// 表格数据
const userList = ref<any[]>([])
const total = ref(0)
const loading = ref(true)
const ids = ref<string[]>([])
const single = ref(true)
const multiple = ref(true)

// 弹窗相关
const open = ref(false)
const title = ref('')
const form = reactive({
  id: undefined,
  username: '',
  name: '',
  email: '',
  phone: '',
  password: '',
  status: '1'
})

// 表单验证
const rules = reactive({
  username: [{ required: true, message: 'Username is required', trigger: 'blur' }],
  name: [{ required: true, message: 'Name is required', trigger: 'blur' }],
  email: [
    { required: true, message: 'Email is required', trigger: 'blur' },
    { type: 'email', message: 'Please enter a valid email', trigger: 'blur' }
  ],
  password: [{ required: true, message: 'Password is required', trigger: 'blur' }]
})

const queryRef = ref<FormInstance>()
const userRef = ref<FormInstance>()

/** 查询用户列表 */
const getList = async () => {
  loading.value = true
  try {
    const response = await getUserList(queryParams)
    userList.value = response.data.items || mockUserData
    total.value = response.data.total || mockUserData.length
  } catch (error) {
    // 模拟数据，实际项目中应该处理错误
    userList.value = mockUserData
    total.value = mockUserData.length
  } finally {
    loading.value = false
  }
}

// 模拟数据
const mockUserData = [
  { id: 1, username: 'admin', name: '管理员', email: 'admin@example.com', phone: '13800138000', status: '1', createTime: '2023-01-01 10:00:00' },
  { id: 2, username: 'editor', name: '编辑者', email: 'editor@example.com', phone: '13800138001', status: '1', createTime: '2023-01-02 10:00:00' },
  { id: 3, username: 'user', name: '普通用户', email: 'user@example.com', phone: '13800138002', status: '0', createTime: '2023-01-03 10:00:00' }
]

/** 搜索按钮操作 */
const handleQuery = () => {
  queryParams.pageNum = 1
  getList()
}

/** 重置按钮操作 */
const resetQuery = () => {
  queryRef.value?.resetFields()
  handleQuery()
}

/** 多选框选中数据 */
const handleSelectionChange = (selection: any[]) => {
  ids.value = selection.map(item => item.id)
  single.value = selection.length !== 1
  multiple.value = !selection.length
}

/** 新增按钮操作 */
const handleAdd = () => {
  reset()
  open.value = true
  title.value = t('user.add')
}

/** 修改按钮操作 */
const handleEdit = (row: any) => {
  reset()
  const userId = row.id
  Object.assign(form, row)
  open.value = true
  title.value = t('user.edit')
}

/** 提交按钮 */
const submitForm = () => {
  userRef.value?.validate(async (valid: boolean) => {
    if (valid) {
      if (form.id) {
        await updateUser(form.id, form)
        ElMessage.success('修改成功')
      } else {
        await createUser(form)
        ElMessage.success('新增成功')
      }
      open.value = false
      getList()
    }
  })
}

/** 删除按钮操作 */
const handleDelete = (row: any) => {
  const userIds = row.id || ids.value
  ElMessageBox.confirm(
    t('user.deleteConfirm'),
    'Warning',
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    await deleteUser(userIds)
    getList()
    ElMessage.success('删除成功')
  })
}

/** 取消按钮 */
const cancel = () => {
  open.value = false
  reset()
}

/** 表单重置 */
const reset = () => {
  form.id = undefined
  form.username = ''
  form.name = ''
  form.email = ''
  form.phone = ''
  form.password = ''
  form.status = '1'
  userRef.value?.resetFields()
}

onMounted(() => {
  getList()
})
</script>

<style lang="scss" scoped>
.search-form {
  padding: 20px;
  background: #fff;
  border-radius: 4px;
  margin-bottom: 20px;
}

.mb8 {
  margin-bottom: 8px;
}
</style>
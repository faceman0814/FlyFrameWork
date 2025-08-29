<template>
  <div class="app-container">
    <CommonTable
      page-key="system-user"
      :data="userList"
      :columns="tableColumns"
      :loading="loading"
      :total="total"
      :search-config="searchConfig"
      :operations="operations"
      :table-title="t('user.title')"
      show-add
      show-batch-delete
      show-export
      @load-data="loadData"
      @add="handleAdd"
      @search="handleSearch"
      @reset="handleReset"
      @batch-delete="handleBatchDelete"
      @export="handleExport"
      @operation="handleOperation"
      @switch-change="handleSwitchChange"
    >
      <!-- 自定义用户信息列 -->
      <template #userInfo="slotProps: any">
        <div class="user-info">
          <el-avatar :size="32" :src="slotProps.row.avatar" class="user-avatar">
            {{ slotProps.row.userName?.charAt(0)?.toUpperCase() }}
          </el-avatar>
          <span class="username">{{ slotProps.row.userName }}</span>
        </div>
      </template>
    </CommonTable>

    <!-- 添加或修改用户对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="userRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item :label="$t('user.username')" prop="userName">
          <el-input v-model="form.userName" :placeholder="$t('user.username')" />
        </el-form-item>
        <el-form-item :label="$t('user.name')" prop="fullName">
          <el-input v-model="form.fullName" :placeholder="$t('user.name')" />
        </el-form-item>
        <el-form-item :label="$t('user.email')" prop="email">
          <el-input v-model="form.email" :placeholder="$t('user.email')" />
        </el-form-item>
        <el-form-item :label="$t('user.phone')" prop="phoneNumber">
          <el-input v-model="form.phoneNumber" :placeholder="$t('user.phone')" />
        </el-form-item>
        <el-form-item v-if="!form.id" :label="$t('login.password')" prop="password">
          <el-input v-model="form.password" type="password" :placeholder="$t('login.password')" />
        </el-form-item>
        <el-form-item :label="$t('user.status')" prop="isActive">
          <el-radio-group v-model="form.isActive">
            <el-radio :label="true">{{ $t('user.active') }}</el-radio>
            <el-radio :label="false">{{ $t('user.inactive') }}</el-radio>
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
import { Edit, Delete, View, RefreshRight } from '@element-plus/icons-vue'
import { UserServiceProxy, GetUsersInput, CreateOrUpdateUserInput, UserDto } from '@/api/service-proxies'
import { CommonTable } from '@/components/CommonTable'
import type { TableColumn, SearchConfig, TableOperation } from '@/components/CommonTable'
import type { FormInstance } from 'element-plus'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// NSwag生成的用户服务代理
const userService = new UserServiceProxy()

// 数据状态
const userList = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 表单状态
const open = ref(false)
const title = ref('')
const isEdit = ref(false)
const form = reactive({
  id: '',
  userName: '',
  fullName: '',
  email: '',
  phoneNumber: '',
  password: '',
  isActive: true
})

// 表单验证
const rules = reactive({
  userName: [{ required: true, message: () => t('user.validation.usernameRequired'), trigger: 'blur' }],
  fullName: [{ required: true, message: () => t('user.validation.nameRequired'), trigger: 'blur' }],
  email: [
    { required: true, message: () => t('user.validation.emailRequired'), trigger: 'blur' },
    { type: 'email' as const, message: () => t('user.validation.emailFormat'), trigger: 'blur' }
  ],
  password: [{ required: true, message: () => t('user.validation.passwordRequired'), trigger: 'blur' }]
})

const userRef = ref<FormInstance>()

// 表格列配置
const tableColumns: TableColumn[] = [
  {
    prop: 'userName',
    label: t('user.username'),
    width: '150',
    slot: 'userInfo'
  },
  {
    prop: 'fullName',
    label: t('user.name'),
    minWidth: '120',
    showOverflowTooltip: true
  },
  {
    prop: 'email',
    label: t('user.email'),
    minWidth: '180',
    render: 'link',
    linkType: 'primary',
    linkHref: (row) => `mailto:${row.email}`
  },
  {
    prop: 'phoneNumber',
    label: t('user.phone'),
    width: '120'
  },
  {
    prop: 'isActive',
    label: t('user.status'),
    width: '80',
    align: 'center',
    render: 'tag',
    tagMap: {
      true: { type: 'success', text: t('user.active') },
      false: { type: 'danger', text: t('user.inactive') }
    }
  },
  {
    prop: 'creationTime',
    label: t('user.createdAt'),
    width: '160',
    render: 'date',
    dateFormat: 'YYYY-MM-DD HH:mm'
  }
]

// 搜索配置
const searchConfig: SearchConfig[] = [
  {
    prop: 'filterText',
    label: t('common.keyword'),
    type: 'input',
    placeholder: t('user.searchPlaceholder')
  },
  {
    prop: 'isActive',
    label: t('user.status'),
    type: 'select',
    options: [
      { label: t('common.all'), value: '' },
      { label: t('user.active'), value: true },
      { label: t('user.inactive'), value: false }
    ]
  }
]

// 操作配置
const operations: TableOperation[] = [
  {
    key: 'edit',
    label: t('common.edit'),
    type: 'primary',
    icon: Edit,
    handler: handleEdit
  },
  {
    key: 'view',
    label: t('user.viewDetails'),
    type: 'info',
    icon: View,
    isMore: true,
    handler: () => {
      ElMessage.info(`${t('user.viewDetails')}`)
    }
  },
  {
    key: 'resetPassword',
    label: t('user.resetPassword'),
    type: 'warning',
    icon: RefreshRight,
    isMore: true,
    handler: (row) => {
      ElMessageBox.confirm(
        t('user.resetPasswordConfirm'),
        t('common.warning'),
        { type: 'warning' }
      ).then(() => {
        ElMessage.success(t('user.resetPasswordSuccess'))
      })
    }
  },
  {
    key: 'delete',
    label: t('common.delete'),
    type: 'danger',
    icon: Delete,
    handler: (row) => handleDelete(row)
  }
]

// 数据加载方法
const loadData = async (params: any) => {
  loading.value = true
  try {
    const input = new GetUsersInput({
      filterText: params.filterText || '',
      sorting: '',
      maxResultCount: params.pageSize || 20,
      skipCount: ((params.page || 1) - 1) * (params.pageSize || 20)
    })

    const response = await userService.getPaged(input)

    if (response.success && response.data) {
      userList.value = response.data.items || []
      total.value = response.data.totalCount || 0
    } else {
      userList.value = []
      total.value = 0
    }
  } catch (error) {
    console.error('获取用户列表失败:', error)
    userList.value = []
    total.value = 0
    ElMessage.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

// 事件处理方法
const handleSearch = (params: any) => {
  console.log('搜索参数:', params)
}

const handleReset = () => {
  console.log('重置搜索')
}

const handleAdd = () => {
  reset()
  isEdit.value = false
  open.value = true
  title.value = t('user.add')
}

function handleEdit(row: any) {
  reset()
  isEdit.value = true

  // 填充表单数据
  Object.assign(form, {
    id: row.id || '',
    userName: row.userName || '',
    fullName: row.fullName || '',
    email: row.email || '',
    phoneNumber: row.phoneNumber || '',
    isActive: row.isActive !== false
  })

  open.value = true
  title.value = t('user.edit')
}

const handleDelete = (row: any) => {
  const userId = row.id
  ElMessageBox.confirm(
    t('user.deleteConfirm'),
    t('common.warning'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    try {
      // TODO: 调用删除API
      console.log('删除用户:', userId)
      ElMessage.success(t('user.deleteSuccess'))
      loadData({ page: 1, pageSize: 20 })
    } catch (error) {
      ElMessage.error('删除用户失败')
    }
  })
}

const handleBatchDelete = (selectedRows: any[]) => {
  ElMessageBox.confirm(
    `确定要删除选中的 ${selectedRows.length} 个用户吗？`,
    t('common.warning'),
    { type: 'warning' }
  ).then(() => {
    ElMessage.success('批量删除成功')
    loadData({ page: 1, pageSize: 20 })
  })
}

const handleExport = () => {
  ElMessage.info('正在导出用户数据...')
}

const handleOperation = (key: string, row: any) => {
  console.log('自定义操作:', key, row)
}

const handleSwitchChange = (value: boolean, row: any) => {
  ElMessage.success(`用户 ${row.fullName} 状态已${value ? '启用' : '禁用'}`)
}

// 表单操作
const submitForm = () => {
  userRef.value?.validate(async (valid: boolean) => {
    if (valid) {
      try {
        const userDto = new UserDto({
          id: isEdit.value ? form.id : undefined,
          userName: form.userName,
          fullName: form.fullName,
          email: form.email,
          phoneNumber: form.phoneNumber,
          password: form.password,
          isActive: form.isActive,
          creationTime: new Date()
        })

        const input = new CreateOrUpdateUserInput({
          user: userDto
        })

        await userService.createOrUpdate(input)

        ElMessage.success(isEdit.value ? t('user.updateSuccess') : t('user.createSuccess'))
        open.value = false
        loadData({ page: 1, pageSize: 20 })
      } catch (error) {
        console.error('保存用户失败:', error)
        ElMessage.error('保存用户失败')
      }
    }
  })
}

const cancel = () => {
  open.value = false
  reset()
}

const reset = () => {
  isEdit.value = false
  Object.assign(form, {
    id: '',
    userName: '',
    fullName: '',
    email: '',
    phoneNumber: '',
    password: '',
    isActive: true
  })
  userRef.value?.resetFields()
}

onMounted(() => {
  loadData({ page: 1, pageSize: 20 })
})
</script>

<style lang="scss" scoped>
.app-container {
  padding: 20px;
  background: transparent;
  min-height: 100vh;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;

  .user-avatar {
    background: var(--primary-gradient);
    color: var(--text-white);
    font-weight: 600;
    flex-shrink: 0;
  }

  .username {
    font-weight: 500;
    color: var(--text-primary);
  }
}

// 对话框样式优化
:deep(.el-dialog) {
  background: var(--bg-card);
  border-radius: 12px;
  border: 1px solid var(--border-light);
  
  .el-dialog__header {
    background: var(--bg-card);
    border-bottom: 1px solid var(--border-light);
    
    .el-dialog__title {
      color: var(--text-primary);
    }
    
    .el-dialog__headerbtn {
      .el-dialog__close {
        color: var(--text-regular);
        
        &:hover {
          color: var(--primary-color);
        }
      }
    }
  }
  
  .el-dialog__body {
    background: var(--bg-card);
    color: var(--text-primary);
  }
  
  .el-dialog__footer {
    background: var(--bg-card);
    border-top: 1px solid var(--border-light);
  }
}

// 表单样式优化
:deep(.el-form) {
  .el-form-item__label {
    color: var(--text-primary);
  }
  
  .el-input {
    .el-input__wrapper {
      background: var(--bg-card);
      border-color: var(--border-light);
      
      &:hover {
        border-color: var(--primary-color);
      }
      
      &.is-focus {
        border-color: var(--primary-color);
        box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
      }
    }
    
    .el-input__inner {
      color: var(--text-primary);
      
      &::placeholder {
        color: var(--text-placeholder);
      }
    }
  }
  
  .el-textarea {
    .el-textarea__inner {
      background: var(--bg-card);
      border-color: var(--border-light);
      color: var(--text-primary);
      
      &:hover {
        border-color: var(--primary-color);
      }
      
      &:focus {
        border-color: var(--primary-color);
        box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
      }
      
      &::placeholder {
        color: var(--text-placeholder);
      }
    }
  }
  
  .el-radio-group {
    .el-radio {
      color: var(--text-primary);
      
      .el-radio__input {
        &.is-checked {
          .el-radio__inner {
            background-color: var(--primary-color);
            border-color: var(--primary-color);
          }
        }
      }
      
      .el-radio__inner {
        background: var(--bg-card);
        border-color: var(--border-light);
        
        &:hover {
          border-color: var(--primary-color);
        }
      }
    }
  }
}

// 按钮样式优化
:deep(.el-button) {
  &.el-button--primary {
    background: var(--primary-gradient);
    border: none;
    color: var(--text-white);
    
    &:hover {
      opacity: 0.9;
      transform: translateY(-1px);
    }
  }
  
  &.el-button--default {
    background: var(--bg-card);
    border-color: var(--border-light);
    color: var(--text-primary);
    
    &:hover {
      color: var(--primary-color);
      border-color: var(--primary-color);
      background: var(--bg-hover);
    }
  }
}

// 消息框样式
:deep(.el-message-box) {
  background: var(--bg-card);
  border: 1px solid var(--border-light);
  border-radius: 12px;
  
  .el-message-box__header {
    .el-message-box__title {
      color: var(--text-primary);
    }
    
    .el-message-box__close {
      color: var(--text-regular);
      
      &:hover {
        color: var(--primary-color);
      }
    }
  }
  
  .el-message-box__content {
    .el-message-box__message {
      color: var(--text-primary);
    }
  }
}

// 响应式优化
@media (max-width: 768px) {
  .app-container {
    padding: 10px;
  }
}
</style>
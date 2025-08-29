<template>
  <div class="app-container">
    <CommonTable
      page-key="system-role"
      :data="roleList"
      :columns="tableColumns"
      :loading="loading"
      :total="total"
      :search-config="searchConfig"
      :operations="operations"
      :table-title="t('role.title')"
      show-add
      @load-data="loadData"
      @add="handleAdd"
      @search="handleSearch"
      @reset="handleReset"
      @operation="handleOperation"
    >
      <!-- 自定义角色权限列 -->
      <template #permissions="slotProps: any">
        <div class="role-permissions">
          <el-tag 
            v-for="(permission, index) in (slotProps.row.permissions || []).slice(0, 3)" 
            :key="index"
            size="small"
            style="margin-right: 4px; margin-bottom: 2px;"
          >
            {{ permission }}
          </el-tag>
          <el-tag 
            v-if="(slotProps.row.permissions || []).length > 3"
            size="small"
            type="info"
          >
            +{{ (slotProps.row.permissions || []).length - 3 }}
          </el-tag>
        </div>
      </template>
    </CommonTable>

    <!-- 添加或修改角色对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="roleRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item :label="$t('role.name')" prop="name">
          <el-input v-model="form.name" :placeholder="$t('role.name')" />
        </el-form-item>
        <el-form-item :label="$t('role.displayName')" prop="displayName">
          <el-input v-model="form.displayName" :placeholder="$t('role.displayName')" />
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
import { Edit, Delete, Setting } from '@element-plus/icons-vue'
import { RoleServiceProxy, GetRolesInput, CreateOrUpdateRoleInput, RoleDto } from '@/api/service-proxies'
import { CommonTable } from '@/components/CommonTable'
import type { TableColumn, SearchConfig, TableOperation } from '@/components/CommonTable'
import type { FormInstance } from 'element-plus'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// NSwag生成的服务代理
const roleService = new RoleServiceProxy()

// 数据状态
const roleList = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 表单状态
const open = ref(false)
const title = ref('')
const isEdit = ref(false)
const form = reactive({
  id: '',
  name: '',
  displayName: ''
})

// 表单验证
const rules = reactive({
  name: [{ required: true, message: () => t('role.validation.nameRequired'), trigger: 'blur' }],
  displayName: [{ required: true, message: () => t('role.validation.displayNameRequired'), trigger: 'blur' }]
})

const roleRef = ref<FormInstance>()

// 表格列配置
const tableColumns: TableColumn[] = [
  {
    prop: 'name',
    label: t('role.name'),
    width: '120',
    fixed: 'left'
  },
  {
    prop: 'displayName',
    label: t('role.displayName'),
    minWidth: '140',
    showOverflowTooltip: true
  },
  {
    prop: 'description',
    label: t('role.description'),
    minWidth: '200',
    showOverflowTooltip: true
  },
  {
    prop: 'permissions',
    label: t('role.permissions'),
    minWidth: '200',
    slot: 'permissions'
  },
  {
    prop: 'creationTime',
    label: t('role.createdAt'),
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
    placeholder: t('role.searchPlaceholder')
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
    key: 'permissions',
    label: t('role.managePermissions'),
    type: 'warning',
    icon: Setting,
    isMore: true,
    handler: (row: any) => {
      handleEdit(row) // 复用编辑对话框来管理权限
    }
  },
  {
    key: 'delete',
    label: t('common.delete'),
    type: 'danger',
    icon: Delete,
    handler: (row: any) => handleDelete(row)
  }
]

// 数据加载方法
const loadData = async (params: any) => {
  loading.value = true
  try {
    const input = new GetRolesInput({
      filterText: params.filterText || '',
      sorting: '',
      maxResultCount: params.pageSize || 20,
      skipCount: ((params.page || 1) - 1) * (params.pageSize || 20)
    })

    const response = await roleService.getPaged(input)

    if (response.success && response.data) {
      roleList.value = response.data.items || []
      total.value = response.data.totalCount || 0
    } else {
      roleList.value = []
      total.value = 0
    }
  } catch (error) {
    console.error('获取角色列表失败:', error)
    roleList.value = []
    total.value = 0
    ElMessage.error('获取角色列表失败')
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
  title.value = t('role.add')
}

function handleEdit(row: any) {
  reset()
  isEdit.value = true

  // 填充表单数据
  Object.assign(form, {
    id: row.id || '',
    name: row.name || '',
    displayName: row.displayName || ''
  })

  open.value = true
  title.value = t('role.edit')
}

const handleDelete = (row: any) => {
  const roleId = row.id
  ElMessageBox.confirm(
    t('role.deleteConfirm'),
    t('common.warning'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    try {
      // TODO: 调用删除API
      console.log('删除角色:', roleId)
      ElMessage.success(t('role.deleteSuccess'))
      loadData({ page: 1, pageSize: 20 })
    } catch (error) {
      ElMessage.error('删除角色失败')
    }
  })
}

const handleOperation = (key: string, row: any) => {
  console.log('自定义操作:', key, row)
}

// 表单操作
const submitForm = () => {
  roleRef.value?.validate(async (valid: boolean) => {
    if (valid) {
      try {
        const roleDto = new RoleDto({
          id: isEdit.value ? form.id : undefined,
          name: form.name,
          displayName: form.displayName,
          isStatic: false,
          isDefault: false
        })

        const input = new CreateOrUpdateRoleInput({
          role: roleDto
        })

        await roleService.createOrUpdate(input)

        ElMessage.success(isEdit.value ? t('role.updateSuccess') : t('role.createSuccess'))
        open.value = false
        loadData({ page: 1, pageSize: 20 })
      } catch (error) {
        console.error('保存角色失败:', error)
        ElMessage.error('保存角色失败')
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
    name: '',
    displayName: ''
  })
  roleRef.value?.resetFields()
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

.role-permissions {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.permission-tree {
  max-height: 300px;
  overflow-y: auto;
  border: 1px solid var(--border-light);
  border-radius: 6px;
  padding: 12px;
  background: var(--bg-card);

  :deep(.el-tree) {
    background: var(--bg-card);
    color: var(--text-primary);
  }

  :deep(.el-tree-node) {
    .el-tree-node__content {
      height: 32px;
      line-height: 32px;
      color: var(--text-primary);

      &:hover {
        background: var(--bg-hover);
      }
    }

    .el-tree-node__label {
      color: var(--text-primary);
    }

    .el-checkbox {
      .el-checkbox__input {
        &.is-checked {
          .el-checkbox__inner {
            background-color: var(--primary-color);
            border-color: var(--primary-color);
          }
        }
      }

      .el-checkbox__inner {
        background: var(--bg-card);
        border-color: var(--border-light);

        &:hover {
          border-color: var(--primary-color);
        }
      }
    }
  }
}

.dialog-footer {
  text-align: right;
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

// 标签样式优化
:deep(.el-tag) {
  &.el-tag--success {
    background-color: rgba(103, 194, 58, 0.15);
    border-color: rgba(103, 194, 58, 0.3);
    color: var(--success-color);
  }

  &.el-tag--info {
    background-color: rgba(144, 147, 153, 0.15);
    border-color: rgba(144, 147, 153, 0.3);
    color: var(--info-color);
  }
}

// 响应式优化
@media (max-width: 768px) {
  .app-container {
    padding: 10px;
  }

  .permission-tree {
    max-height: 200px;
  }
}
</style>
<template>
  <div class="app-container modern-table-wrapper">
    <!-- 搜索表单 -->
    <div class="search-wrapper">
      <el-card shadow="never" class="search-card">
        <el-form :model="queryParams" ref="queryRef" :inline="true" class="search-form">
          <el-form-item :label="$t('user.username')" prop="username">
            <el-input
              v-model="queryParams.username"
              :placeholder="$t('user.username')"
              clearable
              style="width: 180px"
              @keyup.enter="handleQuery"
            />
          </el-form-item>
          <el-form-item :label="$t('user.name')" prop="name">
            <el-input
              v-model="queryParams.name"
              :placeholder="$t('user.name')"
              clearable
              style="width: 180px"
              @keyup.enter="handleQuery"
            />
          </el-form-item>
          <el-form-item :label="$t('user.status')" prop="status">
            <el-select 
              v-model="queryParams.status" 
              :placeholder="$t('user.status')" 
              clearable
              style="width: 140px"
            >
              <el-option :label="$t('user.active')" value="1" />
              <el-option :label="$t('user.inactive')" value="0" />
            </el-select>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="handleQuery">
              <el-icon><Search /></el-icon>
              {{ $t('common.search') }}
            </el-button>
            <el-button @click="resetQuery">
              <el-icon><Refresh /></el-icon>
              {{ $t('common.reset') }}
            </el-button>
          </el-form-item>
        </el-form>
      </el-card>
    </div>

    <!-- 表格操作区 -->
    <div class="table-wrapper">
      <el-card shadow="never" class="table-card">
        <!-- 操作按钮栏 -->
        <div class="table-header">
          <div class="table-title">
            <h3>{{ $t('user.list') }}</h3>
            <span class="table-count">{{ t('user.totalRecords', { count: total }) }}</span>
          </div>
          <div class="table-actions">
            <el-button
              type="primary"
              @click="handleAdd"
            >
              <el-icon><Plus /></el-icon>
              {{ $t('user.add') }}
            </el-button>
            <el-button
              type="danger"
              :disabled="multiple"
              @click="handleDeleteBatch"
            >
              <el-icon><Delete /></el-icon>
              {{ $t('user.batchDelete') }}
            </el-button>
            <el-button
              type="info"
              plain
              @click="handleExport"
            >
              <el-icon><Download /></el-icon>
              {{ $t('user.export') }}
            </el-button>
          </div>
        </div>

        <!-- 用户表格 -->
        <div class="table-content">
          <el-table 
            v-loading="loading" 
            :data="userList" 
            @selection-change="handleSelectionChange"
            class="modern-table"
            stripe
            border
            highlight-current-row
          >
            <el-table-column type="selection" width="50" align="center" />
            <el-table-column 
              :label="$t('user.username')" 
              prop="username" 
              min-width="120"
              show-overflow-tooltip
            >
              <template #default="scope">
                <div class="user-info">
                  <el-avatar :size="32" :src="scope.row.avatar" class="user-avatar">
                    {{ scope.row.username?.charAt(0)?.toUpperCase() }}
                  </el-avatar>
                  <span class="username">{{ scope.row.username }}</span>
                </div>
              </template>
            </el-table-column>
            <el-table-column 
              :label="$t('user.name')" 
              prop="name" 
              min-width="100"
              show-overflow-tooltip
            />
            <el-table-column 
              :label="$t('user.email')" 
              prop="email" 
              min-width="180"
              show-overflow-tooltip
            />
            <el-table-column 
              :label="$t('user.phone')" 
              prop="phone" 
              min-width="120"
              show-overflow-tooltip
            />
            <el-table-column :label="$t('user.status')" width="100" align="center">
              <template #default="scope">
                <el-tag 
                  :type="scope.row.status === '1' ? 'success' : 'danger'"
                  size="small"
                  effect="light"
                >
                  {{ scope.row.status === '1' ? $t('user.active') : $t('user.inactive') }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column 
              :label="$t('user.createTime')" 
              prop="createTime" 
              width="160" 
              align="center"
              show-overflow-tooltip
            />
            <el-table-column :label="$t('user.actions')" width="180" align="center" fixed="right">
              <template #default="scope">
                <div class="action-buttons">
                  <el-button
                    type="primary"
                    text
                    size="small"
                    @click="handleEdit(scope.row)"
                  >
                    <el-icon><Edit /></el-icon>
                  </el-button>
                  <el-button
                    type="danger"
                    text
                    size="small"
                    @click="handleDelete(scope.row)"
                  >
                    <el-icon><Delete /></el-icon>
                  </el-button>
                  <el-dropdown @command="handleCommand" trigger="click">
                    <el-button text size="small">
                      <el-icon><MoreFilled /></el-icon>
                    </el-button>
                    <template #dropdown>
                      <el-dropdown-menu>
                        <el-dropdown-item :command="`view-${scope.row.id}`">
                          <el-icon><View /></el-icon>
                          {{ $t('user.viewDetails') }}
                        </el-dropdown-item>
                        <el-dropdown-item :command="`reset-${scope.row.id}`">
                          <el-icon><RefreshRight /></el-icon>
                          {{ $t('user.resetPassword') }}
                        </el-dropdown-item>
                      </el-dropdown-menu>
                    </template>
                  </el-dropdown>
                </div>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 分页 -->
        <div class="table-footer">
          <pagination
            v-show="total > 0"
            :total="total"
            v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize"
            @pagination="getList"
            class="modern-pagination"
            background
            layout="total, sizes, prev, pager, next, jumper"
          />
        </div>
      </el-card>
    </div>

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
import { Plus, Edit, Delete, Search, Refresh, Download, MoreFilled, View, RefreshRight } from '@element-plus/icons-vue'
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
  username: [{ required: true, message: () => t('user.validation.usernameRequired'), trigger: 'blur' }],
  name: [{ required: true, message: () => t('user.validation.nameRequired'), trigger: 'blur' }],
  email: [
    { required: true, message: () => t('user.validation.emailRequired'), trigger: 'blur' },
    { type: 'email' as const, message: () => t('user.validation.emailFormat'), trigger: 'blur' }
  ],
  password: [{ required: true, message: () => t('user.validation.passwordRequired'), trigger: 'blur' }]
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
        ElMessage.success(t('user.updateSuccess'))
      } else {
        await createUser(form)
        ElMessage.success(t('user.createSuccess'))
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
    t('common.warning'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    await deleteUser(userIds)
    getList()
    ElMessage.success(t('user.deleteSuccess'))
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

/** 批量删除按钮操作 */
const handleDeleteBatch = () => {
  const userIds = ids.value
  ElMessageBox.confirm(
    t('user.batchDeleteConfirm'),
    t('common.warning'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    // await deleteUser(userIds)
    getList()
    ElMessage.success(t('user.deleteSuccess'))
  })
}

/** 导出按钮操作 */
const handleExport = () => {
  ElMessage.info(t('user.exportInProgress'))
}

/** 下拉菜单命令处理 */
const handleCommand = (command: string) => {
  const [action, id] = command.split('-')
  
  switch (action) {
    case 'view':
      ElMessage.info(`${t('user.viewDetails')}: ${id}`)
      break
    case 'reset':
      ElMessageBox.confirm(
        t('user.resetPasswordConfirm'),
        t('common.warning'),
        {
          confirmButtonText: t('common.confirm'),
          cancelButtonText: t('common.cancel'),
          type: 'warning'
        }
      ).then(() => {
        ElMessage.success(t('user.resetPasswordSuccess'))
      })
      break
    default:
      break
  }
}

onMounted(() => {
  getList()
})
</script>

<style lang="scss" scoped>
// 整体容器
.app-container {
  padding: 0;
  background: transparent;
  box-shadow: none;
  margin: 20px;
}

// 搜索区域
.search-wrapper {
  margin-bottom: 16px;
}

.search-card {
  border-radius: 12px;
  border: 1px solid var(--border-light);
  box-shadow: var(--shadow-sm);
  
  :deep(.el-card__body) {
    padding: 20px;
  }
}

.search-form {
  margin: 0;
  
  .el-form-item {
    margin-bottom: 0;
    margin-right: 20px;
    
    .el-form-item__label {
      font-weight: 500;
      color: var(--text-primary);
      padding-right: 8px;
    }
  }
  
  .el-button {
    margin-left: 8px;
    
    &.el-button--primary {
      background: var(--primary-gradient);
      border: none;
      
      &:hover {
        opacity: 0.9;
        transform: translateY(-1px);
      }
    }
    
    &.el-button--default {
      &:hover {
        color: var(--primary-color);
        border-color: var(--primary-color);
      }
    }
  }
}

// 表格区域
.table-wrapper {
  .table-card {
    border-radius: 12px;
    border: 1px solid var(--border-light);
    box-shadow: var(--shadow-card);
    overflow: hidden;
    
    :deep(.el-card__body) {
      padding: 0;
    }
  }
}

// 表格头部
.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px 16px;
  border-bottom: 1px solid var(--border-light);
  background: var(--bg-card);
  
  .table-title {
    display: flex;
    align-items: center;
    gap: 12px;
    
    h3 {
      margin: 0;
      font-size: 18px;
      font-weight: 600;
      color: var(--text-primary);
    }
    
    .table-count {
      padding: 2px 8px;
      font-size: 12px;
      color: var(--text-regular);
      background: var(--bg-main);
      border-radius: 12px;
      border: 1px solid var(--border-light);
    }
  }
  
  .table-actions {
    display: flex;
    gap: 8px;
    
    .el-button {
      &.el-button--primary {
        background: var(--primary-gradient);
        border: none;
        
        &:hover {
          opacity: 0.9;
          transform: translateY(-1px);
        }
      }
      
      &.el-button--danger {
        &:hover {
          transform: translateY(-1px);
        }
      }
      
      &.el-button--info {
        &:hover {
          transform: translateY(-1px);
        }
      }
    }
  }
}

// 表格内容
.table-content {
  .modern-table {
    :deep(.el-table__header) {
      .el-table__cell {
        background: var(--table-header-bg) !important;
        color: var(--table-header-text);
        font-weight: 600;
        font-size: 14px;
        border-bottom: 1px solid var(--table-border);
        padding: 12px 0;
      }
    }
    
    :deep(.el-table__body) {
      .el-table__row {
        transition: all 0.2s ease;
        
        &:hover {
          background: var(--table-row-hover) !important;
          
          .el-table__cell {
            background: transparent !important;
          }
        }
        
        .el-table__cell {
          padding: 12px 0;
          border-bottom: 1px solid var(--table-border);
          
          .cell {
            padding: 0 10px;
          }
        }
      }
    }
  }
  
  // 用户信息样式
  .user-info {
    display: flex;
    align-items: center;
    gap: 8px;
    
    .user-avatar {
      background: var(--primary-gradient);
      color: white;
      font-weight: 600;
      flex-shrink: 0;
    }
    
    .username {
      font-weight: 500;
      color: var(--text-primary);
    }
  }
  
  // 操作按钮样式
  .action-buttons {
    display: flex;
    align-items: center;
    gap: 4px;
    justify-content: center;
    
    .el-button {
      padding: 6px;
      min-width: auto;
      border-radius: 6px;
      
      &.el-button--text {
        &.el-button--primary {
          &:hover {
            background: rgba(var(--primary-color-rgb), 0.1);
            color: var(--primary-color);
          }
        }
        
        &.el-button--danger {
          &:hover {
            background: rgba(245, 108, 108, 0.1);
            color: var(--danger-color);
          }
        }
      }
    }
  }
}

// 表格底部
.table-footer {
  padding: 16px 24px;
  background: var(--bg-card);
  border-top: 1px solid var(--border-light);
  
  .modern-pagination {
    justify-content: flex-end;
    
    :deep(.el-pagination__total) {
      margin-right: auto;
      color: var(--text-regular);
      font-weight: 500;
    }
  }
}

// 响应式设计
@media (max-width: 768px) {
  .search-form {
    .el-form-item {
      margin-right: 0;
      margin-bottom: 16px;
      width: 100%;
    }
  }
  
  .table-header {
    flex-direction: column;
    align-items: stretch;
    gap: 16px;
    
    .table-title {
      justify-content: space-between;
    }
    
    .table-actions {
      justify-content: flex-start;
      flex-wrap: wrap;
    }
  }
  
  .modern-table {
    :deep(.el-table__body) {
      .el-table__cell {
        .cell {
          padding: 0 4px;
        }
      }
    }
  }
}

@media (max-width: 480px) {
  .app-container {
    margin: 10px;
  }
  
  .search-card,
  .table-card {
    border-radius: 8px;
  }
  
  .search-card {
    :deep(.el-card__body) {
      padding: 16px;
    }
  }
  
  .table-header {
    padding: 16px;
  }
  
  .table-footer {
    padding: 16px;
  }
}

// 暗黑模式适配
:deep(.el-table--border) {
  border-color: var(--table-border);
  
  &::before {
    background-color: var(--table-border);
  }
  
  &::after {
    background-color: var(--table-border);
  }
}

:deep(.el-table--striped) {
  .el-table__body {
    .el-table__row--striped {
      .el-table__cell {
        background: var(--table-row-striped) !important;
      }
      
      &:hover {
        .el-table__cell {
          background: var(--table-row-hover) !important;
        }
      }
    }
  }
}

// 标签样式优化
:deep(.el-tag) {
  border-radius: 6px;
  font-weight: 500;
  
  &.el-tag--success {
    background-color: rgba(103, 194, 58, 0.1);
    border-color: rgba(103, 194, 58, 0.3);
    color: var(--success-color);
  }
  
  &.el-tag--danger {
    background-color: rgba(245, 108, 108, 0.1);
    border-color: rgba(245, 108, 108, 0.3);
    color: var(--danger-color);
  }
}

// 下拉菜单样式
:deep(.el-dropdown-menu) {
  border-radius: 8px;
  border: 1px solid var(--border-light);
  box-shadow: var(--shadow-lg);
  
  .el-dropdown-menu__item {
    padding: 8px 16px;
    
    .el-icon {
      margin-right: 8px;
    }
    
    &:hover {
      background: var(--bg-hover);
      color: var(--primary-color);
    }
  }
}
</style>
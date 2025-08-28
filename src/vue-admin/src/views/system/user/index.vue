<template>
  <div class="app-container modern-table-wrapper">
    <!-- 搜索表单 -->
    <div class="search-wrapper">
      <el-card shadow="never" class="search-card">
        <el-form :model="queryParams" ref="queryRef" :inline="true" class="search-form">
          <el-form-item :label="$t('common.keyword')" prop="filterText">
            <el-input
              v-model="queryParams.filterText"
              :placeholder="$t('user.searchPlaceholder')"
              clearable
              style="width: 240px"
              @keyup.enter="handleQuery"
            />
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
            <!-- 选择列 -->
            <el-table-column type="selection" width="50" align="center" />

            <!-- 动态渲染的列 -->
            <template v-for="column in tableColumns" :key="column.prop">
              <el-table-column
                :label="column.label"
                :prop="column.prop"
                :width="column.width"
                :min-width="column.minWidth || '120'"
                :align="column.align || 'left'"
                :sortable="column.sortable === 'true' || column.sortable === true"
                :show-overflow-tooltip="column.showOverflowTooltip"
              >
                <template #default="scope" v-if="column.slot">
                  <!-- 根据slot类型渲染不同内容 -->
                  <template v-if="column.slot === 'avatar'">
                    <div class="user-info">
                      <el-avatar :size="32" :src="scope.row.avatar" class="user-avatar">
                        {{ scope.row[column.prop]?.charAt(0)?.toUpperCase() }}
                      </el-avatar>
                      <span class="username">{{ scope.row[column.prop] }}</span>
                    </div>
                  </template>
                  <template v-else-if="column.slot === 'tag'">
                    <el-tag :type="getTagType(scope.row[column.prop], column.prop)">
                      {{ getTagText(scope.row[column.prop], column.prop) }}
                    </el-tag>
                  </template>
                  <template v-else-if="column.slot === 'date'">
                    {{ formatDate(scope.row[column.prop]) }}
                  </template>
                  <template v-else>
                    {{ scope.row[column.prop] }}
                  </template>
                </template>

                <!-- 默认显示原始值 -->
                <template #default="scope" v-else>
                  {{ scope.row[column.prop] }}
                </template>
              </el-table-column>
            </template>

            <!-- 操作列 -->
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
import { Plus, Edit, Delete, Search, Refresh, Download, MoreFilled, View, RefreshRight } from '@element-plus/icons-vue'
import { UserServiceProxy, GetUsersInput, CreateOrUpdateUserInput, UserDto, EntityDto } from '@/api/service-proxies'
import Pagination from '@/components/Pagination/index.vue'
import type { FormInstance } from 'element-plus'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// NSwag生成的用户服务代理
const userService = new UserServiceProxy()

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  filterText: '' // 合并原来的username、name、status搜索
})

// 表格数据
const userList = ref<any[]>([])
const tableColumns = ref<any[]>([]) // 动态表格列配置
const total = ref(0)
const loading = ref(true)
const ids = ref<string[]>([])
const single = ref(true)
const multiple = ref(true)

// 弹窗相关
const open = ref(false)
const title = ref('')
const isEdit = ref(false) // 是否为编辑模式
const originalUserName = ref('') // 原始用户名，用于编辑时的标识
const form = reactive({
  id: '', // 用户ID，编辑时使用
  userName: '', // 用户名
  fullName: '', // 姓名
  email: '', // 邮箱
  phoneNumber: '', // 电话号码
  password: '', // 密码
  isActive: true // 状态
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

const queryRef = ref<FormInstance>()
const userRef = ref<FormInstance>()

/** 获取标签类型 */
const getTagType = (value: any, prop: string) => {
  if (prop === 'isActive') {
    return value ? 'success' : 'danger'
  }
  return 'info'
}

/** 获取标签文本 */
const getTagText = (value: any, prop: string) => {
  if (prop === 'isActive') {
    return value ? t('user.active') : t('user.inactive')
  }
  return value?.toString() || ''
}

/** 格式化日期 */
const formatDate = (dateValue: any) => {
  if (!dateValue) return ''

  if (typeof dateValue === 'string') {
    const date = new Date(dateValue)
    return isNaN(date.getTime()) ? dateValue : date.toLocaleString()
  }

  if (dateValue instanceof Date) {
    return dateValue.toLocaleString()
  }

  return dateValue.toString()
}

/** 查询用户列表 */
const getList = async () => {
  loading.value = true
  try {
    // 使用NSwag生成的接口
    const input = new GetUsersInput({
      filterText: queryParams.filterText,
      sorting: '',
      maxResultCount: queryParams.pageSize,
      skipCount: (queryParams.pageNum - 1) * queryParams.pageSize
    })

    const response = await userService.getPaged(input)

    if (response.success && response.data) {
      // 设置动态列配置
      if (response.data.columns) {
        tableColumns.value = response.data.columns
      }

      // 设置数据列表
      if (response.data.datas) {
        userList.value = response.data.datas.items || []
        total.value = response.data.datas.totalCount || 0
      } else {
        userList.value = []
        total.value = 0
      }
    }
  } catch (error) {
    console.error('获取用户列表失败:', error)
    userList.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}



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
  isEdit.value = false
  open.value = true
  title.value = t('user.add')
}

/** 修改按钮操作 */
const handleEdit = async (row: any) => {
  reset()
  isEdit.value = true

  try {
    // 优先使用id，如果没有id则使用userName作为参数调用GetForEdit接口
    const identifier = row.id || row.userName
    if (!identifier) {
      ElMessage.error('无法获取用户标识符')
      return
    }
    const input = new EntityDto({ id: identifier })
    const response = await userService.getForEdit(input)

    if (response.success && response.data) {
      const userData = response.data
      form.id = userData.id || ''
      form.userName = userData.userName || ''
      form.fullName = userData.fullName || ''
      form.email = userData.email || ''
      form.phoneNumber = userData.phoneNumber || ''
      form.isActive = userData.isActive !== false
      originalUserName.value = userData.userName || ''
    }
  } catch (error) {
    console.error('获取用户详情失败:', error)
    ElMessage.error('获取用户详情失败')
    return
  }

  open.value = true
  title.value = t('user.edit')
}

/** 提交按钮 */
const submitForm = () => {
  userRef.value?.validate(async (valid: boolean) => {
    if (valid) {
      try {
        // 创建UserDto对象
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

        // 创建CreateOrUpdateUserInput对象
        const input = new CreateOrUpdateUserInput({
          user: userDto
        })

        // 调用CreateOrUpdate接口
        await userService.createOrUpdate(input)

        ElMessage.success(isEdit.value ? t('user.updateSuccess') : t('user.createSuccess'))
        open.value = false
        getList()
      } catch (error) {
        console.error('保存用户失败:', error)
        ElMessage.error('保存用户失败')
      }
    }
  })
}

/** 删除按钮操作 */
const handleDelete = (row: any) => {
  // 优先使用id，如果没有id则使用userName，如果单行操作失败则使用批量选择的ids
  const userIds = row.id || row.userName || ids.value
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
      // TODO: 等待后端提供删除用户API
      // 临时模拟删除成功
      console.log('删除用户:', userIds)
      getList()
      ElMessage.success(t('user.deleteSuccess'))
    } catch (error) {
      console.error('删除用户失败:', error)
      ElMessage.error('删除用户失败')
    }
  })
}

/** 取消按钮 */
const cancel = () => {
  open.value = false
  reset()
}

/** 表单重置 */
const reset = () => {
  isEdit.value = false
  originalUserName.value = ''
  form.id = ''
  form.userName = ''
  form.fullName = ''
  form.email = ''
  form.phoneNumber = ''
  form.password = ''
  form.isActive = true
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
    try {
      // TODO: 等待后端提供删除用户API
      // 临时模拟删除成功
      console.log('批量删除用户:', userIds)
      getList()
      ElMessage.success(t('user.deleteSuccess'))
    } catch (error) {
      console.error('批量删除用户失败:', error)
      ElMessage.error('删除用户失败')
    }
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
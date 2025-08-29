<template>
  <div class="app-container modern-table-wrapper">
    <!-- 搜索表单 -->
    <div class="search-wrapper">
      <el-card shadow="never" class="search-card">
        <el-form :model="queryParams" ref="queryRef" :inline="true" class="search-form">
          <el-form-item :label="$t('common.keyword')" prop="filterText">
            <el-input
              v-model="queryParams.filterText"
              :placeholder="$t('role.searchPlaceholder')"
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
            <h3>{{ $t('role.list') }}</h3>
            <span class="table-count">{{ t('role.totalRecords', { count: total }) }}</span>
          </div>
          <div class="table-actions">
            <el-button
              type="primary"
              @click="handleAdd"
            >
              <el-icon><Plus /></el-icon>
              {{ $t('role.add') }}
            </el-button>
          </div>
        </div>

        <!-- 角色表格 -->
        <div class="table-content">
          <el-table
            v-loading="loading"
            :data="roleList"
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
                  <template v-if="column.slot === 'tag'">
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
            <el-table-column :label="$t('role.actions')" width="220" align="center" fixed="right">
              <template #default="scope">
                <div class="action-buttons">
                  <el-button
                    type="primary"
                    text
                    @click="handleEdit(scope.row)"
                    :title="$t('common.edit')"
                  >
                    <el-icon><Edit /></el-icon>
                  </el-button>
                  <el-button
                    type="warning"
                    text
                    @click="handlePermission(scope.row)"
                    :title="$t('role.assign')"
                  >
                    <el-icon><Key /></el-icon>
                  </el-button>
                  <el-button
                    type="danger"
                    text
                    @click="handleDelete(scope.row)"
                    :title="$t('common.delete')"
                  >
                    <el-icon><Delete /></el-icon>
                  </el-button>
                </div>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 表格底部 -->
        <div class="table-footer">
          <pagination
            v-show="total > 0"
            :total="total"
            v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize"
            @pagination="getList"
            class="modern-pagination"
          />
        </div>
      </el-card>
    </div>

    <!-- 添加或修改角色对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="roleRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item :label="$t('role.name')" prop="displayName">
          <el-input v-model="form.displayName" :placeholder="$t('role.name')" />
        </el-form-item>
        <el-form-item :label="$t('role.code')" prop="name">
          <el-input v-model="form.name" :placeholder="$t('role.code')" />
        </el-form-item>
        <el-form-item :label="$t('role.isDefault')">
          <el-switch v-model="form.isDefault" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ $t('common.confirm') }}</el-button>
          <el-button @click="cancel">{{ $t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配权限对话框 -->
    <el-dialog :title="$t('role.assignPermissions')" v-model="openPermission" width="600px" append-to-body>
      <el-tree
        ref="permissionRef"
        :data="permissionList"
        show-checkbox
        node-key="id"
        :check-strictly="true"
        :empty-text="$t('common.noData')"
        :props="defaultProps"
      />
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitPermission">{{ $t('common.confirm') }}</el-button>
          <el-button @click="cancelPermission">{{ $t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Edit, Delete, Key, Search, Refresh } from '@element-plus/icons-vue'
import { RoleServiceProxy, GetRolesInput, CreateOrUpdateRoleInput, RoleDto, EntityDto, CommonServiceProxy } from '@/api/service-proxies'
import Pagination from '@/components/Pagination/index.vue'
import type { FormInstance } from 'element-plus'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// NSwag生成的角色服务代理
const roleService = new RoleServiceProxy()
// 通用服务代理，用于获取列配置
const commonService = new CommonServiceProxy()

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  filterText: '' // 合并原来的name和code搜索
})

// 表格数据
const roleList = ref<any[]>([])
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
const originalRoleName = ref('') // 原始角色名称，用于编辑时的标识
const form = reactive({
  id: '', // 角色ID，编辑时使用
  name: '', // 角色代码
  displayName: '', // 角色显示名
  isStatic: false,
  isDefault: false
})

// 权限弹窗相关
const openPermission = ref(false)
const permissionList = ref<any[]>([])
const currentRoleId = ref<string>('')
const permissionRef = ref()

// 表单验证
const rules = reactive({
  displayName: [{ required: true, message: () => t('role.validation.nameRequired'), trigger: 'blur' }],
  name: [{ required: true, message: () => t('role.validation.codeRequired'), trigger: 'blur' }]
})

const queryRef = ref<FormInstance>()
const roleRef = ref<FormInstance>()

// 权限树配置
const defaultProps = {
  children: 'children',
  label: 'name'
}

// 模拟权限数据
const mockPermissionData = [
  {
    id: 1,
    name: t('role.permissionTypes.systemManagement'),
    children: [
      { id: 11, name: t('role.permissionTypes.userManagement') },
      { id: 12, name: t('role.permissionTypes.roleManagement') },
      { id: 13, name: t('role.permissionTypes.menuManagement') }
    ]
  },
  {
    id: 2,
    name: t('role.permissionTypes.businessManagement'),
    children: [
      { id: 21, name: t('role.permissionTypes.orderManagement') },
      { id: 22, name: t('role.permissionTypes.productManagement') }
    ]
  }
]

/** 获取标签类型 */
const getTagType = (value: any, prop: string) => {
  if (prop === 'isStatic' || prop === 'isDefault') {
    return value ? (prop === 'isStatic' ? 'danger' : 'success') : (prop === 'isStatic' ? 'success' : 'info')
  }
  return 'info'
}

/** 获取标签文本 */
const getTagText = (value: any, prop: string) => {
  if (prop === 'isStatic' || prop === 'isDefault') {
    return value ? t('common.yes') : t('common.no')
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

/** 获取角色表格列配置 */
const getTableColumns = async () => {
  try {
    const response = await commonService.getColumns('role')
    if (response.success && response.data) {
      tableColumns.value = response.data || []
    }
  } catch (error) {
    console.error('获取角色表格列配置失败:', error)
  }
}

/** 查询角色列表 */
const getList = async () => {
  loading.value = true
  try {
    // 使用NSwag生成的接口
    const input = new GetRolesInput({
      filterText: queryParams.filterText,
      sorting: '',
      maxResultCount: queryParams.pageSize,
      skipCount: (queryParams.pageNum - 1) * queryParams.pageSize
    })

    const response = await roleService.getPaged(input)

    if (response.success && response.data) {
      // 直接使用分页数据
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
  title.value = t('role.add')
}

/** 修改按钮操作 */
const handleEdit = async (row: any) => {
  reset()
  isEdit.value = true

  try {
    // 优先使用id，如果没有id则使用name作为参数调用GetForEdit接口
    const identifier = row.id || row.name
    if (!identifier) {
      ElMessage.error('无法获取角色标识符')
      return
    }
    const input=new EntityDto({id:identifier});
    const response = await roleService.getForEdit(input)

    if (response.success && response.data) {
      const roleData = response.data
      form.id = roleData.id || ''
      form.name = roleData.name || ''
      form.displayName = roleData.displayName || ''
      form.isStatic = roleData.isStatic || false
      form.isDefault = roleData.isDefault || false
      originalRoleName.value = roleData.name || ''
    }
  } catch (error) {
    console.error('获取角色详情失败:', error)
    ElMessage.error('获取角色详情失败')
    return
  }

  open.value = true
  title.value = t('role.edit')
}

/** 获取角色权限 (临时模拟实现) */
const getRolePermissions = async (roleId: string) => {
  // TODO: 替换为真实的权限获取API调用
  console.log('获取角色权限:', roleId)
  return Promise.resolve({
    success: true,
    data: [11, 12] // 模拟已选权限
  })
}

/** 更新角色权限 (临时模拟实现) */
const updateRolePermissions = async (roleId: string, permissionIds: any[]) => {
  // TODO: 替换为真实的权限更新API调用
  console.log('更新角色权限:', { roleId, permissionIds })
  return Promise.resolve({ success: true })
}

/** 删除角色 (临时模拟实现) */
const deleteRole = async (roleIds: string | string[]) => {
  // TODO: 替换为真实的删除API调用
  console.log('删除角色:', roleIds)
  return Promise.resolve({ success: true })
}

/** 分配权限按钮操作 */
const handlePermission = async (row: any) => {
  // 优先使用id，如果没有id则使用name
  currentRoleId.value = row.id || row.name
  permissionList.value = mockPermissionData

  try {
    const response = await getRolePermissions(currentRoleId.value)
    const checkedKeys = response.data || [11, 12] // 模拟已选权限
    permissionRef.value?.setCheckedKeys(checkedKeys)
  } catch (error) {
    // 模拟数据
    permissionRef.value?.setCheckedKeys([11, 12])
  }

  openPermission.value = true
}

/** 提交角色表单 */
const submitForm = () => {
  roleRef.value?.validate(async (valid: boolean) => {
    if (valid) {
      try {
        // 创建RoleDto对象
        const roleDto = new RoleDto({
          id: isEdit.value ? form.id : undefined,
          name: form.name,
          displayName: form.displayName,
          isStatic: form.isStatic,
          isDefault: form.isDefault
        })

        // 创建CreateOrUpdateRoleInput对象
        const input = new CreateOrUpdateRoleInput({
          role: roleDto
        })

        // 调用CreateOrUpdate接口
        await roleService.createOrUpdate(input)

        ElMessage.success(isEdit.value ? t('user.updateSuccess') : t('user.createSuccess'))
        open.value = false
        getList()
      } catch (error) {
        console.error('保存角色失败:', error)
        ElMessage.error('保存角色失败')
      }
    }
  })
}

/** 提交权限分配 */
const submitPermission = async () => {
  const checkedKeys = permissionRef.value?.getCheckedKeys()
  try {
    await updateRolePermissions(currentRoleId.value, checkedKeys)
    ElMessage.success(t('role.permissionAssignSuccess'))
    openPermission.value = false
  } catch (error) {
    ElMessage.success(t('role.permissionAssignSuccess')) // 模拟成功
    openPermission.value = false
  }
}

/** 删除按钮操作 */
const handleDelete = (row: any) => {
  // 优先使用id，如果没有id则使用name，如果单行操作失败则使用批量选择的ids
  const roleIds = row.id || row.name || ids.value
  ElMessageBox.confirm(
    t('role.deleteConfirm'),
    t('common.warning'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    await deleteRole(roleIds)
    getList()
    ElMessage.success(t('user.deleteSuccess'))
  })
}

/** 取消按钮 */
const cancel = () => {
  open.value = false
  reset()
}

/** 取消权限分配 */
const cancelPermission = () => {
  openPermission.value = false
}

/** 表单重置 */
const reset = () => {
  isEdit.value = false
  originalRoleName.value = ''
  form.id = ''
  form.name = ''
  form.displayName = ''
  form.isStatic = false
  form.isDefault = false
  roleRef.value?.resetFields()
}

onMounted(async () => {
  // 先获取表格列配置，再获取数据
  await getTableColumns()
  getList()
})
</script>

<style lang="scss" scoped>
// 移除独立样式，使用统一的现代化表格样式
</style>
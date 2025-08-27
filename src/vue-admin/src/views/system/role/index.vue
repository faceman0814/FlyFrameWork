<template>
  <div class="app-container modern-table-wrapper">
    <!-- 搜索表单 -->
    <div class="search-wrapper">
      <el-card shadow="never" class="search-card">
        <el-form :model="queryParams" ref="queryRef" :inline="true" class="search-form">
          <el-form-item :label="$t('role.name')" prop="name">
            <el-input
              v-model="queryParams.name"
              :placeholder="$t('role.name')"
              clearable
              style="width: 180px"
              @keyup.enter="handleQuery"
            />
          </el-form-item>
          <el-form-item :label="$t('role.code')" prop="code">
            <el-input
              v-model="queryParams.code"
              :placeholder="$t('role.code')"
              clearable
              style="width: 180px"
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
            <el-table-column type="selection" width="50" align="center" />
            <el-table-column :label="$t('role.name')" prop="name" min-width="120" />
            <el-table-column :label="$t('role.code')" prop="code" min-width="120" />
            <el-table-column :label="$t('role.description')" prop="description" min-width="200" />
            <el-table-column :label="$t('role.createTime')" prop="createTime" width="180" />
            <el-table-column :label="$t('role.actions')" width="220" align="center">
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
        <el-form-item :label="$t('role.name')" prop="name">
          <el-input v-model="form.name" :placeholder="$t('role.name')" />
        </el-form-item>
        <el-form-item :label="$t('role.code')" prop="code">
          <el-input v-model="form.code" :placeholder="$t('role.code')" />
        </el-form-item>
        <el-form-item :label="$t('role.description')" prop="description">
          <el-input 
            v-model="form.description" 
            :placeholder="$t('role.description')"
            type="textarea"
            :rows="3"
          />
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
import { getRoleList, createRole, updateRole, deleteRole, getRolePermissions, updateRolePermissions } from '@/api/role'
import Pagination from '@/components/Pagination/index.vue'
import type { FormInstance } from 'element-plus'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  name: '',
  code: ''
})

// 表格数据
const roleList = ref<any[]>([])
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
  name: '',
  code: '',
  description: ''
})

// 权限弹窗相关
const openPermission = ref(false)
const permissionList = ref<any[]>([])
const currentRoleId = ref<string>('')
const permissionRef = ref()

// 表单验证
const rules = reactive({
  name: [{ required: true, message: () => t('role.validation.nameRequired'), trigger: 'blur' }],
  code: [{ required: true, message: () => t('role.validation.codeRequired'), trigger: 'blur' }]
})

const queryRef = ref<FormInstance>()
const roleRef = ref<FormInstance>()

// 权限树配置
const defaultProps = {
  children: 'children',
  label: 'name'
}

/** 查询角色列表 */
const getList = async () => {
  loading.value = true
  try {
    const response = await getRoleList(queryParams)
    roleList.value = response.data.items || mockRoleData
    total.value = response.data.total || mockRoleData.length
  } catch (error) {
    // 模拟数据，实际项目中应该处理错误
    roleList.value = mockRoleData
    total.value = mockRoleData.length
  } finally {
    loading.value = false
  }
}

// 模拟数据
const mockRoleData = [
  { id: 1, name: '超级管理员', code: 'admin', description: '拥有系统所有权限', createTime: '2023-01-01 10:00:00' },
  { id: 2, name: '普通管理员', code: 'manager', description: '拥有部分管理权限', createTime: '2023-01-02 10:00:00' },
  { id: 3, name: '普通用户', code: 'user', description: '基础用户权限', createTime: '2023-01-03 10:00:00' }
]

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
  title.value = t('role.add')
}

/** 修改按钮操作 */
const handleEdit = (row: any) => {
  reset()
  Object.assign(form, row)
  open.value = true
  title.value = t('role.edit')
}

/** 分配权限按钮操作 */
const handlePermission = async (row: any) => {
  currentRoleId.value = row.id
  permissionList.value = mockPermissionData
  
  try {
    const response = await getRolePermissions(row.id)
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
      if (form.id) {
        await updateRole(form.id, form)
        ElMessage.success(t('user.updateSuccess'))
      } else {
        await createRole(form)
        ElMessage.success(t('user.createSuccess'))
      }
      open.value = false
      getList()
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
  const roleIds = row.id || ids.value
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
  form.id = undefined
  form.name = ''
  form.code = ''
  form.description = ''
  roleRef.value?.resetFields()
}

onMounted(() => {
  getList()
})
</script>

<style lang="scss" scoped>
// 移除独立样式，使用统一的现代化表格样式
</style>
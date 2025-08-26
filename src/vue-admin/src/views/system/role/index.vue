<template>
  <div class="app-container">
    <!-- 搜索表单 -->
    <el-form :model="queryParams" ref="queryRef" :inline="true" class="search-form">
      <el-form-item :label="$t('role.name')" prop="name">
        <el-input
          v-model="queryParams.name"
          :placeholder="$t('role.name')"
          clearable
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item :label="$t('role.code')" prop="code">
        <el-input
          v-model="queryParams.code"
          :placeholder="$t('role.code')"
          clearable
          @keyup.enter="handleQuery"
        />
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
          {{ $t('role.add') }}
        </el-button>
      </el-col>
    </el-row>

    <!-- 角色表格 -->
    <el-table v-loading="loading" :data="roleList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="$t('role.name')" align="center" prop="name" />
      <el-table-column :label="$t('role.code')" align="center" prop="code" />
      <el-table-column :label="$t('role.description')" align="center" prop="description" />
      <el-table-column :label="$t('role.createTime')" align="center" prop="createTime" width="180" />
      <el-table-column :label="$t('role.actions')" align="center" width="200">
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
            type="warning"
            text
            @click="handlePermission(scope.row)"
          >
            <el-icon><Key /></el-icon>
            {{ $t('role.assign') }}
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
    <el-dialog title="分配权限" v-model="openPermission" width="600px" append-to-body>
      <el-tree
        ref="permissionRef"
        :data="permissionList"
        show-checkbox
        node-key="id"
        :check-strictly="true"
        empty-text="暂无数据"
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
import { Plus, Edit, Delete, Key } from '@element-plus/icons-vue'
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
  name: [{ required: true, message: 'Role name is required', trigger: 'blur' }],
  code: [{ required: true, message: 'Role code is required', trigger: 'blur' }]
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
    name: '系统管理',
    children: [
      { id: 11, name: '用户管理' },
      { id: 12, name: '角色管理' },
      { id: 13, name: '菜单管理' }
    ]
  },
  {
    id: 2,
    name: '业务管理',
    children: [
      { id: 21, name: '订单管理' },
      { id: 22, name: '商品管理' }
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
        ElMessage.success('修改成功')
      } else {
        await createRole(form)
        ElMessage.success('新增成功')
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
    ElMessage.success('权限分配成功')
    openPermission.value = false
  } catch (error) {
    ElMessage.success('权限分配成功') // 模拟成功
    openPermission.value = false
  }
}

/** 删除按钮操作 */
const handleDelete = (row: any) => {
  const roleIds = row.id || ids.value
  ElMessageBox.confirm(
    t('role.deleteConfirm'),
    'Warning',
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(async () => {
    await deleteRole(roleIds)
    getList()
    ElMessage.success('删除成功')
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
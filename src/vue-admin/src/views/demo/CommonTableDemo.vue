<template>
  <div class="demo-page">
    <CommonTable
      page-key="user-list"
      :data="userList"
      :columns="userColumns"
      :loading="loading"
      :total="total"
      :search-config="searchConfig"
      :operations="operations"
      table-title="用户管理"
      show-add
      show-batch-delete
      show-export
      @load-data="loadUserData"
      @add="handleAdd"
      @search="handleSearch"
      @operation="handleOperation"
      @switch-change="handleStatusChange"
    >
      <!-- 自定义用户头像列 -->
      <template #avatar="{ row }">
        <div class="user-info">
          <el-avatar :size="32" :src="row.avatar" class="user-avatar">
            {{ row.userName?.charAt(0)?.toUpperCase() }}
          </el-avatar>
          <span class="username">{{ row.userName }}</span>
        </div>
      </template>
      
      <!-- 自定义操作按钮 -->
      <template #actions="{ selected }">
        <el-button type="primary" @click="handleAdd">
          <el-icon><Plus /></el-icon>
          新增用户
        </el-button>
        <el-button 
          v-if="selected.length > 0"
          type="danger" 
          @click="handleBatchDelete(selected)"
        >
          <el-icon><Delete /></el-icon>
          批量删除 ({{ selected.length }})
        </el-button>
        <el-button type="success" @click="handleExport">
          <el-icon><Download /></el-icon>
          导出数据
        </el-button>
      </template>
    </CommonTable>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete, Download, Edit, View } from '@element-plus/icons-vue'
import type { TableColumn, SearchConfig, TableOperation } from '@/components/CommonTable'
import { CommonTable } from '@/components/CommonTable'

// 模拟数据
const userList = ref([
  {
    id: 1,
    userName: 'admin',
    fullName: '系统管理员',
    email: 'admin@example.com',
    phone: '13800138000',
    isActive: true,
    createdAt: '2024-01-01 10:00:00',
    avatar: ''
  },
  {
    id: 2,
    userName: 'john_doe',
    fullName: '约翰·多伊',
    email: 'john@example.com',
    phone: '13800138001',
    isActive: true,
    createdAt: '2024-01-02 14:30:00',
    avatar: 'https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png'
  },
  {
    id: 3,
    userName: 'jane_smith',
    fullName: '简·史密斯',
    email: 'jane@example.com',
    phone: '13800138002',
    isActive: false,
    createdAt: '2024-01-03 09:15:00',
    avatar: ''
  }
])

const loading = ref(false)
const total = ref(3)

// 表格列配置
const userColumns: TableColumn[] = [
  {
    prop: 'userName',
    label: '用户名',
    width: '150',
    slot: 'avatar'
  },
  {
    prop: 'fullName',
    label: '姓名',
    minWidth: '120',
    showOverflowTooltip: true
  },
  {
    prop: 'email',
    label: '邮箱',
    minWidth: '180',
    render: 'link',
    linkType: 'primary',
    linkHref: (row) => `mailto:${row.email}`
  },
  {
    prop: 'phone',
    label: '电话',
    width: '120'
  },
  {
    prop: 'isActive',
    label: '状态',
    width: '80',
    align: 'center',
    render: 'tag',
    tagMap: {
      true: { type: 'success', text: '启用' },
      false: { type: 'danger', text: '禁用' }
    }
  },
  {
    prop: 'createdAt',
    label: '创建时间',
    width: '160',
    render: 'date',
    dateFormat: 'YYYY-MM-DD HH:mm'
  }
]

// 搜索配置
const searchConfig: SearchConfig[] = [
  {
    prop: 'userName',
    label: '用户名',
    type: 'input',
    placeholder: '请输入用户名'
  },
  {
    prop: 'isActive',
    label: '状态',
    type: 'select',
    options: [
      { label: '全部', value: '' },
      { label: '启用', value: true },
      { label: '禁用', value: false }
    ]
  },
  {
    prop: 'dateRange',
    label: '创建时间',
    type: 'daterange',
    width: '250px'
  }
]

// 操作配置
const operations: TableOperation[] = [
  {
    key: 'edit',
    label: '编辑',
    type: 'primary',
    icon: Edit,
    handler: (row) => {
      ElMessage.info(`编辑用户: ${row.fullName}`)
    }
  },
  {
    key: 'view',
    label: '查看',
    type: 'info',
    icon: View,
    handler: (row) => {
      ElMessage.info(`查看用户: ${row.fullName}`)
    }
  },
  {
    key: 'delete',
    label: '删除',
    type: 'danger',
    icon: Delete,
    visible: (row) => row.userName !== 'admin', // 管理员不显示删除按钮
    handler: (row) => {
      ElMessageBox.confirm(`确定要删除用户 ${row.fullName} 吗？`, '警告', {
        type: 'warning'
      }).then(() => {
        ElMessage.success('删除成功')
      })
    }
  },
  {
    key: 'resetPwd',
    label: '重置密码',
    type: 'warning',
    isMore: true,
    handler: (row) => {
      ElMessage.success(`已重置用户 ${row.fullName} 的密码`)
    }
  },
  {
    key: 'permissions',
    label: '权限设置',
    type: 'primary',
    isMore: true,
    handler: (row) => {
      ElMessage.info(`设置用户 ${row.fullName} 的权限`)
    }
  }
]

// 事件处理
const loadUserData = (params: any) => {
  console.log('加载数据参数:', params)
  loading.value = true
  
  // 模拟 API 请求
  setTimeout(() => {
    // 这里应该调用真实的 API
    loading.value = false
  }, 1000)
}

const handleAdd = () => {
  ElMessage.success('打开新增用户对话框')
}

const handleSearch = (params: any) => {
  console.log('搜索参数:', params)
  ElMessage.info('执行搜索操作')
}

const handleOperation = (key: string, row: any, index: number) => {
  console.log('自定义操作:', { key, row, index })
  ElMessage.info(`执行操作: ${key}`)
}

const handleStatusChange = (value: boolean, row: any) => {
  console.log('状态变更:', { value, row })
  ElMessage.success(`用户 ${row.fullName} 状态已${value ? '启用' : '禁用'}`)
}

const handleBatchDelete = (selectedRows: any[]) => {
  ElMessageBox.confirm(
    `确定要删除选中的 ${selectedRows.length} 个用户吗？`,
    '批量删除',
    { type: 'warning' }
  ).then(() => {
    ElMessage.success('批量删除成功')
  })
}

const handleExport = () => {
  ElMessage.info('正在导出用户数据...')
}

onMounted(() => {
  // 初始加载数据
  loadUserData({ page: 1, pageSize: 20 })
})
</script>

<style lang="scss" scoped>
.demo-page {
  padding: 20px;
  background: #f5f7fa;
  min-height: 100vh;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;

  .user-avatar {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    font-weight: 600;
    flex-shrink: 0;
  }

  .username {
    font-weight: 500;
    color: var(--el-text-color-primary);
  }
}
</style>
<template>
  <div class="common-table-container">
    <!-- 搜索区域 -->
    <div v-if="showSearch" class="search-wrapper">
      <el-card shadow="never" class="search-card">
        <el-form
          :model="searchParams"
          ref="searchFormRef"
          :inline="true"
          class="search-form"
          @submit.prevent="handleSearch"
        >
          <!-- 动态渲染搜索项 -->
          <template v-for="searchItem in searchConfig" :key="searchItem.prop">
            <el-form-item
              :label="searchItem.label"
              :prop="searchItem.prop"
            >
              <!-- 输入框 -->
              <template v-if="!searchItem.type || searchItem.type === 'input'">
                <el-input
                  v-model="searchParams[searchItem.prop]"
                  :placeholder="searchItem.placeholder || `请输入${searchItem.label}`"
                  :style="`width: ${searchItem.width || '240px'}`"
                  clearable
                  @keyup.enter="handleSearch"
                />
              </template>
              
              <!-- 选择器 -->
              <template v-else-if="searchItem.type === 'select'">
                <el-select
                  v-model="searchParams[searchItem.prop]"
                  :placeholder="searchItem.placeholder || `请选择${searchItem.label}`"
                  :style="`width: ${searchItem.width || '180px'}`"
                  clearable
                >
                  <el-option
                    v-for="option in searchItem.options"
                    :key="option.value"
                    :label="option.label"
                    :value="option.value"
                  />
                </el-select>
              </template>
              
              <!-- 日期选择器 -->
              <template v-else-if="searchItem.type === 'date'">
                <el-date-picker
                  v-model="searchParams[searchItem.prop]"
                  type="date"
                  :placeholder="searchItem.placeholder || `选择${searchItem.label}`"
                  :style="`width: ${searchItem.width || '180px'}`"
                  clearable
                />
              </template>
              
              <!-- 日期范围选择器 -->
              <template v-else-if="searchItem.type === 'daterange'">
                <el-date-picker
                  v-model="searchParams[searchItem.prop]"
                  type="daterange"
                  range-separator="至"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                  :style="`width: ${searchItem.width || '240px'}`"
                  clearable
                />
              </template>
            </el-form-item>
          </template>
          
          <!-- 搜索按钮 -->
          <el-form-item>
            <el-button type="primary" @click="handleSearch">
              <el-icon><Search /></el-icon>
              {{ t('common.search') }}
            </el-button>
            <el-button @click="handleReset">
              <el-icon><Refresh /></el-icon>
              {{ t('common.reset') }}
            </el-button>
          </el-form-item>
        </el-form>
      </el-card>
    </div>

    <!-- 表格区域 -->
    <div class="table-wrapper">
      <el-card shadow="never" class="table-card">
        <!-- 表格头部 -->
        <div class="table-header">
          <div class="table-title">
            <h3>{{ tableTitle }}</h3>
            <span v-if="showTotal" class="table-count">
              共 {{ total }} 条记录
            </span>
          </div>
          <div class="table-actions">
            <!-- 自定义操作按钮插槽 -->
            <slot name="actions" :selected="selectedRows">
              <el-button
                v-if="showAdd"
                type="primary"
                @click="handleAdd"
              >
                <el-icon><Plus /></el-icon>
                {{ t('common.add') }}
              </el-button>
              <el-button
                v-if="showBatchDelete && selectedRows.length > 0"
                type="danger"
                @click="handleBatchDelete"
              >
                <el-icon><Delete /></el-icon>
                {{ t('common.batchDelete') }} ({{ selectedRows.length }})
              </el-button>
              <el-button
                v-if="showExport"
                type="info"
                plain
                @click="handleExport"
              >
                <el-icon><Download /></el-icon>
                {{ t('common.export') }}
              </el-button>
              <el-button
                type="info"
                plain
                @click="handleRefresh"
              >
                <el-icon><Refresh /></el-icon>
                {{ t('common.refresh') }}
              </el-button>
            </slot>
          </div>
        </div>

        <!-- 表格内容 -->
        <div class="table-content">
          <el-table
            ref="tableRef"
            v-loading="loading"
            :data="tableData"
            :stripe="stripe"
            :border="border"
            :highlight-current-row="highlightCurrentRow"
            :row-key="rowKey"
            @selection-change="handleSelectionChange"
            @row-click="handleRowClick"
            @sort-change="handleSortChange"
            class="modern-table"
          >
            <!-- 选择列 -->
            <el-table-column
              v-if="selection"
              type="selection"
              width="50"
              align="center"
              :reserve-selection="true"
            />

            <!-- 序号列 -->
            <el-table-column
              v-if="showIndex"
              type="index"
              :label="indexLabel"
              width="60"
              align="center"
              :index="getTableIndex"
            />

            <!-- 动态列渲染 -->
            <template v-for="column in processedColumns" :key="column.prop">
              <el-table-column
                :label="column.label"
                :prop="column.prop"
                :width="column.width"
                :min-width="column.minWidth"
                :max-width="column.maxWidth"
                :align="column.align || 'left'"
                :sortable="column.sortable"
                :sort-by="column.sortBy"
                :show-overflow-tooltip="column.showOverflowTooltip !== false"
                :fixed="column.fixed"
                :class-name="column.className"
                :label-class-name="column.labelClassName"
              >
                <template #default="scope">
                  <!-- 自定义插槽 -->
                  <template v-if="column.slot">
                    <slot
                      :name="column.slot"
                      :row="scope.row"
                      :column="column"
                      :index="scope.$index"
                      :value="getColumnValue(scope.row, column.prop)"
                    >
                      {{ getColumnValue(scope.row, column.prop) }}
                    </slot>
                  </template>
                  
                  <!-- 预定义的渲染类型 -->
                  <template v-else-if="column.render">
                    <!-- 标签类型 -->
                    <template v-if="column.render === 'tag'">
                      <el-tag
                        :type="getTagType(scope.row, column)"
                        :effect="column.tagEffect || 'light'"
                        :size="column.tagSize || 'default'"
                      >
                        {{ getTagText(scope.row, column) }}
                      </el-tag>
                    </template>
                    
                    <!-- 开关类型 -->
                    <template v-else-if="column.render === 'switch'">
                      <el-switch
                        :model-value="!!getColumnValue(scope.row, column.prop)"
                        :disabled="column.switchDisabled || false"
                        @change="(val: any) => handleSwitchChange(val, scope.row, column)"
                      />
                    </template>
                    
                    <!-- 图片类型 -->
                    <template v-else-if="column.render === 'image'">
                      <el-image
                        :src="getColumnValue(scope.row, column.prop)"
                        :style="`width: ${column.imageSize || '40px'}; height: ${column.imageSize || '40px'}`"
                        :preview-src-list="[getColumnValue(scope.row, column.prop)]"
                        fit="cover"
                        class="table-image"
                      >
                        <template #error>
                          <div class="image-slot">
                            <el-icon><Picture /></el-icon>
                          </div>
                        </template>
                      </el-image>
                    </template>
                    
                    <!-- 链接类型 -->
                    <template v-else-if="column.render === 'link'">
                      <el-link
                        :type="column.linkType || 'primary'"
                        :href="column.linkHref ? column.linkHref(scope.row) : '#'"
                        :target="column.linkTarget || '_blank'"
                        @click="column.linkClick && column.linkClick(scope.row)"
                      >
                        {{ getColumnValue(scope.row, column.prop) }}
                      </el-link>
                    </template>
                    
                    <!-- 日期格式化 -->
                    <template v-else-if="column.render === 'date'">
                      {{ formatDate(getColumnValue(scope.row, column.prop), column.dateFormat) }}
                    </template>
                    
                    <!-- 数字格式化 -->
                    <template v-else-if="column.render === 'number'">
                      {{ formatNumber(getColumnValue(scope.row, column.prop), column.numberOptions) }}
                    </template>
                    
                    <!-- 进度条 -->
                    <template v-else-if="column.render === 'progress'">
                      <el-progress
                        :percentage="getColumnValue(scope.row, column.prop)"
                        :color="column.progressColor"
                        :show-text="column.showProgressText !== false"
                      />
                    </template>
                    
                    <!-- 自定义渲染函数 -->
                    <template v-else-if="typeof column.render === 'function'">
                      <span v-html="column.render(getColumnValue(scope.row, column.prop), scope.row, column)"></span>
                    </template>
                  </template>
                  
                  <!-- 默认显示 -->
                  <template v-else>
                    {{ getColumnValue(scope.row, column.prop) }}
                  </template>
                </template>
                
                <!-- 自定义表头 -->
                <template v-if="column.headerSlot" #header="headerScope">
                  <slot
                    :name="column.headerSlot"
                    :column="headerScope.column"
                    :index="headerScope.$index"
                  >
                    {{ column.label }}
                  </slot>
                </template>
              </el-table-column>
            </template>

            <!-- 操作列 -->
            <el-table-column
              v-if="operations && operations.length > 0"
              :label="operationLabel"
              :width="operationWidth"
              :min-width="operationMinWidth"
              align="center"
              fixed="right"
              class-name="operation-column"
            >
              <template #default="scope">
                <div class="operation-buttons">
                  <!-- 主要操作按钮 -->
                  <template v-for="operation in getVisibleOperations(scope.row)" :key="operation.key">
                    <el-button
                      v-if="!operation.isMore"
                      :type="operation.type || 'primary'"
                      :size="operation.size || 'small'"
                      :disabled="operation.disabled && operation.disabled(scope.row)"
                      :text="operation.text !== false"
                      :link="operation.link === true"
                      @click="operation.handler && operation.handler(scope.row, scope.$index)"
                    >
                      <el-icon v-if="operation.icon">
                        <component :is="operation.icon" />
                      </el-icon>
                      {{ operation.label }}
                    </el-button>
                  </template>
                  
                  <!-- 更多操作下拉菜单 -->
                  <template v-if="getMoreOperations(scope.row).length > 0">
                    <el-dropdown trigger="click" @command="(command: any) => handleMoreOperation(command, scope.row, scope.$index)">
                      <el-button text size="small">
                        <el-icon><MoreFilled /></el-icon>
                      </el-button>
                      <template #dropdown>
                        <el-dropdown-menu>
                          <el-dropdown-item
                            v-for="operation in getMoreOperations(scope.row)"
                            :key="operation.key"
                            :command="operation.key"
                            :disabled="operation.disabled && operation.disabled(scope.row)"
                          >
                            <el-icon v-if="operation.icon">
                              <component :is="operation.icon" />
                            </el-icon>
                            {{ operation.label }}
                          </el-dropdown-item>
                        </el-dropdown-menu>
                      </template>
                    </el-dropdown>
                  </template>
                </div>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 表格底部 -->
        <div v-if="showPagination" class="table-footer">
          <pagination
            :total="total"
            v-model:page="currentPage"
            v-model:limit="currentPageSize"
            :page-sizes="pageSizes"
            :layout="paginationLayout"
            :background="paginationBackground"
            @pagination="handlePagination"
          />
        </div>
      </el-card>
    </div>

    <!-- 空状态 -->
    <div v-if="!loading && tableData.length === 0 && showEmpty" class="empty-wrapper">
      <el-empty :description="emptyText || t('common.noData')" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useI18n } from 'vue-i18n'
import {
  Search,
  Refresh,
  Plus,
  Delete,
  Download,
  MoreFilled,
  Picture
} from '@element-plus/icons-vue'
import Pagination from '../Pagination/index.vue'
import { useTableState } from './useTableState'
import type { TableColumn, TableOperation, SearchConfig } from './types'
import type { FormInstance } from 'element-plus'

// 多语言
const { t } = useI18n()

// 组件属性定义
interface Props {
  // 页面标识
  pageKey: string
  
  // 基础数据
  data: any[]
  columns: TableColumn[]
  loading?: boolean
  total?: number
  
  // 表格配置
  stripe?: boolean
  border?: boolean
  highlightCurrentRow?: boolean
  selection?: boolean
  showIndex?: boolean
  indexLabel?: string
  rowKey?: string | ((row: any) => string)
  
  // 表格标题和统计
  tableTitle?: string
  showTotal?: boolean
  
  // 搜索配置
  showSearch?: boolean
  searchConfig?: SearchConfig[]
  
  // 操作配置
  operations?: TableOperation[]
  operationLabel?: string
  operationWidth?: string | number
  operationMinWidth?: string | number
  
  // 分页配置
  showPagination?: boolean
  page?: number
  pageSize?: number
  pageSizes?: number[]
  paginationLayout?: string
  paginationBackground?: boolean
  
  // 功能开关
  showAdd?: boolean
  showBatchDelete?: boolean
  showExport?: boolean
  showEmpty?: boolean
  
  // 空状态
  emptyText?: string
}

const props = withDefaults(defineProps<Props>(), {
  // 基础配置
  tableTitle: '数据列表',
  
  // 表格配置
  columns: () => [],
  data: () => [],
  loading: false,
  total: 0,
  stripe: true,
  border: true,
  highlightCurrentRow: true,
  selection: true,
  showIndex: false,
  indexLabel: '序号',
  rowKey: 'id',
  
  // 搜索配置
  showSearch: true,
  searchConfig: () => [],
  
  // 操作配置
  operations: () => [],
  operationLabel: '操作',
  operationWidth: '180',
  operationMinWidth: '120',
  
  // 分页配置
  showPagination: true,
  page: 1,
  pageSize: 20,
  pageSizes: () => [10, 20, 50, 100],
  paginationLayout: 'total, sizes, prev, pager, next, jumper',
  paginationBackground: true,
  
  // 功能开关
  showAdd: true,
  showBatchDelete: false,
  showExport: false,
  showTotal: true,
  showEmpty: true,
  
  // 文案配置
  emptyText: '暂无数据'
})

// 事件定义
const emit = defineEmits<{
  // 数据操作事件
  'load-data': [params: any]
  'add': []
  'edit': [row: any, index: number]
  'delete': [row: any, index: number]
  'batch-delete': [rows: any[]]
  'export': [params: any]
  'refresh': []
  
  // 表格事件
  'selection-change': [selection: any[]]
  'row-click': [row: any, column: any, event: Event]
  'sort-change': [sort: any]
  
  // 分页事件
  'page-change': [page: number]
  'page-size-change': [size: number]
  
  // 搜索事件
  'search': [params: any]
  'reset': []
  
  // 自定义事件
  'operation': [key: string, row: any, index: number]
  'switch-change': [value: boolean, row: any, column: TableColumn]
}>()

// 使用状态管理 Hook
const {
  searchParams,
  selectedRows,
  currentPage,
  currentPageSize,
  saveState,
  loadState,
  clearState
} = useTableState(props.pageKey)

// 模板引用
const searchFormRef = ref<FormInstance>()
const tableRef = ref()

// 计算属性
const tableData = computed(() => props.data)

const processedColumns = computed(() => {
  return props.columns.filter((column: TableColumn) => !column.hidden)
})

// 获取表格索引（支持分页）
const getTableIndex = (index: number) => {
  return (currentPage.value - 1) * currentPageSize.value + index + 1
}

// 获取列值（支持嵌套属性）
const getColumnValue = (row: any, prop: string) => {
  if (!prop || !row) return ''
  
  const keys = prop.split('.')
  let value = row
  
  for (const key of keys) {
    value = value?.[key]
    if (value === undefined || value === null) return ''
  }
  
  return value
}

// 获取标签类型
const getTagType = (row: any, column: TableColumn) => {
  if (column.tagType && typeof column.tagType === 'function') {
    return column.tagType(getColumnValue(row, column.prop), row)
  }
  if (column.tagMap) {
    const value = getColumnValue(row, column.prop)
    const config = column.tagMap[value]
    return config?.type || 'info'
  }
  return 'info'
}

// 获取标签文本
const getTagText = (row: any, column: TableColumn) => {
  if (column.tagFormatter && typeof column.tagFormatter === 'function') {
    return column.tagFormatter(getColumnValue(row, column.prop), row)
  }
  if (column.tagMap) {
    const value = getColumnValue(row, column.prop)
    const config = column.tagMap[value]
    return config?.text || value
  }
  return getColumnValue(row, column.prop)
}

// 日期格式化
const formatDate = (value: any, format = 'YYYY-MM-DD HH:mm:ss') => {
  if (!value) return ''
  
  const date = new Date(value)
  if (isNaN(date.getTime())) return value
  
  // 简单的日期格式化，可以根据需要使用更强大的日期库
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  
  return format
    .replace('YYYY', year.toString())
    .replace('MM', month)
    .replace('DD', day)
    .replace('HH', hours)
    .replace('mm', minutes)
    .replace('ss', seconds)
}

// 数字格式化
const formatNumber = (value: any, options: any = {}) => {
  if (isNaN(Number(value))) return value
  
  const num = Number(value)
  
  if (options.currency) {
    return new Intl.NumberFormat('zh-CN', {
      style: 'currency',
      currency: options.currency
    }).format(num)
  }
  
  if (options.percent) {
    return new Intl.NumberFormat('zh-CN', {
      style: 'percent',
      minimumFractionDigits: options.minimumFractionDigits || 0,
      maximumFractionDigits: options.maximumFractionDigits || 2
    }).format(num / 100)
  }
  
  return new Intl.NumberFormat('zh-CN', options).format(num)
}

// 获取可见的操作按钮
const getVisibleOperations = (row: any) => {
  return props.operations.filter((operation: TableOperation) => {
    if (operation.isMore) return false
    if (operation.visible && typeof operation.visible === 'function') {
      return operation.visible(row)
    }
    return operation.visible !== false
  }).slice(0, 3) // 最多显示3个主要操作
}

// 获取更多操作
const getMoreOperations = (row: any) => {
  const visibleOperations = props.operations.filter((operation: TableOperation) => {
    if (operation.visible && typeof operation.visible === 'function') {
      return operation.visible(row)
    }
    return operation.visible !== false
  })
  
  const moreOperations = visibleOperations.filter((operation: TableOperation) => operation.isMore)
  const extraOperations = visibleOperations.filter((operation: TableOperation) => !operation.isMore).slice(3)
  
  return [...moreOperations, ...extraOperations]
}

// 事件处理方法
const handleSearch = () => {
  currentPage.value = 1
  const params = {
    ...searchParams,
    page: currentPage.value,
    pageSize: currentPageSize.value
  }
  saveState()
  emit('search', params)
  emit('load-data', params)
}

const handleReset = () => {
  searchFormRef.value?.resetFields()
  Object.keys(searchParams).forEach(key => {
    searchParams[key] = ''
  })
  currentPage.value = 1
  saveState()
  emit('reset')
  emit('load-data', {
    page: currentPage.value,
    pageSize: currentPageSize.value
  })
}

const handleAdd = () => {
  emit('add')
}

const handleBatchDelete = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning(t('common.pleaseSelect'))
    return
  }
  
  ElMessageBox.confirm(
    t('common.batchDeleteConfirm', { count: selectedRows.value.length }),
    t('common.warning'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning'
    }
  ).then(() => {
    emit('batch-delete', selectedRows.value)
  })
}

const handleExport = () => {
  const params = {
    ...searchParams,
    page: currentPage.value,
    pageSize: currentPageSize.value
  }
  emit('export', params)
}

const handleRefresh = () => {
  // 重新加载数据
  const params = {
    ...searchParams,
    page: currentPage.value,
    pageSize: currentPageSize.value
  }
  emit('load-data', params)
  emit('refresh')
}

const handleSelectionChange = (selection: any[]) => {
  selectedRows.value = selection
  saveState()
  emit('selection-change', selection)
}

const handleRowClick = (row: any, column: any, event: Event) => {
  emit('row-click', row, column, event)
}

const handleSortChange = (sort: any) => {
  emit('sort-change', sort)
}

const handlePagination = ({ page, limit }: { page: number; limit: number }) => {
  currentPage.value = page
  currentPageSize.value = limit
  saveState()
  
  const params = {
    ...searchParams,
    page,
    pageSize: limit
  }
  
  emit('page-change', page)
  emit('page-size-change', limit)
  emit('load-data', params)
}

const handleSwitchChange = (value: any, row: any, column: TableColumn) => {
  emit('switch-change', value, row, column)
}

const handleMoreOperation = (key: any, row: any, index: number) => {
  const operation = props.operations.find((op: TableOperation) => op.key === key)
  if (operation && operation.handler) {
    operation.handler(row, index)
  } else {
    emit('operation', key, row, index)
  }
}

// 清除当前页面状态
const clearCurrentState = () => {
  clearState()
  Object.keys(searchParams).forEach(key => {
    searchParams[key] = ''
  })
  selectedRows.value = []
  currentPage.value = 1
  currentPageSize.value = props.pageSize
}

// 刷新表格数据
const refresh = () => {
  const params = {
    ...searchParams,
    page: currentPage.value,
    pageSize: currentPageSize.value
  }
  emit('load-data', params)
}

// 获取选中的行数据
const getSelectedRows = () => {
  return selectedRows.value
}

// 设置选中的行
const setSelectedRows = (rows: any[]) => {
  selectedRows.value = rows
  nextTick(() => {
    rows.forEach(row => {
      tableRef.value?.toggleRowSelection(row, true)
    })
  })
}

// 清除选中状态
const clearSelection = () => {
  selectedRows.value = []
  tableRef.value?.clearSelection()
  saveState()
}

// 监听页面变化，保存状态
watch([currentPage, currentPageSize], () => {
  saveState()
})

watch(searchParams, () => {
  saveState()
}, { deep: true })

// 组件挂载时恢复状态
onMounted(() => {
  loadState()
  // 初始化搜索参数
  if (props.searchConfig.length > 0) {
    props.searchConfig.forEach((config: SearchConfig) => {
      if (searchParams[config.prop] === undefined) {
        searchParams[config.prop] = config.defaultValue || ''
      }
    })
  }
})

// 向父组件暴露的方法
defineExpose({
  refresh,
  clearCurrentState,
  getSelectedRows,
  setSelectedRows,
  clearSelection,
  searchFormRef,
  tableRef
})
</script>

<style lang="scss" scoped>
// 基础样式
.common-table-container {
  width: 100%;
}

// 搜索区域
.search-wrapper {
  margin-bottom: 16px;

  .search-card {
    border-radius: 12px;
    border: 1px solid var(--border-light);
    box-shadow: var(--shadow-card);
    background: var(--bg-card);

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
  }
}

// 表格区域
.table-wrapper {
  .table-card {
    border-radius: 12px;
    border: 1px solid var(--border-light);
    box-shadow: var(--shadow-card);
    overflow: hidden;
    background: var(--bg-card);

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
  background: var(--table-bg);

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
      font-weight: 500;
      color: var(--text-primary);
      background: var(--bg-container);
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
        color: var(--text-white);

        &:hover {
          opacity: 0.9;
          transform: translateY(-1px);
        }
      }

      &.el-button--danger {
        background: var(--danger-color);
        border-color: var(--danger-color);
        color: var(--text-white);
        
        &:hover {
          transform: translateY(-1px);
          opacity: 0.9;
        }
      }

      &.el-button--info {
        background: var(--info-color);
        border-color: var(--info-color);
        color: var(--text-white);
        
        &:hover {
          transform: translateY(-1px);
          opacity: 0.9;
        }
      }
    }
  }
}

// 表格内容
.table-content {
  .modern-table {
    background: var(--table-bg);
    border-radius: 8px;
    overflow: hidden;
    
    :deep(.el-table) {
      background: var(--table-bg) !important;
      color: var(--table-text);
    }

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
      background: var(--table-bg) !important;
      
      .el-table__row {
        background: var(--table-bg) !important;
        transition: all 0.2s ease;

        &:hover {
          background: var(--table-row-hover) !important;

          .el-table__cell {
            background: transparent !important;
          }
        }

        &.el-table__row--striped {
          background: var(--table-row-striped) !important;
          
          &:hover {
            background: var(--table-row-hover) !important;
          }
        }

        .el-table__cell {
          background: transparent !important;
          border-bottom: 1px solid var(--table-border);
          color: var(--table-text);
          padding: 12px 0;

          .cell {
            padding: 0 10px;
            color: var(--table-text);
          }
        }
      }
    }

    // 表格底部固定背景
    :deep(.el-table__fixed),
    :deep(.el-table__fixed-right) {
      background: var(--table-bg) !important;
    }

    // 边框样式
    :deep(.el-table--border) {
      border-color: var(--table-border);

      &::before {
        background-color: var(--table-border);
      }

      &::after {
        background-color: var(--table-border);
      }

      .el-table__cell {
        border-right-color: var(--table-border);
      }
    }

    // 空状态
    :deep(.el-table__empty-block) {
      background: var(--table-bg);
      color: var(--text-regular);
    }
  }

  // 图片样式
  .table-image {
    border-radius: 4px;
    overflow: hidden;

    .image-slot {
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--bg-main);
      color: var(--text-placeholder);
      width: 100%;
      height: 100%;
    }
  }

  // 操作按钮样式
  .operation-buttons {
    display: flex;
    align-items: center;
    gap: 4px;
    justify-content: center;
    flex-wrap: wrap;

    .el-button {
      padding: 6px;
      min-width: auto;
      border-radius: 6px;

      &.el-button--text {
        &.el-button--primary {
          color: var(--primary-color);
          
          &:hover {
            background: rgba(102, 126, 234, 0.1);
            color: var(--primary-color);
          }
        }

        &.el-button--danger {
          color: var(--danger-color);
          
          &:hover {
            background: rgba(245, 108, 108, 0.1);
            color: var(--danger-color);
          }
        }

        &.el-button--warning {
          color: var(--warning-color);
          
          &:hover {
            background: rgba(230, 162, 60, 0.1);
            color: var(--warning-color);
          }
        }

        &.el-button--success {
          color: var(--success-color);
          
          &:hover {
            background: rgba(103, 194, 58, 0.1);
            color: var(--success-color);
          }
        }

        &.el-button--info {
          color: var(--info-color);
          
          &:hover {
            background: rgba(144, 147, 153, 0.1);
            color: var(--info-color);
          }
        }
      }
    }
  }
}

// 表格底部
.table-footer {
  padding: 16px 24px;
  background: var(--table-bg);
  border-top: 1px solid var(--table-border);
  border-radius: 0 0 8px 8px;

  :deep(.pagination-container) {
    justify-content: flex-end;
    padding: 0;
  }

  :deep(.el-pagination) {
    .el-pagination__total {
      color: var(--text-primary) !important;
      font-weight: 600;
      font-size: 14px;
    }
    
    .el-pagination__sizes,
    .el-pagination__jump {
      .el-select__wrapper,
      .el-input__wrapper {
        background: var(--bg-card) !important;
        border-color: var(--border-light) !important;
        
        .el-select__selected-item,
        .el-input__inner {
          color: var(--text-primary) !important;
        }
      }
    }
    
    .el-pager {
      li {
        background: var(--bg-card) !important;
        color: var(--text-primary) !important;
        border-color: var(--border-light) !important;
        
        &.is-active {
          background: var(--primary-color) !important;
          color: #fff !important;
        }
        
        &:hover {
          background: var(--bg-hover) !important;
          color: var(--primary-color) !important;
        }
      }
    }
    
    .btn-prev,
    .btn-next {
      background: var(--bg-card) !important;
      color: var(--text-primary) !important;
      border-color: var(--border-light) !important;
      
      &:hover {
        background: var(--bg-hover) !important;
        color: var(--primary-color) !important;
      }
    }
  }
}

// 空状态
.empty-wrapper {
  padding: 40px 0;
  background: var(--table-bg);
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

  .operation-buttons {
    .el-button {
      font-size: 12px;
      padding: 4px 6px;
    }
  }
}

@media (max-width: 480px) {
  .search-card,
  .table-card {
    border-radius: 8px;
    margin: 0 10px;
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

  .table-content {
    .modern-table {
      :deep(.el-table__body) {
        .el-table__cell {
          .cell {
            padding: 0 4px;
            font-size: 13px;
          }
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
    background-color: rgba(103, 194, 58, 0.15);
    border-color: rgba(103, 194, 58, 0.3);
    color: var(--success-color);
  }

  &.el-tag--danger {
    background-color: rgba(245, 108, 108, 0.15);
    border-color: rgba(245, 108, 108, 0.3);
    color: var(--danger-color);
  }

  &.el-tag--warning {
    background-color: rgba(230, 162, 60, 0.15);
    border-color: rgba(230, 162, 60, 0.3);
    color: var(--warning-color);
  }

  &.el-tag--info {
    background-color: rgba(144, 147, 153, 0.15);
    border-color: rgba(144, 147, 153, 0.3);
    color: var(--info-color);
  }

  &.el-tag--primary {
    background-color: rgba(102, 126, 234, 0.15);
    border-color: rgba(102, 126, 234, 0.3);
    color: var(--primary-color);
  }
}

// 下拉菜单样式
:deep(.el-dropdown-menu) {
  border-radius: 8px;
  border: 1px solid var(--border-light);
  box-shadow: var(--shadow-lg);
  background: var(--bg-card);

  .el-dropdown-menu__item {
    padding: 8px 16px;
    color: var(--text-primary);

    .el-icon {
      margin-right: 8px;
    }

    &:hover {
      background: var(--bg-hover);
      color: var(--primary-color);
    }
  }
}

// 开关样式
:deep(.el-switch) {
  &.is-checked {
    .el-switch__core {
      background-color: var(--primary-color);
    }
  }
  
  .el-switch__core {
    background-color: var(--bg-main);
    border-color: var(--border-light);
  }
}

// 进度条样式
:deep(.el-progress) {
  .el-progress-bar {
    .el-progress-bar__outer {
      background-color: var(--bg-main);
    }
  }
}

// 输入框样式优化
:deep(.el-input) {
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

// 选择器样式优化
:deep(.el-select) {
  .el-select__wrapper {
    background: var(--bg-card);
    border-color: var(--border-light);
    
    &:hover {
      border-color: var(--primary-color);
    }
    
    &.is-focused {
      border-color: var(--primary-color);
      box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
    }
  }
  
  .el-select__placeholder {
    color: var(--text-placeholder);
  }
}

// 日期选择器样式
:deep(.el-date-editor) {
  background: var(--bg-card);
  border-color: var(--border-light);
  
  &:hover {
    border-color: var(--primary-color);
  }
  
  &.is-active {
    border-color: var(--primary-color);
  }
  
  .el-input__inner {
    color: var(--text-primary);
    
    &::placeholder {
      color: var(--text-placeholder);
    }
  }
}
</style>
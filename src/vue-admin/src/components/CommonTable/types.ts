export interface TableColumn {
  // 基础属性
  prop: string
  label: string
  width?: string | number
  minWidth?: string | number
  maxWidth?: string | number
  align?: 'left' | 'center' | 'right'
  
  // 排序相关
  sortable?: boolean | 'custom'
  sortBy?: string | string[] | ((row: any, index: number) => any)
  
  // 显示控制
  fixed?: boolean | 'left' | 'right'
  hidden?: boolean
  showOverflowTooltip?: boolean
  className?: string
  labelClassName?: string
  
  // 渲染相关
  render?: 'tag' | 'switch' | 'image' | 'link' | 'date' | 'number' | 'progress' | ((value: any, row: any, column: TableColumn) => string)
  slot?: string
  headerSlot?: string
  
  // 标签渲染配置
  tagType?: (value: any, row: any) => string
  tagFormatter?: (value: any, row: any) => string
  tagMap?: Record<string | number, { type: string; text: string }>
  tagEffect?: 'dark' | 'light' | 'plain'
  tagSize?: 'large' | 'default' | 'small'
  
  // 开关配置
  switchDisabled?: boolean
  
  // 图片配置
  imageSize?: string
  
  // 链接配置
  linkType?: 'primary' | 'success' | 'warning' | 'danger' | 'info'
  linkHref?: (row: any) => string
  linkTarget?: '_blank' | '_self' | '_parent' | '_top'
  linkClick?: (row: any) => void
  
  // 日期格式化配置
  dateFormat?: string
  
  // 数字格式化配置
  numberOptions?: {
    currency?: string
    percent?: boolean
    minimumFractionDigits?: number
    maximumFractionDigits?: number
  }
  
  // 进度条配置
  progressColor?: string | ((percentage: number) => string)
  showProgressText?: boolean
}

export interface TableOperation {
  key: string
  label: string
  type?: 'primary' | 'success' | 'warning' | 'danger' | 'info'
  size?: 'large' | 'default' | 'small'
  icon?: any
  text?: boolean
  link?: boolean
  isMore?: boolean
  visible?: boolean | ((row: any) => boolean)
  disabled?: (row: any) => boolean
  handler?: (row: any, index: number) => void
}

export interface SearchConfig {
  prop: string
  label: string
  type?: 'input' | 'select' | 'date' | 'daterange' | 'number' | 'textarea'
  placeholder?: string
  width?: string
  options?: Array<{ label: string; value: any }>
  defaultValue?: any
  
  // 针对不同类型的特殊配置
  inputType?: 'text' | 'number' | 'email' | 'tel' | 'url' | 'password'
  dateType?: 'date' | 'datetime' | 'year' | 'month' | 'week' | 'daterange' | 'datetimerange'
  selectProps?: {
    multiple?: boolean
    filterable?: boolean
    remote?: boolean
    remoteMethod?: (query: string) => void
    loading?: boolean
  }
}

export interface TableProps {
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

export interface TableState {
  searchParams: Record<string, any>
  selectedRows: any[]
  currentPage: number
  currentPageSize: number
}

import type { Ref } from 'vue'

export interface UseTableStateReturn {
  searchParams: Record<string, any>
  selectedRows: Ref<any[]>
  currentPage: Ref<number>
  currentPageSize: Ref<number>
  saveState: () => void
  loadState: () => void
  clearState: () => void
}
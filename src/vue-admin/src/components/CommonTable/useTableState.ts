import { ref, reactive } from 'vue'
import type { UseTableStateReturn, TableState } from './types'

// 全局状态存储，用于页面状态隔离
const globalTableStates = new Map<string, TableState>()

/**
 * 表格状态管理 Hook
 * 实现页面间状态隔离，每个页面根据 pageKey 独立管理状态
 */
export function useTableState(pageKey: string): UseTableStateReturn {
  // 创建响应式状态
  const searchParams = reactive<Record<string, any>>({})
  const selectedRows = ref<any[]>([])
  const currentPage = ref(1)
  const currentPageSize = ref(20)

  // 生成存储 key
  const getStorageKey = (key: string) => `table-state-${pageKey}-${key}`
  
  // 保存状态到 localStorage 和内存
  const saveState = () => {
    if (!pageKey) return
    
    const state: TableState = {
      searchParams: { ...searchParams },
      selectedRows: [...selectedRows.value],
      currentPage: currentPage.value,
      currentPageSize: currentPageSize.value
    }
    
    // 保存到内存
    globalTableStates.set(pageKey, state)
    
    // 保存到 localStorage（持久化）
    try {
      localStorage.setItem(getStorageKey('search'), JSON.stringify(searchParams))
      localStorage.setItem(getStorageKey('page'), currentPage.value.toString())
      localStorage.setItem(getStorageKey('pageSize'), currentPageSize.value.toString())
      
      // 选中状态通常不需要持久化，因为数据可能已经变化
      // 如果需要持久化，可以基于 rowKey 来实现
      sessionStorage.setItem(getStorageKey('selectedRows'), JSON.stringify(selectedRows.value))
    } catch (error) {
      console.warn('保存表格状态失败:', error)
    }
  }
  
  // 从 localStorage 和内存恢复状态
  const loadState = () => {
    if (!pageKey) return
    
    try {
      // 优先从内存读取（同一会话内切换页面）
      const memoryState = globalTableStates.get(pageKey)
      if (memoryState) {
        Object.assign(searchParams, memoryState.searchParams)
        selectedRows.value = [...memoryState.selectedRows]
        currentPage.value = memoryState.currentPage
        currentPageSize.value = memoryState.currentPageSize
        return
      }
      
      // 从 localStorage 恢复（页面刷新后）
      const savedSearch = localStorage.getItem(getStorageKey('search'))
      if (savedSearch) {
        const parsedSearch = JSON.parse(savedSearch)
        Object.assign(searchParams, parsedSearch)
      }
      
      const savedPage = localStorage.getItem(getStorageKey('page'))
      if (savedPage) {
        currentPage.value = parseInt(savedPage) || 1
      }
      
      const savedPageSize = localStorage.getItem(getStorageKey('pageSize'))
      if (savedPageSize) {
        currentPageSize.value = parseInt(savedPageSize) || 20
      }
      
      // 从 sessionStorage 恢复选中状态（谨慎使用）
      const savedSelectedRows = sessionStorage.getItem(getStorageKey('selectedRows'))
      if (savedSelectedRows) {
        selectedRows.value = JSON.parse(savedSelectedRows) || []
      }
      
    } catch (error) {
      console.warn('恢复表格状态失败:', error)
    }
  }
  
  // 清除状态
  const clearState = () => {
    if (!pageKey) return
    
    // 清除内存状态
    globalTableStates.delete(pageKey)
    
    // 清除持久化状态
    try {
      localStorage.removeItem(getStorageKey('search'))
      localStorage.removeItem(getStorageKey('page'))
      localStorage.removeItem(getStorageKey('pageSize'))
      sessionStorage.removeItem(getStorageKey('selectedRows'))
    } catch (error) {
      console.warn('清除表格状态失败:', error)
    }
    
    // 重置响应式状态
    Object.keys(searchParams).forEach(key => {
      delete searchParams[key]
    })
    selectedRows.value = []
    currentPage.value = 1
    currentPageSize.value = 20
  }

  return {
    searchParams,
    selectedRows,
    currentPage,
    currentPageSize,
    saveState,
    loadState,
    clearState
  }
}

// 全局清理函数，可以在应用卸载时调用
export function clearAllTableStates() {
  globalTableStates.clear()
  
  // 清理所有相关的 localStorage 和 sessionStorage
  const keysToRemove: string[] = []
  for (let i = 0; i < localStorage.length; i++) {
    const key = localStorage.key(i)
    if (key && key.startsWith('table-state-')) {
      keysToRemove.push(key)
    }
  }
  keysToRemove.forEach(key => localStorage.removeItem(key))
  
  const sessionKeysToRemove: string[] = []
  for (let i = 0; i < sessionStorage.length; i++) {
    const key = sessionStorage.key(i)
    if (key && key.startsWith('table-state-')) {
      sessionKeysToRemove.push(key)
    }
  }
  sessionKeysToRemove.forEach(key => sessionStorage.removeItem(key))
}

// 获取指定页面的状态（用于调试）
export function getTableState(pageKey: string): TableState | undefined {
  return globalTableStates.get(pageKey)
}

// 设置指定页面的状态（用于高级场景）
export function setTableState(pageKey: string, state: Partial<TableState>) {
  const existingState = globalTableStates.get(pageKey)
  const newState: TableState = {
    searchParams: {},
    selectedRows: [],
    currentPage: 1,
    currentPageSize: 20,
    ...existingState,
    ...state
  }
  globalTableStates.set(pageKey, newState)
}
// 全局加载状态管理
import { ref, readonly } from 'vue'

interface LoadingState {
  [key: string]: boolean
}

const loadingState = ref<LoadingState>({})
const globalLoading = ref(false)

export const useLoading = () => {
  // 设置特定key的加载状态
  const setLoading = (key: string, loading: boolean) => {
    loadingState.value[key] = loading
  }

  // 获取特定key的加载状态
  const getLoading = (key: string): boolean => {
    return loadingState.value[key] || false
  }

  // 设置全局加载状态
  const setGlobalLoading = (loading: boolean) => {
    globalLoading.value = loading
  }

  // 创建异步任务包装器
  const withLoading = async <T>(
    key: string,
    asyncFn: () => Promise<T>
  ): Promise<T> => {
    try {
      setLoading(key, true)
      return await asyncFn()
    } finally {
      setLoading(key, false)
    }
  }

  return {
    loadingState: readonly(loadingState),
    globalLoading: readonly(globalLoading),
    setLoading,
    getLoading,
    setGlobalLoading,
    withLoading
  }
}
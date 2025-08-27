// 全局错误处理组件
import { defineComponent, h } from 'vue'
import { ElResult, ElButton } from 'element-plus'

interface ErrorInfo {
  code: number
  message: string
  stack?: string
}

export const ErrorBoundary = defineComponent({
  name: 'ErrorBoundary',
  props: {
    fallback: {
      type: Function,
      default: null
    }
  },
  data() {
    return {
      hasError: false,
      error: null as Error | null
    }
  },
  errorCaptured(error: Error, instance, info) {
    console.error('ErrorBoundary捕获到错误:', error, info)
    this.hasError = true
    this.error = error
    
    // 发送错误报告到后端
    this.reportError({
      code: 500,
      message: error.message,
      stack: error.stack
    })
    
    return false
  },
  methods: {
    reset() {
      this.hasError = false
      this.error = null
    },
    reportError(errorInfo: ErrorInfo) {
      // 发送错误报告到监控服务
      console.log('发送错误报告:', errorInfo)
      // TODO: 实现错误报告API调用
    }
  },
  render() {
    if (this.hasError) {
      if (this.fallback) {
        return this.fallback(this.error, this.reset)
      }
      
      return h(ElResult, {
        icon: 'error',
        title: '页面出现错误',
        'sub-title': this.error?.message || '未知错误'
      }, {
        extra: () => h(ElButton, {
          type: 'primary',
          onClick: () => {
            this.reset()
            window.location.reload()
          }
        }, '重新加载')
      })
    }
    
    return this.$slots.default?.()
  }
})
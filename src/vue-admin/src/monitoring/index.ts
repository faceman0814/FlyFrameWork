export interface ErrorInfo {
  message: string
  stack?: string
  componentStack?: string
  url: string
  line?: number
  column?: number
  timestamp: number
  userAgent: string
  userId?: string
  sessionId: string
  buildVersion?: string
}

export interface PerformanceMetrics {
  cls: number | null // Cumulative Layout Shift
  fcp: number | null // First Contentful Paint  
  fid: number | null // First Input Delay
  lcp: number | null // Largest Contentful Paint
  ttfb: number | null // Time to First Byte
}

class MonitoringService {
  private sessionId: string
  private userId?: string
  private errors: ErrorInfo[] = []
  private isEnabled: boolean

  constructor() {
    this.sessionId = this.generateSessionId()
    this.isEnabled = import.meta.env.PROD
  }

  init() {
    if (!this.isEnabled) {
      console.log('🔍 监控已禁用 (开发环境)')
      return
    }

    this.initErrorHandlers()
    this.initPerformanceMonitoring()
    console.log('🔍 监控服务已启用')
  }

  private generateSessionId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2)
  }

  private initErrorHandlers() {
    // JavaScript错误监听
    window.addEventListener('error', (event) => {
      this.captureError({
        message: event.message,
        stack: event.error?.stack,
        url: event.filename,
        line: event.lineno,
        column: event.colno,
        timestamp: Date.now(),
        userAgent: navigator.userAgent,
        userId: this.userId,
        sessionId: this.sessionId
      })
    })

    // Promise rejection监听
    window.addEventListener('unhandledrejection', (event) => {
      this.captureError({
        message: `Unhandled Promise Rejection: ${event.reason}`,
        stack: event.reason?.stack,
        url: window.location.href,
        timestamp: Date.now(),
        userAgent: navigator.userAgent,
        userId: this.userId,
        sessionId: this.sessionId
      })
    })

    // 资源加载错误
    window.addEventListener('error', (event) => {
      if (event.target !== window) {
        const target = event.target as HTMLElement
        this.captureError({
          message: `Resource Load Error: ${target.tagName} ${target.getAttribute('src') || target.getAttribute('href')}`,
          url: window.location.href,
          timestamp: Date.now(),
          userAgent: navigator.userAgent,
          userId: this.userId,
          sessionId: this.sessionId
        })
      }
    }, true)
  }

  private initPerformanceMonitoring() {
    // 监控页面加载性能
    window.addEventListener('load', () => {
      setTimeout(() => {
        this.collectPerformanceMetrics()
      }, 0)
    })

    // 监控路由变化性能
    this.monitorRouteChanges()
  }

  private collectPerformanceMetrics() {
    const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming
    
    if (navigation) {
      const metrics = {
        domContentLoaded: navigation.domContentLoadedEventEnd - navigation.domContentLoadedEventStart,
        loadComplete: navigation.loadEventEnd - navigation.loadEventStart,
        domInteractive: navigation.domInteractive - navigation.fetchStart,
        networkLatency: navigation.responseEnd - navigation.fetchStart,
        renderTime: navigation.loadEventEnd - navigation.responseEnd
      }

      this.sendAnalytics({
        type: 'performance',
        metrics,
        url: window.location.href,
        timestamp: Date.now()
      })
    }

    // 收集资源性能
    const resources = performance.getEntriesByType('resource')
    const slowResources = resources.filter((resource: PerformanceEntry) => resource.duration > 1000)
    
    if (slowResources.length > 0) {
      this.sendAnalytics({
        type: 'slow-resources',
        resources: slowResources.map(resource => ({
          name: resource.name,
          duration: resource.duration
        })),
        timestamp: Date.now()
      })
    }
  }

  private monitorRouteChanges() {
    let routeStartTime = Date.now()

    // 监听路由变化
    const originalPushState = history.pushState
    const originalReplaceState = history.replaceState

    history.pushState = function(...args) {
      routeStartTime = Date.now()
      return originalPushState.apply(history, args)
    }

    history.replaceState = function(...args) {
      routeStartTime = Date.now()
      return originalReplaceState.apply(history, args)
    }

    window.addEventListener('popstate', () => {
      routeStartTime = Date.now()
    })

    // 监听DOM变化完成
    const observer = new MutationObserver(() => {
      const routeEndTime = Date.now()
      const routeTime = routeEndTime - routeStartTime
      
      if (routeTime > 0 && routeTime < 10000) { // 有效的路由切换时间
        this.sendAnalytics({
          type: 'route-performance',
          duration: routeTime,
          url: window.location.href,
          timestamp: routeEndTime
        })
      }
    })

    observer.observe(document.body, {
      childList: true,
      subtree: true
    })
  }

  // 手动捕获错误
  captureError(errorInfo: Partial<ErrorInfo>) {
    const fullErrorInfo: ErrorInfo = {
      message: errorInfo.message || 'Unknown error',
      stack: errorInfo.stack,
      componentStack: errorInfo.componentStack,
      url: errorInfo.url || window.location.href,
      line: errorInfo.line,
      column: errorInfo.column,
      timestamp: errorInfo.timestamp || Date.now(),
      userAgent: errorInfo.userAgent || navigator.userAgent,
      userId: errorInfo.userId || this.userId,
      sessionId: errorInfo.sessionId || this.sessionId
    }

    this.errors.push(fullErrorInfo)
    console.error('Captured Error:', fullErrorInfo)

    // 发送错误信息
    this.sendAnalytics({
      type: 'error',
      error: fullErrorInfo,
      timestamp: Date.now()
    })

    // 限制内存中保存的错误数量
    if (this.errors.length > 50) {
      this.errors = this.errors.slice(-30)
    }
  }

  // 捕获用户行为
  captureUserAction(action: string, data?: Record<string, any>) {
    console.log('User Action:', action, data)

    this.sendAnalytics({
      type: 'user-action',
      action,
      data,
      url: window.location.href,
      timestamp: Date.now(),
      sessionId: this.sessionId,
      userId: this.userId
    })
  }

  // 设置用户信息
  setUser(userId: string, userInfo?: Record<string, any>) {
    this.userId = userId
    
    this.sendAnalytics({
      type: 'user-identity',
      userId,
      userInfo,
      timestamp: Date.now(),
      sessionId: this.sessionId
    })
  }

  // 监控API请求
  trackApiRequest(config: {
    url: string
    method: string
    duration: number
    status: number
    size?: number
    error?: string
  }) {
    this.sendAnalytics({
      type: 'api-request',
      ...config,
      timestamp: Date.now(),
      sessionId: this.sessionId
    })

    // 记录慢请求
    if (config.duration > 3000) {
      console.warn(`⚠️ Slow API Request: ${config.method} ${config.url} took ${config.duration}ms`)
    }

    // 记录错误请求
    if (config.status >= 400) {
      console.warn(`⚠️ API Error: ${config.method} ${config.url} returned ${config.status}`)
    }
  }

  // 发送分析数据
  private sendAnalytics(data: any) {
    if (!this.isEnabled) return

    // 添加通用信息
    const payload = {
      ...data,
      sessionId: this.sessionId,
      userId: this.userId,
      buildVersion: import.meta.env.VITE_BUILD_VERSION || 'unknown',
      environment: import.meta.env.MODE
    }

    // 发送到分析服务
    try {
      // 使用 Navigator.sendBeacon 确保数据能够发送
      if ('sendBeacon' in navigator) {
        navigator.sendBeacon('/api/analytics', JSON.stringify(payload))
      } else {
        // 降级到 fetch
        fetch('/api/analytics', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(payload),
          keepalive: true
        }).catch(() => {
          // 静默失败，不影响用户体验
        })
      }
    } catch (error) {
      // 静默失败
    }

    // 开发环境下打印日志
    if (import.meta.env.DEV) {
      console.log('📊 Analytics:', payload)
    }
  }

  // 获取错误列表
  getErrors(): ErrorInfo[] {
    return [...this.errors]
  }

  // 清理错误
  clearErrors() {
    this.errors = []
  }

  // 获取会话ID
  getSessionId(): string {
    return this.sessionId
  }

  // 销毁监控服务
  destroy() {
    this.errors = []
    this.userId = undefined
  }
}

// 导出单例实例
export const monitoringService = new MonitoringService()

// Vue 插件形式
export default {
  install() {
    monitoringService.init()
  }
}
import { getCLS, getFCP, getFID, getLCP, getTTFB, type Metric } from 'web-vitals'

export interface PerformanceMetrics {
  cls: number | null // Cumulative Layout Shift
  fcp: number | null // First Contentful Paint
  fid: number | null // First Input Delay
  lcp: number | null // Largest Contentful Paint
  ttfb: number | null // Time to First Byte
}

export interface ResourceTiming {
  name: string
  duration: number
  size: number
  type: string
}

class PerformanceMonitor {
  private metrics: PerformanceMetrics = {
    cls: null,
    fcp: null,
    fid: null,
    lcp: null,
    ttfb: null
  }

  private resourceTimings: ResourceTiming[] = []
  private observers: PerformanceObserver[] = []

  init() {
    this.initWebVitals()
    this.initResourceObserver()
    this.initNavigationObserver()
    console.log('📊 性能监控已启用')
  }

  private initWebVitals() {
    getCLS((metric: Metric) => {
      this.metrics.cls = metric.value
      this.reportMetric('CLS', metric.value)
    })

    getFCP((metric: Metric) => {
      this.metrics.fcp = metric.value
      this.reportMetric('FCP', metric.value)
    })

    getFID((metric: Metric) => {
      this.metrics.fid = metric.value
      this.reportMetric('FID', metric.value)
    })

    getLCP((metric: Metric) => {
      this.metrics.lcp = metric.value
      this.reportMetric('LCP', metric.value)
    })

    getTTFB((metric: Metric) => {
      this.metrics.ttfb = metric.value
      this.reportMetric('TTFB', metric.value)
    })
  }

  private initResourceObserver() {
    if ('PerformanceObserver' in window) {
      const observer = new PerformanceObserver((list) => {
        const entries = list.getEntries()
        entries.forEach((entry) => {
          if (entry.entryType === 'resource') {
            const resourceEntry = entry as PerformanceResourceTiming
            this.resourceTimings.push({
              name: resourceEntry.name,
              duration: resourceEntry.duration,
              size: resourceEntry.transferSize || 0,
              type: this.getResourceType(resourceEntry.name)
            })
          }
        })
      })

      observer.observe({ entryTypes: ['resource'] })
      this.observers.push(observer)
    }
  }

  private initNavigationObserver() {
    if ('PerformanceObserver' in window) {
      const observer = new PerformanceObserver((list) => {
        const entries = list.getEntries()
        entries.forEach((entry) => {
          if (entry.entryType === 'navigation') {
            const navEntry = entry as PerformanceNavigationTiming
            this.reportNavigationTiming(navEntry)
          }
        })
      })

      observer.observe({ entryTypes: ['navigation'] })
      this.observers.push(observer)
    }
  }

  private getResourceType(url: string): string {
    if (url.includes('.js')) return 'script'
    if (url.includes('.css')) return 'stylesheet'
    if (url.match(/\.(png|jpg|jpeg|gif|webp|svg)$/)) return 'image'
    if (url.includes('/api/')) return 'api'
    return 'other'
  }

  private reportMetric(name: string, value: number) {
    console.log(`📊 ${name}:`, value)
    
    // 发送到监控服务
    this.sendToAnalytics({
      type: 'web-vital',
      name,
      value,
      timestamp: Date.now(),
      url: window.location.href
    })

    // 检查性能阈值
    this.checkPerformanceThreshold(name, value)
  }

  private reportNavigationTiming(entry: PerformanceNavigationTiming) {
    const metrics = {
      domContentLoaded: entry.domContentLoadedEventEnd - entry.navigationStart,
      loadComplete: entry.loadEventEnd - entry.navigationStart,
      domInteractive: entry.domInteractive - entry.navigationStart,
      firstPaint: this.getFirstPaint(),
      firstContentfulPaint: this.getFirstContentfulPaint()
    }

    console.log('📊 Navigation Timing:', metrics)
    
    this.sendToAnalytics({
      type: 'navigation-timing',
      metrics,
      timestamp: Date.now(),
      url: window.location.href
    })
  }

  private getFirstPaint(): number | null {
    const paintEntries = performance.getEntriesByType('paint')
    const fpEntry = paintEntries.find(entry => entry.name === 'first-paint')
    return fpEntry ? fpEntry.startTime : null
  }

  private getFirstContentfulPaint(): number | null {
    const paintEntries = performance.getEntriesByType('paint')
    const fcpEntry = paintEntries.find(entry => entry.name === 'first-contentful-paint')
    return fcpEntry ? fcpEntry.startTime : null
  }

  private checkPerformanceThreshold(name: string, value: number) {
    const thresholds = {
      CLS: { good: 0.1, poor: 0.25 },
      FCP: { good: 1800, poor: 3000 },
      FID: { good: 100, poor: 300 },
      LCP: { good: 2500, poor: 4000 },
      TTFB: { good: 800, poor: 1800 }
    }

    const threshold = thresholds[name as keyof typeof thresholds]
    if (threshold) {
      let rating = 'good'
      if (value > threshold.poor) {
        rating = 'poor'
      } else if (value > threshold.good) {
        rating = 'needs-improvement'
      }

      if (rating !== 'good') {
        console.warn(`⚠️ Performance Warning: ${name} is ${rating} (${value})`)
      }
    }
  }

  private sendToAnalytics(data: any) {
    // 这里可以发送到你的分析服务
    // 例如 Google Analytics, 自定义分析服务等
    if (import.meta.env.DEV) {
      console.log('📊 Performance Data:', data)
    }

    // 示例：发送到自定义分析端点
    try {
      fetch('/api/analytics/performance', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      }).catch(() => {
        // 静默失败，不影响用户体验
      })
    } catch (error) {
      // 静默失败
    }
  }

  // 手动记录性能标记
  mark(name: string) {
    performance.mark(name)
  }

  // 测量两个标记之间的时间
  measure(name: string, startMark: string, endMark: string) {
    performance.measure(name, startMark, endMark)
    const entries = performance.getEntriesByName(name, 'measure')
    if (entries.length > 0) {
      const duration = entries[entries.length - 1].duration
      console.log(`📊 ${name}:`, duration + 'ms')
      return duration
    }
    return 0
  }

  // 监控API请求性能
  trackApiRequest(url: string, method: string, duration: number, status: number) {
    this.sendToAnalytics({
      type: 'api-request',
      url,
      method,
      duration,
      status,
      timestamp: Date.now()
    })

    if (duration > 5000) {
      console.warn(`⚠️ Slow API Request: ${method} ${url} took ${duration}ms`)
    }
  }

  // 监控页面加载性能
  trackPageLoad(route: string, loadTime: number) {
    this.sendToAnalytics({
      type: 'page-load',
      route,
      loadTime,
      timestamp: Date.now()
    })
  }

  // 监控内存使用
  trackMemoryUsage() {
    if ('memory' in performance) {
      const memory = (performance as any).memory
      const memoryInfo = {
        usedJSHeapSize: memory.usedJSHeapSize,
        totalJSHeapSize: memory.totalJSHeapSize,
        jsHeapSizeLimit: memory.jsHeapSizeLimit
      }

      this.sendToAnalytics({
        type: 'memory-usage',
        ...memoryInfo,
        timestamp: Date.now()
      })

      console.log('📊 Memory Usage:', memoryInfo)
    }
  }

  // 获取当前性能指标
  getMetrics(): PerformanceMetrics {
    return { ...this.metrics }
  }

  // 获取资源加载信息
  getResourceTimings(): ResourceTiming[] {
    return [...this.resourceTimings]
  }

  // 清理观察者
  destroy() {
    this.observers.forEach(observer => observer.disconnect())
    this.observers = []
  }
}

// 导出单例实例
export const performanceMonitor = new PerformanceMonitor()

// Vue插件形式
export default {
  install() {
    performanceMonitor.init()
  }
}
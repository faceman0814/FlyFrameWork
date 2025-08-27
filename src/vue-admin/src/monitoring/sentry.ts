import * as Sentry from '@sentry/vue'
import { BrowserTracing } from '@sentry/tracing'
import type { App } from 'vue'
import type { Router } from 'vue-router'

export interface MonitoringConfig {
  dsn?: string
  environment?: string
  sampleRate?: number
  tracesSampleRate?: number
  enabled?: boolean
}

const defaultConfig: MonitoringConfig = {
  environment: import.meta.env.MODE || 'development',
  sampleRate: 1.0,
  tracesSampleRate: 0.1,
  enabled: import.meta.env.PROD
}

export function initErrorMonitoring(app: App, router: Router, config: MonitoringConfig = {}) {
  const finalConfig = { ...defaultConfig, ...config }
  
  if (!finalConfig.enabled) {
    console.log('🔍 监控已禁用 (开发环境)')
    return
  }

  // 初始化 Sentry
  Sentry.init({
    app,
    dsn: finalConfig.dsn || process.env.VITE_SENTRY_DSN,
    environment: finalConfig.environment,
    sampleRate: finalConfig.sampleRate,
    tracesSampleRate: finalConfig.tracesSampleRate,
    integrations: [
      new BrowserTracing({
        router,
        routingInstrumentation: Sentry.vueRouterInstrumentation(router),
      }),
    ],
    beforeSend: (event) => {
      // 过滤掉一些不重要的错误
      if (event.exception) {
        const error = event.exception.values?.[0]?.value || ''
        
        // 忽略网络错误
        if (error.includes('Network Error') || error.includes('timeout')) {
          return null
        }
        
        // 忽略取消的请求
        if (error.includes('canceled') || error.includes('aborted')) {
          return null
        }
        
        // 忽略脚本加载错误（通常是广告拦截器）
        if (error.includes('Script error') || error.includes('Non-Error promise rejection')) {
          return null
        }
      }
      
      return event
    }
  })
  
  console.log('🔍 错误监控已启用')
}

// 手动上报错误
export function captureError(error: Error, extra?: Record<string, any>) {
  console.error('Captured Error:', error)
  
  if (defaultConfig.enabled) {
    Sentry.captureException(error, {
      extra
    })
  }
}

// 上报用户行为
export function captureUserAction(action: string, data?: Record<string, any>) {
  console.log('User Action:', action, data)
  
  if (defaultConfig.enabled) {
    Sentry.addBreadcrumb({
      message: action,
      data,
      level: 'info',
      category: 'user-action'
    })
  }
}

// 设置用户上下文
export function setUserContext(user: {
  id: string
  username?: string
  email?: string
}) {
  if (defaultConfig.enabled) {
    Sentry.setUser(user)
  }
}

// 设置标签
export function setTag(key: string, value: string) {
  if (defaultConfig.enabled) {
    Sentry.setTag(key, value)
  }
}

// 性能监控
export function startTransaction(name: string, operation: string) {
  if (defaultConfig.enabled) {
    return Sentry.startTransaction({
      name,
      op: operation
    })
  }
  return null
}
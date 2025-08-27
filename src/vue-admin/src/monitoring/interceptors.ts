import axios, { type AxiosRequestConfig, type AxiosResponse } from 'axios'
import { monitoringService } from './index'

// 扩展 axios 配置接口
declare module 'axios' {
  interface AxiosRequestConfig {
    _startTime?: number
    _skipLogging?: boolean
  }
}

// 请求拦截器 - 记录开始时间
export const requestInterceptor = (config: AxiosRequestConfig) => {
  config._startTime = Date.now()
  
  // 记录请求开始
  if (!config._skipLogging) {
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`)
  }
  
  return config
}

// 响应拦截器 - 成功响应
export const responseSuccessInterceptor = (response: AxiosResponse) => {
  const config = response.config
  const duration = config._startTime ? Date.now() - config._startTime : 0
  
  // 获取响应大小（估算）
  const responseSize = JSON.stringify(response.data).length
  
  // 记录到监控服务
  monitoringService.trackApiRequest({
    url: config.url || 'unknown',
    method: config.method?.toUpperCase() || 'GET',
    duration,
    status: response.status,
    size: responseSize
  })
  
  // 控制台日志
  if (!config._skipLogging) {
    const statusColor = response.status < 300 ? 'color: green' : 'color: orange'
    console.log(
      `✅ API Response: %c${config.method?.toUpperCase()} ${config.url} - ${response.status} (${duration}ms)`,
      statusColor
    )
  }
  
  return response
}

// 响应拦截器 - 错误响应
export const responseErrorInterceptor = (error: any) => {
  const config = error.config || {}
  const duration = config._startTime ? Date.now() - config._startTime : 0
  const status = error.response?.status || 0
  
  // 记录到监控服务
  monitoringService.trackApiRequest({
    url: config.url || 'unknown',
    method: config.method?.toUpperCase() || 'GET',
    duration,
    status,
    error: error.message
  })
  
  // 记录错误详情
  monitoringService.captureError({
    message: `API Error: ${config.method?.toUpperCase()} ${config.url} - ${error.message}`,
    stack: error.stack,
    url: window.location.href
  })
  
  // 控制台日志
  if (!config._skipLogging) {
    console.error(`❌ API Error: ${config.method?.toUpperCase()} ${config.url}`, {
      status,
      duration: `${duration}ms`,
      error: error.message,
      response: error.response?.data
    })
  }
  
  return Promise.reject(error)
}

// 设置 axios 拦截器
export function setupAxiosInterceptors() {
  // 请求拦截器
  axios.interceptors.request.use(requestInterceptor, (error) => {
    monitoringService.captureError({
      message: `Request Setup Error: ${error.message}`,
      stack: error.stack,
      url: window.location.href
    })
    return Promise.reject(error)
  })
  
  // 响应拦截器
  axios.interceptors.response.use(
    responseSuccessInterceptor,
    responseErrorInterceptor
  )
  
  console.log('🔧 Axios 拦截器已设置')
}

// Vue Router 性能监控
export function setupRouterMonitoring(router: any) {
  let routeStartTime = Date.now()
  
  router.beforeEach((to: any, from: any, next: any) => {
    routeStartTime = Date.now()
    
    // 记录路由切换开始
    monitoringService.captureUserAction('route-start', {
      from: from.path,
      to: to.path
    })
    
    next()
  })
  
  router.afterEach((to: any, from: any) => {
    const routeDuration = Date.now() - routeStartTime
    
    // 记录路由切换完成
    monitoringService.captureUserAction('route-complete', {
      from: from.path,
      to: to.path,
      duration: routeDuration
    })
    
    // 如果路由切换过慢，记录为性能问题
    if (routeDuration > 2000) {
      console.warn(`⚠️ Slow Route Change: ${from.path} → ${to.path} took ${routeDuration}ms`)
    }
  })
  
  console.log('🔧 Router 监控已设置')
}
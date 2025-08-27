// 组件懒加载工具
import { defineAsyncComponent, AsyncComponentLoader, Component } from 'vue'
import { ElSkeleton } from 'element-plus'

interface LazyComponentOptions {
  loader: AsyncComponentLoader
  loadingComponent?: Component
  errorComponent?: Component
  delay?: number
  timeout?: number
  suspensible?: boolean
}

// 创建懒加载组件的工厂函数
export const createLazyComponent = (options: LazyComponentOptions | AsyncComponentLoader) => {
  if (typeof options === 'function') {
    options = { loader: options }
  }

  return defineAsyncComponent({
    loader: options.loader,
    loadingComponent: options.loadingComponent || ElSkeleton,
    errorComponent: options.errorComponent,
    delay: options.delay || 200,
    timeout: options.timeout || 10000,
    suspensible: options.suspensible || false
  })
}

// 路由组件懒加载
export const lazyRouteComponent = (importFn: () => Promise<any>) => {
  return createLazyComponent({
    loader: importFn,
    delay: 0, // 路由组件不需要延迟
    timeout: 15000 // 路由组件超时时间稍长
  })
}

// 普通组件懒加载
export const lazyComponent = (importFn: () => Promise<any>) => {
  return createLazyComponent({
    loader: importFn,
    delay: 200,
    timeout: 10000
  })
}
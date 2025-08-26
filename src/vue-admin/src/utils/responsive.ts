// 响应式断点配置
export const BREAKPOINTS = {
  MOBILE: 768,
  TABLET: 992,
  DESKTOP: 1200,
  LARGE: 1920
} as const

// 设备类型
export type DeviceType = 'mobile' | 'tablet' | 'desktop' | 'large'

// 检测设备类型
export function getDeviceType(): DeviceType {
  const width = window.innerWidth
  
  if (width < BREAKPOINTS.MOBILE) {
    return 'mobile'
  } else if (width < BREAKPOINTS.TABLET) {
    return 'tablet'
  } else if (width < BREAKPOINTS.DESKTOP) {
    return 'desktop'
  } else {
    return 'large'
  }
}

// 检查是否为移动设备
export function isMobile(): boolean {
  return window.innerWidth < BREAKPOINTS.MOBILE
}

// 检查是否为触摸设备
export function isTouchDevice(): boolean {
  return 'ontouchstart' in window || navigator.maxTouchPoints > 0
}

// 响应式监听器
export function useResponsive(callback: (deviceType: DeviceType) => void) {
  let currentDeviceType = getDeviceType()
  
  const handleResize = () => {
    const newDeviceType = getDeviceType()
    if (newDeviceType !== currentDeviceType) {
      currentDeviceType = newDeviceType
      callback(newDeviceType)
    }
  }

  window.addEventListener('resize', handleResize)
  
  // 初始调用
  callback(currentDeviceType)
  
  // 返回清理函数
  return () => {
    window.removeEventListener('resize', handleResize)
  }
}

// 媒体查询匹配器
export function createMediaQueryMatcher(query: string) {
  const mediaQuery = window.matchMedia(query)
  
  return {
    matches: mediaQuery.matches,
    addListener: (callback: (matches: boolean) => void) => {
      const handler = (e: MediaQueryListEvent) => callback(e.matches)
      mediaQuery.addListener(handler)
      return () => mediaQuery.removeListener(handler)
    }
  }
}
// 响应式工具增强
import { ref, onMounted, onUnmounted } from 'vue'

export type DeviceType = 'mobile' | 'tablet' | 'desktop' | 'large'
export type Orientation = 'portrait' | 'landscape'

interface ScreenInfo {
  width: number
  height: number
  deviceType: DeviceType
  orientation: Orientation
  isMobile: boolean
  isTablet: boolean
  isDesktop: boolean
  isLarge: boolean
}

// 断点配置
const breakpoints = {
  mobile: 768,
  tablet: 1024,
  desktop: 1200,
  large: 1920
}

export const useResponsive = () => {
  const screenInfo = ref<ScreenInfo>({
    width: 0,
    height: 0,
    deviceType: 'desktop',
    orientation: 'landscape',
    isMobile: false,
    isTablet: false,
    isDesktop: true,
    isLarge: false
  })

  const updateScreenInfo = () => {
    const width = window.innerWidth
    const height = window.innerHeight
    
    let deviceType: DeviceType = 'desktop'
    if (width < breakpoints.mobile) {
      deviceType = 'mobile'
    } else if (width < breakpoints.tablet) {
      deviceType = 'tablet'
    } else if (width < breakpoints.desktop) {
      deviceType = 'desktop'
    } else {
      deviceType = 'large'
    }

    const orientation: Orientation = width > height ? 'landscape' : 'portrait'

    screenInfo.value = {
      width,
      height,
      deviceType,
      orientation,
      isMobile: deviceType === 'mobile',
      isTablet: deviceType === 'tablet',
      isDesktop: deviceType === 'desktop',
      isLarge: deviceType === 'large'
    }

    // 设置CSS自定义属性
    document.documentElement.style.setProperty('--screen-width', `${width}px`)
    document.documentElement.style.setProperty('--screen-height', `${height}px`)
    document.documentElement.setAttribute('data-device', deviceType)
    document.documentElement.setAttribute('data-orientation', orientation)
  }

  const handleResize = () => {
    updateScreenInfo()
  }

  onMounted(() => {
    updateScreenInfo()
    window.addEventListener('resize', handleResize)
  })

  onUnmounted(() => {
    window.removeEventListener('resize', handleResize)
  })

  return {
    screenInfo: readonly(screenInfo)
  }
}

// 媒体查询工具
export const useMediaQuery = (query: string) => {
  const matches = ref(false)
  let mediaQuery: MediaQueryList

  const updateMatches = () => {
    matches.value = mediaQuery.matches
  }

  onMounted(() => {
    mediaQuery = window.matchMedia(query)
    updateMatches()
    mediaQuery.addEventListener('change', updateMatches)
  })

  onUnmounted(() => {
    mediaQuery?.removeEventListener('change', updateMatches)
  })

  return matches
}

// 预定义媒体查询
export const useBreakpoints = () => {
  return {
    isMobile: useMediaQuery(`(max-width: ${breakpoints.mobile - 1}px)`),
    isTablet: useMediaQuery(`(min-width: ${breakpoints.mobile}px) and (max-width: ${breakpoints.tablet - 1}px)`),
    isDesktop: useMediaQuery(`(min-width: ${breakpoints.tablet}px) and (max-width: ${breakpoints.desktop - 1}px)`),
    isLarge: useMediaQuery(`(min-width: ${breakpoints.desktop}px)`),
    isPortrait: useMediaQuery('(orientation: portrait)'),
    isLandscape: useMediaQuery('(orientation: landscape)'),
    prefersDark: useMediaQuery('(prefers-color-scheme: dark)')
  }
}
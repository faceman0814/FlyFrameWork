import { defineStore } from 'pinia'
import { getDeviceType, type DeviceType } from '@/utils/responsive'

interface AppState {
  sidebar: {
    opened: boolean
    withoutAnimation: boolean
  }
  device: DeviceType
  language: string
  tagsView: boolean
}

export const useAppStore = defineStore('app', {
  state: (): AppState => ({
    sidebar: {
      opened: localStorage.getItem('sidebarStatus') 
        ? !!+localStorage.getItem('sidebarStatus')! 
        : true,
      withoutAnimation: false
    },
    device: getDeviceType(),
    language: localStorage.getItem('language') || 'zh-cn',
    tagsView: true
  }),

  getters: {
    getSidebar: (state) => state.sidebar,
    getDevice: (state) => state.device,
    isMobile: (state) => state.device === 'mobile',
    isTablet: (state) => state.device === 'tablet',
    isDesktop: (state) => state.device === 'desktop' || state.device === 'large'
  },

  actions: {
    toggleSidebar() {
      this.sidebar.opened = !this.sidebar.opened
      this.sidebar.withoutAnimation = false
      if (this.sidebar.opened) {
        localStorage.setItem('sidebarStatus', '1')
      } else {
        localStorage.setItem('sidebarStatus', '0')
      }
    },

    closeSidebar(withoutAnimation: boolean) {
      localStorage.setItem('sidebarStatus', '0')
      this.sidebar.opened = false
      this.sidebar.withoutAnimation = withoutAnimation
    },

    openSidebar() {
      localStorage.setItem('sidebarStatus', '1')
      this.sidebar.opened = true
      this.sidebar.withoutAnimation = false
    },

    toggleDevice(device: DeviceType) {
      this.device = device
      
      // 移动设备默认收起侧边栏
      if (device === 'mobile' && this.sidebar.opened) {
        this.closeSidebar(true)
      }
    },

    setLanguage(language: string) {
      this.language = language
      localStorage.setItem('language', language)
      // 更新文档语言属性
      document.documentElement.lang = language
    }
  }
})
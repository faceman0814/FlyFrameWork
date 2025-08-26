import { defineStore } from 'pinia'

export type ThemeMode = 'light' | 'dark' | 'auto'

interface ThemeState {
  mode: ThemeMode
  isDark: boolean
}

export const useThemeStore = defineStore('theme', {
  state: (): ThemeState => {
    const savedTheme = localStorage.getItem('theme-mode') as ThemeMode || 'auto'
    return {
      mode: savedTheme,
      isDark: false
    }
  },

  getters: {
    currentTheme: (state) => state.mode,
    isDarkMode: (state) => state.isDark,
    isLightMode: (state) => !state.isDark,
    isAutoMode: (state) => state.mode === 'auto'
  },

  actions: {
    // 初始化主题
    initTheme() {
      this.applyTheme()
      
      // 监听系统主题变化（仅当模式为 auto 时）
      if (this.mode === 'auto') {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
        mediaQuery.addEventListener('change', this.handleSystemThemeChange)
      }
    },

    // 设置主题模式
    setTheme(mode: ThemeMode) {
      this.mode = mode
      localStorage.setItem('theme-mode', mode)
      this.applyTheme()
      
      // 重新设置系统主题监听
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
      mediaQuery.removeEventListener('change', this.handleSystemThemeChange)
      
      if (mode === 'auto') {
        mediaQuery.addEventListener('change', this.handleSystemThemeChange)
      }
    },

    // 切换主题
    toggleTheme() {
      const modes: ThemeMode[] = ['light', 'dark', 'auto']
      const currentIndex = modes.indexOf(this.mode)
      const nextIndex = (currentIndex + 1) % modes.length
      this.setTheme(modes[nextIndex])
    },

    // 应用主题
    applyTheme() {
      let isDark = false

      switch (this.mode) {
        case 'dark':
          isDark = true
          break
        case 'light':
          isDark = false
          break
        case 'auto':
          isDark = window.matchMedia('(prefers-color-scheme: dark)').matches
          break
      }

      this.isDark = isDark
      document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light')
      
      // 更新 meta 主题色
      this.updateMetaTheme(isDark)
    },

    // 处理系统主题变化
    handleSystemThemeChange(e: MediaQueryListEvent) {
      if (this.mode === 'auto') {
        this.isDark = e.matches
        document.documentElement.setAttribute('data-theme', e.matches ? 'dark' : 'light')
        this.updateMetaTheme(e.matches)
      }
    },

    // 更新 meta 主题色
    updateMetaTheme(isDark: boolean) {
      const metaThemeColor = document.querySelector('meta[name="theme-color"]')
      const color = isDark ? '#2d3748' : '#ffffff'
      
      if (metaThemeColor) {
        metaThemeColor.setAttribute('content', color)
      } else {
        const meta = document.createElement('meta')
        meta.name = 'theme-color'
        meta.content = color
        document.head.appendChild(meta)
      }
    }
  }
})
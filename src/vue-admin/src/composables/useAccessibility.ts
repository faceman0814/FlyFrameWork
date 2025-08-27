// 无障碍支持工具
import { ref, onMounted, onUnmounted } from 'vue'

interface A11yOptions {
  announceRouteChanges?: boolean
  skipLinks?: boolean
  focusManagement?: boolean
  keyboardNavigation?: boolean
}

export const useAccessibility = (options: A11yOptions = {}) => {
  const {
    announceRouteChanges = true,
    skipLinks = true,
    focusManagement = true,
    keyboardNavigation = true
  } = options

  const currentFocus = ref<HTMLElement | null>(null)
  const skipLinkVisible = ref(false)

  // 屏幕阅读器公告
  const announce = (message: string, priority: 'polite' | 'assertive' = 'polite') => {
    const announcer = document.createElement('div')
    announcer.setAttribute('aria-live', priority)
    announcer.setAttribute('aria-atomic', 'true')
    announcer.setAttribute('class', 'sr-only')
    announcer.textContent = message
    
    document.body.appendChild(announcer)
    
    setTimeout(() => {
      document.body.removeChild(announcer)
    }, 1000)
  }

  // 焦点管理
  const setFocus = (element: HTMLElement | string) => {
    const target = typeof element === 'string' 
      ? document.querySelector(element) as HTMLElement
      : element
    
    if (target) {
      target.focus()
      currentFocus.value = target
    }
  }

  // 焦点陷阱（用于模态框）
  const trapFocus = (container: HTMLElement) => {
    const focusableElements = container.querySelectorAll(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    ) as NodeListOf<HTMLElement>

    if (focusableElements.length === 0) return

    const firstElement = focusableElements[0]
    const lastElement = focusableElements[focusableElements.length - 1]

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Tab') {
        if (e.shiftKey) {
          if (document.activeElement === firstElement) {
            e.preventDefault()
            lastElement.focus()
          }
        } else {
          if (document.activeElement === lastElement) {
            e.preventDefault()
            firstElement.focus()
          }
        }
      }
      
      if (e.key === 'Escape') {
        container.dispatchEvent(new CustomEvent('escape'))
      }
    }

    container.addEventListener('keydown', handleKeyDown)
    firstElement.focus()

    return () => {
      container.removeEventListener('keydown', handleKeyDown)
    }
  }

  // 跳转链接处理
  const handleSkipLink = (targetId: string) => {
    const target = document.getElementById(targetId)
    if (target) {
      target.setAttribute('tabindex', '-1')
      target.focus()
      skipLinkVisible.value = false
    }
  }

  // 键盘导航
  const handleKeyboardNavigation = (e: KeyboardEvent) => {
    // Tab键显示跳转链接
    if (e.key === 'Tab' && !skipLinkVisible.value) {
      skipLinkVisible.value = true
    }
    
    // ESC键隐藏跳转链接
    if (e.key === 'Escape') {
      skipLinkVisible.value = false
    }
  }

  // 检查颜色对比度
  const checkColorContrast = (foreground: string, background: string): number => {
    const getLuminance = (color: string): number => {
      const rgb = parseInt(color.replace('#', ''), 16)
      const r = (rgb >> 16) & 0xff
      const g = (rgb >> 8) & 0xff  
      const b = (rgb >> 0) & 0xff
      
      const [rLum, gLum, bLum] = [r, g, b].map(c => {
        c = c / 255
        return c <= 0.03928 ? c / 12.92 : Math.pow((c + 0.055) / 1.055, 2.4)
      })
      
      return 0.2126 * rLum + 0.7152 * gLum + 0.0722 * bLum
    }
    
    const fgLum = getLuminance(foreground)
    const bgLum = getLuminance(background)
    const brightest = Math.max(fgLum, bgLum)
    const darkest = Math.min(fgLum, bgLum)
    
    return (brightest + 0.05) / (darkest + 0.05)
  }

  onMounted(() => {
    if (keyboardNavigation) {
      document.addEventListener('keydown', handleKeyboardNavigation)
    }

    // 添加跳转链接到页面
    if (skipLinks) {
      const skipLink = document.createElement('a')
      skipLink.href = '#main-content'
      skipLink.textContent = '跳转到主要内容'
      skipLink.className = 'skip-link'
      skipLink.style.cssText = `
        position: absolute;
        top: -40px;
        left: 6px;
        background: #000;
        color: #fff;
        padding: 8px;
        text-decoration: none;
        border-radius: 4px;
        z-index: 10000;
        transition: top 0.3s;
      `
      
      skipLink.addEventListener('focus', () => {
        skipLink.style.top = '6px'
      })
      
      skipLink.addEventListener('blur', () => {
        skipLink.style.top = '-40px'
      })
      
      skipLink.addEventListener('click', (e) => {
        e.preventDefault()
        handleSkipLink('main-content')
      })
      
      document.body.insertBefore(skipLink, document.body.firstChild)
    }
  })

  onUnmounted(() => {
    if (keyboardNavigation) {
      document.removeEventListener('keydown', handleKeyboardNavigation)
    }
  })

  return {
    currentFocus,
    skipLinkVisible,
    announce,
    setFocus,
    trapFocus,
    handleSkipLink,
    checkColorContrast
  }
}
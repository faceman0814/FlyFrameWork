<template>
  <div class="theme-switcher">
    <el-tooltip 
      :content="getTooltipText()" 
      placement="bottom"
      effect="dark"
    >
      <div 
        class="theme-toggle" 
        @click="toggleTheme"
      >
        <el-icon>
          <component :is="getThemeIcon()" />
        </el-icon>
      </div>
    </el-tooltip>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Sunny, Moon, Monitor } from '@element-plus/icons-vue'
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

const currentTheme = computed(() => themeStore.currentTheme)
const isDark = computed(() => themeStore.isDarkMode)

const getThemeIcon = () => {
  switch (currentTheme.value) {
    case 'light':
      return Sunny
    case 'dark':
      return Moon
    case 'auto':
      return Monitor
    default:
      return Sunny
  }
}

const getTooltipText = () => {
  switch (currentTheme.value) {
    case 'light':
      return '当前: 浅色模式'
    case 'dark':
      return '当前: 深色模式'
    case 'auto':
      return `当前: 跟随系统 (${isDark.value ? '深色' : '浅色'})`
    default:
      return '切换主题'
  }
}

const toggleTheme = () => {
  themeStore.toggleTheme()
}
</script>

<style lang="scss" scoped>
.theme-switcher {
  display: flex;
  align-items: center;
  
  .theme-toggle {
    position: relative;
    overflow: hidden;
    
    &::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: var(--primary-gradient);
      opacity: 0;
      transition: opacity 0.3s ease;
      border-radius: 50%;
    }
    
    &:hover::before {
      opacity: 0.1;
    }
    
    .el-icon {
      position: relative;
      z-index: 1;
      transition: all 0.3s ease;
    }
    
    &:hover .el-icon {
      color: var(--primary-color);
    }
  }
}
</style>
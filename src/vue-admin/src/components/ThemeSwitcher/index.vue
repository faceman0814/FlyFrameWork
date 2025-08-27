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
import { useI18n } from 'vue-i18n'
import { Sunny, Moon, Monitor } from '@element-plus/icons-vue'
import { useThemeStore } from '@/stores/theme'

const { t } = useI18n()
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
      return t('theme.light')
    case 'dark':
      return t('theme.dark')
    case 'auto':
      return `${t('theme.auto')} (${isDark.value ? t('theme.dark') : t('theme.light')})`
    default:
      return t('theme.light')
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
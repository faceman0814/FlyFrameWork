<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useThemeStore } from '@/stores/theme'

const { locale } = useI18n()
const themeStore = useThemeStore()

onMounted(() => {
  // 设置页面语言属性
  document.documentElement.lang = locale.value
  
  // 初始化主题系统
  themeStore.initTheme()
})
</script>

<style lang="scss">
#app {
  height: 100vh;
  width: 100vw;
  background: var(--bg-main);
  color: var(--text-primary);
  transition: background-color 0.3s ease, color 0.3s ease;
}

html,
body {
  margin: 0;
  padding: 0;
  height: 100%;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  background: var(--bg-main);
}

* {
  box-sizing: border-box;
}
</style>
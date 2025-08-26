<template>
  <Teleport to="body">
    <div v-if="isLoading" class="route-loading-indicator show">
      <div class="loading-icon"></div>
      <span>{{ loadingText }}</span>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const isLoading = ref(false)
const loadingText = ref('加载中...')
const loadingTimer = ref<NodeJS.Timeout>()

// 加载文本列表
const loadingTexts = [
  '正在切换页面...',
  '加载页面资源...',
  '初始化组件...',
  '即将完成...'
]

let textIndex = 0

const startLoading = () => {
  isLoading.value = true
  textIndex = 0
  loadingText.value = loadingTexts[textIndex]
  
  // 循环显示不同的加载文本
  loadingTimer.value = setInterval(() => {
    textIndex = (textIndex + 1) % loadingTexts.length
    loadingText.value = loadingTexts[textIndex]
  }, 800)
}

const stopLoading = () => {
  if (loadingTimer.value) {
    clearInterval(loadingTimer.value)
  }
  
  // 显示完成状态，然后隐藏
  loadingText.value = '加载完成'
  
  setTimeout(() => {
    isLoading.value = false
  }, 300)
}

// 监听路由变化
watch(route, (newRoute, oldRoute) => {
  if (newRoute.path !== oldRoute?.path) {
    startLoading()
  }
}, { immediate: false })

// 暴露方法供外部调用
defineExpose({
  startLoading,
  stopLoading
})
</script>

<style lang="scss" scoped>
// 样式已在nprogress.scss中定义
</style>
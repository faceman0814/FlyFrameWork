<template>
  <section class="app-main">
    <div class="app-main-inner">
      <router-view v-slot="{ Component, route }">
        <transition name="fade-transform" mode="out-in">
          <keep-alive :include="cachedViews">
            <component :is="Component" :key="route.path" />
          </keep-alive>
        </transition>
      </router-view>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useTagsViewStore } from '@/stores/tagsView'

const tagsViewStore = useTagsViewStore()
const cachedViews = computed(() => tagsViewStore.cachedViews)
</script>

<style lang="scss" scoped>
.app-main {
  min-height: calc(100vh - 60px);
  width: 100%;
  position: relative;
  background: var(--bg-main) !important;
  transition: background-color 0.3s ease;
  overflow: hidden; // 防止外层滚动
  
  .app-main-inner {
    height: calc(100vh - 60px);
    overflow-y: auto;
    overflow-x: hidden;
    padding: 20px;
    background: var(--bg-main) !important;
    transition: background-color 0.3s ease;
    
    // 自定义滚动条样式
    &::-webkit-scrollbar {
      width: 8px;
    }
    
    &::-webkit-scrollbar-track {
      background: var(--bg-main);
      border-radius: 4px;
    }
    
    &::-webkit-scrollbar-thumb {
      background: var(--border-base);
      border-radius: 4px;
      transition: background 0.3s ease;
      
      &:hover {
        background: var(--border-dark);
      }
    }
    
    // 火狐滚动条
    scrollbar-width: thin;
    scrollbar-color: var(--border-base) var(--bg-main);
  }
  
  // 移动端适配
  @media (max-width: 768px) {
    min-height: calc(100vh - 56px);
    
    .app-main-inner {
      height: calc(100vh - 56px);
      padding: 15px;
    }
  }
  
  @media (max-width: 480px) {
    .app-main-inner {
      padding: 10px;
    }
  }
}

// 页面过渡动画
.fade-transform-leave-active,
.fade-transform-enter-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateY(20px) scale(0.98);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateY(-20px) scale(1.02);
}

.fade-transform-enter-to,
.fade-transform-leave-from {
  opacity: 1;
  transform: translateY(0) scale(1);
}
</style>
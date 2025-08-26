<template>
  <div :class="classObj" class="app-wrapper" @click="handleClickOutside">
    <!-- 移动端遮罩层 -->
    <div v-if="device === 'mobile' && sidebar.opened" class="drawer-bg" @click="handleClickOutside" />
    
    <!-- 侧边栏 -->
    <sidebar class="sidebar-container" />
    
    <!-- 主内容区 -->
    <div class="main-container">
      <!-- 头部导航 -->
      <navbar />
      
      <!-- 标签页 -->
      <tags-view v-if="needTagsView" />
      
      <!-- 页面内容 -->
      <app-main />
    </div>
    
    <!-- 路由加载指示器 -->
    <route-loading-indicator ref="loadingIndicatorRef" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onBeforeUnmount, ref } from 'vue'
import Sidebar from './components/Sidebar/index.vue'
import Navbar from './components/Navbar.vue'
import AppMain from './components/AppMain.vue'
import TagsView from './components/TagsView/index.vue'
import RouteLoadingIndicator from '@/components/RouteLoadingIndicator/index.vue'
import { useAppStore } from '@/stores/app'
import { useResponsive, type DeviceType } from '@/utils/responsive'

const appStore = useAppStore()
const loadingIndicatorRef = ref<InstanceType<typeof RouteLoadingIndicator>>()

const sidebar = computed(() => appStore.sidebar)
const device = computed(() => appStore.device)
const needTagsView = computed(() => appStore.tagsView)

const classObj = computed(() => ({
  hideSidebar: !sidebar.value.opened,
  openSidebar: sidebar.value.opened,
  withoutAnimation: sidebar.value.withoutAnimation,
  mobile: device.value === 'mobile',
  tablet: device.value === 'tablet',
  desktop: appStore.isDesktop
}))

let cleanupResponsive: (() => void) | null = null

const handleDeviceChange = (deviceType: DeviceType) => {
  appStore.toggleDevice(deviceType)
}

const handleClickOutside = () => {
  if (device.value === 'mobile' && sidebar.value.opened) {
    appStore.closeSidebar(false)
  }
}

onMounted(() => {
  // 设置响应式监听
  cleanupResponsive = useResponsive(handleDeviceChange)
})

onBeforeUnmount(() => {
  if (cleanupResponsive) {
    cleanupResponsive()
  }
})
</script>

<style lang="scss" scoped>
.app-wrapper {
  position: relative;
  height: 100vh;
  width: 100%;
  background: var(--bg-main);
  
  &.mobile {
    &.openSidebar {
      position: fixed;
      top: 0;
    }
  }
}

.drawer-bg {
  background: rgba(0, 0, 0, 0.3);
  width: 100%;
  top: 0;
  height: 100%;
  position: absolute;
  z-index: 999;
}

.sidebar-container {
  transition: width 0.28s ease-in-out;
  width: 240px;
  height: 100%;
  position: fixed;
  font-size: 0px;
  top: 0;
  bottom: 0;
  left: 0;
  z-index: 1001;
  overflow: hidden;
  background: var(--sidebar-bg);
  border-right: 1px solid var(--sidebar-border);
  box-shadow: var(--shadow-base);

  // 桌面端收起状态
  .hideSidebar & {
    width: 54px !important; // 调整收缩宽度，与菜单项对齐
  }

  .mobile & {
    transition: transform 0.28s ease-in-out;
    width: 240px !important;
  }

  .mobile.hideSidebar & {
    transform: translateX(-240px);
  }
  
  .mobile.openSidebar & {
    transform: translateX(0);
  }

  // 菜单文字动画
  .hideSidebar & {
    :deep(.el-submenu__title),
    :deep(.el-menu-item) {
      span {
        opacity: 0;
        transition: opacity 0.15s ease;
      }
    }
    
    :deep(.sidebar-logo-title) {
      opacity: 0;
      transition: opacity 0.15s ease;
    }
  }

  // 展开时文字显示
  :deep(.el-submenu__title),
  :deep(.el-menu-item) {
    span {
      opacity: 1;
      transition: opacity 0.3s ease 0.1s;
    }
  }
}

.main-container {
  min-height: 100vh;
  transition: margin-left 0.28s ease-in-out;
  margin-left: 240px;
  position: relative;
  background: var(--bg-main);

  .hideSidebar & {
    margin-left: 54px; // 与侧边栏宽度保持一致
  }

  .mobile & {
    margin-left: 0;
  }
}

// 移动端适配
@media screen and (max-width: 992px) {
  .sidebar-container {
    transition: transform 0.28s ease-in-out;
    width: 240px !important;
  }

  .main-container {
    margin-left: 0;
  }
}

// 平板适配
@media screen and (max-width: 768px) {
  .app-wrapper {
    :deep(.navbar) {
      padding: 0 15px;
    }
    
    :deep(.tags-view-container) {
      padding: 0 15px;
    }
    
    :deep(.app-main) {
      padding: 15px;
    }
  }
}

// 手机适配
@media screen and (max-width: 480px) {
  .app-wrapper {
    :deep(.navbar) {
      padding: 0 12px;
      
      .hamburger-container {
        float: left;
        height: 50px;
        line-height: 50px;
        padding: 0 8px;
      }

      .breadcrumb-container {
        float: right;
        .el-breadcrumb {
          display: inline-block;
          font-size: 12px;
          line-height: 50px;
          margin-left: 8px;

          .no-redirect {
            color: var(--text-secondary);
            cursor: text;
          }
        }
      }
    }

    :deep(.app-main) {
      padding: 10px;
    }
  }
}

// 确保无动画切换
.withoutAnimation {
  .sidebar-container {
    transition: none !important;
  }

  .main-container {
    transition: none !important;
  }
}
</style>
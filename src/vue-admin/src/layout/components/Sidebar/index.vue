<template>
  <div :class="{'has-logo': showLogo}" class="sidebar-wrapper">
    <logo v-if="showLogo" :collapse="isCollapse"/>
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="String(activeMenu)"
        :collapse="isCollapse"
        :background-color="'transparent'"
        :text-color="'inherit'"
        :unique-opened="false"
        :active-text-color="'inherit'"
        :collapse-transition="false"
        mode="vertical"
        class="sidebar-menu"
      >
        <sidebar-item 
          v-for="route in routes" 
          :key="route.path" 
          :item="route" 
          :base-path="route.path" 
        />
      </el-menu>
    </el-scrollbar>
  </div>
</template>

<script setup lang="ts">
import { computed, provide } from 'vue'
import { useRoute } from 'vue-router'
import Logo from './Logo.vue'
import SidebarItem from './SidebarItem.vue'
import { useAppStore } from '@/stores/app'
import { useUserStore } from '@/stores/user'

const route = useRoute()
const appStore = useAppStore()
const userStore = useUserStore()

const showLogo = computed(() => true)
const isCollapse = computed(() => !appStore.sidebar.opened)
const routes = computed(() => {
  console.log('侧边栏路由:', userStore.dynamicRoutes)
  return userStore.dynamicRoutes.filter(route => !route.meta?.hidden)
})
const activeMenu = computed(() => {
  const { meta, path } = route
  if (meta?.activeMenu) {
    return meta.activeMenu
  }
  return path
})

// 提供收缩状态给子组件
provide('sidebarCollapse', isCollapse)
</script>

<style lang="scss" scoped>
.sidebar-wrapper {
  height: 100%;
  display: flex;
  flex-direction: column;
  background: var(--sidebar-bg);
  position: relative;
  
  // 添加现代化的边框和阴影效果
  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: var(--sidebar-bg);
    z-index: -1;
  }
  
  // 右侧边框
  &::after {
    content: '';
    position: absolute;
    top: 0;
    right: 0;
    width: 1px;
    height: 100%;
    background: linear-gradient(180deg, 
      transparent 0%, 
      var(--sidebar-border) 20%, 
      var(--sidebar-border) 80%, 
      transparent 100%);
    opacity: 0.8;
  }
  
  &.has-logo {
    :deep(.el-scrollbar) {
      height: calc(100% - 54px); // 对应logo高度的调整  - 60px);    
    } 
    
    :deep(.scrollbar-wrapper) {
      height: 100%;
    }
  }
  
  :deep(.el-scrollbar) {
    height: 100%;
    background: transparent;
    
    .el-scrollbar__wrap {
      overflow-x: hidden;
    }
    
    .el-scrollbar__bar.is-vertical {
      right: 4px;
      width: 4px;
      
      .el-scrollbar__thumb {
        background: linear-gradient(180deg, 
          rgba(102, 126, 234, 0.3) 0%, 
          rgba(118, 75, 162, 0.4) 100%);
        border-radius: 2px;
        transition: all 0.3s ease;
        
        &:hover {
          background: linear-gradient(180deg, 
            rgba(102, 126, 234, 0.5) 0%, 
            rgba(118, 75, 162, 0.6) 100%);
          width: 6px;
        }
      }
    }
  }
}

.sidebar-menu {
  border: none;
  height: 100%;
  width: 100% !important;
  background: transparent !important;
  padding: 8px 0;
  
  :deep(.el-menu-item),
  :deep(.el-submenu__title) {
    height: 48px;
    line-height: 48px;
    border-radius: 12px;
    margin: 3px 16px;
    padding: 0 20px; // 与logo的padding保持一致
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    color: var(--sidebar-text) !important;
    background: var(--sidebar-item-bg) !important;
    border: 1px solid var(--sidebar-item-border) !important;
    backdrop-filter: blur(12px);
    position: relative;
    overflow: hidden;
    font-weight: 500;
    letter-spacing: 0.3px;
    
    // 简化阴影，更清爽
    box-shadow: 0 1px 3px rgba(102, 126, 234, 0.04);
    
    // 添加渐变高光效果
    &::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      height: 1px;
      background: linear-gradient(90deg, 
        transparent 0%, 
        rgba(102, 126, 234, 0.3) 50%, 
        transparent 100%);
      opacity: 0;
      transition: opacity 0.3s ease;
    }
    
    &:hover {
      background: var(--sidebar-hover-bg) !important;
      color: var(--sidebar-active-text) !important;
      border-color: rgba(102, 126, 234, 0.25) !important;
      transform: translateX(4px) scale(1.02);
      box-shadow: 0 4px 8px rgba(102, 126, 234, 0.08);
      
      &::before {
        opacity: 1;
      }
      
      .el-icon {
        color: var(--sidebar-active-text) !important;
        transform: scale(1.1);
      }
    }
    
    &.is-active {
      background: var(--primary-gradient) !important;
      color: #ffffff !important;
      border: 1px solid transparent !important;
      transform: translateX(6px);
      font-weight: 600;
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
      
      &::after {
        content: '';
        position: absolute;
        right: 12px;
        top: 50%;
        transform: translateY(-50%);
        width: 4px;
        height: 24px;
        background: rgba(255, 255, 255, 0.9);
        border-radius: 2px;
        box-shadow: 0 0 6px rgba(255, 255, 255, 0.4);
      }
      
      &::before {
        opacity: 0;
      }
      
      .el-icon {
        color: #ffffff !important;
        transform: scale(1.15);
      }
    }
    
    .el-icon {
      color: var(--sidebar-text) !important;
      font-size: 18px;
      margin-right: 12px !important;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      filter: drop-shadow(0 1px 2px rgba(102, 126, 234, 0.1));
    }
    
    // 文字样式优化
    span {
      font-size: 14px;
      font-weight: inherit;
      transition: all 0.3s ease;
    }
  }
  
  :deep(.el-submenu) {
    .el-submenu__title {
      &:hover {
        .el-icon {
          color: var(--sidebar-active-text) !important;
        }
        
        .el-submenu__icon-arrow {
          color: var(--sidebar-active-text) !important;
          transform: rotate(90deg) scale(1.1);
        }
      }
      
      .el-submenu__icon-arrow {
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        color: var(--sidebar-text) !important;
        font-size: 12px;
      }
    }
    
    &.is-opened {
      .el-submenu__title {
        background: rgba(102, 126, 234, 0.08) !important;
        color: var(--sidebar-active-text) !important;
        
        .el-submenu__icon-arrow {
          transform: rotate(90deg);
          color: var(--sidebar-active-text) !important;
        }
      }
    }
    
    .el-menu {
      background: rgba(102, 126, 234, 0.02) !important;
      border-radius: 0 0 12px 12px;
      margin: -1px 16px 6px 16px;
      padding: 8px 0 4px 0;
      border: none;
      backdrop-filter: blur(4px);
      position: relative;
      
      // 移除阴影，使用更清爽的分隔线
      &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 12px;
        right: 12px;
        height: 1px;
        background: linear-gradient(90deg, 
          transparent 0%, 
          rgba(102, 126, 234, 0.1) 50%, 
          transparent 100%);
      }
      
      .el-menu-item {
        height: 36px;
        line-height: 36px;
        background: transparent !important;
        padding-left: 48px !important;
        margin: 1px 12px;
        border-radius: 6px;
        position: relative;
        font-size: 13px;
        font-weight: 450;
        border: none !important;
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        
        // 简化的指示点，不使用复杂阴影
        &::before {
          content: '';
          position: absolute;
          left: 20px;
          top: 50%;
          transform: translateY(-50%);
          width: 4px;
          height: 4px;
          background: var(--sidebar-text);
          border-radius: 50%;
          opacity: 0.4;
          transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        &:hover {
          background: rgba(102, 126, 234, 0.06) !important;
          color: var(--sidebar-active-text) !important;
          transform: translateX(2px);
          
          &::before {
            background: var(--primary-color);
            transform: translateY(-50%) scale(1.5);
            opacity: 0.8;
          }
        }
        
        &.is-active {
          background: rgba(102, 126, 234, 0.1) !important;
          color: var(--primary-color) !important;
          font-weight: 600;
          transform: translateX(4px);
          
          &::after {
            display: none;
          }
          
          &::before {
            background: var(--primary-color);
            transform: translateY(-50%) scale(2);
            opacity: 1;
            box-shadow: 0 0 6px rgba(102, 126, 234, 0.4);
          }
        }
      }
    }
  }
  
  // 收缩状态样式
  &.el-menu--collapse {
    :deep(.el-menu-item),
    :deep(.el-submenu__title) {
      margin: 4px 6px; // 恢复较小的边距，确保图标完全居中
      justify-content: center;
      padding: 0;
      color: var(--sidebar-text) !important; // 确保文字颜色正确
      
      .el-icon {
        margin-right: 0 !important;
        transform: scale(1.1);
        color: var(--sidebar-text) !important; // 图标颜色
      }
      
      span {
        opacity: 0;
        transform: translateX(-10px);
        color: var(--sidebar-text) !important; // 文字颜色
      }
      
      &:hover {
        transform: translateX(0) scale(1.02);
        color: var(--sidebar-active-text) !important;
        
        .el-icon {
          transform: scale(1.2);
          color: var(--sidebar-active-text) !important;
        }
        
        span {
          color: var(--sidebar-active-text) !important;
        }
      }
      
      &.is-active {
        transform: translateX(0);
        color: var(--sidebar-active-text) !important; // 活跃状态文字颜色
        
        .el-icon {
          color: var(--sidebar-active-text) !important; // 活跃状态图标颜色
        }
        
        span {
          color: var(--sidebar-active-text) !important; // 活跃状态文字颜色
        }
        
        // 收缩状态下隐藏右侧指示条，避免与图标重叠
        &::after {
          display: none !important;
        }
        
        // 收缩状态下使用左侧指示条
        &::before {
          content: '';
          position: absolute;
          left: 0;
          top: 50% !important;
          bottom: unset !important;
          transform: translateY(-50%);
          width: 4px;
          height: 28px;
          background: linear-gradient(180deg, #ffffff 0%, rgba(255, 255, 255, 0.8) 100%);
          border-radius: 0 3px 3px 0;
          box-shadow: 0 0 8px rgba(255, 255, 255, 0.5), 0 2px 4px rgba(102, 126, 234, 0.3);
          opacity: 1;
          z-index: 2;
        }
        
        .el-icon {
          transform: scale(1.25);
          color: #ffffff !important;
        }
      }
    }
    
    :deep(.el-submenu) {
      .el-submenu__icon-arrow {
        display: none;
      }
      
      .el-menu {
        display: none !important;
      }
    }
  }
  
  // 菜单项进入动画
  :deep(.el-menu-item),
  :deep(.el-submenu__title) {
    animation: slideInRight 0.3s ease-out;
    animation-fill-mode: both;
    
    @for $i from 1 through 10 {
      &:nth-child(#{$i}) {
        animation-delay: #{$i * 0.05}s;
      }
    }
  }
}

@keyframes slideInRight {
  from {
    transform: translateX(-30px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

// 隐藏滚动条并增加更多交互效果
:deep(.scrollbar-wrapper) {
  overflow-x: hidden !important;
  
  &::-webkit-scrollbar {
    width: 4px;
    height: 4px;
  }
  
  &::-webkit-scrollbar-track {
    background: transparent;
    border-radius: 2px;
  }
  
  &::-webkit-scrollbar-thumb {
    background: linear-gradient(180deg, 
      rgba(102, 126, 234, 0.2) 0%, 
      rgba(118, 75, 162, 0.3) 100%);
    border-radius: 2px;
    transition: all 0.3s ease;
    
    &:hover {
      background: linear-gradient(180deg, 
        rgba(102, 126, 234, 0.4) 0%, 
        rgba(118, 75, 162, 0.5) 100%);
    }
  }
  
  // 鼠标悬停时显示滚动条
  &:hover {
    &::-webkit-scrollbar-thumb {
      background: linear-gradient(180deg, 
        rgba(102, 126, 234, 0.3) 0%, 
        rgba(118, 75, 162, 0.4) 100%);
    }
  }
}

// 添加菜单容器的微妙动效
.sidebar-menu {
  &:hover {
    :deep(.el-menu-item),
    :deep(.el-submenu__title) {
      &:not(:hover) {
        opacity: 0.85;
        transform: translateX(0) scale(0.98);
      }
    }
  }
}
</style>
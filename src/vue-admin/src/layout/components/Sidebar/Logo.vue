<template>
  <div class="sidebar-logo-container" :class="{'collapse': collapse}">
    <router-link class="sidebar-logo-link" to="/">
      <div class="logo-icon">
        <div class="logo-inner"></div>
      </div>
      <span v-if="!collapse" class="sidebar-title">{{ title }}</span>
    </router-link>
  </div>
</template>

<script setup lang="ts">
interface Props {
  collapse: boolean
}

defineProps<Props>()

const title = 'FlyFramework'
</script>

<style lang="scss" scoped>
.sidebar-logo-container {
  position: relative;
  width: 100%;
  height: 54px; // 调整高度使其更协调
  background: transparent;
  border-bottom: 1px solid var(--sidebar-border);
  display: flex;
  align-items: center;
  justify-content: flex-start; // 默认左对齐
  overflow: hidden;

  .sidebar-logo-link {
    height: 100%;
    width: 100%;
    display: flex;
    align-items: center;
    text-decoration: none;
    padding: 0 20px; // 稍微增加左右内边距，让logo与菜单项对齐更好
    transition: all 0.3s ease;

    .logo-icon {
      width: 36px;
      height: 36px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, 
        rgba(102, 126, 234, 0.12) 0%, 
        rgba(118, 75, 162, 0.1) 100%);
      border: 1px solid rgba(102, 126, 234, 0.2);
      border-radius: 10px;
      // margin-right: 10px;
      margin-top: 5%;
      transition: all 0.3s ease;
      flex-shrink: 0;
      position: relative;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(102, 126, 234, 0.08);
      
      // 修复图标内部结构 - 确保完全居中
      .logo-inner {
        width: 20px;
        height: 20px;
        background: var(--primary-gradient);
        border-radius: 5px;
        position: relative;
        display: flex;
        align-items: center;
        justify-content: center;
        
        &::before {
          content: 'F';
          color: #ffffff;
          font-size: 13px;
          font-weight: 800;
          font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
          text-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
          line-height: 1;
          display: flex;
          align-items: center;
          justify-content: center;
          width: 100%;
          height: 100%;
          position: absolute;
          top: 0;
          left: 0;
        }
        
        &::after {
          content: '';
          position: absolute;
          top: 3px;
          right: 3px;
          width: 3px;
          height: 3px;
          background: rgba(255, 255, 255, 0.9);
          border-radius: 50%;
          box-shadow: 0 0 6px rgba(255, 255, 255, 0.6);
        }
      }
      
      // 添加微妙的光效
      &::before {
        content: '';
        position: absolute;
        top: -50%;
        left: -50%;
        width: 200%;
        height: 200%;
        background: linear-gradient(45deg, 
          transparent 30%, 
          rgba(255, 255, 255, 0.1) 50%, 
          transparent 70%);
        transform: translateX(-100%) translateY(-100%) rotate(45deg);
        transition: transform 0.6s ease;
      }
    }

    .sidebar-title {
      font-weight: 700;
      font-size: 16px;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      white-space: nowrap;
      transition: all 0.3s ease;
      background: var(--primary-gradient);
      -webkit-background-clip: text;
      background-clip: text;
      -webkit-text-fill-color: transparent;
    }

    &:hover {
      .logo-icon {
        transform: scale(1.05);
        background: linear-gradient(135deg, 
          rgba(102, 126, 234, 0.18) 0%, 
          rgba(118, 75, 162, 0.15) 100%);
        border-color: rgba(102, 126, 234, 0.3);
        box-shadow: 0 6px 16px rgba(102, 126, 234, 0.2);
        
        &::before {
          transform: translateX(100%) translateY(-100%) rotate(45deg);
        }
        
        .logo-inner {
          transform: scale(1.08);
          box-shadow: 0 3px 12px rgba(102, 126, 234, 0.25);
          
          &::after {
            opacity: 1;
            transform: scale(1.3);
            box-shadow: 0 0 8px rgba(255, 255, 255, 0.8);
          }
        }
      }
      
      .sidebar-title {
        transform: translateX(2px);
      }
    }
  }

  &.collapse {
    justify-content: center; // 收缩时居中对齐
    
    .sidebar-logo-link {
      justify-content: center;
      padding: 0;
      
      .logo-icon {
        // margin-right: 0;
        margin-top: 20%;
        width: 28px; // 进一步缩小以匹配菜单图标大小
        height: 28px;
        border-radius: 8px;
        
        .logo-inner {
          width: 16px;
          height: 16px;
          border-radius: 4px;
          
          &::before {
            font-size: 11px;
          }
          
          &::after {
            top: 2px;
            right: 2px;
            width: 2px;
            height: 2px;
          }
        }
      }
    }
  }
}
</style>
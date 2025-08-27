<template>
  <div class="navbar">
    <hamburger
      id="hamburger-container"
      :is-active="sidebar.opened"
      class="hamburger-container"
      @toggleClick="toggleSidebar"
    />

    <breadcrumb id="breadcrumb-container" class="breadcrumb-container" />

    <div class="right-menu">
      <template v-if="device !== 'mobile'">
        <!-- 主题切换 -->
        <theme-switcher class="right-menu-item" />
        
        <el-dropdown class="right-menu-item" trigger="click" @command="handleSetLanguage">
          <div class="icon-wrapper">
            <el-icon><Operation /></el-icon>
          </div>
          <template #dropdown>
            <el-dropdown-menu class="language-dropdown">
              <el-dropdown-item command="zh-cn" :class="{ 'is-active': locale === 'zh-cn' }">
                <span class="flag">🇨🇳</span>
                <span class="text">中文简体</span>
              </el-dropdown-item>
              <el-dropdown-item command="en" :class="{ 'is-active': locale === 'en' }">
                <span class="flag">🇺🇸</span>
                <span class="text">English</span>
              </el-dropdown-item>
              <el-dropdown-item command="ja" :class="{ 'is-active': locale === 'ja' }">
                <span class="flag">🇯🇵</span>
                <span class="text">日本語</span>
              </el-dropdown-item>
              <el-dropdown-item command="ru" :class="{ 'is-active': locale === 'ru' }">
                <span class="flag">🇷🇺</span>
                <span class="text">Русский</span>
              </el-dropdown-item>
              <el-dropdown-item command="fr" :class="{ 'is-active': locale === 'fr' }">
                <span class="flag">🇫🇷</span>
                <span class="text">Français</span>
              </el-dropdown-item>
              <el-dropdown-item command="es" :class="{ 'is-active': locale === 'es' }">
                <span class="flag">🇪🇸</span>
                <span class="text">Español</span>
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </template>

      <!-- 用户下拉菜单 -->
      <el-dropdown class="right-menu-item" trigger="click" @command="handleCommand">
        <div class="avatar-wrapper">
          <el-avatar :src="avatar" :size="32">
            {{ name.charAt(0) }}
          </el-avatar>
          <span class="user-name">{{ name }}</span>
          <el-icon class="el-icon--right"><CaretBottom /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu class="user-dropdown">
            <el-dropdown-item command="profile">
              <el-icon><User /></el-icon>
              <span>{{ $t('navbar.profile') }}</span>
            </el-dropdown-item>
            <el-dropdown-item command="settings">
              <el-icon><Setting /></el-icon>
              <span>{{ $t('navbar.settings') }}</span>
            </el-dropdown-item>
            <el-dropdown-item divided command="logout">
              <el-icon><SwitchButton /></el-icon>
              <span>{{ $t('navbar.logout') }}</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  Operation, 
  CaretBottom, 
  User, 
  Setting, 
  SwitchButton 
} from '@element-plus/icons-vue'
import Breadcrumb from './Breadcrumb/index.vue'
import Hamburger from './Hamburger/index.vue'
import ThemeSwitcher from '@/components/ThemeSwitcher/index.vue'
import { useUserStore } from '@/stores/user'
import { useAppStore } from '@/stores/app'

const router = useRouter()
const { locale, t } = useI18n()
const userStore = useUserStore()
const appStore = useAppStore()

const sidebar = computed(() => appStore.sidebar)
const device = computed(() => appStore.device)
const name = computed(() => userStore.name || 'Admin')
const avatar = computed(() => userStore.avatar || '')

const toggleSidebar = () => {
  appStore.toggleSidebar()
}

const handleSetLanguage = (lang: string) => {
  locale.value = lang
  appStore.setLanguage(lang)
  
  // 根据不同语言显示不同的成功消息
  const languageNames = {
    'zh-cn': '中文简体',
    'en': 'English',
    'ja': '日本語',
    'ru': 'Русский',
    'fr': 'Français',
    'es': 'Español'
  }
  
  ElMessage({
    message: t('navbar.switchLanguageSuccess', { language: languageNames[lang as keyof typeof languageNames] }),
    type: 'success',
    duration: 2000
  })
}

const handleCommand = (command: string) => {
  switch (command) {
    case 'profile':
      router.push('/profile')
      break
    case 'settings':
      router.push('/settings')
      break
    case 'logout':
      handleLogout()
      break
  }
}

const handleLogout = async () => {
  try {
    await ElMessageBox.confirm(
      t('navbar.logoutConfirm'),
      t('common.warning'),
      {
        confirmButtonText: t('common.confirm'),
        cancelButtonText: t('common.cancel'),
        type: 'warning'
      }
    )
    
    await userStore.logout()
    router.push('/login')
  } catch (error) {
    console.log(error)
  }
}
</script>

<style lang="scss" scoped>
.navbar {
  height: 60px;
  overflow: hidden;
  position: relative;
  background: var(--navbar-bg);
  box-shadow: var(--navbar-shadow);
  border-bottom: 1px solid var(--navbar-border);
  display: flex;
  align-items: center;
  padding: 0 24px;
  backdrop-filter: blur(8px);
  z-index: 999;

  .hamburger-container {
    line-height: 60px;
    height: 60px;
    display: flex;
    align-items: center;
    cursor: pointer;
    transition: all 0.3s ease;
    -webkit-tap-highlight-color: transparent;
    padding: 0 12px;
    border-radius: 8px;
    color: var(--text-regular);

    &:hover {
      background: var(--bg-hover);
      color: var(--primary-color);
    }
  }

  .breadcrumb-container {
    flex: 1;
    margin-left: 20px;
    display: flex;
    align-items: center;
    height: 60px;
  }

  .right-menu {
    display: flex;
    align-items: center;
    height: 60px;
    gap: 4px;

    &:focus {
      outline: none;
    }

    .right-menu-item {
      display: flex;
      align-items: center;
      padding: 0 8px;
      height: 60px;
      color: var(--text-regular);
      cursor: pointer;
      transition: all 0.3s ease;
      border-radius: 8px;

      &:hover {
        background: var(--bg-hover);
        color: var(--primary-color);
      }

      .icon-wrapper {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 40px;
        height: 40px;
        border-radius: 8px;
        transition: all 0.3s ease;
        
        .el-icon {
          font-size: 18px;
        }
      }
    }

    .avatar-wrapper {
      display: flex;
      align-items: center;
      cursor: pointer;
      padding: 8px 16px;
      border-radius: 12px;
      transition: all 0.3s ease;
      position: relative;
      
      &::before {
        content: '';
        position: absolute;
        inset: 0;
        background: var(--primary-gradient);
        border-radius: 12px;
        opacity: 0;
        transition: opacity 0.3s ease;
      }
      
      &:hover {
        background: var(--bg-hover);
        
        &::before {
          opacity: 0.08;
        }
      }
      
      .el-avatar {
        margin-right: 12px;
        flex-shrink: 0;
        position: relative;
        z-index: 1;
        border: 2px solid var(--border-light);
        transition: border-color 0.3s ease;
      }
      
      .user-name {
        font-size: 14px;
        font-weight: 500;
        color: var(--text-primary);
        margin-right: 8px;
        white-space: nowrap;
        position: relative;
        z-index: 1;
        
        @media (max-width: 768px) {
          display: none;
        }
      }

      .el-icon {
        font-size: 12px;
        color: var(--text-secondary);
        transition: all 0.3s ease;
        position: relative;
        z-index: 1;
      }

      &:hover {
        .el-avatar {
          border-color: var(--primary-color);
        }
        
        .el-icon {
          transform: rotate(180deg);
          color: var(--primary-color);
        }
      }
    }
  }

  // 响应式处理
  @media (max-width: 768px) {
    padding: 0 16px;
    height: 56px;
    
    .hamburger-container {
      padding: 0 8px;
      height: 56px;
      line-height: 56px;
    }
    
    .breadcrumb-container {
      margin-left: 12px;
      height: 56px;
    }
    
    .right-menu {
      height: 56px;
      gap: 2px;
      
      .right-menu-item {
        padding: 0 6px;
        height: 56px;
        
        .icon-wrapper {
          width: 36px;
          height: 36px;
        }
      }
      
      .avatar-wrapper {
        padding: 6px 12px;
        
        .el-avatar {
          margin-right: 8px;
        }
      }
    }
  }
}

// 下拉菜单样式
:deep(.user-dropdown) {
  .el-dropdown-menu__item {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 12px 16px;
    transition: all 0.3s ease;
    
    .el-icon {
      font-size: 16px;
      color: var(--text-secondary);
    }
    
    span {
      font-size: 14px;
      color: var(--text-primary);
    }
    
    &:hover {
      background: var(--bg-hover);
      
      .el-icon,
      span {
        color: var(--primary-color);
      }
    }
  }
}

:deep(.language-dropdown) {
  min-width: 180px;
  
  .el-dropdown-menu__item {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 12px 16px;
    transition: all 0.3s ease;
    position: relative;
    
    .flag {
      font-size: 16px;
      flex-shrink: 0;
      width: 20px;
      text-align: center;
    }
    
    .text {
      font-size: 14px;
      color: var(--text-primary);
      flex: 1;
      font-weight: 400;
    }
    
    &.is-active {
      background: var(--primary-color-light-9);
      color: var(--primary-color);
      
      .text {
        color: var(--primary-color);
        font-weight: 500;
      }
      
      &::after {
        content: '✓';
        position: absolute;
        right: 16px;
        color: var(--primary-color);
        font-weight: bold;
        font-size: 12px;
      }
    }
    
    &:hover {
      background: var(--bg-hover);
      
      .text {
        color: var(--primary-color);
      }
      
      &.is-active {
        background: var(--primary-color-light-8);
      }
    }
  }
}
</style>
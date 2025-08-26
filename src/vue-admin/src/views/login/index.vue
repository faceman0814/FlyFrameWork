<template>
  <div class="login-container">
    <!-- 背景装饰 -->
    <div class="background-decoration">
      <div class="decoration-circle circle-1"></div>
      <div class="decoration-circle circle-2"></div>
      <div class="decoration-circle circle-3"></div>
      <div class="floating-particles">
        <div class="particle" v-for="n in 20" :key="n" :style="getParticleStyle(n)"></div>
      </div>
    </div>
    
    <!-- 左侧品牌区域 -->
    <div class="brand-section">
      <div class="brand-content">
        <div class="logo-wrapper">
          <div class="logo-icon">
            <el-icon size="60"><Monitor /></el-icon>
          </div>
          <h1 class="brand-title">FlyFrameWork</h1>
          <p class="brand-subtitle">Modern Admin Dashboard</p>
        </div>
        <div class="feature-list">
          <div class="feature-item" v-for="(feature, index) in features" :key="index">
            <el-icon class="feature-icon"><component :is="feature.icon" /></el-icon>
            <span>{{ feature.text }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 登录表单区域 -->
    <div class="login-section">
      <div class="login-wrapper">
        <el-form
          ref="loginFormRef"
          :model="loginForm"
          :rules="loginRules"
          class="login-form"
          autocomplete="on"
          label-position="left"
        >
          <div class="form-header">
            <h2 class="form-title">{{ $t('login.title') }}</h2>
            <p class="form-subtitle">Welcome back! Please sign in to your account</p>
          </div>

          <el-form-item prop="username" class="input-item">
            <div class="input-wrapper">
              <el-icon class="input-icon"><User /></el-icon>
              <el-input
                ref="usernameRef"
                v-model="loginForm.username"
                :placeholder="$t('login.username')"
                name="username"
                type="text"
                tabindex="1"
                autocomplete="on"
                class="form-input"
                size="large"
              />
            </div>
          </el-form-item>

          <el-tooltip :visible="capsTooltip" :content="'Caps lock is On'" placement="right">
            <el-form-item prop="password" class="input-item">
              <div class="input-wrapper">
                <el-icon class="input-icon"><Lock /></el-icon>
                <el-input
                  :key="passwordType"
                  ref="passwordRef"
                  v-model="loginForm.password"
                  :type="passwordType"
                  :placeholder="$t('login.password')"
                  name="password"
                  tabindex="2"
                  autocomplete="on"
                  class="form-input"
                  size="large"
                  @keyup="checkCapslock"
                  @blur="capsTooltip = false"
                  @keyup.enter="handleLogin"
                />
                <div class="password-toggle" @click="showPwd">
                  <el-icon><component :is="passwordType === 'password' ? 'View' : 'Hide'" /></el-icon>
                </div>
              </div>
            </el-form-item>
          </el-tooltip>

          <div class="form-options">
            <el-checkbox v-model="rememberMe" class="remember-checkbox">
              Remember me
            </el-checkbox>
            <a href="#" class="forgot-link">Forgot password?</a>
          </div>

          <el-button
            :loading="loading"
            type="primary"
            size="large"
            class="login-button"
            @click.prevent="handleLogin"
          >
            <span v-if="!loading">{{ $t('login.login') }}</span>
            <span v-else>Signing in...</span>
          </el-button>

          <div class="demo-tips">
            <div class="tips-header">
              <el-icon><InfoFilled /></el-icon>
              <span>Demo Account</span>
            </div>
          </div>
        </el-form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted, nextTick } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { User, Lock, View, Hide, Monitor, Lock as Shield, Cpu, Setting, InfoFilled } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import type { FormInstance } from 'element-plus'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

const loginFormRef = ref<FormInstance>()
const usernameRef = ref()
const passwordRef = ref()

const loginForm = reactive({
  username: 'admin',
  password: 'bb123456'
})

const loginRules = reactive({
  username: [{ required: true, trigger: 'blur', message: () => t('login.usernameRequired') }],
  password: [{ required: true, trigger: 'blur', message: () => t('login.passwordRequired') }]
})

const passwordType = ref('password')
const capsTooltip = ref(false)
const loading = ref(false)
const rememberMe = ref(true)

// 功能特性列表
const features = ref([
  { icon: Shield, text: 'Security First' },
  { icon: Cpu, text: 'High Performance' },
  { icon: Setting, text: 'Easy Configuration' }
])

// 生成随机粒子样式
const getParticleStyle = (_index: number) => {
  const size = Math.random() * 4 + 2
  const left = Math.random() * 100
  const animationDelay = Math.random() * 15
  const animationDuration = Math.random() * 10 + 10
  
  return {
    width: `${size}px`,
    height: `${size}px`,
    left: `${left}%`,
    animationDelay: `${animationDelay}s`,
    animationDuration: `${animationDuration}s`
  }
}

const showPwd = () => {
  if (passwordType.value === 'password') {
    passwordType.value = ''
  } else {
    passwordType.value = 'password'
  }
  nextTick(() => {
    passwordRef.value.focus()
  })
}

const checkCapslock = (e: KeyboardEvent) => {
  const { key } = e
  capsTooltip.value = !!(key && key.length === 1 && (key >= 'A' && key <= 'Z'))
}

const handleLogin = () => {
  loginFormRef.value?.validate(async (valid: boolean) => {
    if (valid) {
      loading.value = true
      try {
        await userStore.login(loginForm)
        router.push({ path: (route.query.redirect as string) || '/', replace: true })
        ElMessage.success(t('login.loginSuccess'))
      } catch (error) {
        ElMessage.error(t('login.loginError'))
      } finally {
        loading.value = false
      }
    }
  })
}

onMounted(() => {
  if (loginForm.username === '') {
    usernameRef.value.focus()
  } else if (loginForm.password === '') {
    passwordRef.value.focus()
  }
})
</script>

<style lang="scss" scoped>
.login-container {
  min-height: 100vh;
  width: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  position: relative;
  overflow: hidden;

  // 响应式布局
  @media (max-width: 1024px) {
    flex-direction: column;
  }
}

// 背景装饰
.background-decoration {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 1;

  .decoration-circle {
    position: absolute;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(10px);
    
    &.circle-1 {
      width: 300px;
      height: 300px;
      top: -150px;
      right: -150px;
      animation: float 6s ease-in-out infinite;
    }
    
    &.circle-2 {
      width: 200px;
      height: 200px;
      bottom: -100px;
      left: -100px;
      animation: float 8s ease-in-out infinite reverse;
    }
    
    &.circle-3 {
      width: 150px;
      height: 150px;
      top: 50%;
      left: 20%;
      animation: float 10s ease-in-out infinite;
    }
  }

  .floating-particles {
    .particle {
      position: absolute;
      background: rgba(255, 255, 255, 0.3);
      border-radius: 50%;
      animation: particle-float 20s linear infinite;
    }
  }
}

@keyframes float {
  0%, 100% { transform: translateY(0) rotate(0deg); }
  50% { transform: translateY(-20px) rotate(180deg); }
}

@keyframes particle-float {
  0% {
    opacity: 0;
    transform: translateY(100vh) rotateX(0deg);
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    opacity: 0;
    transform: translateY(-10vh) rotateX(360deg);
  }
}

// 左侧品牌区域
.brand-section {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 60px;
  position: relative;
  z-index: 2;
  
  @media (max-width: 1024px) {
    padding: 40px 20px;
    min-height: 40vh;
  }

  .brand-content {
    text-align: center;
    color: white;

    .logo-wrapper {
      margin-bottom: 60px;

      .logo-icon {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 120px;
        height: 120px;
        background: rgba(255, 255, 255, 0.1);
        border-radius: 30px;
        margin-bottom: 30px;
        backdrop-filter: blur(10px);
        border: 1px solid rgba(255, 255, 255, 0.2);
        
        @media (max-width: 768px) {
          width: 80px;
          height: 80px;
          border-radius: 20px;
          
          .el-icon {
            font-size: 40px !important;
          }
        }
      }

      .brand-title {
        font-size: 48px;
        font-weight: 700;
        margin: 0 0 15px 0;
        text-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
        
        @media (max-width: 768px) {
          font-size: 32px;
        }
      }

      .brand-subtitle {
        font-size: 18px;
        opacity: 0.9;
        font-weight: 300;
        
        @media (max-width: 768px) {
          font-size: 16px;
        }
      }
    }

    .feature-list {
      display: flex;
      flex-direction: column;
      gap: 20px;
      
      @media (max-width: 1024px) {
        flex-direction: row;
        justify-content: center;
        flex-wrap: wrap;
        gap: 30px;
      }

      .feature-item {
        display: flex;
        align-items: center;
        justify-content: flex-start;
        gap: 15px;
        padding: 20px;
        background: rgba(255, 255, 255, 0.1);
        border-radius: 15px;
        backdrop-filter: blur(10px);
        border: 1px solid rgba(255, 255, 255, 0.2);
        transition: all 0.3s ease;

        &:hover {
          background: rgba(255, 255, 255, 0.15);
          transform: translateY(-2px);
        }
        
        @media (max-width: 1024px) {
          flex-direction: column;
          text-align: center;
          padding: 15px;
          min-width: 120px;
        }

        .feature-icon {
          font-size: 24px;
          opacity: 0.9;
        }

        span {
          font-size: 16px;
          font-weight: 500;
        }
      }
    }
  }
}

// 右侧登录区域
.login-section {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 60px;
  position: relative;
  z-index: 2;
  
  @media (max-width: 1024px) {
    padding: 40px 20px;
  }

  .login-wrapper {
    width: 100%;
    max-width: 420px;
    background: rgba(255, 255, 255, 0.95);
    backdrop-filter: blur(20px);
    border-radius: 24px;
    padding: 50px 40px;
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
    border: 1px solid rgba(255, 255, 255, 0.3);
    
    @media (max-width: 768px) {
      padding: 40px 30px;
      border-radius: 20px;
    }
  }

  .form-header {
    text-align: center;
    margin-bottom: 40px;

    .form-title {
      font-size: 28px;
      font-weight: 700;
      color: #2c3e50;
      margin: 0 0 10px 0;
      
      @media (max-width: 768px) {
        font-size: 24px;
      }
    }

    .form-subtitle {
      font-size: 14px;
      color: #7f8c8d;
      margin: 0;
    }
  }

  .input-item {
    margin-bottom: 24px;
    
    .input-wrapper {
      position: relative;
      display: flex;
      align-items: center;

      .input-icon {
        position: absolute;
        left: 16px;
        z-index: 3;
        color: #9ca3af;
        font-size: 18px;
      }

      .form-input {
        :deep(.el-input__wrapper) {
          padding-left: 50px;
          padding-right: 50px;
          height: 52px;
          border-radius: 12px;
          border: 2px solid #e5e7eb;
          box-shadow: none;
          background: #f9fafb;
          transition: all 0.3s ease;

          &:hover {
            border-color: #d1d5db;
            background: #fff;
          }

          &.is-focus {
            border-color: #667eea;
            background: #fff;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
          }
        }

        :deep(.el-input__inner) {
          color: #374151;
          font-size: 15px;
          
          &::placeholder {
            color: #9ca3af;
          }
        }
      }

      .password-toggle {
        position: absolute;
        right: 16px;
        z-index: 3;
        cursor: pointer;
        color: #9ca3af;
        font-size: 18px;
        transition: color 0.3s ease;
        
        &:hover {
          color: #6b7280;
        }
      }
    }
  }

  .form-options {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;

    .remember-checkbox {
      :deep(.el-checkbox__label) {
        color: #6b7280;
        font-size: 14px;
      }
    }

    .forgot-link {
      color: #667eea;
      font-size: 14px;
      text-decoration: none;
      font-weight: 500;
      transition: color 0.3s ease;

      &:hover {
        color: #5a67d8;
        text-decoration: underline;
      }
    }
  }

  .login-button {
    width: 100%;
    height: 52px;
    border-radius: 12px;
    font-size: 16px;
    font-weight: 600;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border: none;
    margin-bottom: 30px;
    transition: all 0.3s ease;

    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 25px rgba(102, 126, 234, 0.4);
    }

    &:active {
      transform: translateY(0);
    }
  }

  .demo-tips {
    background: linear-gradient(135deg, #f8f9ff 0%, #e8f2ff 100%);
    border: 1px solid #e1e8f7;
    border-radius: 12px;
    padding: 20px;
    text-align: center;

    .tips-header {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      margin-bottom: 12px;
      color: #4f46e5;
      font-weight: 600;
      font-size: 14px;
    }

    .tips-content {
      p {
        margin: 8px 0;
        color: #6b7280;
        font-size: 14px;
        
        strong {
          color: #374151;
          font-weight: 600;
        }
      }
    }
  }
}

// 表单项错误状态
:deep(.el-form-item.is-error) {
  .input-wrapper .form-input .el-input__wrapper {
    border-color: #ef4444;
    background: #fef2f2;
  }
}

:deep(.el-form-item__error) {
  color: #ef4444;
  font-size: 12px;
  margin-top: 6px;
}
</style>
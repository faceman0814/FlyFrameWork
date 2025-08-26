import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import en from 'element-plus/es/locale/lang/en'
// 只导入需要的图标，避免全部导入
import { 
  User, UserFilled, House, Setting, View, Tools, Monitor,
  Lock, CaretBottom, Operation, Close, Hide
} from '@element-plus/icons-vue'
import { createI18n } from 'vue-i18n'
import 'nprogress/nprogress.css'

console.log('🚀 启动FlyFramework管理系统...')

// 导入主要组件
import App from './App.vue'
import router from './router'
import { useUserStore } from './stores/user'

// 导入样式文件
import './styles/index.scss'

// 国际化配置
import zhLocale from './locales/zh-cn.json'
import enLocale from './locales/en.json'

console.log('✅ 所有模块导入成功')

// 配置国际化
const messages = {
  'zh-cn': {
    ...zhLocale,
    el: zhCn.el
  },
  en: {
    ...enLocale,
    el: en.el
  }
}

const i18n = createI18n({
  legacy: false,
  locale: localStorage.getItem('language') || 'zh-cn',
  fallbackLocale: 'zh-cn',
  messages
})

// 创建Vue应用
const app = createApp(App)
const pinia = createPinia()

// 注册常用的ElementPlus图标
const icons = {
  User, UserFilled, House, Setting, View, Tools, Monitor,
  Lock, CaretBottom, Operation, Close, Hide
}

Object.entries(icons).forEach(([key, component]) => {
  app.component(key, component)
})

// 安装插件
app.use(pinia)
app.use(router)
app.use(ElementPlus, {
  locale: i18n.global.locale.value === 'zh-cn' ? zhCn : en
})
app.use(i18n)

// 全局错误处理
app.config.errorHandler = (err, vm, info) => {
  console.error('Vue应用错误:', err, vm, info)
}

// 初始化用户信息并挂载应用
try {
  const userStore = useUserStore()
  
  // 临时设置token以便调试菜单显示
  if (!userStore.token) {
    userStore.token = 'temp-token'
    userStore.roles = ['admin']
    userStore.permissions = ['*:*:*']
    userStore.name = '管理员'
    userStore.avatar = 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif'
    console.log('设置临时用户信息')
  }
  
  userStore.initUserInfo()
  
  app.mount('#app')
  console.log('🎉 FlyFramework管理系统启动成功!')
} catch (error) {
  console.error('❌ 应用挂载失败:', error)
}
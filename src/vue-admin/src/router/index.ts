import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import NProgress from 'nprogress'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import Layout from '@/layout/index.vue'

// NProgress配置
NProgress.configure({ 
  showSpinner: true,    // 显示右上角旋转器
  speed: 500,          // 动画速度
  minimum: 0.1,        // 最小百分比
  easing: 'ease',      // 缓动函数
  trickleSpeed: 200    // 自动递增间隔
})

// 静态路由
export const constantRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    component: () => import('@/views/login/index.vue'),
    meta: { hidden: true }
  },
  {
    path: '/404',
    component: () => import('@/views/error-page/404.vue'),
    meta: { hidden: true }
  },
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/dashboard',
    component: Layout,
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/index.vue'),
        meta: {
          title: 'dashboard.title',
          icon: 'House',
          affix: true
        }
      }
    ]
  }
]

// 动态路由
export const asyncRoutes: RouteRecordRaw[] = [
  {
    path: '/system',
    component: Layout,
    redirect: '/system/user',
    name: 'System',
    meta: {
      title: 'system.title',
      icon: 'Setting',
      roles: ['admin', 'editor']
    },
    children: [
      {
        path: 'user',
        name: 'User',
        component: () => import('@/views/system/user/index.vue'),
        meta: {
          title: 'system.user',
          icon: 'User',
          roles: ['admin']
        }
      },
      {
        path: 'role',
        name: 'Role',
        component: () => import('@/views/system/role/index.vue'),
        meta: {
          title: 'system.role',
          icon: 'UserFilled',
          roles: ['admin']
        }
      },
      {
        path: 'api-test',
        name: 'ApiTest',
        component: () => import('@/views/system/user/ApiTest.vue'),
        meta: {
          title: 'system.apiTest',
          icon: 'Connection',
          roles: ['admin']
        }
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/404',
    meta: { hidden: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes: constantRoutes,
  scrollBehavior: () => ({ top: 0 })
})

// 路由守卫
router.beforeEach(async (to, _from, next) => {
  // 开始NProgress和自定义loading
  NProgress.start()
  
  const userStore = useUserStore()
  const hasToken = userStore.token
  
  console.log('路由守卫 - hasToken:', hasToken, 'to.path:', to.path)
  console.log('当前用户角色:', userStore.roles)
  console.log('当前动态路由数量:', userStore.dynamicRoutes.length)
  
  if (hasToken) {
    if (to.path === '/login') {
      next({ path: '/' })
    } else {
      // 确保用户信息和动态路由已生成
      if (userStore.roles.length === 0 || userStore.dynamicRoutes.length === 0) {
        try {
          console.log('获取用户信息...')
          await userStore.getUserInfo()
          console.log('生成动态路由...')
          const accessRoutes = await userStore.generateRoutes()
          console.log('动态路由生成完成:', accessRoutes)
          
          // 动态添加路由
          accessRoutes.forEach(route => {
            console.log('添加路由:', route.path)
            router.addRoute(route)
          })
          
          next({ ...to, replace: true })
        } catch (error) {
          console.error('获取用户信息失败:', error)
          userStore.resetToken()
          ElMessage.error('登录已过期，请重新登录')
          next(`/login?redirect=${to.path}`)
        }
      } else {
        console.log('用户信息已存在，直接访问')
        next()
      }
    }
  } else {
    console.log('无token，跳转登录')
    if (to.path === '/login') {
      next()
    } else {
      next(`/login?redirect=${to.path}`)
    }
  }
})

router.afterEach(() => {
  // 完成NProgress和自定义loading
  NProgress.done()
})

export default router
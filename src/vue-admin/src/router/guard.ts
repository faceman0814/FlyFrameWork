// 路由守卫增强 - 添加更多安全检查
import type { Router } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { tokenManager } from '@/utils/token-manager'
import { ElMessage } from 'element-plus'
import NProgress from 'nprogress'

// 白名单路由（无需认证）
const whiteList = ['/login', '/404', '/401']

// 权限验证
const hasPermission = (userRoles: string[], routeRoles?: string[]): boolean => {
  if (!routeRoles || routeRoles.length === 0) return true
  if (userRoles.includes('admin')) return true
  return routeRoles.some(role => userRoles.includes(role))
}

export const setupRouterGuard = (router: Router) => {
  router.beforeEach(async (to, from, next) => {
    NProgress.start()
    
    const userStore = useUserStore()
    const hasToken = userStore.token

    if (hasToken) {
      // 已登录用户访问登录页面，重定向到首页
      if (to.path === '/login') {
        next({ path: '/' })
        NProgress.done()
        return
      }

      // 检查Token是否需要刷新
      const tokenRefreshed = await tokenManager.refreshTokenIfNeeded()
      if (!tokenRefreshed) {
        userStore.resetToken()
        next('/login')
        NProgress.done()
        return
      }

      // 检查用户信息是否已获取
      if (!userStore.name || userStore.roles.length === 0) {
        try {
          await userStore.getUserInfo()
          const accessRoutes = await userStore.generateRoutes()
          
          // 动态添加路由
          accessRoutes.forEach(route => {
            router.addRoute(route)
          })
          
          next({ ...to, replace: true })
        } catch (error) {
          console.error('获取用户信息失败:', error)
          userStore.resetToken()
          ElMessage.error('获取用户信息失败，请重新登录')
          next('/login')
        }
      } else {
        // 检查路由权限
        const routeRoles = to.meta?.roles as string[]
        if (hasPermission(userStore.roles, routeRoles)) {
          next()
        } else {
          ElMessage.error('您没有权限访问该页面')
          next('/401')
        }
      }
    } else {
      // 未登录处理
      if (whiteList.includes(to.path)) {
        next()
      } else {
        next(`/login?redirect=${to.path}`)
      }
    }
    
    NProgress.done()
  })

  router.afterEach((to) => {
    NProgress.done()
    
    // 设置页面标题
    if (to.meta?.title) {
      document.title = `${to.meta.title} - FlyFramework`
    }
    
    // 记录页面访问日志
    console.log(`页面访问: ${to.path}`)
  })

  // 路由错误处理
  router.onError((error) => {
    console.error('路由错误:', error)
    NProgress.done()
    ElMessage.error('页面加载失败，请刷新重试')
  })
}
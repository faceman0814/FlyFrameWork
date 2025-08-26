import { defineStore } from 'pinia'
import { RouteRecordRaw } from 'vue-router'
import { serviceProxies, AccountLoginDto } from '@/api'
import { getToken, setToken, removeToken } from '@/utils/auth'
import { asyncRoutes, constantRoutes } from '@/router'
import { ElMessage } from 'element-plus'

interface UserState {
  token: string
  name: string
  avatar: string
  roles: string[]
  permissions: string[]
  dynamicRoutes: RouteRecordRaw[]
  refreshToken: string
  userId: string
}

// 过滤路由权限
function hasPermission(roles: string[], route: RouteRecordRaw): boolean {
  if (route.meta && route.meta.roles) {
    return roles.some(role => (route.meta!.roles as string[]).includes(role))
  } else {
    return true
  }
}

// 过滤异步路由
function filterAsyncRoutes(routes: RouteRecordRaw[], roles: string[]): RouteRecordRaw[] {
  const res: RouteRecordRaw[] = []

  routes.forEach(route => {
    const tmp = { ...route }
    if (hasPermission(roles, tmp)) {
      if (tmp.children) {
        tmp.children = filterAsyncRoutes(tmp.children, roles)
      }
      res.push(tmp)
    }
  })

  return res
}

export const useUserStore = defineStore('user', {
  state: (): UserState => ({
    token: getToken() || '',
    name: '',
    avatar: '',
    roles: ['admin'], // 默认设置为admin角色
    permissions: ['*:*:*'], // 默认全权限
    dynamicRoutes: [],
    refreshToken: '',
    userId: ''
  }),

  getters: {
    getRoles: (state) => state.roles,
    getPermissions: (state) => state.permissions,
    isLoggedIn: (state) => !!state.token
  },

  actions: {
    // 使用 NSwag 自动生成的 AccountClientServiceProxy 登录
    async login(userInfo: { username: string; password: string }) {
      const { username, password } = userInfo
      try {
        const loginDto = new AccountLoginDto({
          userName: username.trim(),
          password,
          phoneNumber: undefined,
          clientType: 'PC',
          rememberMe: false,
          isRefresh: false,
          isApiLogin: false  // 设置 IsApiLogin 为 false
        })
        
        const response = await serviceProxies.auth.login(loginDto)
        
        if (response.success && response.data?.accessToken) {
          // 设置 token 和其他信息
          this.token = response.data.accessToken
          this.refreshToken = response.data.refreshToken || ''
          
          setToken(response.data.accessToken)
          
          // 使用登录响应中的用户信息
          const userData = response.data
          this.userId = userData.userId || ''
          this.name = userData.nickName || userData.userName || 'Admin'
          this.avatar = userData.avatar || 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif'
          this.roles = userData.roles || ['admin'] // 确保有 admin 角色
          this.permissions = userData.permissions || ['*:*:*']
          
          // 存储到 localStorage 以便刷新页面后恢复
          localStorage.setItem('userInfo', JSON.stringify({
            userId: this.userId,
            name: this.name,
            avatar: this.avatar,
            roles: this.roles,
            permissions: this.permissions
          }))
          
          ElMessage.success('登录成功')
          return Promise.resolve(response.data)
        } else {
          throw new Error(response.message || '登录失败：未获取到有效令牌')
        }
      } catch (error: any) {
        console.error('登录失败:', error)
        ElMessage.error(error.message || '登录失败，请检查用户名和密码')
        return Promise.reject(error)
      }
    },

    // 获取用户信息
    async getUserInfo() {
      try {
        // 如果已有用户信息，直接从 localStorage 恢复
        const cachedUserInfo = localStorage.getItem('userInfo')
        if (cachedUserInfo) {
          const userInfo = JSON.parse(cachedUserInfo)
          this.userId = userInfo.userId
          this.name = userInfo.name
          this.avatar = userInfo.avatar
          this.roles = userInfo.roles
          this.permissions = userInfo.permissions
          return userInfo
        }

        // 如果没有缓存信息但有token，使用默认用户信息
        if (this.token) {
          const defaultUserInfo = {
            roles: ['admin'], // 改为 admin 角色以便访问所有菜单
            name: '管理员',
            avatar: 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif',
            permissions: ['*:*:*'],
            id: 'default'
          }

          this.roles = defaultUserInfo.roles
          this.name = defaultUserInfo.name
          this.avatar = defaultUserInfo.avatar
          this.permissions = defaultUserInfo.permissions
          this.userId = defaultUserInfo.id
          
          // 缓存用户信息
          localStorage.setItem('userInfo', JSON.stringify({
            userId: this.userId,
            name: this.name,
            avatar: this.avatar,
            roles: this.roles,
            permissions: this.permissions
          }))
          
          return defaultUserInfo
        }

        throw new Error('No token available')
      } catch (error: any) {
        console.error('获取用户信息失败:', error)
        
        // 如果获取用户信息失败，使用默认值
        const defaultUserInfo = {
          roles: ['admin'], // 改为 admin 角色
          name: '管理员',
          avatar: 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif',
          permissions: ['*:*:*'],
          id: 'default'
        }

        this.roles = defaultUserInfo.roles
        this.name = defaultUserInfo.name
        this.avatar = defaultUserInfo.avatar
        this.permissions = defaultUserInfo.permissions
        this.userId = defaultUserInfo.id
        
        return defaultUserInfo
      }
    },

    // 生成路由
    generateRoutes() {
      console.log('开始生成路由...')
      // 给用户 admin 权限以便看到所有菜单
      const roles = this.roles.length > 0 ? this.roles : ['admin']
      console.log('用户角色:', roles)
      console.log('原始异步路由:', asyncRoutes)
      
      const accessedRoutes = filterAsyncRoutes(asyncRoutes, roles)
      console.log('过滤后的路由:', accessedRoutes)
      
      this.dynamicRoutes = [...constantRoutes, ...accessedRoutes]
      console.log('最终动态路由:', this.dynamicRoutes)
      
      return accessedRoutes
    },

    // 登出
    async logout() {
      try {
        // 注意：目前没有对应的 logout API，所以只是清除本地数据
        // 如果后端有 logout API，可以这样调用：
        // await serviceProxies.auth.logout()
        
        this.resetToken()
        ElMessage.success('登出成功')
        return Promise.resolve()
      } catch (error: any) {
        console.error('登出失败:', error)
        // 即使登出 API 失败，也要清除本地数据
        this.resetToken()
        return Promise.resolve()
      }
    },

    // 重置token和用户信息
    resetToken() {
      this.token = ''
      this.refreshToken = ''
      this.roles = []
      this.permissions = []
      this.name = ''
      this.avatar = ''
      this.userId = ''
      this.dynamicRoutes = []
      removeToken()
      localStorage.removeItem('userInfo')
    },

    // 初始化用户信息
    initUserInfo() {
      const token = getToken()
      if (token) {
        this.token = token
        
        // 尝试从 localStorage 恢复用户信息
        const cachedUserInfo = localStorage.getItem('userInfo')
        if (cachedUserInfo) {
          try {
            const userInfo = JSON.parse(cachedUserInfo)
            this.userId = userInfo.userId
            this.name = userInfo.name
            this.avatar = userInfo.avatar
            this.roles = userInfo.roles
            this.permissions = userInfo.permissions
          } catch (error) {
            console.error('恢复用户信息失败:', error)
          }
        }
        
        // 如果没有角色信息，设置默认角色
        if (this.roles.length === 0) {
          this.roles = ['admin']
          this.permissions = ['*:*:*']
          this.name = '管理员'
          this.avatar = 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif'
        }
      }
    },

    // 刷新 token
    async refreshUserToken() {
      try {
        if (!this.refreshToken) {
          throw new Error('No refresh token available')
        }
        
        const response = await serviceProxies.auth.refreshToken(this.refreshToken)
        
        if (response.success && response.data?.accessToken) {
          this.token = response.data.accessToken
          this.refreshToken = response.data.refreshToken || this.refreshToken
          setToken(response.data.accessToken)
          return Promise.resolve()
        } else {
          throw new Error('Token refresh failed')
        }
      } catch (error: any) {
        console.error('刷新 token 失败:', error)
        this.resetToken()
        return Promise.reject(error)
      }
    }
  }
})
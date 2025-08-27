// Token管理工具 - 增强安全性
import { ElMessage } from 'element-plus'

interface TokenInfo {
  accessToken: string
  refreshToken: string
  expiresAt: number
}

class TokenManager {
  private static instance: TokenManager
  private readonly ACCESS_TOKEN_KEY = 'access_token'
  private readonly REFRESH_TOKEN_KEY = 'refresh_token'
  private readonly EXPIRES_AT_KEY = 'token_expires_at'

  static getInstance(): TokenManager {
    if (!TokenManager.instance) {
      TokenManager.instance = new TokenManager()
    }
    return TokenManager.instance
  }

  // 设置Token信息
  setTokens(tokenInfo: TokenInfo): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, tokenInfo.accessToken)
    localStorage.setItem(this.REFRESH_TOKEN_KEY, tokenInfo.refreshToken)
    localStorage.setItem(this.EXPIRES_AT_KEY, tokenInfo.expiresAt.toString())
  }

  // 获取访问Token
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY)
  }

  // 获取刷新Token
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY)
  }

  // 检查Token是否即将过期（提前5分钟刷新）
  isTokenExpiringSoon(): boolean {
    const expiresAt = localStorage.getItem(this.EXPIRES_AT_KEY)
    if (!expiresAt) return true
    
    const expirationTime = parseInt(expiresAt)
    const currentTime = Date.now()
    const fiveMinutes = 5 * 60 * 1000
    
    return currentTime >= (expirationTime - fiveMinutes)
  }

  // 清除所有Token
  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY)
    localStorage.removeItem(this.REFRESH_TOKEN_KEY)
    localStorage.removeItem(this.EXPIRES_AT_KEY)
  }

  // 自动刷新Token
  async refreshTokenIfNeeded(): Promise<boolean> {
    if (!this.isTokenExpiringSoon()) {
      return true
    }

    try {
      const refreshToken = this.getRefreshToken()
      if (!refreshToken) {
        this.clearTokens()
        return false
      }

      // 调用刷新Token API
      // const response = await refreshTokenApi(refreshToken)
      // this.setTokens(response.data)
      return true
    } catch (error) {
      console.error('Token刷新失败:', error)
      this.clearTokens()
      ElMessage.error('登录已过期，请重新登录')
      return false
    }
  }
}

export const tokenManager = TokenManager.getInstance()
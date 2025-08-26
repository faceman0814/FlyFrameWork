import { getToken } from '@/utils/auth'

// 导出异常类型
export class ApiException extends Error {
  message: string;
  status: number;
  response: string;
  headers: { [key: string]: any; };
  result?: any;

  constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any) {
    super(message);
    this.message = message;
    this.status = status;
    this.response = response;
    this.headers = headers;
    this.result = result;
  }
}

export class ApiClientBase {
  private baseUrl: string
  
  constructor(baseUrl?: string) {
    this.baseUrl = baseUrl || ''
  }

  protected transformOptions = (options: RequestInit): Promise<RequestInit> => {
    const token = getToken()
    if (token) {
      options.headers = {
        ...options.headers,
        Authorization: `Bearer ${token}`
      }
    }
    
    // 设置默认的 Content-Type
    if (!options.headers || !Object.keys(options.headers).find(key => key.toLowerCase() === 'content-type')) {
      options.headers = {
        ...options.headers,
        'Content-Type': 'application/json'
      }
    }

    return Promise.resolve(options)
  }

  protected transformResult = (_url: string, response: Response, processor: (response: Response) => Promise<any>): Promise<any> => {
    return processor(response).catch(async (error) => {
      // 统一错误处理
      if (response.status === 401) {
        // 处理未授权，清除token并跳转到登录页
        localStorage.removeItem('token')
        window.location.href = '/login'
        throw new ApiException('Unauthorized', 401, await response.text(), {}, null)
      } else if (response.status === 403) {
        // 处理权限不足
        throw new ApiException('Forbidden', 403, await response.text(), {}, null)
      } else if (response.status >= 400) {
        // 其他错误处理
        console.error('API Error:', error)
        const errorText = await response.text()
        throw new ApiException(`HTTP ${response.status}`, response.status, errorText, {}, null)
      }
      throw error
    })
  }

  protected getBaseUrl(_defaultUrl: string, baseUrl?: string): string {
    return baseUrl || this.baseUrl
  }
}
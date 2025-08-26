// API 服务封装 - 使用生成的客户端
import { 
  ApiClient, 
  ApiException,
  GetUsersInput,
  CreateOrUpdateUserInput,
  UserDto,
  AssignRoleInput,
  GetRolesInput,
  CreateOrUpdateRoleInput,
  GetDropDownListInput,
  AccountLoginDto,
  CreateOrUpdateOrgUnitNodeInput
} from './generated'
import { ElMessage } from 'element-plus'

// 创建配置好的 API 客户端实例
class ApiService {
  private apiClient: ApiClient
  
  constructor() {
    // 直接使用后端地址，不添加 /api 前缀，因为 NSwag 生成的客户端已经包含了路径
    const baseUrl = import.meta.env.DEV 
      ? 'http://localhost:6298' 
      : ''
    this.apiClient = new ApiClient(baseUrl)
  }

  // 封装通用的错误处理
  private async handleRequest<T>(request: () => Promise<T>): Promise<T> {
    try {
      return await request()
    } catch (error: any) {
      if (error instanceof ApiException) {
        // 统一错误处理
        switch (error.status) {
          case 401:
            ElMessage.error('未授权，请重新登录')
            localStorage.removeItem('token')
            window.location.href = '/login'
            break
          case 403:
            ElMessage.error('权限不足')
            break
          case 404:
            ElMessage.error('请求的资源不存在')
            break
          case 500:
            ElMessage.error('服务器内部错误')
            break
          default:
            ElMessage.error(error.message || '请求失败')
        }
      } else {
        console.error('Unexpected error:', error)
        ElMessage.error('网络错误，请稍后重试')
      }
      throw error
    }
  }

  // 用户相关 API
  async getUsers(filterText?: string, sorting?: string, maxResultCount?: number, skipCount?: number) {
    const input = new GetUsersInput({
      filterText,
      sorting,
      maxResultCount: maxResultCount || 10,
      skipCount: skipCount || 0
    })
    
    return this.handleRequest(() => this.apiClient.getPaged2(input))
  }

  async getUser(id: string) {
    return this.handleRequest(() => this.apiClient.getForEdit3(id))
  }

  async createOrUpdateUser(user: UserDto) {
    const input = new CreateOrUpdateUserInput({ user })
    return this.handleRequest(() => this.apiClient.createOrUpdate3(input))
  }

  async assignRoles(userIds: string[], roleIds: string[]) {
    const input = new AssignRoleInput({ userIds, roleIds })
    return this.handleRequest(() => this.apiClient.assignRole(input))
  }

  // 角色相关 API
  async getRoles(filterText?: string, sorting?: string, maxResultCount?: number, skipCount?: number) {
    const input = new GetRolesInput({
      filterText,
      sorting,
      maxResultCount: maxResultCount || 10,
      skipCount: skipCount || 0
    })
    
    return this.handleRequest(() => this.apiClient.getPaged(input))
  }

  async getRole(id: string) {
    return this.handleRequest(() => this.apiClient.getForEdit2(id))
  }

  async createOrUpdateRole(role: any) {
    const input = new CreateOrUpdateRoleInput({ role })
    return this.handleRequest(() => this.apiClient.createOrUpdate2(input))
  }

  async getRoleDropDownList() {
    const input = new GetDropDownListInput()
    return this.handleRequest(() => this.apiClient.getDropDownList(input))
  }

  // 认证相关 API
  async login(userName: string, password: string) {
    const loginDto = new AccountLoginDto({ 
      userName, 
      password,
      isApiLogin: false  // 设置 IsApiLogin 为 false
    })
    return this.handleRequest(() => this.apiClient.login(loginDto))
  }

  async refreshToken(refreshToken: string) {
    return this.handleRequest(() => this.apiClient.refreshToken(refreshToken))
  }

  // 权限相关 API
  async getAllPermissions() {
    return this.handleRequest(() => this.apiClient.getAllPermission())
  }

  // 组织架构相关 API
  async getOrgTree(orgUnitNodeId?: string, parentOrgUnitNodeId?: string, filterText?: string) {
    return this.handleRequest(() => 
      this.apiClient.getTree(orgUnitNodeId, parentOrgUnitNodeId, filterText)
    )
  }

  async createOrUpdateOrgUnit(orgUnitNode: any) {
    const input = new CreateOrUpdateOrgUnitNodeInput({ orgUnitNode })
    return this.handleRequest(() => this.apiClient.createOrUpdate(input))
  }

  // 文件相关 API
  async getFile() {
    return this.handleRequest(() => this.apiClient.getFile())
  }
}

// 创建单例实例
export const apiService = new ApiService()

// 导出类型定义和类（从生成的代码中重新导出）
export {
  ApiException,
  GetUsersInput,
  CreateOrUpdateUserInput,
  UserDto,
  AssignRoleInput,
  GetRolesInput,
  CreateOrUpdateRoleInput,
  GetDropDownListInput,
  AccountLoginDto,
  CreateOrUpdateOrgUnitNodeInput
} from './generated'

export type {
  UserListDto,
  RoleDto,
  RoleListDto,
  DropDownListDto,
  AuthenticateResultModel,
  OrgUnitNodeEditDto,
  PermissionDto
} from './generated'

export default apiService
// 使用 NSwag 自动生成的 Service Proxies - 优雅的API服务层
import { 
  AccountClientServiceProxy,
  UserServiceProxy,
  RoleServiceProxy,
  OrgUnitNodeServiceProxy,
  FileServiceProxy
} from './service-proxies'

// 服务代理配置
const baseUrl = import.meta.env.DEV ? 'http://localhost:6298' : ''

// 创建服务代理实例
const authServiceProxy = new AccountClientServiceProxy(baseUrl)
const userServiceProxy = new UserServiceProxy(baseUrl)
const roleServiceProxy = new RoleServiceProxy(baseUrl)
const orgUnitServiceProxy = new OrgUnitNodeServiceProxy(baseUrl)
const fileServiceProxy = new FileServiceProxy(baseUrl)

// 统一导出所有服务代理 - 优雅的服务分类，解决了歧义方法名问题
export const serviceProxies = {
  // 认证服务 - AccountClientServiceProxy
  auth: authServiceProxy,
  // 用户服务 - UserServiceProxy (不再有 createOrUpdate3、getForEdit3 等歧义名称)
  user: userServiceProxy,
  // 角色服务 - RoleServiceProxy (不再有 createOrUpdate2、getForEdit2 等歧义名称)  
  role: roleServiceProxy,
  // 组织架构服务 - OrgUnitNodeServiceProxy (不再有 createOrUpdate、getForEdit 等歧义名称)
  orgUnit: orgUnitServiceProxy,
  // 文件服务 - FileServiceProxy
  file: fileServiceProxy
}

// 导出服务实例
export {
  authServiceProxy,
  userServiceProxy,
  roleServiceProxy,
  orgUnitServiceProxy,
  fileServiceProxy
}

// 导出服务代理类型 - 用于需要扩展或继承的场景
export type {
  AccountClientServiceProxy,
  UserServiceProxy,
  RoleServiceProxy,
  OrgUnitNodeServiceProxy,  
  FileServiceProxy
}

// 从 service-proxies 文件导出常用类型和异常
export {
  ApiException,
  AccountLoginDto
} from './service-proxies'

export type {
  AuthenticateResultModel,
  UserDto,
  UserListDto,
  RoleDto,
  RoleListDto,
  DropDownListDto,
  OrgUnitNodeEditDto,
  PermissionDto,
  CreateOrUpdateUserInput,
  GetUsersInput,
  AssignRoleInput,
  GetRolesInput,
  CreateOrUpdateRoleInput,
  GetDropDownListInput,
  CreateOrUpdateOrgUnitNodeInput,
  EntityDto
} from './service-proxies'

export default serviceProxies
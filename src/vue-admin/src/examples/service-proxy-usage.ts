// Service Proxy 使用示例 - 演示如何使用新的优雅API服务层

import { serviceProxies } from '@/api'
import { 
  GetUsersInput,
  CreateOrUpdateUserInput,
  EntityDto,
  GetRolesInput,
  CreateOrUpdateRoleInput,
  GetDropDownListInput,
  CreateOrUpdateOrgUnitNodeInput
} from '@/api/generated'

// 示例 1: 用户认证
export async function authExample() {
  try {
    console.log('=== 认证服务示例 ===')
    
    // 登录 - 方法名清晰，不再有歧义
    const loginResult = await serviceProxies.auth.login('admin', 'password')
    console.log('登录成功:', loginResult)
    
    // 刷新Token
    const refreshResult = await serviceProxies.auth.refreshToken('some_refresh_token')
    console.log('Token刷新成功:', refreshResult)
    
  } catch (error) {
    console.error('认证示例失败:', error)
  }
}

// 示例 2: 用户管理
export async function userManagementExample() {
  try {
    console.log('=== 用户管理服务示例 ===')
    
    // 获取用户列表 - 替代了歧义的 getPaged2
    const getUsersInput = new GetUsersInput({
      filterText: 'admin',
      maxResultCount: 10,
      skipCount: 0
    })
    const usersList = await serviceProxies.user.getUsers(getUsersInput)
    console.log('用户列表:', usersList)
    
    // 获取用户详情 - 替代了歧义的 getForEdit3
    const userDetail = await serviceProxies.user.getUserForEdit('user-id-123')
    console.log('用户详情:', userDetail)
    
    // 创建或更新用户 - 替代了歧义的 createOrUpdate3
    const createUserInput = new CreateOrUpdateUserInput({
      userName: 'newuser',
      name: '新用户'
      // 注意：根据实际的 CreateOrUpdateUserInput 类型调整字段
    })
    await serviceProxies.user.createOrUpdateUser(createUserInput)
    console.log('用户创建/更新成功')
    
    // 删除用户 - 方法名明确
    const deleteUserInput = new EntityDto({ id: 'user-to-delete' })
    await serviceProxies.user.deleteUser(deleteUserInput)
    console.log('用户删除成功')
    
  } catch (error) {
    console.error('用户管理示例失败:', error)
  }
}

// 示例 3: 角色管理
export async function roleManagementExample() {
  try {
    console.log('=== 角色管理服务示例 ===')
    
    // 获取角色列表 - 替代了歧义的 getPaged
    const getRolesInput = new GetRolesInput({
      filterText: 'admin'
    })
    const rolesList = await serviceProxies.role.getRoles(getRolesInput)
    console.log('角色列表:', rolesList)
    
    // 获取角色详情 - 替代了歧义的 getForEdit2
    const roleDetail = await serviceProxies.role.getRoleForEdit('role-id-123')
    console.log('角色详情:', roleDetail)
    
    // 创建或更新角色 - 替代了歧义的 createOrUpdate2
    const createRoleInput = new CreateOrUpdateRoleInput({
      name: 'Editor',
      displayName: '编辑者'
      // 注意：根据实际的 CreateOrUpdateRoleInput 类型调整字段
    })
    await serviceProxies.role.createOrUpdateRole(createRoleInput)
    console.log('角色创建/更新成功')
    
    // 获取角色下拉列表
    const roleDropDownInput = new GetDropDownListInput()
    const roleDropDown = await serviceProxies.role.getRoleDropDownList(roleDropDownInput)
    console.log('角色下拉列表:', roleDropDown)
    
  } catch (error) {
    console.error('角色管理示例失败:', error)
  }
}

// 示例 4: 组织架构管理
export async function orgUnitManagementExample() {
  try {
    console.log('=== 组织架构管理服务示例 ===')
    
    // 获取组织树 - 方法名清晰
    const orgTree = await serviceProxies.orgUnit.getOrgTree()
    console.log('组织架构树:', orgTree)
    
    // 获取组织节点详情 - 替代了歧义的 getForEdit
    const orgDetail = await serviceProxies.orgUnit.getOrgUnitForEdit('org-id-123')
    console.log('组织节点详情:', orgDetail)
    
    // 创建或更新组织节点 - 替代了歧义的 createOrUpdate
    const createOrgInput = new CreateOrUpdateOrgUnitNodeInput({
      displayName: '新部门'
      // 注意：根据实际的 CreateOrUpdateOrgUnitNodeInput 类型调整字段
    })
    await serviceProxies.orgUnit.createOrUpdateOrgUnit(createOrgInput)
    console.log('组织节点创建/更新成功')
    
    // 删除组织节点
    const deleteOrgInput = new EntityDto({ id: 'org-to-delete' })
    await serviceProxies.orgUnit.deleteOrgUnit(deleteOrgInput)
    console.log('组织节点删除成功')
    
  } catch (error) {
    console.error('组织架构管理示例失败:', error)
  }
}

// 示例 5: 文件服务
export async function fileServiceExample() {
  try {
    console.log('=== 文件服务示例 ===')
    
    // 获取文件
    const fileInfo = await serviceProxies.file.getFile()
    console.log('文件信息:', fileInfo)
    
  } catch (error) {
    console.error('文件服务示例失败:', error)
  }
}

// 整合示例
export async function runAllExamples() {
  console.log('🚀 Service Proxy 优雅API服务层使用示例')
  console.log('✅ 解决了 NSwag 生成的 createOrUpdate2、createOrUpdate3 等歧义方法名')
  console.log('✅ 提供了清晰、语义化的API方法')
  console.log('✅ 统一的错误处理和请求处理')
  console.log('✅ 参考 service-proxies 模式，代码更加优雅和可维护')
  
  await authExample()
  await userManagementExample() 
  await roleManagementExample()
  await orgUnitManagementExample()
  await fileServiceExample()
  
  console.log('🎉 所有示例运行完成')
}

// 对比：旧的歧义方法 vs 新的清晰方法
export const methodComparison = {
  '旧的歧义方法': {
    'createOrUpdate': '不知道是创建什么',
    'createOrUpdate2': '不知道和 createOrUpdate 有什么区别',  
    'createOrUpdate3': '更加令人困惑',
    'getForEdit': '不知道获取什么进行编辑',
    'getForEdit2': '歧义性方法',
    'getForEdit3': '更多歧义',
    'getPaged': '不知道分页获取什么',
    'getPaged2': '歧义性方法'
  },
  '新的清晰方法': {
    'serviceProxies.user.createOrUpdateUser': '明确是用户操作',
    'serviceProxies.role.createOrUpdateRole': '明确是角色操作',
    'serviceProxies.orgUnit.createOrUpdateOrgUnit': '明确是组织架构操作',
    'serviceProxies.user.getUserForEdit': '明确获取用户编辑信息',
    'serviceProxies.role.getRoleForEdit': '明确获取角色编辑信息',
    'serviceProxies.orgUnit.getOrgUnitForEdit': '明确获取组织节点编辑信息',
    'serviceProxies.user.getUsers': '明确获取用户列表',
    'serviceProxies.role.getRoles': '明确获取角色列表'
  }
}

export default {
  authExample,
  userManagementExample,
  roleManagementExample,
  orgUnitManagementExample,
  fileServiceExample,
  runAllExamples,
  methodComparison
}
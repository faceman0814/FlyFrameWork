import request from '@/utils/request'

export function getRoleList(params?: any) {
  return request({
    url: '/roles',
    method: 'get',
    params
  })
}

export function createRole(data: any) {
  return request({
    url: '/roles',
    method: 'post',
    data
  })
}

export function updateRole(id: string, data: any) {
  return request({
    url: `/roles/${id}`,
    method: 'put',
    data
  })
}

export function deleteRole(id: string) {
  return request({
    url: `/roles/${id}`,
    method: 'delete'
  })
}

export function getRolePermissions(roleId: string) {
  return request({
    url: `/roles/${roleId}/permissions`,
    method: 'get'
  })
}

export function updateRolePermissions(roleId: string, permissions: string[]) {
  return request({
    url: `/roles/${roleId}/permissions`,
    method: 'put',
    data: { permissions }
  })
}
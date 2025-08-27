import request from '@/utils/request'

export function getUserList(params?: any) {
  return request({
    url: '/users',
    method: 'get',
    params
  })
}

export function createUser(data: any) {
  return request({
    url: '/users',
    method: 'post',
    data
  })
}

export function updateUser(id: string, data: any) {
  return request({
    url: `/users/${id}`,
    method: 'put',
    data
  })
}

export function deleteUser(id: string) {
  return request({
    url: `/users/${id}`,
    method: 'delete'
  })
}

export function getUserById(id: string) {
  return request({
    url: `/users/${id}`,
    method: 'get'
  })
}

export function changeUserStatus(id: string, status: number) {
  return request({
    url: `/users/${id}/status`,
    method: 'put',
    data: { status }
  })
}

export function resetUserPassword(id: string, password: string) {
  return request({
    url: `/users/${id}/reset-password`,
    method: 'put',
    data: { password }
  })
}

export function exportUsers(params?: any) {
  return request({
    url: '/users/export',
    method: 'get',
    params,
    responseType: 'blob'
  })
}
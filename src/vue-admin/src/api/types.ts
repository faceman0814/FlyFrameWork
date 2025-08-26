// 临时类型定义，等 NSwag 生成后会被替换
export interface UserDto {
  id: number
  userName: string
  email: string
  createdAt: Date
  isActive: boolean
}

export interface CreateUserDto {
  userName: string
  email: string
  password: string
}

export interface UpdateUserDto {
  userName: string
  email: string
  isActive: boolean
}

export interface UserQueryDto {
  page?: number
  pageSize?: number
  search?: string
  isActive?: boolean
}

export interface PagedResultOfUserDto {
  items: UserDto[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

// 用户客户端接口
export interface UserClient {
  getUsers(page?: number, pageSize?: number, search?: string, isActive?: boolean): Promise<PagedResultOfUserDto>
  getUser(id: number): Promise<UserDto>
  createUser(createUserDto: CreateUserDto): Promise<UserDto>
  updateUser(id: number, updateUserDto: UpdateUserDto): Promise<UserDto>
  deleteUser(id: number): Promise<void>
}
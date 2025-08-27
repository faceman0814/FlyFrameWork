# 组件文档

## 公共组件

### ErrorBoundary
错误边界组件，用于捕获子组件的错误。

**使用示例：**
```vue
<template>
  <ErrorBoundary>
    <YourComponent />
  </ErrorBoundary>
</template>
```

### Pagination
分页组件，支持自定义页码范围和页面大小。

**Props：**
- `currentPage`: 当前页码
- `pageSize`: 每页显示数量
- `total`: 总数据量
- `pageSizes`: 可选页面大小数组

**使用示例：**
```vue
<template>
  <Pagination
    v-model:currentPage="currentPage"
    v-model:pageSize="pageSize"
    :total="total"
    :page-sizes="[10, 20, 50, 100]"
    @size-change="handleSizeChange"
    @current-change="handleCurrentChange"
  />
</template>
```

### RouteLoadingIndicator
路由加载指示器，在路由切换时显示加载状态。

**使用示例：**
```vue
<template>
  <RouteLoadingIndicator />
</template>
```

### ThemeSwitcher
主题切换器，支持明暗主题切换。

**Props：**
- `mode`: 主题模式 ('light' | 'dark' | 'auto')

**使用示例：**
```vue
<template>
  <ThemeSwitcher v-model:mode="themeMode" />
</template>
```

### VirtualTable
虚拟滚动表格组件，适用于大数据量展示。

**Props：**
- `data`: 表格数据数组
- `columns`: 列配置数组
- `height`: 表格高度
- `itemHeight`: 每行高度

**使用示例：**
```vue
<template>
  <VirtualTable
    :data="tableData"
    :columns="columns"
    :height="400"
    :item-height="50"
  />
</template>
```

## 布局组件

### Layout
主布局组件，包含侧边栏、顶部导航和主内容区域。

### AppMain
主内容区域组件。

### Navbar
顶部导航栏组件。

### Sidebar
侧边栏组件，支持菜单折叠和路由导航。

**特性：**
- 支持多级菜单
- 菜单折叠/展开
- 活动状态指示
- 权限控制

### Breadcrumb
面包屑导航组件。

### TagsView
标签页视图组件，显示已访问的页面标签。

**特性：**
- 标签页管理
- 右键菜单
- 标签页关闭
- 标签页固定

## 开发指南

### 创建新组件

1. 在 `src/components` 目录下创建组件文件夹
2. 创建 `index.vue` 文件
3. 添加 TypeScript 类型定义
4. 编写单元测试
5. 更新组件文档

### 组件规范

- 使用 TypeScript 进行类型约束
- 提供清晰的 Props 接口
- 支持事件触发
- 提供插槽扩展
- 添加单元测试覆盖
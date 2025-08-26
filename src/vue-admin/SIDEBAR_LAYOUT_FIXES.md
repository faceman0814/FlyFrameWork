# 侧边栏布局优化修复

## 问题描述
1. 侧边栏收缩时logo位置太靠上，首页图标位置相比较下面设置太靠右
2. 侧边栏展开时，logo位置太靠右没有跟随位置变化

## 解决方案

### 1. Logo组件布局调整 (`src/layout/components/Sidebar/Logo.vue`)

#### 展开状态优化
- 修改容器对齐方式：从 `justify-content: center` 改为 `justify-content: flex-start`
- 增加左右内边距：从 `padding: 0 12px` 调整为 `padding: 0 20px`
- 调整logo容器高度：从 `60px` 调整为 `54px`，与菜单项更协调

#### 收缩状态优化
- 保持居中对齐：`justify-content: center`
- 调整logo尺寸：缩小到 `28x28px`，与菜单图标大小协调
- 优化内部元素尺寸，保持视觉平衡

### 2. 侧边栏整体宽度调整 (`src/layout/index.vue`)

#### 收缩宽度优化
- 将收缩状态宽度从 `64px` 调整为 `54px`
- 相应调整主内容区左边距从 `64px` 到 `54px`
- 确保图标在收缩状态下完全居中

### 3. 菜单项样式统一 (`src/layout/components/Sidebar/index.vue`)

#### 对齐一致性
- 统一菜单项内边距：`padding: 0 20px`，与logo保持一致
- 优化收缩状态下的图标位置和大小
- 调整菜单项边距以实现更好的视觉对齐

#### 滚动区域调整
- 更新滚动区域高度：`calc(100% - 54px)` 对应新的logo高度

## 技术细节

### 关键CSS变更
```scss
// Logo容器 - 展开状态左对齐
.sidebar-logo-container {
  justify-content: flex-start;
  height: 54px;
  
  .sidebar-logo-link {
    padding: 0 20px;
  }
}

// Logo容器 - 收缩状态居中
.sidebar-logo-container.collapse {
  justify-content: center;
  
  .logo-icon {
    width: 28px;
    height: 28px;
  }
}

// 侧边栏宽度
.hideSidebar .sidebar-container {
  width: 54px !important;
}

// 菜单项对齐
.el-menu-item, .el-submenu__title {
  padding: 0 20px;
  margin: 3px 16px;
}
```

### 视觉效果改进
1. **展开状态**：logo与菜单项左边距完全对齐，视觉统一
2. **收缩状态**：logo和菜单图标都完美居中，尺寸协调
3. **过渡动画**：所有变化都有平滑的过渡效果
4. **响应式**：在不同屏幕尺寸下保持良好的布局

## 验证方法
1. 启动开发服务器：`npm run dev`
2. 访问应用并测试侧边栏收缩/展开功能
3. 检查logo和菜单项的对齐情况
4. 确认在不同状态下的视觉协调性

## 兼容性
- 保持了原有的主题样式兼容性
- 支持响应式布局
- 保持了动画过渡效果
- 兼容深色/浅色主题切换
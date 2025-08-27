# 🎨 主题定制指南

Vue Admin 提供了强大的主题定制系统，支持多种预设主题、自定义配色和实时预览。

## 主题系统概览

### 🌈 预设主题
系统提供12+精心设计的主题配色：

#### 经典主题
- **默认蓝紫** (`default`): 专业的蓝紫渐变，适合企业应用
- **海洋蓝** (`ocean`): 清新的蓝色系，营造宁静氛围
- **森林绿** (`forest`): 自然的绿色系，护眼舒适
- **日落橙** (`sunset`): 温暖的橙色系，活力四射

#### 季节主题
- **春天** (`spring`): 清新嫩绿，充满生机
- **夏天** (`summer`): 热烈红橙，激情洋溢
- **秋天** (`autumn`): 温暖金黄，收获满满
- **冬天** (`winter`): 冷静蓝白，纯净简约

#### 特殊效果主题
- **霓虹** (`neon`): 炫酷的霓虹配色，科技感十足
- **星河** (`galaxy`): 深邃的宇宙配色，神秘优雅
- **极光** (`aurora`): 梦幻的极光效果，绚丽多彩
- **玫瑰金** (`rose`): 优雅的玫瑰金配色，时尚高端

### 🎯 定制特性
- **实时预览**: 选择主题立即生效
- **自定义颜色**: 支持任意颜色选择
- **布局设置**: 圆角、阴影、透明度调节
- **配置导入导出**: 主题配置可保存和分享
- **暗色模式**: 所有主题都支持深色版本

## 快速开始

### 使用预设主题

```vue
<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

// 切换到海洋主题
themeStore.setTheme('ocean')

// 启用暗色模式
themeStore.setDarkMode(true)
</script>
```

### 使用主题定制器

主题定制器组件提供了完整的主题管理界面：

```vue
<template>
  <ThemeCustomizer />
</template>

<script setup lang="ts">
import ThemeCustomizer from '@/components/ThemeCustomizer/index.vue'
</script>
```

## 自定义主题

### 创建新的预设主题

在 `src/utils/theme-colors.ts` 中添加新主题：

```typescript
// 添加新主题配置
export const themePresets: Record<string, ThemeConfig> = {
  // 现有主题...
  
  // 新增自定义主题
  myCustomTheme: {
    primary: '#FF6B6B',      // 主色
    secondary: '#4ECDC4',    // 辅助色  
    accent: '#45B7D1',       // 强调色
    success: '#96CEB4',      // 成功色
    warning: '#FFEAA7',      // 警告色
    error: '#DDA0DD',        // 错误色
    info: '#74B9FF',         // 信息色
    
    // 语义化颜色
    background: '#FFFFFF',
    surface: '#F8F9FA',
    onPrimary: '#FFFFFF',
    onSurface: '#2C3E50'
  }
}
```

### 动态应用主题

```typescript
import { applyTheme } from '@/utils/theme-colors'

// 应用预设主题
applyTheme('myCustomTheme')

// 应用自定义颜色
applyTheme('custom', {
  primary: '#FF5722',
  secondary: '#03DAC6'
})
```

## 主题配置详解

### ThemeConfig 接口

```typescript
interface ThemeConfig {
  // 主要颜色
  primary: string        // 主色调，用于按钮、链接等主要元素
  secondary: string      // 辅助色，用于次要元素
  accent: string         // 强调色，用于突出显示
  
  // 状态颜色  
  success: string        // 成功状态色
  warning: string        // 警告状态色
  error: string          // 错误状态色
  info: string           // 信息状态色
  
  // 背景和表面
  background: string     // 页面背景色
  surface: string        // 卡片、面板背景色
  
  // 文字颜色
  onPrimary: string      // 主色上的文字颜色
  onSurface: string      // 表面上的文字颜色
}
```

### CSS变量映射

主题配置会自动转换为CSS变量：

```css
:root {
  /* 主题颜色 */
  --color-primary: #409EFF;
  --color-secondary: #909399;
  --color-accent: #E6A23C;
  
  /* 状态颜色 */
  --color-success: #67C23A;
  --color-warning: #E6A23C;
  --color-error: #F56C6C;
  --color-info: #909399;
  
  /* 背景颜色 */
  --color-background: #FFFFFF;
  --color-surface: #F5F7FA;
  
  /* 文字颜色 */
  --color-on-primary: #FFFFFF;
  --color-on-surface: #303133;
  
  /* 自动生成的变体颜色 */
  --color-primary-light: #79BBFF;
  --color-primary-dark: #337ECC;
  --color-primary-rgb: 64, 158, 255;
}
```

## 样式集成

### 在组件中使用主题色

```vue
<template>
  <div class="themed-component">
    <button class="primary-button">主要按钮</button>
    <div class="info-card">信息卡片</div>
  </div>
</template>

<style scoped>
.themed-component {
  background-color: var(--color-surface);
  color: var(--color-on-surface);
}

.primary-button {
  background-color: var(--color-primary);
  color: var(--color-on-primary);
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  transition: background-color 0.3s ease;
}

.primary-button:hover {
  background-color: var(--color-primary-dark);
}

.info-card {
  background-color: var(--color-info);
  color: white;
  padding: 12px;
  border-radius: 6px;
  margin-top: 8px;
}
</style>
```

### 响应主题变化

```vue
<script setup lang="ts">
import { computed } from 'vue'
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

// 响应式主题样式
const dynamicStyles = computed(() => ({
  backgroundColor: themeStore.isDark 
    ? 'var(--color-surface-dark)' 
    : 'var(--color-surface)',
  color: themeStore.isDark
    ? 'var(--color-on-surface-dark)'
    : 'var(--color-on-surface)'
}))
</script>

<template>
  <div :style="dynamicStyles">
    动态主题内容
  </div>
</template>
```

## 暗色模式

### 暗色主题配置

每个主题都会自动生成对应的暗色版本：

```typescript
// 自动生成的暗色变量
:root[data-theme="dark"] {
  --color-background: #1E1E1E;
  --color-surface: #2D2D2D;
  --color-on-surface: #FFFFFF;
  // ...其他暗色变量
}
```

### 使用暗色模式

```typescript
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

// 切换暗色模式
themeStore.toggleDarkMode()

// 跟随系统主题
themeStore.setFollowSystem(true)
```

## 高级定制

### 创建主题变体

```typescript
// 生成主题变体
export function createThemeVariant(
  baseTheme: string,
  modifications: Partial<ThemeConfig>
): ThemeConfig {
  const base = themePresets[baseTheme]
  return {
    ...base,
    ...modifications
  }
}

// 使用示例
const customOcean = createThemeVariant('ocean', {
  primary: '#2196F3',
  accent: '#FF9800'
})
```

### 主题动画过渡

```css
/* 全局主题切换动画 */
* {
  transition: 
    background-color 0.3s cubic-bezier(0.4, 0, 0.2, 1),
    color 0.3s cubic-bezier(0.4, 0, 0.2, 1),
    border-color 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

/* 禁用过渡的元素 */
.no-transition * {
  transition: none !important;
}
```

### 主题持久化

```typescript
// 主题配置本地存储
export class ThemePersistence {
  private static STORAGE_KEY = 'vue-admin-theme'
  
  static save(config: ThemeState): void {
    localStorage.setItem(
      this.STORAGE_KEY, 
      JSON.stringify(config)
    )
  }
  
  static load(): ThemeState | null {
    const stored = localStorage.getItem(this.STORAGE_KEY)
    return stored ? JSON.parse(stored) : null
  }
  
  static clear(): void {
    localStorage.removeItem(this.STORAGE_KEY)
  }
}
```

## Element Plus 集成

### 覆盖Element Plus变量

```scss
// src/styles/element-ui.scss
@use 'element-plus/theme-chalk/src/common/var.scss' with (
  $colors: (
    'primary': (
      'base': var(--color-primary)
    ),
    'success': (
      'base': var(--color-success)
    ),
    'warning': (
      'base': var(--color-warning)
    ),
    'danger': (
      'base': var(--color-error)
    ),
    'info': (
      'base': var(--color-info)
    )
  )
);
```

### 动态组件主题

```vue
<script setup lang="ts">
import { ElConfigProvider } from 'element-plus'
import { computed } from 'vue'
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

const elementTheme = computed(() => ({
  colorPrimary: themeStore.primaryColor,
  colorSuccess: themeStore.successColor,
  colorWarning: themeStore.warningColor,
  colorDanger: themeStore.errorColor,
  colorInfo: themeStore.infoColor
}))
</script>

<template>
  <ElConfigProvider :theme="elementTheme">
    <router-view />
  </ElConfigProvider>
</template>
```

## 调试和测试

### 主题调试工具

```typescript
// 主题调试助手
export class ThemeDebugger {
  static logCurrentTheme(): void {
    const root = document.documentElement
    const computedStyle = getComputedStyle(root)
    
    console.group('🎨 当前主题配置')
    console.log('主色:', computedStyle.getPropertyValue('--color-primary'))
    console.log('背景色:', computedStyle.getPropertyValue('--color-background'))
    console.log('暗色模式:', root.dataset.theme === 'dark')
    console.groupEnd()
  }
  
  static exportTheme(): string {
    const themeStore = useThemeStore()
    return JSON.stringify(themeStore.$state, null, 2)
  }
}
```

### 主题测试用例

```typescript
// tests/theme.test.ts
import { describe, it, expect } from 'vitest'
import { applyTheme, themePresets } from '@/utils/theme-colors'

describe('主题系统', () => {
  it('应该正确应用预设主题', () => {
    applyTheme('ocean')
    
    const root = document.documentElement
    const primaryColor = getComputedStyle(root)
      .getPropertyValue('--color-primary')
    
    expect(primaryColor).toBe(themePresets.ocean.primary)
  })
  
  it('应该支持自定义颜色', () => {
    const customColors = { primary: '#FF0000' }
    applyTheme('custom', customColors)
    
    const root = document.documentElement
    const primaryColor = getComputedStyle(root)
      .getPropertyValue('--color-primary')
    
    expect(primaryColor).toBe('#FF0000')
  })
})
```

## 性能优化

### 主题切换优化

```typescript
// 使用防抖优化频繁切换
import { debounce } from 'lodash-es'

const debouncedApplyTheme = debounce(applyTheme, 100)

// 批量更新CSS变量
export function batchUpdateTheme(updates: Record<string, string>): void {
  const root = document.documentElement
  
  // 临时禁用过渡
  root.classList.add('no-transition')
  
  // 批量更新
  Object.entries(updates).forEach(([property, value]) => {
    root.style.setProperty(`--${property}`, value)
  })
  
  // 重新启用过渡
  requestAnimationFrame(() => {
    root.classList.remove('no-transition')
  })
}
```

### 懒加载主题资源

```typescript
// 按需加载主题资源
export async function loadThemeAssets(themeName: string): Promise<void> {
  if (themeName === 'galaxy') {
    // 动态导入特殊主题资源
    await import('@/styles/themes/galaxy-effects.scss')
  }
}
```

## 故障排除

### 常见问题

1. **主题不生效**
   - 检查CSS变量是否正确设置
   - 确认主题配置格式正确
   - 验证Element Plus集成

2. **切换卡顿**
   - 优化过渡动画
   - 减少同时更新的CSS属性
   - 使用requestAnimationFrame

3. **暗色模式异常**
   - 检查data-theme属性
   - 验证暗色CSS规则
   - 确认媒体查询设置

### 调试技巧

```typescript
// 主题状态检查
function checkThemeHealth(): ThemeHealthReport {
  const root = document.documentElement
  const themeStore = useThemeStore()
  
  return {
    cssVariablesLoaded: !!getComputedStyle(root).getPropertyValue('--color-primary'),
    storeInitialized: !!themeStore.currentTheme,
    darkModeConsistent: (root.dataset.theme === 'dark') === themeStore.isDark,
    elementPlusIntegrated: !!document.querySelector('.el-button')
  }
}
```

## 最佳实践

### 主题设计原则

1. **色彩和谐**: 使用色彩理论确保配色和谐
2. **对比度**: 保证文字和背景有足够对比度
3. **一致性**: 保持整体视觉风格统一
4. **可访问性**: 考虑色盲用户和无障碍需求

### 开发建议

1. **使用语义化变量**: 优先使用语义化的CSS变量名
2. **渐进增强**: 提供回退方案支持旧浏览器
3. **性能优先**: 避免不必要的重绘和重排
4. **测试覆盖**: 在不同主题下测试所有功能

---

🎨 通过强大的主题系统，让您的应用拥有独特的视觉体验！
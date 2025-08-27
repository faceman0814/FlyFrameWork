# 多语言设置指南

本项目支持多种语言，并提供了完整的国际化解决方案。

## 支持的语言

- 🇨🇳 中文简体 (zh-cn)
- 🇺🇸 English (en)
- 🇯🇵 日本語 (ja)
- 🇷🇺 Русский (ru)  
- 🇫🇷 Français (fr)
- 🇪🇸 Español (es)

## 语言文件结构

```
src/locales/
├── zh-cn.json    # 中文简体
├── en.json       # 英语
├── ja.json       # 日语
├── ru.json       # 俄语
├── fr.json       # 法语
└── es.json       # 西班牙语
```

## 添加新语言

### 1. 创建语言文件

在 `src/locales/` 目录下创建新的语言文件，例如 `de.json`（德语）：

```json
{
  "navbar": {
    "dashboard": "Dashboard",
    "profile": "Profil",
    // ... 其他翻译
  }
}
```

### 2. 更新主配置

在 `src/main.ts` 中添加新语言：

```typescript
// 导入语言文件
import deLocale from './locales/de.json'

// 导入Element Plus语言包
import de from 'element-plus/es/locale/lang/de'

// 添加到messages对象
const messages = {
  // ... 现有语言
  de: {
    ...deLocale,
    el: de.el
  }
}

// 添加到语言映射
const elementLocales = {
  // ... 现有映射
  de: de
}
```

### 3. 更新语言选择器

在语言选择组件中添加新选项（如果有语言选择器的话）。

## 使用方法

### 在Vue组件中使用

```vue
<template>
  <div>
    <!-- 直接使用 -->
    <h1>{{ $t('dashboard.title') }}</h1>
    
    <!-- 带参数 -->
    <p>{{ $t('pagination.total', { total: 100 }) }}</p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t, locale } = useI18n()

// 在JavaScript中使用
console.log(t('common.loading'))

// 切换语言
const switchLanguage = (lang: string) => {
  locale.value = lang
  localStorage.setItem('language', lang)
}
</script>
```

### 在JavaScript/TypeScript中使用

```typescript
import { useI18n } from 'vue-i18n'

export function useTranslation() {
  const { t } = useI18n()
  return { t }
}

// 使用示例
const { t } = useTranslation()
const message = t('login.loginSuccess')
```

## 语言切换

系统会自动保存用户选择的语言到 `localStorage`，下次访问时会自动使用上次选择的语言。

```typescript
// 获取当前语言
const currentLang = localStorage.getItem('language') || 'zh-cn'

// 设置语言
localStorage.setItem('language', 'en')
```

## Element Plus 国际化

系统会根据当前选择的语言自动加载对应的 Element Plus 语言包，确保UI组件的文字也会相应改变。

## 最佳实践

### 1. 翻译键命名规范

- 使用小写字母和连字符
- 按功能模块组织
- 保持层次结构清晰

```json
{
  "user": {
    "title": "用户管理",
    "list": "用户列表",
    "actions": {
      "add": "新增用户",
      "edit": "编辑用户"
    }
  }
}
```

### 2. 处理复数形式

对于需要处理复数的情况，可以使用参数化翻译：

```json
{
  "pagination": {
    "total": "共 {count} 条记录",
    "selected": "已选择 {count} 项"
  }
}
```

### 3. 处理长文本

对于较长的文本，建议拆分成多个键：

```json
{
  "errors": {
    "pageNotFoundTitle": "页面不存在",
    "pageNotFoundDescription": "抱歉，您访问的页面不存在...",
    "pageNotFoundAction": "返回首页"
  }
}
```

### 4. 默认值处理

在使用翻译时，建议提供默认值：

```typescript
const message = t('some.key', 'Default message')
```

## 注意事项

1. **保持键的一致性**：确保所有语言文件中都有相同的键
2. **测试所有语言**：添加新翻译后要测试所有支持的语言
3. **处理RTL语言**：如果需要支持从右到左的语言，需要额外的CSS配置
4. **日期和数字格式化**：不同语言可能需要不同的日期和数字格式

## 开发工具

建议使用以下工具来管理翻译：

1. **i18n Ally** - VSCode插件，用于管理翻译文件
2. **vue-i18n-extract** - 提取和验证翻译键的工具
3. **翻译服务集成** - 可以集成Google Translate等服务来辅助翻译
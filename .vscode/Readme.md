# VSCode 插件文档

## 常用插件推荐

### 代码质量与格式化

- **Prettier** - 代码格式化工具
- **ESLint** - JavaScript/TypeScript 代码检查
- **C#** - C# 语言支持
- **C# Extensions** - C# 扩展功能
- **Auto Rename Tag** - 自动重命名配对的HTML/XML标签

### 主题与图标

- **Material Icon Theme** - 文件图标主题
- **One Dark Pro** - 深色主题
- **Dracula Official** - 经典深色主题

### 开发效率

- **Auto Close Tag** - 自动闭合标签
- **Bracket Pair Colorizer** - 括号对颜色标识
- **Path Intellisense** - 路径自动补全
- **GitLens** - Git 增强功能
- **Thunder Client** - API 测试工具

### 项目管理

- **Project Manager** - 项目管理
- **File Utils** - 文件操作工具
- **Code Spell Checker** - 拼写检查

## 格式化配置

### Prettier 配置

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "[javascript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[typescript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[vue]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  }
}
```

### C# 格式化

参考：[C# 格式化配置](https://blog.csdn.net/preserveXing/article/details/122252357?spm=1001.2014.3001.5506)

## 快捷键

### 常用快捷键

- `Ctrl + Shift + P` - 命令面板
- `Ctrl + ,` - 设置
- `Ctrl + Shift + F` - 全局搜索
- `Ctrl + F` - 文件内搜索
- `Ctrl + D` - 选择下一个相同内容
- `Alt + Shift + F` - 格式化代码

### 自定义快捷键

可以在 `keybindings.json` 中自定义快捷键配置。

## 工作区设置

### 推荐设置

```json
{
  "editor.tabSize": 2,
  "editor.insertSpaces": true,
  "editor.detectIndentation": false,
  "files.autoSave": "afterDelay",
  "files.autoSaveDelay": 1000,
  "editor.minimap.enabled": false,
  "workbench.colorTheme": "One Dark Pro"
}
```

## 扩展资源

- [VSCode 插件市场](https://marketplace.visualstudio.com/vscode)
- [插件开发指南](https://www.cnblogs.com/itiaotiao/p/12750033.html)
- [格式化配置详解](https://blog.csdn.net/preserveXing/article/details/122252357?spm=1001.2014.3001.5506)

## 注意事项

1. 安装插件后可能需要重启 VSCode
2. 某些插件可能需要额外的配置文件
3. 建议定期清理不使用的插件以保持性能
4. 团队开发时建议统一插件配置

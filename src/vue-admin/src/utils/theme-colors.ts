// 主题配色方案
export interface ThemeColors {
  name: string
  label: string
  primary: string
  primaryRgb: string
  primaryLight: string
  primaryGradient: string
  secondary?: string
  accent?: string
}

// 预定义主题配色
export const themePresets: ThemeColors[] = [
  {
    name: 'default',
    label: '默认蓝紫',
    primary: '#667eea',
    primaryRgb: '102, 126, 234',
    primaryLight: '#764ba2',
    primaryGradient: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  },
  {
    name: 'ocean',
    label: '海洋蓝',
    primary: '#00b4db',
    primaryRgb: '0, 180, 219',
    primaryLight: '#0083b0',
    primaryGradient: 'linear-gradient(135deg, #00b4db 0%, #0083b0 100%)',
    secondary: '#e3f2fd',
    accent: '#0277bd'
  },
  {
    name: 'forest',
    label: '森林绿',
    primary: '#00c851',
    primaryRgb: '0, 200, 81',
    primaryLight: '#00a844',
    primaryGradient: 'linear-gradient(135deg, #00c851 0%, #00a844 100%)',
    secondary: '#e8f5e8',
    accent: '#2e7d32'
  },
  {
    name: 'sunset',
    label: '日落橙',
    primary: '#ff6b35',
    primaryRgb: '255, 107, 53',
    primaryLight: '#ff5722',
    primaryGradient: 'linear-gradient(135deg, #ff6b35 0%, #ff5722 100%)',
    secondary: '#fff3e0',
    accent: '#e64a19'
  },
  {
    name: 'lavender',
    label: '薰衣草紫',
    primary: '#9c27b0',
    primaryRgb: '156, 39, 176',
    primaryLight: '#7b1fa2',
    primaryGradient: 'linear-gradient(135deg, #9c27b0 0%, #7b1fa2 100%)',
    secondary: '#f3e5f5',
    accent: '#6a1b9a'
  },
  {
    name: 'cherry',
    label: '樱花粉',
    primary: '#e91e63',
    primaryRgb: '233, 30, 99',
    primaryLight: '#c2185b',
    primaryGradient: 'linear-gradient(135deg, #e91e63 0%, #c2185b 100%)',
    secondary: '#fce4ec',
    accent: '#ad1457'
  },
  {
    name: 'midnight',
    label: '午夜蓝',
    primary: '#1a237e',
    primaryRgb: '26, 35, 126',
    primaryLight: '#303f9f',
    primaryGradient: 'linear-gradient(135deg, #1a237e 0%, #303f9f 100%)',
    secondary: '#e8eaf6',
    accent: '#3f51b5'
  },
  {
    name: 'emerald',
    label: '翡翠绿',
    primary: '#10b981',
    primaryRgb: '16, 185, 129',
    primaryLight: '#059669',
    primaryGradient: 'linear-gradient(135deg, #10b981 0%, #059669 100%)',
    secondary: '#d1fae5',
    accent: '#047857'
  },
  {
    name: 'amber',
    label: '琥珀金',
    primary: '#f59e0b',
    primaryRgb: '245, 158, 11',
    primaryLight: '#d97706',
    primaryGradient: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)',
    secondary: '#fef3c7',
    accent: '#b45309'
  },
  {
    name: 'rose',
    label: '玫瑰红',
    primary: '#f43f5e',
    primaryRgb: '244, 63, 94',
    primaryLight: '#e11d48',
    primaryGradient: 'linear-gradient(135deg, #f43f5e 0%, #e11d48 100%)',
    secondary: '#ffe4e6',
    accent: '#be123c'
  },
  {
    name: 'violet',
    label: '紫罗兰',
    primary: '#8b5cf6',
    primaryRgb: '139, 92, 246',
    primaryLight: '#7c3aed',
    primaryGradient: 'linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%)',
    secondary: '#f3e8ff',
    accent: '#6d28d9'
  },
  {
    name: 'teal',
    label: '青蓝色',
    primary: '#14b8a6',
    primaryRgb: '20, 184, 166',
    primaryLight: '#0d9488',
    primaryGradient: 'linear-gradient(135deg, #14b8a6 0%, #0d9488 100%)',
    secondary: '#ccfbf1',
    accent: '#0f766e'
  }
]

// 季节性主题
export const seasonalThemes: ThemeColors[] = [
  {
    name: 'spring',
    label: '春天',
    primary: '#8bc34a',
    primaryRgb: '139, 195, 74',
    primaryLight: '#689f38',
    primaryGradient: 'linear-gradient(135deg, #8bc34a 0%, #689f38 100%)'
  },
  {
    name: 'summer',
    label: '夏天',
    primary: '#ff9800',
    primaryRgb: '255, 152, 0',
    primaryLight: '#f57c00',
    primaryGradient: 'linear-gradient(135deg, #ff9800 0%, #f57c00 100%)'
  },
  {
    name: 'autumn',
    label: '秋天',
    primary: '#ff5722',
    primaryRgb: '255, 87, 34',
    primaryLight: '#d84315',
    primaryGradient: 'linear-gradient(135deg, #ff5722 0%, #d84315 100%)'
  },
  {
    name: 'winter',
    label: '冬天',
    primary: '#607d8b',
    primaryRgb: '96, 125, 139',
    primaryLight: '#455a64',
    primaryGradient: 'linear-gradient(135deg, #607d8b 0%, #455a64 100%)'
  }
]

// 特殊效果主题
export const specialThemes: ThemeColors[] = [
  {
    name: 'neon',
    label: '霓虹',
    primary: '#00ffff',
    primaryRgb: '0, 255, 255',
    primaryLight: '#00e5ff',
    primaryGradient: 'linear-gradient(135deg, #00ffff 0%, #00e5ff 100%)'
  },
  {
    name: 'galaxy',
    label: '星河',
    primary: '#5c6bc0',
    primaryRgb: '92, 107, 192',
    primaryLight: '#3f51b5',
    primaryGradient: 'linear-gradient(135deg, #5c6bc0 0%, #3f51b5 100%)'
  },
  {
    name: 'aurora',
    label: '极光',
    primary: '#26c6da',
    primaryRgb: '38, 198, 218',
    primaryLight: '#00acc1',
    primaryGradient: 'linear-gradient(135deg, #26c6da 0%, #00acc1 100%)'
  }
]

// 获取所有主题
export const getAllThemes = (): ThemeColors[] => [
  ...themePresets,
  ...seasonalThemes,
  ...specialThemes
]

// 根据名称获取主题
export const getThemeByName = (name: string): ThemeColors | undefined => {
  return getAllThemes().find(theme => theme.name === name)
}

// 应用主题颜色
export const applyThemeColors = (theme: ThemeColors): void => {
  const root = document.documentElement
  
  root.style.setProperty('--primary-color', theme.primary)
  root.style.setProperty('--primary-color-rgb', theme.primaryRgb)
  root.style.setProperty('--primary-light', theme.primaryLight)
  root.style.setProperty('--primary-gradient', theme.primaryGradient)
  
  if (theme.secondary) {
    root.style.setProperty('--secondary-color', theme.secondary)
  }
  
  if (theme.accent) {
    root.style.setProperty('--accent-color', theme.accent)
  }
  
  // 更新相关的hover和active状态颜色
  const [r, g, b] = theme.primaryRgb.split(', ').map(Number)
  root.style.setProperty('--hover-bg', `rgba(${r}, ${g}, ${b}, 0.08)`)
  root.style.setProperty('--bg-active', `rgba(${r}, ${g}, ${b}, 0.12)`)
  root.style.setProperty('--sidebar-hover-bg', `rgba(${r}, ${g}, ${b}, 0.08)`)
  root.style.setProperty('--sidebar-active-bg', `rgba(${r}, ${g}, ${b}, 0.12)`)
  root.style.setProperty('--tags-item-hover-bg', `rgba(${r}, ${g}, ${b}, 0.08)`)
}
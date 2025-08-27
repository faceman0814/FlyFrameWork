<template>
  <div class="theme-customizer">
    <el-drawer
      v-model="drawerVisible"
      title="主题定制"
      direction="rtl"
      size="400px"
      class="theme-drawer"
    >
      <div class="theme-content">
        <!-- 亮暗模式切换 -->
        <div class="theme-section">
          <h3>{{ $t('theme.mode') || '主题模式' }}</h3>
          <div class="theme-mode-grid">
            <div 
              v-for="mode in themeModes" 
              :key="mode.value"
              :class="['theme-mode-item', { active: currentMode === mode.value }]"
              @click="switchThemeMode(mode.value)"
            >
              <div class="mode-preview" :class="mode.value">
                <div class="mode-bg"></div>
                <div class="mode-sidebar"></div>
                <div class="mode-content"></div>
              </div>
              <span class="mode-label">{{ mode.label }}</span>
            </div>
          </div>
        </div>

        <!-- 主题配色 -->
        <div class="theme-section">
          <h3>主题配色</h3>
          <div class="color-presets">
            <div 
              v-for="theme in themePresets" 
              :key="theme.name"
              :class="['color-preset', { active: currentTheme === theme.name }]"
              @click="applyTheme(theme)"
              :title="theme.label"
            >
              <div 
                class="color-circle" 
                :style="{ background: theme.primaryGradient }"
              ></div>
              <span class="preset-label">{{ theme.label }}</span>
            </div>
          </div>
        </div>

        <!-- 季节性主题 -->
        <div class="theme-section">
          <h3>季节主题</h3>
          <div class="seasonal-themes">
            <div 
              v-for="theme in seasonalThemes" 
              :key="theme.name"
              :class="['seasonal-theme', { active: currentTheme === theme.name }]"
              @click="applyTheme(theme)"
            >
              <div 
                class="seasonal-preview" 
                :style="{ background: theme.primaryGradient }"
              >
                <el-icon class="seasonal-icon">
                  <component :is="getSeasonIcon(theme.name)" />
                </el-icon>
              </div>
              <span class="seasonal-label">{{ theme.label }}</span>
            </div>
          </div>
        </div>

        <!-- 特殊效果主题 -->
        <div class="theme-section">
          <h3>特殊效果</h3>
          <div class="special-themes">
            <div 
              v-for="theme in specialThemes" 
              :key="theme.name"
              :class="['special-theme', { active: currentTheme === theme.name }]"
              @click="applyTheme(theme)"
            >
              <div 
                class="special-preview" 
                :style="{ background: theme.primaryGradient }"
              >
                <div class="special-effect" :class="theme.name"></div>
              </div>
              <span class="special-label">{{ theme.label }}</span>
            </div>
          </div>
        </div>

        <!-- 自定义颜色 -->
        <div class="theme-section">
          <h3>自定义颜色</h3>
          <div class="custom-color-picker">
            <el-color-picker
              v-model="customPrimaryColor"
              :predefine="predefineColors"
              @change="applyCustomColor"
            />
            <span class="custom-color-label">主色调</span>
          </div>
        </div>

        <!-- 主题设置 -->
        <div class="theme-section">
          <h3>布局设置</h3>
          <div class="layout-settings">
            <el-form label-position="left" label-width="80px">
              <el-form-item label="圆角大小">
                <el-slider
                  v-model="borderRadius"
                  :min="0"
                  :max="20"
                  @change="applyBorderRadius"
                />
              </el-form-item>
              <el-form-item label="阴影强度">
                <el-slider
                  v-model="shadowIntensity"
                  :min="0"
                  :max="100"
                  @change="applyShadowIntensity"
                />
              </el-form-item>
              <el-form-item label="透明度">
                <el-slider
                  v-model="backgroundOpacity"
                  :min="80"
                  :max="100"
                  @change="applyBackgroundOpacity"
                />
              </el-form-item>
            </el-form>
          </div>
        </div>

        <!-- 重置按钮 -->
        <div class="theme-actions">
          <el-button @click="resetTheme" type="info">重置默认</el-button>
          <el-button @click="exportTheme" type="primary">导出主题</el-button>
          <el-button @click="importTheme" type="success">导入主题</el-button>
        </div>
      </div>
    </el-drawer>

    <!-- 主题定制按钮 -->
    <div class="theme-customizer-trigger" @click="drawerVisible = true">
      <el-tooltip content="主题定制" placement="left">
        <el-button type="primary" :icon="Setting" circle />
      </el-tooltip>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Setting } from '@element-plus/icons-vue'
import { useThemeStore } from '@/stores/theme'
import { 
  themePresets, 
  seasonalThemes, 
  specialThemes,
  type ThemeColors,
  applyThemeColors 
} from '@/utils/theme-colors'
import { ElMessage } from 'element-plus'

const themeStore = useThemeStore()
const drawerVisible = ref(false)

// 主题模式
const themeModes = ref([
  { value: 'light', label: '浅色' },
  { value: 'dark', label: '深色' },
  { value: 'auto', label: '跟随系统' }
])

const currentMode = computed(() => themeStore.mode)
const currentTheme = ref('default')

// 自定义颜色
const customPrimaryColor = ref('#667eea')
const predefineColors = ref([
  '#ff4500', '#ff8c00', '#ffd700', '#90ee90', '#00ced1', '#1e90ff',
  '#c71585', '#ff1493', '#00bfff', '#00ffff', '#00ff00', '#ffff00',
  '#ff00ff', '#ff0000', '#00ff7f', '#dc143c', '#4169e1', '#8a2be2'
])

// 布局设置
const borderRadius = ref(8)
const shadowIntensity = ref(50)
const backgroundOpacity = ref(100)

// 切换主题模式
const switchThemeMode = (mode: string) => {
  themeStore.setMode(mode)
}

// 应用主题
const applyTheme = (theme: ThemeColors) => {
  currentTheme.value = theme.name
  applyThemeColors(theme)
  localStorage.setItem('currentTheme', theme.name)
  ElMessage.success(`已切换到${theme.label}主题`)
}

// 应用自定义颜色
const applyCustomColor = (color: string) => {
  const customTheme: ThemeColors = {
    name: 'custom',
    label: '自定义',
    primary: color,
    primaryRgb: hexToRgb(color),
    primaryLight: adjustBrightness(color, -20),
    primaryGradient: `linear-gradient(135deg, ${color} 0%, ${adjustBrightness(color, -20)} 100%)`
  }
  applyTheme(customTheme)
}

// 应用圆角大小
const applyBorderRadius = (value: number) => {
  document.documentElement.style.setProperty('--border-radius', `${value}px`)
  localStorage.setItem('borderRadius', value.toString())
}

// 应用阴影强度
const applyShadowIntensity = (value: number) => {
  const intensity = value / 100
  document.documentElement.style.setProperty('--shadow-opacity', intensity.toString())
  localStorage.setItem('shadowIntensity', value.toString())
}

// 应用背景透明度
const applyBackgroundOpacity = (value: number) => {
  const opacity = value / 100
  document.documentElement.style.setProperty('--bg-opacity', opacity.toString())
  localStorage.setItem('backgroundOpacity', value.toString())
}

// 获取季节图标
const getSeasonIcon = (seasonName: string) => {
  const iconMap: Record<string, string> = {
    spring: 'Sunny',
    summer: 'Sunrise',
    autumn: 'Orange',
    winter: 'Snowflake'
  }
  return iconMap[seasonName] || 'Sunny'
}

// 工具函数：hex转rgb
const hexToRgb = (hex: string): string => {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
  if (result) {
    const r = parseInt(result[1], 16)
    const g = parseInt(result[2], 16)
    const b = parseInt(result[3], 16)
    return `${r}, ${g}, ${b}`
  }
  return '102, 126, 234'
}

// 工具函数：调整颜色亮度
const adjustBrightness = (hex: string, percent: number): string => {
  const num = parseInt(hex.replace('#', ''), 16)
  const amt = Math.round(2.55 * percent)
  const R = (num >> 16) + amt
  const G = (num >> 8 & 0x00FF) + amt
  const B = (num & 0x0000FF) + amt
  return `#${(0x1000000 + (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000
    + (G < 255 ? G < 1 ? 0 : G : 255) * 0x100
    + (B < 255 ? B < 1 ? 0 : B : 255)).toString(16).slice(1)}`
}

// 重置主题
const resetTheme = () => {
  const defaultTheme = themePresets[0]
  applyTheme(defaultTheme)
  borderRadius.value = 8
  shadowIntensity.value = 50
  backgroundOpacity.value = 100
  applyBorderRadius(8)
  applyShadowIntensity(50)
  applyBackgroundOpacity(100)
  localStorage.removeItem('currentTheme')
  localStorage.removeItem('borderRadius')
  localStorage.removeItem('shadowIntensity')
  localStorage.removeItem('backgroundOpacity')
  ElMessage.success('主题已重置为默认设置')
}

// 导出主题
const exportTheme = () => {
  const themeConfig = {
    mode: currentMode.value,
    theme: currentTheme.value,
    borderRadius: borderRadius.value,
    shadowIntensity: shadowIntensity.value,
    backgroundOpacity: backgroundOpacity.value,
    customColor: customPrimaryColor.value
  }
  
  const dataStr = JSON.stringify(themeConfig, null, 2)
  const dataBlob = new Blob([dataStr], { type: 'application/json' })
  const url = URL.createObjectURL(dataBlob)
  
  const link = document.createElement('a')
  link.href = url
  link.download = 'theme-config.json'
  link.click()
  
  URL.revokeObjectURL(url)
  ElMessage.success('主题配置已导出')
}

// 导入主题
const importTheme = () => {
  const input = document.createElement('input')
  input.type = 'file'
  input.accept = '.json'
  input.onchange = (event) => {
    const file = (event.target as HTMLInputElement).files?.[0]
    if (file) {
      const reader = new FileReader()
      reader.onload = (e) => {
        try {
          const config = JSON.parse(e.target?.result as string)
          
          // 应用配置
          if (config.mode) themeStore.setMode(config.mode)
          if (config.theme && config.theme !== 'custom') {
            const theme = [...themePresets, ...seasonalThemes, ...specialThemes]
              .find(t => t.name === config.theme)
            if (theme) applyTheme(theme)
          }
          if (config.customColor) {
            customPrimaryColor.value = config.customColor
            applyCustomColor(config.customColor)
          }
          if (config.borderRadius) {
            borderRadius.value = config.borderRadius
            applyBorderRadius(config.borderRadius)
          }
          if (config.shadowIntensity) {
            shadowIntensity.value = config.shadowIntensity
            applyShadowIntensity(config.shadowIntensity)
          }
          if (config.backgroundOpacity) {
            backgroundOpacity.value = config.backgroundOpacity
            applyBackgroundOpacity(config.backgroundOpacity)
          }
          
          ElMessage.success('主题配置已导入')
        } catch (error) {
          ElMessage.error('主题配置文件格式错误')
        }
      }
      reader.readAsText(file)
    }
  }
  input.click()
}

// 初始化
onMounted(() => {
  // 恢复保存的主题设置
  const savedTheme = localStorage.getItem('currentTheme')
  if (savedTheme) {
    currentTheme.value = savedTheme
  }
  
  const savedBorderRadius = localStorage.getItem('borderRadius')
  if (savedBorderRadius) {
    borderRadius.value = parseInt(savedBorderRadius)
    applyBorderRadius(borderRadius.value)
  }
  
  const savedShadowIntensity = localStorage.getItem('shadowIntensity')
  if (savedShadowIntensity) {
    shadowIntensity.value = parseInt(savedShadowIntensity)
    applyShadowIntensity(shadowIntensity.value)
  }
  
  const savedBackgroundOpacity = localStorage.getItem('backgroundOpacity')
  if (savedBackgroundOpacity) {
    backgroundOpacity.value = parseInt(savedBackgroundOpacity)
    applyBackgroundOpacity(backgroundOpacity.value)
  }
})
</script>

<style lang="scss" scoped>
.theme-customizer-trigger {
  position: fixed;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  z-index: 1000;
}

:deep(.theme-drawer) {
  .el-drawer__body {
    padding: 0;
  }
}

.theme-content {
  padding: 20px;
  height: 100%;
  overflow-y: auto;
}

.theme-section {
  margin-bottom: 30px;
  
  h3 {
    margin-bottom: 15px;
    font-size: 16px;
    font-weight: 600;
    color: var(--text-primary);
  }
}

.theme-mode-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
}

.theme-mode-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px;
  border: 2px solid var(--border-light);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  
  &:hover, &.active {
    border-color: var(--primary-color);
    transform: translateY(-2px);
  }
  
  .mode-preview {
    width: 40px;
    height: 30px;
    border-radius: 4px;
    position: relative;
    overflow: hidden;
    margin-bottom: 8px;
    
    &.light {
      background: #f5f7fa;
      
      .mode-sidebar {
        background: #ffffff;
      }
      
      .mode-content {
        background: #ffffff;
      }
    }
    
    &.dark {
      background: #1e2028;
      
      .mode-sidebar {
        background: #262a36;
      }
      
      .mode-content {
        background: #262a36;
      }
    }
    
    &.auto {
      background: linear-gradient(90deg, #f5f7fa 50%, #1e2028 50%);
    }
    
    .mode-bg {
      position: absolute;
      inset: 0;
    }
    
    .mode-sidebar {
      position: absolute;
      left: 0;
      top: 0;
      width: 12px;
      height: 100%;
    }
    
    .mode-content {
      position: absolute;
      right: 0;
      top: 0;
      width: 26px;
      height: 100%;
    }
  }
  
  .mode-label {
    font-size: 12px;
    color: var(--text-secondary);
  }
}

.color-presets {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
}

.color-preset {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px;
  border: 2px solid var(--border-light);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  
  &:hover, &.active {
    border-color: var(--primary-color);
    transform: translateY(-2px);
  }
  
  .color-circle {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    margin-bottom: 8px;
    border: 2px solid #fff;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  }
  
  .preset-label {
    font-size: 12px;
    color: var(--text-secondary);
    text-align: center;
  }
}

.seasonal-themes, .special-themes {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.seasonal-theme, .special-theme {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px;
  border: 2px solid var(--border-light);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  
  &:hover, &.active {
    border-color: var(--primary-color);
    transform: translateY(-2px);
  }
}

.seasonal-preview, .special-preview {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
  position: relative;
  overflow: hidden;
}

.seasonal-icon {
  font-size: 20px;
  color: white;
}

.special-effect {
  position: absolute;
  inset: 0;
  
  &.neon {
    animation: neonGlow 2s ease-in-out infinite alternate;
  }
  
  &.galaxy {
    background: radial-gradient(circle, rgba(255,255,255,0.8) 1px, transparent 1px);
    background-size: 8px 8px;
    animation: twinkle 3s ease-in-out infinite;
  }
  
  &.aurora {
    background: linear-gradient(45deg, 
      transparent 30%, 
      rgba(255, 255, 255, 0.5) 50%, 
      transparent 70%);
    animation: wave 2s ease-in-out infinite;
  }
}

@keyframes neonGlow {
  from { box-shadow: inset 0 0 20px rgba(255, 255, 255, 0.5); }
  to { box-shadow: inset 0 0 20px rgba(255, 255, 255, 0.9); }
}

@keyframes twinkle {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.3; }
}

@keyframes wave {
  0%, 100% { transform: translateX(-100%); }
  50% { transform: translateX(100%); }
}

.custom-color-picker {
  display: flex;
  align-items: center;
  gap: 12px;
  
  .custom-color-label {
    font-size: 14px;
    color: var(--text-secondary);
  }
}

.layout-settings {
  .el-form-item {
    margin-bottom: 20px;
  }
}

.theme-actions {
  display: flex;
  gap: 12px;
  padding-top: 20px;
  border-top: 1px solid var(--border-light);
}
</style>
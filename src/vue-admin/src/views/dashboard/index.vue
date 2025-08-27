<template>
  <div class="dashboard-container">
    <div class="dashboard-header">
      <h1 class="page-title gradient-text">
        {{ $t('dashboard.welcome') }}
      </h1>
      <p class="page-subtitle">
        {{ currentTime }} | {{ $t('dashboard.welcome') }}
      </p>
    </div>
    
    <!-- 统计卡片 -->
    <div class="stats-grid">
      <div class="stat-card modern-card" v-for="stat in stats" :key="stat.title">
        <div class="stat-icon" :style="{ background: stat.color }">
          <el-icon :size="24">
            <component :is="stat.icon" />
          </el-icon>
        </div>
        <div class="stat-content">
          <h3 class="stat-value">{{ stat.value }}</h3>
          <p class="stat-title">{{ stat.title }}</p>
          <span class="stat-trend" :class="stat.trend > 0 ? 'positive' : 'negative'">
            {{ stat.trend > 0 ? '+' : '' }}{{ stat.trend }}%
          </span>
        </div>
      </div>
    </div>

    <!-- 快捷操作和主题展示 -->
    <div class="feature-showcase">
      <div class="feature-card modern-card">
        <h2>🚀 {{ $t('dashboard.quickActions.title') }}</h2>
        <div class="quick-actions-grid">
          <div 
            class="quick-action" 
            v-for="action in quickActions" 
            :key="action.title"
            @click="action.onClick"
          >
            <div class="action-icon" :style="{ background: action.color }">
              <el-icon :size="20">
                <component :is="action.icon" />
              </el-icon>
            </div>
            <span>{{ action.title }}</span>
          </div>
        </div>
      </div>
      
      <div class="feature-card glass-effect">
        <h2>🎨 {{ $t('dashboard.features.title') }}</h2>
        <ul class="feature-list">
          <li>💡 {{ $t('dashboard.features.smartTheme') }}</li>
          <li>🌈 {{ $t('dashboard.features.consistentDesign') }}</li>
          <li>🎪 {{ $t('dashboard.features.glassEffect') }}</li>
          <li>📱 {{ $t('dashboard.features.responsive') }}</li>
          <li>⚡ {{ $t('dashboard.features.smoothTransition') }}</li>
          <li>🔧 {{ $t('dashboard.features.extensibleTheme') }}</li>
        </ul>
      </div>
    </div>
    
    <div class="usage-guide modern-card">
      <h2>📖 {{ $t('dashboard.guide.title') }}</h2>
      <div class="guide-steps">
        <div class="step">
          <div class="step-number">1</div>
          <div class="step-content">
            <h4>{{ $t('dashboard.guide.step1.title') }}</h4>
            <p>{{ $t('dashboard.guide.step1.description') }}</p>
          </div>
        </div>
        <div class="step">
          <div class="step-number">2</div>
          <div class="step-content">
            <h4>{{ $t('dashboard.guide.step2.title') }}</h4>
            <p>{{ $t('dashboard.guide.step2.description') }}</p>
          </div>
        </div>
        <div class="step">
          <div class="step-number">3</div>
          <div class="step-content">
            <h4>{{ $t('dashboard.guide.step3.title') }}</h4>
            <p>{{ $t('dashboard.guide.step3.description') }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { 
  User, 
  UserFilled, 
  Tools, 
  View, 
  Setting, 
  Monitor,
  DataAnalysis,
  Document
} from '@element-plus/icons-vue'

const router = useRouter()
const { t } = useI18n()
const currentTime = ref('')

const stats = ref([
  {
    title: computed(() => t('dashboard.stats.totalUsers')),
    value: '1,024',
    trend: 12.5,
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    icon: User
  },
  {
    title: computed(() => t('dashboard.stats.totalRoles')),
    value: '12',
    trend: 8.2,
    color: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    icon: UserFilled
  },
  {
    title: computed(() => t('dashboard.stats.onlineUsers')),
    value: '89',
    trend: -2.1,
    color: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    icon: Tools
  },
  {
    title: computed(() => t('dashboard.stats.todayVisits')),
    value: '5,678',
    trend: 15.8,
    color: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
    icon: View
  }
])

const quickActions = ref([
  {
    title: computed(() => t('dashboard.quickActions.userManagement')),
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    icon: User,
    onClick: () => router.push('/system/user')
  },
  {
    title: computed(() => t('dashboard.quickActions.roleManagement')),
    color: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    icon: UserFilled,
    onClick: () => router.push('/system/role')
  },
  {
    title: computed(() => t('dashboard.quickActions.dataAnalysis')),
    color: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    icon: DataAnalysis,
    onClick: () => console.log('数据分析')
  },
  {
    title: computed(() => t('dashboard.quickActions.documentCenter')),
    color: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
    icon: Document,
    onClick: () => console.log('文档中心')
  },
  {
    title: computed(() => t('dashboard.quickActions.systemSettings')),
    color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    icon: Setting,
    onClick: () => console.log('系统设置')
  },
  {
    title: computed(() => t('dashboard.quickActions.systemMonitor')),
    color: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    icon: Monitor,
    onClick: () => console.log('系统监控')
  }
])

const updateTime = () => {
  const now = new Date()
  currentTime.value = now.toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    weekday: 'long',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

let timer: NodeJS.Timeout

onMounted(() => {
  updateTime()
  timer = setInterval(updateTime, 1000)
})

onUnmounted(() => {
  if (timer) {
    clearInterval(timer)
  }
})
</script>

<style lang="scss" scoped>
.dashboard-container {
  padding: 0;
}

.dashboard-header {
  text-align: center;
  margin-bottom: 40px;
  
  .page-title {
    font-size: 36px;
    font-weight: 700;
    margin: 0 0 16px 0;
    
    @media (max-width: 768px) {
      font-size: 28px;
    }
  }
  
  .page-subtitle {
    font-size: 16px;
    color: var(--text-secondary);
    margin: 0;
    line-height: 1.6;
    font-weight: 500;
    
    // 深色模式下增强可读性
    [data-theme="dark"] & {
      color: var(--text-regular);
      text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
    }
  }
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
  margin-bottom: 40px;
}

.stat-card {
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 20px;
  
  // 深色模式下增强卡片对比度
  [data-theme="dark"] & {
    background: var(--bg-card);
    border: 1px solid var(--border-base);
  }
  
  .stat-icon {
    width: 60px;
    height: 60px;
    border-radius: 16px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    flex-shrink: 0;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }
  
  .stat-content {
    flex: 1;
    
    .stat-value {
      font-size: 28px;
      font-weight: 700;
      color: var(--text-primary);
      margin: 0 0 8px 0;
    }
    
    .stat-title {
      font-size: 14px;
      color: var(--text-secondary);
      margin: 0 0 8px 0;
      font-weight: 500;
      
      // 深色模式下增强可读性
      [data-theme="dark"] & {
        color: var(--text-regular);
      }
    }
    
    .stat-trend {
      font-size: 12px;
      font-weight: 600;
      padding: 4px 8px;
      border-radius: 6px;
      
      &.positive {
        color: var(--success-color);
        background: rgba(103, 194, 58, 0.1);
      }
      
      &.negative {
        color: var(--danger-color);
        background: rgba(245, 108, 108, 0.1);
      }
    }
  }
}

.feature-showcase {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 32px;
  margin-bottom: 40px;
  
  @media (max-width: 768px) {
    grid-template-columns: 1fr;
    gap: 24px;
  }
}

.feature-card {
  padding: 32px;
  
  // 针对毛玻璃效果卡片的深色模式优化
  &.glass-effect {
    [data-theme="dark"] & {
      background: rgba(38, 42, 54, 0.3);
      border: 1px solid rgba(240, 242, 245, 0.1);
      backdrop-filter: blur(20px);
    }
  }
  
  h2 {
    font-size: 24px;
    font-weight: 700;
    color: var(--text-primary);
    margin: 0 0 24px 0;
    
    // 深色模式下增强标题可读性
    [data-theme="dark"] & {
      color: var(--text-primary);
      text-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
    }
    
    @media (max-width: 768px) {
      font-size: 20px;
    }
  }
  
  .quick-actions-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
    gap: 16px;
    
    .quick-action {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 20px;
      border-radius: 12px;
      background: var(--bg-hover);
      transition: all 0.3s ease;
      cursor: pointer;
      
      &:hover {
        transform: translateY(-4px);
        box-shadow: var(--shadow-dark);
        background: var(--bg-active);
      }
      
      .action-icon {
        width: 48px;
        height: 48px;
        border-radius: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: white;
      }
      
      span {
        font-size: 14px;
        font-weight: 500;
        color: var(--text-primary);
        text-align: center;
      }
    }
  }
  
  .feature-list {
    list-style: none;
    padding: 0;
    margin: 0;
    
    li {
      padding: 12px 0;
      border-bottom: 1px solid var(--border-light);
      color: var(--text-regular);
      font-size: 15px;
      line-height: 1.5;
      font-weight: 500;
      
      // 深色模式下增强对比度
      [data-theme="dark"] & {
        color: var(--text-primary);
        border-bottom-color: var(--border-base);
      }
      
      &:last-child {
        border-bottom: none;
      }
      
      // 添加emoji的样式优化
      &::first-letter {
        margin-right: 8px;
      }
    }
  }
}

.usage-guide {
  padding: 32px;
  
  h2 {
    font-size: 24px;
    font-weight: 700;
    color: var(--text-primary);
    margin: 0 0 24px 0;
    
    @media (max-width: 768px) {
      font-size: 20px;
    }
  }
  
  .guide-steps {
    .step {
      display: flex;
      gap: 20px;
      margin-bottom: 24px;
      
      &:last-child {
        margin-bottom: 0;
      }
      
      .step-number {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        background: var(--primary-gradient);
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: 700;
        flex-shrink: 0;
        font-size: 16px;
      }
      
      .step-content {
        flex: 1;
        
        h4 {
          font-size: 16px;
          font-weight: 600;
          color: var(--text-primary);
          margin: 0 0 8px 0;
        }
        
        p {
          font-size: 14px;
          color: var(--text-regular);
          line-height: 1.6;
          margin: 0;
        }
      }
    }
  }
}

// 响应式调整
@media (max-width: 768px) {
  .dashboard-container {
    padding: 0;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .stat-card {
    padding: 20px;
    
    .stat-icon {
      width: 50px;
      height: 50px;
    }
    
    .stat-content {
      .stat-value {
        font-size: 24px;
      }
    }
  }
  
  .feature-card,
  .usage-guide {
    padding: 24px;
    
    .guide-steps .step {
      gap: 16px;
      
      .step-number {
        width: 32px;
        height: 32px;
        font-size: 14px;
      }
    }
    
    .quick-actions-grid {
      grid-template-columns: repeat(2, 1fr);
      gap: 12px;
      
      .quick-action {
        padding: 16px;
        
        .action-icon {
          width: 40px;
          height: 40px;
        }
        
        span {
          font-size: 13px;
        }
      }
    }
  }
}
</style>
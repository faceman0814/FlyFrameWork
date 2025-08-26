<template>
  <div class="login-flow-demo">
    <el-card shadow="hover" class="demo-card">
      <template #header>
        <div class="card-header">
          <h3>NSwag 登录流程演示</h3>
        </div>
      </template>

      <div class="demo-content">
        <!-- 登录状态显示 -->
        <div class="status-section">
          <h4>当前登录状态</h4>
          <el-row :gutter="20">
            <el-col :span="6">
              <el-tag :type="isLoggedIn ? 'success' : 'danger'">
                {{ isLoggedIn ? '已登录' : '未登录' }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              <span>Token: {{ token ? '已设置' : '未设置' }}</span>
            </el-col>
            <el-col :span="6">
              <span>用户ID: {{ userId || '无' }}</span>
            </el-col>
            <el-col :span="6">
              <span>用户名: {{ name || '无' }}</span>
            </el-col>
          </el-row>
        </div>

        <!-- 用户信息显示 -->
        <div class="info-section" v-if="isLoggedIn">
          <h4>用户信息</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="用户ID">{{ userId }}</el-descriptions-item>
            <el-descriptions-item label="用户名">{{ name }}</el-descriptions-item>
            <el-descriptions-item label="角色">{{ roles.join(', ') }}</el-descriptions-item>
            <el-descriptions-item label="权限数">{{ permissions.length }}</el-descriptions-item>
          </el-descriptions>
        </div>

        <!-- 操作按钮 -->
        <div class="action-section">
          <h4>登录流程测试</h4>
          <el-row :gutter="20">
            <el-col :span="8">
              <el-button 
                type="primary" 
                :loading="loginLoading" 
                @click="testLogin"
                :disabled="isLoggedIn"
              >
                测试登录
              </el-button>
            </el-col>
            <el-col :span="8">
              <el-button 
                type="info" 
                :loading="infoLoading" 
                @click="testGetUserInfo"
                :disabled="!isLoggedIn"
              >
                获取用户信息
              </el-button>
            </el-col>
            <el-col :span="8">
              <el-button 
                type="warning" 
                :loading="logoutLoading" 
                @click="testLogout"
                :disabled="!isLoggedIn"
              >
                退出登录
              </el-button>
            </el-col>
          </el-row>
        </div>

        <!-- 日志显示 -->
        <div class="log-section">
          <h4>操作日志</h4>
          <el-scrollbar height="200px">
            <div class="log-content">
              <div v-for="(log, index) in logs" :key="index" class="log-item">
                <el-tag size="small" :type="log.type">{{ log.timestamp }}</el-tag>
                <span class="log-message">{{ log.message }}</span>
              </div>
            </div>
          </el-scrollbar>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import { storeToRefs } from 'pinia'

const userStore = useUserStore()
const { token, name, userId, roles, permissions, isLoggedIn } = storeToRefs(userStore)

const loginLoading = ref(false)
const infoLoading = ref(false)
const logoutLoading = ref(false)

interface LogItem {
  timestamp: string
  type: 'success' | 'warning' | 'danger' | 'info'
  message: string
}

const logs = ref<LogItem[]>([])

const addLog = (message: string, type: LogItem['type'] = 'info') => {
  logs.value.unshift({
    timestamp: new Date().toLocaleTimeString(),
    type,
    message
  })
  
  // 保持最多50条日志
  if (logs.value.length > 50) {
    logs.value = logs.value.slice(0, 50)
  }
}

// 测试登录
const testLogin = async () => {
  loginLoading.value = true
  try {
    addLog('开始测试登录...', 'info')
    
    await userStore.login({
      username: 'admin',
      password: 'admin123'
    })
    
    addLog('登录成功！用户信息已更新', 'success')
    ElMessage.success('登录测试成功')
    
  } catch (error: any) {
    addLog(`登录失败: ${error.message}`, 'danger')
    ElMessage.error('登录测试失败')
    console.error('Login test failed:', error)
  } finally {
    loginLoading.value = false
  }
}

// 测试获取用户信息
const testGetUserInfo = async () => {
  infoLoading.value = true
  try {
    addLog('开始获取用户信息...', 'info')
    
    const userInfo = await userStore.getUserInfo()
    
    addLog(`用户信息获取成功: ${userInfo.name || userInfo.username}`, 'success')
    ElMessage.success('用户信息获取成功')
    
  } catch (error: any) {
    addLog(`获取用户信息失败: ${error.message}`, 'danger')
    ElMessage.error('获取用户信息失败')
    console.error('Get user info failed:', error)
  } finally {
    infoLoading.value = false
  }
}

// 测试退出登录
const testLogout = async () => {
  logoutLoading.value = true
  try {
    addLog('开始退出登录...', 'info')
    
    await userStore.logout()
    
    addLog('退出登录成功', 'success')
    ElMessage.success('退出登录成功')
    
  } catch (error: any) {
    addLog(`退出登录失败: ${error.message}`, 'danger')
    ElMessage.error('退出登录失败')
    console.error('Logout failed:', error)
  } finally {
    logoutLoading.value = false
  }
}

onMounted(() => {
  addLog('登录流程演示组件已加载', 'info')
  
  // 检查当前登录状态
  if (isLoggedIn.value) {
    addLog(`当前已登录，用户: ${name.value}`, 'success')
  } else {
    addLog('当前未登录', 'warning')
  }
})
</script>

<style lang="scss" scoped>
.login-flow-demo {
  padding: 20px;
}

.demo-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  
  h3 {
    margin: 0;
    color: #409eff;
  }
}

.demo-content {
  .status-section,
  .info-section,
  .action-section,
  .log-section {
    margin-bottom: 24px;
    
    h4 {
      margin-bottom: 12px;
      color: #303133;
      font-size: 16px;
    }
  }
}

.log-content {
  .log-item {
    display: flex;
    align-items: center;
    padding: 4px 8px;
    border-bottom: 1px solid #f0f0f0;
    
    &:hover {
      background-color: #f9f9f9;
    }
    
    .log-message {
      margin-left: 8px;
      font-size: 14px;
      color: #606266;
    }
  }
}
</style>
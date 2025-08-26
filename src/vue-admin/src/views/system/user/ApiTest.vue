<template>
  <div class="api-test">
    <el-card shadow="hover">
      <template #header>
        <h3>🔧 API 连接诊断工具</h3>
      </template>
      
      <div class="test-section">
        <h4>环境配置检查</h4>
        <el-descriptions :column="2" border>
          <el-descriptions-item label="API 基础地址">{{ apiBaseUrl }}</el-descriptions-item>
          <el-descriptions-item label="当前环境">{{ currentEnv }}</el-descriptions-item>
          <el-descriptions-item label="开发服务器端口">{{ devServerPort }}</el-descriptions-item>
          <el-descriptions-item label="代理目标">{{ proxyTarget }}</el-descriptions-item>
        </el-descriptions>
      </div>

      <div class="test-section">
        <h4>网络连接测试</h4>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-button 
              type="primary" 
              :loading="testing.swagger" 
              @click="testSwagger"
            >
              测试 Swagger 接口
            </el-button>
          </el-col>
          <el-col :span="8">
            <el-button 
              type="info" 
              :loading="testing.login" 
              @click="testLogin"
            >
              测试登录接口
            </el-button>
          </el-col>
          <el-col :span="8">
            <el-button 
              type="success" 
              :loading="testing.apiClient" 
              @click="testApiClient"
            >
              测试 NSwag 客户端
            </el-button>
          </el-col>
        </el-row>
      </div>

      <div class="test-section">
        <h4>测试结果</h4>
        <el-scrollbar height="300px">
          <div class="log-content">
            <div v-for="(log, index) in logs" :key="index" class="log-item">
              <el-tag size="small" :type="log.type">{{ log.timestamp }}</el-tag>
              <span class="log-message">{{ log.message }}</span>
              <pre v-if="log.data" class="log-data">{{ JSON.stringify(log.data, null, 2) }}</pre>
            </div>
          </div>
        </el-scrollbar>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { apiService } from '@/api/service'

const apiBaseUrl = ref('')
const currentEnv = ref('')
const devServerPort = ref('')
const proxyTarget = ref('')

const testing = ref({
  swagger: false,
  login: false,
  apiClient: false
})

interface LogItem {
  timestamp: string
  type: 'success' | 'warning' | 'danger' | 'info'
  message: string
  data?: any
}

const logs = ref<LogItem[]>([])

const addLog = (message: string, type: LogItem['type'] = 'info', data?: any) => {
  logs.value.unshift({
    timestamp: new Date().toLocaleTimeString(),
    type,
    message,
    data
  })
  
  if (logs.value.length > 20) {
    logs.value = logs.value.slice(0, 20)
  }
}

// 测试 Swagger 接口
const testSwagger = async () => {
  testing.value.swagger = true
  try {
    addLog('开始测试 Swagger 接口...', 'info')
    
    const swaggerUrl = 'http://localhost:6298/swagger/v1/swagger.json'
    const response = await fetch(swaggerUrl, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      }
    })
    
    if (response.ok) {
      const data = await response.json()
      addLog('✅ Swagger 接口测试成功', 'success', { 
        status: response.status, 
        title: data.info?.title,
        version: data.info?.version
      })
      ElMessage.success('Swagger 接口可访问')
    } else {
      throw new Error(`HTTP ${response.status}`)
    }
  } catch (error: any) {
    addLog(`❌ Swagger 接口测试失败: ${error.message}`, 'danger', error)
    ElMessage.error('Swagger 接口不可访问')
  } finally {
    testing.value.swagger = false
  }
}

// 测试登录接口
const testLogin = async () => {
  testing.value.login = true
  try {
    addLog('开始测试登录接口...', 'info')
    
    const loginUrl = 'http://localhost:6298/api/AccountClient/Login'
    const response = await fetch(loginUrl, {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        userName: 'admin',
        password: 'admin123'
      })
    })
    
    if (response.ok) {
      const data = await response.json()
      addLog('✅ 登录接口测试成功', 'success', { 
        status: response.status,
        success: data.success,
        code: data.code,
        hasToken: !!data.data?.accessToken
      })
      ElMessage.success('登录接口可访问')
    } else {
      const errorData = await response.text()
      addLog(`❌ 登录接口测试失败: HTTP ${response.status}`, 'danger', { 
        status: response.status,
        error: errorData
      })
      ElMessage.error('登录接口访问失败')
    }
  } catch (error: any) {
    addLog(`❌ 登录接口测试失败: ${error.message}`, 'danger', error)
    ElMessage.error('登录接口网络错误')
  } finally {
    testing.value.login = false
  }
}

// 测试 NSwag 客户端
const testApiClient = async () => {
  testing.value.apiClient = true
  try {
    addLog('开始测试 NSwag 客户端...', 'info')
    
    // 测试使用 NSwag 客户端登录
    const result = await apiService.login('admin', 'admin123')
    
    addLog('✅ NSwag 客户端测试成功', 'success', {
      success: result.success,
      code: result.code,
      hasData: !!result.data,
      hasToken: !!result.data?.accessToken,
      actualUrl: 'http://localhost:6298/api/AccountClient/Login' // 现在应该是正确的 URL
    })
    ElMessage.success('NSwag 客户端工作正常')
  } catch (error: any) {
    addLog(`❌ NSwag 客户端测试失败: ${error.message}`, 'danger', {
      name: error.constructor.name,
      status: error.status,
      message: error.message
    })
    ElMessage.error('NSwag 客户端调用失败')
  } finally {
    testing.value.apiClient = false
  }
}

onMounted(() => {
  // 获取环境配置信息
  apiBaseUrl.value = import.meta.env.VITE_API_BASE_URL || '未设置'
  currentEnv.value = import.meta.env.MODE || '未知'
  devServerPort.value = '3000'  // 从 vite.config.ts 获取
  proxyTarget.value = 'http://localhost:6298'  // 从 vite.config.ts 获取
  
  addLog('API 诊断工具已加载', 'info', {
    baseUrl: apiBaseUrl.value,
    env: currentEnv.value
  })
})
</script>

<style lang="scss" scoped>
.api-test {
  padding: 20px;
}

.test-section {
  margin-bottom: 24px;
  
  h4 {
    margin-bottom: 12px;
    color: #303133;
    font-size: 16px;
  }
}

.log-content {
  .log-item {
    margin-bottom: 12px;
    padding: 8px;
    border: 1px solid #e4e7ed;
    border-radius: 4px;
    background-color: #fafafa;
    
    .log-message {
      margin-left: 8px;
      font-size: 14px;
      color: #606266;
    }
    
    .log-data {
      margin: 8px 0 0 0;
      padding: 8px;
      background-color: #f5f5f5;
      border-radius: 4px;
      font-size: 12px;
      color: #333;
      overflow-x: auto;
    }
  }
}
</style>
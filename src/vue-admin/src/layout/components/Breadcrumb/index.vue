<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item v-for="(item, index) in levelList" :key="item.path">
        <span
          v-if="item.redirect === 'noRedirect' || index === levelList.length - 1"
          class="no-redirect"
        >{{ $t(item.meta?.title as string) }}</span>
        <a v-else @click.prevent="handleLink(item)">{{ $t(item.meta?.title as string) }}</a>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute, useRouter, RouteLocationMatched } from 'vue-router'

const route = useRoute()
const router = useRouter()

const levelList = ref<RouteLocationMatched[]>([])

const isDashboard = (route: RouteLocationMatched) => {
  const name = route && route.name
  if (!name) {
    return false
  }
  return (name as string).trim().toLocaleLowerCase() === 'Dashboard'.toLocaleLowerCase()
}

const getBreadcrumb = () => {
  let matched = route.matched.filter(item => item.meta && item.meta.title)
  const first = matched[0]

  if (!isDashboard(first)) {
    matched = [{ 
      path: '/dashboard', 
      meta: { title: 'dashboard.title' },
      name: 'Dashboard',
      components: {},
      redirect: undefined
    } as unknown as RouteLocationMatched].concat(matched)
  }

  levelList.value = matched.filter(item => item.meta && item.meta.title && item.meta.breadcrumb !== false)
}

const handleLink = (item: RouteLocationMatched) => {
  const { redirect, path } = item
  if (redirect) {
    router.push(redirect as string)
    return
  }
  router.push(path)
}

watch(route, getBreadcrumb, { immediate: true })
</script>

<style lang="scss" scoped>
.app-breadcrumb.el-breadcrumb {
  display: inline-flex;
  align-items: center;
  font-size: 14px;
  line-height: 20px;
  height: 50px;
  margin-left: 8px;
  color: var(--text-regular);

  :deep(.el-breadcrumb__item) {
    display: flex;
    align-items: center;
    
    .el-breadcrumb__inner {
      color: var(--text-regular) !important;
      font-weight: 400;
      transition: color 0.3s ease;
      
      &:hover {
        color: var(--primary-color) !important;
      }
      
      a {
        color: var(--text-regular) !important;
        text-decoration: none;
        transition: color 0.3s ease;
        
        &:hover {
          color: var(--primary-color) !important;
        }
      }
    }
    
    .el-breadcrumb__separator {
      color: var(--text-placeholder) !important;
      margin: 0 8px;
      font-weight: 500;
    }

    &:last-child {
      .el-breadcrumb__inner {
        color: var(--text-primary) !important;
        font-weight: 500;
        
        &:hover {
          color: var(--text-primary) !important;
        }
      }
    }
  }

  .no-redirect {
    color: var(--text-primary) !important;
    cursor: text;
    font-weight: 500;
  }

  // 响应式处理
  @media (max-width: 768px) {
    font-size: 12px;
    margin-left: 4px;
  }
}

// 面包屑动画
.breadcrumb-enter-active,
.breadcrumb-leave-active {
  transition: all 0.3s;
}

.breadcrumb-enter-from,
.breadcrumb-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

.breadcrumb-move {
  transition: all 0.3s;
}
</style>
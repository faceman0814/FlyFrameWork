<template>
  <div v-if="!item.meta?.hidden">
    <template v-if="hasOneShowingChild(item.children, item) && (!onlyOneChild.children || onlyOneChild.noShowingChildren) && !item.meta?.alwaysShow">
      <app-link v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path)">
        <el-tooltip
          :content="getTitle(onlyOneChild.meta)"
          placement="right"
          :disabled="!isCollapse"
          effect="dark"
        >
          <el-menu-item :index="resolvePath(onlyOneChild.path)" :class="{'submenu-title-noDropdown': !isNest}">
            <el-icon v-if="onlyOneChild.meta.icon" class="menu-icon">
              <component :is="onlyOneChild.meta.icon" />
            </el-icon>
            <template #title>
              <span class="menu-title">{{ getTitle(onlyOneChild.meta) }}</span>
            </template>
          </el-menu-item>
        </el-tooltip>
      </app-link>
    </template>

    <el-sub-menu v-else ref="subMenu" :index="resolvePath(item.path)" popper-append-to-body>
      <template #title>
        <el-tooltip
          :content="getTitle(item.meta)"
          placement="right"
          :disabled="!isCollapse"
          effect="dark"
        >
          <div class="submenu-title-wrapper">
            <el-icon v-if="item.meta?.icon" class="menu-icon">
              <component :is="item.meta.icon" />
            </el-icon>
            <span class="menu-title">{{ getTitle(item.meta) }}</span>
          </div>
        </el-tooltip>
      </template>
      <sidebar-item
        v-for="child in item.children"
        :key="child.path"
        :is-nest="true"
        :item="child"
        :base-path="resolvePath(child.path)"
        class="nest-menu"
      />
    </el-sub-menu>
  </div>
</template>

<script setup lang="ts">
import { ref, inject } from 'vue'
import { RouteRecordRaw } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { isExternal } from '@/utils/validate'
import AppLink from './Link.vue'

interface Props {
  item: RouteRecordRaw
  isNest?: boolean
  basePath?: string
}

const props = withDefaults(defineProps<Props>(), {
  isNest: false,
  basePath: ''
})

// 获取侧边栏收缩状态
const isCollapse = inject('sidebarCollapse', false)
const { t } = useI18n()

const onlyOneChild = ref<RouteRecordRaw & { noShowingChildren?: boolean }>({} as RouteRecordRaw)

const hasOneShowingChild = (children: RouteRecordRaw[] = [], parent: RouteRecordRaw) => {
  const showingChildren = children.filter((item) => {
    if (item.meta?.hidden) {
      return false
    } else {
      onlyOneChild.value = item
      return true
    }
  })

  if (showingChildren.length === 1) {
    return true
  }

  if (showingChildren.length === 0) {
    onlyOneChild.value = { ...parent, path: '', noShowingChildren: true }
    return true
  }

  return false
}

const resolvePath = (routePath: string) => {
  if (isExternal(routePath)) {
    return routePath
  }
  if (isExternal(props.basePath)) {
    return props.basePath
  }
  return path.resolve(props.basePath, routePath)
}

// 获取标题的辅助函数，支持国际化
const getTitle = (meta: any) => {
  if (!meta || !meta.title) return ''
  
  // 尝试进行国际化翻译，如果没有对应翻译则返回原标题
  const title = meta.title
  try {
    return t(title) !== title ? t(title) : title
  } catch {
    return title
  }
}
</script>

<script lang="ts">
import path from 'path-browserify'
</script>

<style lang="scss" scoped>
.submenu-title-wrapper {
  display: flex;
  align-items: center;
  width: 100%;
}

.menu-icon {
  margin-right: 12px;
  font-size: 16px;
  flex-shrink: 0;
}

.menu-title {
  transition: opacity 0.3s ease;
  overflow: hidden;
  white-space: nowrap;
}

// 收缩状态下隐藏标题
.el-menu--collapse {
  .menu-title {
    opacity: 0;
    width: 0;
  }
}

// 嵌套菜单样式
.nest-menu {
  .el-menu-item {
    padding-left: 20px !important;
    
    &:before {
      content: '';
      position: absolute;
      left: 10px;
      top: 0;
      bottom: 0;
      width: 2px;
      background-color: rgba(233, 227, 227, 0.1);
    }
  }
}
</style>
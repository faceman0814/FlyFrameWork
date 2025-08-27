<template>
  <div id="tags-view-container" class="tags-view-container">
    <scroll-pane ref="scrollPane" class="tags-view-wrapper" @scroll="handleScroll">
      <router-link
        v-for="tag in visitedViews"
        :key="tag.path"
        :class="isActive(tag) ? 'active' : ''"
        :to="{ path: tag.path, query: tag.query }"
        tag="span"
        class="tags-view-item"
        @click.middle="!isAffix(tag) ? closeSelectedTag(tag) : ''"
        @contextmenu.prevent="openMenu(tag, $event)"
      >
        {{ getTagTitle(tag) }}
        <span v-if="!isAffix(tag)" class="el-icon-close" @click.prevent.stop="closeSelectedTag(tag)">
          <el-icon><Close /></el-icon>
        </span>
      </router-link>
    </scroll-pane>
    <ul v-show="visible" :style="{ left: left + 'px', top: top + 'px' }" class="contextmenu">
      <li @click="refreshSelectedTag(selectedTag)">{{ t('common.refresh') }}</li>
      <li v-if="!isAffix(selectedTag)" @click="closeSelectedTag(selectedTag)">{{ t('common.close') }}</li>
      <li @click="closeOthersTags">{{ t('common.closeOthers') }}</li>
      <li @click="closeAllTags(selectedTag)">{{ t('common.closeAll') }}</li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { nextTick, ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter, RouteLocationNormalized } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { Close } from '@element-plus/icons-vue'
import ScrollPane from './ScrollPane.vue'
import { useTagsViewStore } from '@/stores/tagsView'

const route = useRoute()
const router = useRouter()
const tagsViewStore = useTagsViewStore()
const { t } = useI18n()

const visible = ref(false)
const top = ref(0)
const left = ref(0)
const selectedTag = ref<RouteLocationNormalized>({} as RouteLocationNormalized)
const scrollPane = ref()

const visitedViews = computed(() => tagsViewStore.visitedViews)

const isActive = (tag: RouteLocationNormalized) => {
  return tag.path === route.path
}

const isAffix = (tag: RouteLocationNormalized) => {
  return tag.meta?.affix
}

const getTagTitle = (tag: RouteLocationNormalized) => {
  const title = tag.meta?.title
  if (title) {
    // 尝试进行国际化翻译
    return t(title as string) || title
  }
  return tag.name || 'Unknown'
}

const addTags = () => {
  const { name } = route
  if (name) {
    tagsViewStore.addView(route)
  }
  return false
}

const moveToCurrentTag = () => {
  const tags = visitedViews.value
  nextTick(() => {
    for (const tag of tags) {
      if (tag.path === route.path) {
        scrollPane.value?.moveToTarget?.(tag)
        if (tag.fullPath !== route.fullPath) {
          tagsViewStore.updateVisitedView(route)
        }
        break
      }
    }
  })
}

const refreshSelectedTag = (view: RouteLocationNormalized) => {
  tagsViewStore.delCachedView(view)
  const { fullPath } = view
  nextTick(() => {
    router.replace({
      path: '/redirect' + fullPath
    })
  })
}

const closeSelectedTag = (view: RouteLocationNormalized) => {
  tagsViewStore.delView(view).then(({ visitedViews }: any) => {
    if (isActive(view)) {
      toLastView(visitedViews, view)
    }
  })
}

const closeOthersTags = () => {
  router.push(selectedTag.value)
  tagsViewStore.delOthersViews(selectedTag.value).then(() => {
    moveToCurrentTag()
  })
}

const closeAllTags = (view: RouteLocationNormalized) => {
  tagsViewStore.delAllViews().then(({ visitedViews }: any) => {
    toLastView(visitedViews, view)
  })
}

const toLastView = (visitedViews: RouteLocationNormalized[], _view: RouteLocationNormalized) => {
  const latestView = visitedViews.slice(-1)[0]
  if (latestView) {
    router.push(latestView.fullPath)
  } else {
    router.push('/')
  }
}

const openMenu = (tag: RouteLocationNormalized, e: MouseEvent) => {
  const menuMinWidth = 120
  const offsetLeft = 50 
  const offsetWidth = 50 
  const maxLeft = offsetLeft + offsetWidth - menuMinWidth

  left.value = Math.max(maxLeft, e.clientX - offsetLeft)
  top.value = e.clientY
  visible.value = true
  selectedTag.value = tag
}

const closeMenu = () => {
  visible.value = false
}

const handleScroll = () => {
  closeMenu()
}

watch(route, () => {
  addTags()
  moveToCurrentTag()
})

watch(visible, (value) => {
  if (value) {
    document.body.addEventListener('click', closeMenu)
  } else {
    document.body.removeEventListener('click', closeMenu)
  }
})

onMounted(() => {
  addTags()
})
</script>

<style lang="scss" scoped>
.tags-view-container {
  height: 34px;
  width: 100%;
  background: var(--tags-bg);
  border-bottom: 1px solid var(--border-color);
  box-shadow: var(--shadow-sm);
  transition: all 0.3s ease;

  .tags-view-wrapper {
    .tags-view-item {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      position: relative;
      cursor: pointer;
      height: 26px;
      border: 1px solid var(--border-color);
      color: var(--tags-item-text);
      background: var(--tags-item-bg);
      padding: 0 8px;
      font-size: 12px;
      margin-left: 5px;
      margin-top: 4px;
      border-radius: 4px;
      transition: all 0.3s ease;
      text-align: center;
      white-space: nowrap;

      &:first-of-type {
        margin-left: 15px;
      }

      &:last-of-type {
        margin-right: 15px;
      }

      &:hover {
        background: var(--tags-item-hover-bg);
        color: var(--primary-color);
        border-color: var(--primary-color);
      }

      &.active {
        background: var(--tags-item-active-bg);
        color: var(--tags-item-active-text);
        border: 1px solid transparent;
        box-shadow: var(--shadow-sm);

        &::before {
          content: '';
          background: rgba(255, 255, 255, 0.9);
          display: inline-block;
          width: 8px;
          height: 8px;
          border-radius: 50%;
          position: relative;
          margin-right: 2px;
        }
      }
    }
  }

  .contextmenu {
    margin: 0;
    background: var(--bg-container);
    z-index: 3000;
    position: absolute;
    list-style-type: none;
    padding: 8px 0;
    border-radius: 8px;
    font-size: 12px;
    font-weight: 400;
    color: var(--text-primary);
    box-shadow: var(--shadow-lg);
    border: 1px solid var(--border-color);
    backdrop-filter: var(--glass-blur);
    min-width: 120px;

    li {
      margin: 0;
      padding: 8px 16px;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--hover-bg);
        color: var(--primary-color);
      }
    }
  }
}

.el-icon-close {
  width: 16px;
  height: 16px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  text-align: center;
  transition: all 0.3s cubic-bezier(0.645, 0.045, 0.355, 1);
  transform-origin: 100% 50%;
  margin-left: 4px;
  
  .el-icon {
    font-size: 12px;
  }

  &:hover {
    background-color: rgba(255, 255, 255, 0.2);
    color: #fff;
    transform: scale(1.1);
  }
}
</style>
<!-- 虚拟滚动表格组件 - 用于大数据量表格 -->
<template>
  <div class="virtual-table-container" ref="containerRef">
    <div 
      class="virtual-table-wrapper"
      :style="{ height: `${totalHeight}px` }"
    >
      <div 
        class="virtual-table-content"
        :style="{ transform: `translateY(${offsetY}px)` }"
      >
        <table class="virtual-table">
          <thead>
            <tr>
              <th 
                v-for="column in columns" 
                :key="column.key"
                :style="{ width: column.width }"
              >
                {{ column.title }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="item in visibleData"
              :key="item[rowKey]"
              class="virtual-table-row"
              :style="{ height: `${itemHeight}px` }"
            >
              <td 
                v-for="column in columns"
                :key="column.key"
                :style="{ width: column.width }"
              >
                <slot 
                  :name="column.key" 
                  :item="item" 
                  :column="column"
                >
                  {{ item[column.key] }}
                </slot>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'

interface Column {
  key: string
  title: string
  width?: string
}

interface Props {
  data: any[]
  columns: Column[]
  itemHeight: number
  containerHeight: number
  rowKey: string
  buffer?: number
}

const props = withDefaults(defineProps<Props>(), {
  buffer: 5
})

const containerRef = ref<HTMLElement>()
const scrollTop = ref(0)

// 计算可见区域
const visibleRange = computed(() => {
  const start = Math.floor(scrollTop.value / props.itemHeight)
  const visibleCount = Math.ceil(props.containerHeight / props.itemHeight)
  
  return {
    start: Math.max(0, start - props.buffer),
    end: Math.min(props.data.length, start + visibleCount + props.buffer)
  }
})

// 可见数据
const visibleData = computed(() => {
  return props.data.slice(visibleRange.value.start, visibleRange.value.end)
})

// 总高度
const totalHeight = computed(() => {
  return props.data.length * props.itemHeight
})

// 偏移量
const offsetY = computed(() => {
  return visibleRange.value.start * props.itemHeight
})

// 滚动事件处理
const handleScroll = (event: Event) => {
  const target = event.target as HTMLElement
  scrollTop.value = target.scrollTop
}

onMounted(() => {
  containerRef.value?.addEventListener('scroll', handleScroll)
})

onUnmounted(() => {
  containerRef.value?.removeEventListener('scroll', handleScroll)
})

// 监听数据变化，重置滚动位置
watch(() => props.data, () => {
  scrollTop.value = 0
  if (containerRef.value) {
    containerRef.value.scrollTop = 0
  }
})
</script>

<style lang="scss" scoped>
.virtual-table-container {
  height: v-bind(containerHeight + 'px');
  overflow-y: auto;
  position: relative;
}

.virtual-table-wrapper {
  position: relative;
}

.virtual-table-content {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
}

.virtual-table {
  width: 100%;
  table-layout: fixed;
  border-collapse: collapse;

  th, td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid var(--border-light);
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  th {
    background: var(--table-header-bg);
    font-weight: 600;
    position: sticky;
    top: 0;
    z-index: 1;
  }
}

.virtual-table-row {
  transition: background-color 0.2s ease;
  
  &:hover {
    background-color: var(--table-row-hover);
  }
}
</style>
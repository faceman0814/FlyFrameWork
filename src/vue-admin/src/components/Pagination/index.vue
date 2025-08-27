<template>
  <div :class="{ 'hidden': hidden }" class="pagination-container">
    <el-pagination
      :background="background"
      :current-page="currentPage"
      :page-size="pageSize"
      :layout="layout"
      :page-sizes="pageSizes"
      :total="total"
      v-bind="$attrs"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { scrollTo } from '@/utils/scroll-to'

interface Props {
  total: number
  page?: number
  limit?: number
  pageSizes?: number[]
  layout?: string
  background?: boolean
  autoScroll?: boolean
  hidden?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  page: 1,
  limit: 20,
  pageSizes: () => [10, 20, 30, 50],
  layout: 'total, sizes, prev, pager, next, jumper',
  background: true,
  autoScroll: true,
  hidden: false
})

const emit = defineEmits<{
  'update:page': [page: number]
  'update:limit': [limit: number]
  pagination: [data: { page: number; limit: number }]
}>()

const currentPage = computed({
  get() {
    return props.page
  },
  set(val) {
    emit('update:page', val)
  }
})

const pageSize = computed({
  get() {
    return props.limit
  },
  set(val) {
    emit('update:limit', val)
  }
})

function handleSizeChange(val: number) {
  emit('update:limit', val)
  emit('pagination', { page: currentPage.value, limit: val })
  if (props.autoScroll) {
    scrollTo(0, 800)
  }
}

function handleCurrentChange(val: number) {
  emit('update:page', val)
  emit('pagination', { page: val, limit: pageSize.value })
  if (props.autoScroll) {
    scrollTo(0, 800)
  }
}
</script>

<style scoped>
.pagination-container {
  background: transparent;
  padding: 16px 0;
  display: flex;
  justify-content: flex-end;
  align-items: center;
}

.pagination-container.hidden {
  display: none;
}

:deep(.el-pagination) {
  .el-pagination__total {
    margin-right: auto;
    color: var(--text-regular);
    font-weight: 500;
  }
  
  .el-pager {
    li {
      min-width: 32px;
      height: 32px;
      line-height: 30px;
      border-radius: 6px;
      border: 1px solid var(--border-light);
      background: var(--bg-container);
      color: var(--text-regular);
      font-weight: 500;
      transition: all 0.2s ease;
      
      &:hover {
        background: var(--bg-hover);
        border-color: var(--primary-color);
        color: var(--primary-color);
        transform: translateY(-1px);
      }
      
      &.is-active {
        background: var(--primary-color);
        border-color: var(--primary-color);
        color: #fff;
        box-shadow: var(--shadow-sm);
        
        &:hover {
          transform: translateY(-1px);
        }
      }
    }
  }
  
  .btn-prev,
  .btn-next {
    min-width: 32px;
    height: 32px;
    line-height: 30px;
    border-radius: 6px;
    border: 1px solid var(--border-light);
    background: var(--bg-container);
    color: var(--text-regular);
    transition: all 0.2s ease;
    
    &:hover {
      background: var(--bg-hover);
      border-color: var(--primary-color);
      color: var(--primary-color);
      transform: translateY(-1px);
    }
    
    &:disabled {
      background: var(--bg-container);
      border-color: var(--border-light);
      color: var(--text-disabled);
      transform: none;
      cursor: not-allowed;
    }
  }
  
  .el-pagination__sizes {
    .el-select {
      .el-select__wrapper {
        border-radius: 6px;
        border-color: var(--border-light);
        background: var(--bg-container);
        
        &:hover {
          border-color: var(--primary-color);
        }
        
        &.is-focused {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 2px rgba(var(--primary-color-rgb), 0.1);
        }
      }
    }
  }
  
  .el-pagination__jump {
    .el-pagination__editor {
      .el-input__wrapper {
        border-radius: 6px;
        border-color: var(--border-light);
        background: var(--bg-container);
        
        &:hover {
          border-color: var(--primary-color);
        }
        
        &.is-focus {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 2px rgba(var(--primary-color-rgb), 0.1);
        }
      }
    }
  }
}

@media (max-width: 768px) {
  .pagination-container {
    justify-content: center;
  }
  
  :deep(.el-pagination) {
    .el-pagination__total {
      margin-right: 0;
      margin-bottom: 8px;
      text-align: center;
      width: 100%;
    }
  }
}
</style>
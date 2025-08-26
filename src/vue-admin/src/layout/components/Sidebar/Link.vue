<template>
  <component
    :is="type"
    v-bind="linkProps"
  >
    <slot />
  </component>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { isExternal } from '@/utils/validate'

interface Props {
  to: string
}

const props = defineProps<Props>()

const isExt = computed(() => isExternal(props.to))
const type = computed(() => {
  if (isExt.value) {
    return 'a'
  }
  return 'router-link'
})

const linkProps = computed(() => {
  if (isExt.value) {
    return {
      href: props.to,
      target: '_blank',
      rel: 'noopener'
    }
  }
  return {
    to: props.to
  }
})
</script>
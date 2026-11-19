<template>
  <article
    class="rounded-2xl border p-5 shadow-sm transition-shadow hover:shadow-md"
    :class="variantClasses.card">
    <div class="flex items-center justify-between">
      <p class="text-xs font-bold uppercase tracking-wide" :class="variantClasses.label">
        {{ label }}
      </p>
      <span
        class="flex h-11 w-11 items-center justify-center rounded-2xl text-lg"
        :class="variantClasses.iconBox">
        <i :class="icon" aria-hidden="true"></i>
      </span>
    </div>

    <div
      v-if="loading"
      class="mt-3 h-11 w-16 animate-pulse rounded-lg"
      :class="variantClasses.skeleton">
    </div>
    <p v-else class="mt-3 font-black tracking-tight text-[#2d3627]" :class="valueSizeClass">
      {{ value ?? '—' }}
    </p>

    <p class="mt-1 text-sm" :class="variantClasses.description">
      {{ description }}
    </p>
  </article>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  label: string
  value: string | number | null
  description: string
  icon: string
  variant?: 'default' | 'sage' | 'warm'
  loading?: boolean
  compactValue?: boolean
}>(), {
  variant: 'default',
  loading: false,
  compactValue: false
})

const VARIANT_MAP = {
  default: {
    card: 'border-[#dce3d4] bg-white',
    label: 'text-[#4a5542]',
    iconBox: 'bg-[#f4f7f0] text-[#718161]',
    skeleton: 'bg-[#f4f7f0]',
    description: 'text-gray-500'
  },
  sage: {
    card: 'border-[#dce3d4] bg-[#f4f7f0]',
    label: 'text-[#4a5542]',
    iconBox: 'bg-white/80 text-[#718161]',
    skeleton: 'bg-white/80',
    description: 'text-[#4a5542]'
  },
  warm: {
    card: 'border-[#ead8c3] bg-[#fbf2e8]',
    label: 'text-[#76512f]',
    iconBox: 'bg-white/80 text-[#a66a2c]',
    skeleton: 'bg-white/80',
    description: 'text-[#76512f]'
  }
} as const

const variantClasses = computed(() => VARIANT_MAP[props.variant])
const valueSizeClass = computed(() => (props.compactValue ? 'text-3xl' : 'text-4xl'))
</script>
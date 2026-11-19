<template>
  <header class="mb-6 flex flex-row items-center justify-between gap-3 md:mb-8">
    <div>
      <p v-if="eyebrow" class="text-xs font-semibold uppercase tracking-wide text-[#718161] md:text-sm">
        {{ eyebrow }}
      </p>
      <h1 class="mt-1 text-2xl font-bold tracking-tight text-[#2d3627] md:text-3xl">
        {{ title }}
      </h1>
      <p v-if="subtitle" class="mt-1 text-xs font-medium text-[#4a5542] md:text-sm">
        {{ subtitle }}
      </p>
    </div>

    <div v-if="$slots.actions" class="shrink-0">
      <slot name="actions" />
    </div>
  </header>
</template>

<script setup lang="ts">
defineProps<{
  eyebrow?: string
  title: string
  subtitle?: string
}>()
</script>
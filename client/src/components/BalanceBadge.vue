<template>
    <div class="inline-flex items-center gap-2 rounded-xl border px-3 py-1.5" :class="meta.classes">
        <i v-if="showDetails" :class="meta.icon" aria-hidden="true"></i>
        <span class="text-sm font-bold">{{ balance }} ders</span>
        <span v-if="showDetails" class="text-xs font-semibold">{{ meta.label }}</span>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
    balance: number
    hasAnyPayment?: boolean
    showDetails?: boolean
}>(), {
    hasAnyPayment: true,
    showDetails: true
})

const meta = computed(() => {

    if (props.balance === 0 && !props.hasAnyPayment) {
        return {
            classes: 'border-gray-200 bg-[#f8f9f5] text-[#4a5542]',
            icon: 'pi pi-user-plus',
            label: ''
        }
    }
    if (props.balance < 0) {
        return {
            classes: 'border-red-300 bg-red-50 text-red-800',
            icon: 'pi pi-exclamation-circle',
            label: 'Eksi bakiye'
        }
    }
    if (props.balance === 0) {
        return {
            classes: 'border-red-200 bg-red-50 text-red-700',
            icon: 'pi pi-exclamation-circle',
            label: 'Ders hakkı bitti'
        }
    }
    if (props.balance <= 2) {
        return {
            classes: 'border-amber-300 bg-amber-50 text-amber-800',
            icon: 'pi pi-exclamation-triangle',
            label: 'Az kaldı'
        }
    }
    return {
        classes: 'border-[#dce3d4] bg-[#f4f7f0] text-[#2d3627]',
        icon: 'pi pi-check-circle',
        label: 'Kullanılabilir'
    }
})
</script>
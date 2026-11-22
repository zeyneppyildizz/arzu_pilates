<template>
  <section
    v-if="mySlots.length"
    class="rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-6">
    <div class="mb-4 flex items-center justify-between gap-2">
      <div>
        <h2 class="text-base font-bold text-[#2d3627]">Haftalık ders günlerim</h2>
      </div>
      <span class="flex h-10 w-10 shrink-0 items-center justify-center rounded-2xl bg-[#f4f7f0] text-[#718161]">
        <i class="pi pi-calendar text-base"></i>
      </span>
    </div>

    <div class="grid grid-cols-1 gap-2.5 sm:grid-cols-2">
      <div
        v-for="slot in mySlots"
        :key="slot.id"
        class="flex items-center justify-between rounded-2xl border px-3.5 py-3 transition-colors"
        :class="slot.dayOfWeek === todayDayOfWeek
          ? 'border-[#718161] bg-[#f4f7f0]'
          : 'border-gray-100 bg-[#fdfdfb]'">
        <div class="flex items-center gap-2.5">
          <span
            class="flex h-8 w-8 shrink-0 items-center justify-center rounded-xl text-xs font-bold"
            :class="slot.dayOfWeek === todayDayOfWeek
              ? 'bg-[#2d3627] text-[#f3f4ef]'
              : 'bg-[#eef2e9] text-[#4a5542]'">
            {{ getDayShort(slot.dayOfWeek) }}
          </span>
          <div>
            <p class="text-sm font-bold text-[#2d3627]">{{ getDayLabel(slot.dayOfWeek) }}</p>
            <p v-if="slot.dayOfWeek === todayDayOfWeek" class="text-[11px] font-semibold text-[#718161]">
              Bugün dersiniz var
            </p>
          </div>
        </div>

        <span class="inline-flex items-center gap-1.5 rounded-full border border-[#dce3d4] bg-white px-2.5 py-1 text-xs font-bold text-[#2d3627]">
          <i class="pi pi-clock text-[11px] text-[#718161]"></i>
          {{ slot.startTime }}
        </span>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useScheduleStore } from '../stores/schedule.store'

const DAYS: Record<number, { label: string; short: string }> = {
  1: { label: 'Pazartesi', short: 'Pzt' },
  2: { label: 'Salı', short: 'Sal' },
  3: { label: 'Çarşamba', short: 'Çar' },
  4: { label: 'Perşembe', short: 'Per' },
  5: { label: 'Cuma', short: 'Cum' },
  6: { label: 'Cumartesi', short: 'Cmt' },
  7: { label: 'Pazar', short: 'Paz' }
}

const scheduleStore = useScheduleStore()
const { mySlots } = storeToRefs(scheduleStore)

const todayDayOfWeek = new Date().getDay() || 7

const getDayLabel = (day: number) => DAYS[day]?.label ?? ''
const getDayShort = (day: number) => DAYS[day]?.short ?? ''

onMounted(() => {
  scheduleStore.fetchMySchedule()
})
</script>
<template>
  <div class="max-w-6xl mx-auto p-4 md:p-8">
    <PageHeader eyebrow="ÖĞRENCİ PANELİ" title="İşlem Geçmişim"
      subtitle="Derse katılım ve paket alımlarınızı takip edin." />

    <div v-if="isLoading" class="flex justify-center py-16">
      <i class="pi pi-spin pi-spinner text-4xl text-[#718161]"></i>
    </div>

    <div v-else-if="error" role="alert"
      class="rounded-2xl border border-red-200 bg-red-50 px-5 py-4 text-sm font-medium text-red-700">
      {{ error }}
    </div>

    <template v-else>
      <section class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard label="Bu ay katılım" :value="currentMonthAttendanceCount" description="katıldığınız ders"
          icon="pi pi-calendar" />
        <StatCard label="Alınan toplam ders" :value="totalAddedSessions" description="paketlerle eklenen ders"
          icon="pi pi-ticket" variant="sage" />
        <StatCard label="Toplam ödeme" :value="formatCurrency(totalPayments)" description="paket alımlarınız"
          icon="pi pi-wallet" compact-value />
      </section>

      <section class="mt-6 rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-7">
        <div class="mb-5">
          <h2 class="text-lg font-bold text-[#2d3627]">Son hareketler</h2>
          <p class="mt-1 text-sm text-gray-500">Yoklama ve paket işlemleriniz tarih sırasıyla gösterilir.</p>
        </div>

        <div v-if="pageActivities.length" class="space-y-3">
          <article v-for="activity in pageActivities" :key="activity.key"
            class="flex items-start gap-3 rounded-2xl border border-gray-100 bg-[#fdfdfb] p-4 sm:items-center sm:gap-4">
            <span class="flex h-11 w-11 shrink-0 items-center justify-center rounded-2xl"
              :class="getActivityIconBoxClass(activity)">
              <i :class="getActivityIcon(activity)"></i>
            </span>

            <div class="min-w-0 flex-1">
              <p class="truncate font-bold text-[#2d3627]">
                {{ getActivityTitle(activity) }}
              </p>
              <p class="mt-1 text-xs text-gray-500">{{ formatDate(activity.date) }}</p>
              <p v-if="activity.type === 'attendance' && activity.note"
                class="mt-1.5 text-xs leading-relaxed text-[#4a5542] break-words">
                Not: {{ activity.note }}
              </p>
            </div>

            <div class="shrink-0 text-right">
              <template v-if="activity.type === 'attendance'">
                <span class="inline-flex rounded-full border px-2.5 py-1 text-xs font-bold" :class="activity.attendanceType === 2
                  ? 'border-amber-200 bg-amber-50 text-amber-800'
                  : 'border-[#dce3d4] bg-[#f4f7f0] text-[#2d3627]'">
                  {{ activity.balanceAfter }} ders kaldı
                </span>
              </template>
              <template v-else>
                <p class="font-bold text-[#2d3627]">{{ formatCurrency(activity.amount) }}</p>
                <p class="mt-1 text-xs font-semibold text-[#718161]">+{{ activity.addedSessions }} ders</p>
              </template>
            </div>
          </article>
        </div>

        <div v-else class="rounded-2xl bg-[#f8f9f5] px-4 py-8 text-center text-sm font-medium text-gray-500">
          Henüz işlem geçmişiniz bulunmuyor.
        </div>

        <Paginator v-if="activities.length > rows" v-model:first="first" :rows="rows" :totalRecords="activities.length"
          class="!bg-transparent mt-5" />
      </section>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import type { HistoryActivity } from '../types/history.type'
import { useAuthStore } from '../stores/auth.store'
import { useHistoryStore } from '../stores/history.store'
import { formatDate } from '../utils/date.util'
import { formatCurrency } from '../utils/currency.util'

import Paginator from 'primevue/paginator'
import PageHeader from '../components/PageHeader.vue'
import StatCard from '../components/StatCard.vue'

const authStore = useAuthStore()
const historyStore = useHistoryStore()
const {
  currentMonthAttendanceCount,
  totalAddedSessions,
  totalPayments,
  activities,
  isLoading,
  error
} = storeToRefs(historyStore)

const first = ref(0)
const rows = 10

const pageActivities = computed(() =>
  activities.value.slice(first.value, first.value + rows)
)

const getActivityTitle = (activity: HistoryActivity) => {
  if (activity.type === 'payment') return 'Paket alımı'
  if (activity.attendanceType === 2) return 'Son dakika iptali'
  if (activity.attendanceType === 1) return 'Manuel yoklama'
  return 'Derse katılım'
}

const getActivityIcon = (activity: HistoryActivity) => {
  if (activity.type === 'payment') return 'pi pi-wallet'
  if (activity.attendanceType === 2) return 'pi pi-clock'
  if (activity.attendanceType === 1) return 'pi pi-check-circle'
  return 'pi pi-calendar'
}

const getActivityIconBoxClass = (activity: HistoryActivity) => {
  if (activity.type === 'payment') return 'bg-[#eef2e9] text-[#4a5542]'
  if (activity.attendanceType === 2) return 'bg-amber-50 text-amber-700'
  return 'bg-[#f4f7f0] text-[#718161]'
}

onMounted(async () => {
  if (authStore.user?.id) {
    await historyStore.fetchHistory(authStore.user.id)
  }
})
</script>
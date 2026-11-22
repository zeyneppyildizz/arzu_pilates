<template>
  <div class="max-w-6xl mx-auto p-4 md:p-8">
    <PageHeader eyebrow="ÖĞRENCİ PANELİ" :title="`Merhaba, ${user?.firstName ?? ''}`"
      subtitle="Ders durumunuzu ve bilgilerinizi buradan takip edin." />

    <div v-if="balance !== null && (balance < 0 || (balance === 0 && hasAnyPayment))" role="alert"
      class="mb-5 flex items-center gap-3 rounded-2xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-medium text-amber-800">
      <i class="pi pi-exclamation-triangle text-base text-amber-600"></i>
      <span>
        {{ balance === 0
          ? 'Ders hakkınız bitti. Devam edebilmek için eğitmeninizden yeni paket yüklemesi talep edebilirsiniz.'
          : `Bakiyeniz ekside (${balance} ders). Lütfen eğitmeninize ödeme yapın.` }}
      </span>
    </div>

    <div class="grid gap-5 lg:grid-cols-[1.4fr_1fr]">
      <section class="relative overflow-hidden rounded-3xl bg-[#2d3627] p-6 md:p-8 text-white shadow-sm">
        <div class="relative z-10">
          <div class="flex items-start justify-between gap-4">
            <div>
              <p class="text-sm font-semibold text-white/70">GÜNCEL DERS HAKKINIZ</p>
              <p v-if="balance !== null" class="mt-3 text-5xl font-black tracking-tight">{{ balance }}</p>
              <Skeleton v-else width="5rem" height="3rem" class="mt-3 !bg-white/20" />
              <p class="mt-2 text-sm text-white/70">kullanılabilir ders</p>
            </div>
            <span class="flex h-12 w-12 items-center justify-center rounded-2xl bg-white/10 text-xl">
              <i class="pi pi-ticket"></i>
            </span>
          </div>
          <router-link to="/history"
            class="mt-7 inline-flex items-center gap-2 rounded-xl bg-[#f4f7f0] px-4 py-2.5 text-sm font-bold text-[#2d3627] transition-colors hover:bg-white">
            İşlem geçmişim
            <i class="pi pi-arrow-up-right text-xs"></i>
          </router-link>
        </div>
        <div class="absolute -right-10 -top-16 h-56 w-56 rounded-full border border-white/10"></div>
      </section>

      <section class="flex flex-col items-center rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-6">
        <div class="w-full">
          <h2 class="font-bold text-[#2d3627]">Yoklama QR kodunuz</h2>
          <p class="mt-1 text-sm text-gray-500">Derse katıldığınızda eğitmeninize gösterin.</p>
        </div>
        <div class="mt-4 rounded-2xl border border-[#dce3d4] bg-[#f8f9f5] p-3">
          <qrcode-vue :value="qrValue" :size="168" level="H" render-as="svg" />
        </div>
      </section>
    </div>

    <StudentWeeklyScheduleCard class="mt-5" />

    <section class="mt-5 rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-7">
      <div class="flex items-start justify-between gap-4">
        <div>
          <h2 class="font-bold text-[#2d3627]">Vücut ölçülerim</h2>
          <p class="mt-1 text-sm text-gray-500">Güncel ölçülerinize hızlıca göz atın.</p>
        </div>
        <router-link to="/measurements" class="shrink-0 text-sm font-bold text-[#718161] hover:text-[#4a5542]">
          Düzenle
        </router-link>
      </div>

      <div class="mt-5 grid grid-cols-2 gap-3 md:grid-cols-3">
        <div v-for="item in measurementSummary" :key="item.label" class="rounded-2xl bg-[#f8f9f5] p-4">
          <p class="text-xs font-semibold text-[#4a5542]">{{ item.label }}</p>
          <p class="mt-2 text-xl font-bold text-[#2d3627]">
            {{ item.value ?? '—' }}<span v-if="item.value !== null" class="ml-1 text-xs font-medium text-gray-500">{{
              item.unit }}</span>
          </p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted, watch } from 'vue'
import QrcodeVue from 'qrcode.vue'
import { storeToRefs } from 'pinia'

import { useAuthStore } from '../stores/auth.store'
import { useStudentStore } from '../stores/student.store'
import { useMeasurementStore } from '../stores/measurement.store'
import { generateAttendanceQrPayload } from '../utils/qr.util'

import Skeleton from 'primevue/skeleton'
import PageHeader from '../components/PageHeader.vue'
import StudentWeeklyScheduleCard from '../components/StudentWeeklyScheduleCard.vue'

const authStore = useAuthStore()
const studentStore = useStudentStore()
const measurementStore = useMeasurementStore()

const { user } = storeToRefs(authStore)

const qrValue = ref('')
const isBalanceLoading = ref(true)
const hasAnyPayment = ref(false)
let qrInterval: ReturnType<typeof setInterval> | null = null

const balance = computed(() => (isBalanceLoading.value ? null : (user.value?.balance ?? 0)))

watch(balance, (newVal) => {
  if (newVal !== null && newVal > 0) {
    hasAnyPayment.value = true
  }
})

const measurementSummary = computed(() => [
  { label: 'Boy', value: measurementStore.measurements.height, unit: 'cm' },
  { label: 'Kilo', value: measurementStore.measurements.weight, unit: 'kg' },
  { label: 'Bel', value: measurementStore.measurements.waist, unit: 'cm' }
])

const updateQrCode = () => {
  if (user.value?.id) {
    qrValue.value = generateAttendanceQrPayload(user.value.id)
  }
}

const loadStudentData = async () => {
  try {
    const myProfile = await studentStore.fetchMyProfile()
    if (myProfile && user.value) {
      user.value.balance = myProfile.balance
      hasAnyPayment.value = myProfile.hasAnyPayment
      localStorage.setItem('user', JSON.stringify(user.value))
    }
  } finally {
    isBalanceLoading.value = false
  }
}

onMounted(async () => {
  updateQrCode()
  qrInterval = setInterval(updateQrCode, 600000)

  if (user.value?.id) {
    await Promise.all([
      loadStudentData(),
      measurementStore.fetchMeasurements(user.value.id)
    ])
  } else {
    isBalanceLoading.value = false
  }
})

onUnmounted(() => {
  if (qrInterval) clearInterval(qrInterval)
})
</script>
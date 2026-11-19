<template>
  <div class="max-w-6xl mx-auto p-4 md:p-8">
    <section class="relative overflow-hidden rounded-3xl bg-[#2d3627] px-5 py-5 md:px-7 md:py-6 text-white shadow-sm">
      <div class="relative z-10 max-w-2xl">
        <p class="text-xs font-semibold tracking-[0.16em] text-[#dce3d4]">ARZU PILATES · EĞİTMEN PANELİ</p>
        <h1 class="mt-2 text-2xl md:text-3xl font-bold tracking-tight">
          Merhaba, {{ authStore.user?.firstName }}
        </h1>
        <p class="mt-1 text-sm text-white/75">
          Stüdyonuzdaki güncel durumu buradan takip edebilirsiniz.
        </p>
        <div class="mt-4 flex flex-wrap gap-2">
          <router-link to="/scanner"
            class="inline-flex items-center gap-2 rounded-xl bg-[#f4f7f0] px-4 py-2 text-sm font-bold text-[#2d3627] transition-colors hover:bg-white">
            <i class="pi pi-qrcode"></i>
            Yoklama Al
          </router-link>
          <router-link to="/list"
            class="inline-flex items-center gap-2 rounded-xl border border-white/30 px-4 py-2 text-sm font-semibold text-white transition-colors hover:bg-white/10">
            <i class="pi pi-users"></i>
            Öğrenci Listesi
          </router-link>
        </div>
      </div>
      <div class="absolute -right-10 -top-16 h-48 w-48 rounded-full border border-white/10"></div>
      <div class="absolute -right-2 -top-8 h-32 w-32 rounded-full border border-white/10"></div>
    </section>

    <p v-if="loadError" role="alert" class="mt-5 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm font-medium text-red-700">
      {{ loadError }}
      <button type="button" class="ml-2 font-bold underline" @click="loadDashboard">Tekrar dene</button>
    </p>

    <section class="mt-5 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <StatCard
        label="KAYITLI ÖĞRENCİ"
        :value="students.length"
        description="Aktif öğrenci hesabı"
        icon="pi pi-users"
        :loading="isLoadingStudents"
      />
      <StatCard
        label="Bugünkü yoklama"
        :value="todayAttendanceCount"
        description="Bugün alınan yoklama"
        icon="pi pi-calendar"
        variant="sage"
        :loading="isLoadingTodayCount"
      />
      <StatCard
        label="Ders hakkı azalan"
        :value="lowBalanceStudents.length"
        description="2 veya daha az dersi kalan"
        icon="pi pi-exclamation-circle"
        variant="warm"
        :loading="isLoadingStudents"
      />
    </section>

    <section class="mt-6 rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-7">
      <div class="flex items-start justify-between gap-4">
        <div>
          <h2 class="text-lg font-bold text-[#2d3627]">Takip edilmesi gereken öğrenciler</h2>
          <p class="mt-1 text-sm text-gray-500">Ders hakkı 2 veya daha az olan öğrenciler</p>
        </div>
        <router-link to="/list" class="shrink-0 text-sm font-bold text-[#718161] hover:text-[#4a5542]">
          Tümünü gör
        </router-link>
      </div>

      <div v-if="isLoadingStudents" class="mt-5 space-y-3">
        <div v-for="row in 3" :key="row" class="h-14 animate-pulse rounded-xl bg-[#f8f9f5]"></div>
      </div>
      <p v-else-if="lowBalanceStudents.length === 0" class="mt-5 rounded-2xl bg-[#f8f9f5] px-4 py-6 text-center text-sm font-medium text-[#4a5542]">
        Takip gerektiren öğrenci bulunmuyor.
      </p>
      <ul v-else class="mt-4 divide-y divide-gray-100">
        <li v-for="student in lowBalanceStudents" :key="student.id" class="flex items-center justify-between gap-4 py-3">
          <div class="flex min-w-0 items-center gap-3">
            <span class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-[#f4f7f0] font-bold text-[#2d3627]">
              {{ student.firstName.charAt(0) }}{{ student.lastName.charAt(0) }}
            </span>
            <span class="truncate font-semibold text-[#2d3627]">{{ student.firstName }} {{ student.lastName }}</span>
          </div>
          <BalanceBadge :balance="student.balance" :show-details="false" class="shrink-0" />
        </li>
      </ul>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '../stores/auth.store'
import { useAttendanceStore } from '../stores/attendance.store'
import { useStudentStore } from '../stores/student.store'
import StatCard from '../components/StatCard.vue'
import BalanceBadge from '../components/BalanceBadge.vue'

const LOW_BALANCE_THRESHOLD = 2

const authStore = useAuthStore()
const attendanceStore = useAttendanceStore()
const studentStore = useStudentStore()

const {
  todayAttendanceCount,
  isLoadingTodayCount,
  todayAttendanceError
} = storeToRefs(attendanceStore)

const {
  students,
  isLoading: isLoadingStudents,
  error: studentsError
} = storeToRefs(studentStore)

const lowBalanceStudents = computed(() =>
  students.value
    .filter(
      student =>
        student.balance < 0 ||
        (student.hasAnyPayment && student.balance <= LOW_BALANCE_THRESHOLD)
    )
    .sort((a, b) => a.balance - b.balance)
)
const loadError = computed(() =>
  [studentsError.value, todayAttendanceError.value].filter(Boolean).join(' ')
)

const loadDashboard = async () => {
  await Promise.all([
    studentStore.fetchStudents(),
    attendanceStore.fetchTodayCount()
  ])
}

onMounted(loadDashboard)
</script>
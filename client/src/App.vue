<template>
  <div class="relative min-h-screen font-sans text-[#2d3627] bg-[#f3f4ef]">
    <div class="relative z-10 min-h-screen flex flex-col">
      <AppHeader v-if="showHeader" />

      <main class="flex-grow" :class="showHeader ? 'pb-20 md:pb-0' : ''">
        <router-view />
      </main>

      <Toast />
      <ConfirmDialog />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, watch, onUnmounted } from 'vue'
import { useAlert } from './composables/useAlert'
import { useAuthStore } from './stores/auth.store'
import { useStudentStore } from './stores/student.store'
import { useHistoryStore } from './stores/history.store'
import { useAttendanceStore } from './stores/attendance.store'
import signalrService from './services/signalr.service'
import { useRouter } from 'vue-router'

import AppHeader from './components/Header.vue'
import Toast from 'primevue/toast'
import ConfirmDialog from 'primevue/confirmdialog'

const alert = useAlert()

const authStore = useAuthStore()
const studentStore = useStudentStore()
const historyStore = useHistoryStore()
const attendanceStore = useAttendanceStore()
const router = useRouter()

const isLoggedIn = computed(() => !!authStore.user)
const showHeader = computed(() => isLoggedIn.value && !authStore.user?.mustChangePassword)

const handleNewStudent = async () => {
  if (Number(authStore.user?.role) !== 1) return
  await studentStore.fetchStudents()
  alert.showSuccess('Sisteme yeni bir öğrenci kayıt oldu!')
}

const handleUserDeleted = (deletedUserId: number) => {
  if (Number(authStore.user?.id) === Number(deletedUserId)) {
    authStore.logout()
    alert.showWarn('Hesabınız yönetici tarafından silindi.')
    router.push('/login')
  }
}

const handleAttendanceUpdate = async (updatedStudentId: number, newBalance: number) => {
  const currentUser = authStore.user
  if (!currentUser) return

  if (Number(currentUser.role) === 1) {
    const student = studentStore.students.find((s) => Number(s.id) === Number(updatedStudentId))
    if (student) {
      student.balance = newBalance
    }
    await attendanceStore.fetchTodayCount()
    return
  }

  if (Number(currentUser.id) === Number(updatedStudentId)) {
    const previousBalance = currentUser.balance ?? 0
    currentUser.balance = newBalance
    localStorage.setItem('user', JSON.stringify(currentUser))

    if (newBalance < previousBalance && newBalance <= 0) {
      const message =
        newBalance === 0
          ? 'Ders hakkınız bitti. Yeni dersler için ödeme yapmanız gerekiyor.'
          : 'Ders hakkınız eksiye düştü. Lütfen en kısa sürede ödeme yapın.'

      alert.showWarn(message)
    }

    await historyStore.fetchHistory(currentUser.id)
  }
}

watch(
  isLoggedIn,
  async (loggedIn) => {
    if (loggedIn) {
      const connection = signalrService.initConnection()
      connection.off('ReceiveAttendanceUpdate', handleAttendanceUpdate)
      connection.on('ReceiveAttendanceUpdate', handleAttendanceUpdate)

      connection.off('ReceiveNewStudent', handleNewStudent)
      connection.on('ReceiveNewStudent', handleNewStudent)

      connection.off('UserDeleted', handleUserDeleted)
      connection.on('UserDeleted', handleUserDeleted)

      await signalrService.start()
    } else {
      signalrService.stop()
    }
  },
  { immediate: true }
)

onUnmounted(() => {
  signalrService.stop()
})
</script>
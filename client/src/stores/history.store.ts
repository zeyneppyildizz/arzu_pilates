import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { historyService } from '../services/history.service'
import type { AttendanceDto, HistoryActivity, PaymentDto } from '../types/history.type'

export const useHistoryStore = defineStore('history', () => {
  const attendances = ref<AttendanceDto[]>([])
  const payments = ref<PaymentDto[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const currentMonthAttendanceCount = computed(() => {
    const now = new Date()
    return attendances.value.filter(attendance => {
      if (attendance.type === 2) return false
      const date = new Date(attendance.date)
      return date.getFullYear() === now.getFullYear() && date.getMonth() === now.getMonth()
    }).length
  })

  const totalAddedSessions = computed(() =>
    payments.value.reduce((total, payment) => total + payment.addedSessions, 0)
  )

  const totalPayments = computed(() =>
    payments.value.reduce((total, payment) => total + payment.amount, 0)
  )

  const activities = computed<HistoryActivity[]>(() => [
    ...attendances.value.map(attendance => ({
      key: `attendance-${attendance.id}`,
      type: 'attendance' as const,
      date: attendance.date,
      balanceAfter: attendance.balanceAfter,
      attendanceType: attendance.type ?? 0,
      note: attendance.note ?? null
    })),
    ...payments.value.map(payment => ({
      key: `payment-${payment.id}`,
      type: 'payment' as const,
      date: payment.date,
      addedSessions: payment.addedSessions,
      amount: payment.amount
    }))
  ].sort((firstActivity, secondActivity) =>
    new Date(secondActivity.date).getTime() - new Date(firstActivity.date).getTime()
  ))

  const fetchHistory = async (studentId: number) => {
    isLoading.value = true
    error.value = null
    try {
      const data = await historyService.getStudentHistory(studentId)
      attendances.value = data.attendances || []
      payments.value = data.payments || []
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Geçmiş işlemler yüklenirken bir hata oluştu.'
      console.error(err)
    } finally {
      isLoading.value = false
    }
  }

  return {
    attendances,
    payments,
    currentMonthAttendanceCount,
    totalAddedSessions,
    totalPayments,
    activities,
    isLoading,
    error,
    fetchHistory
  }
})
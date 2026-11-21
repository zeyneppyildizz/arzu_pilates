<template>
  <div class="max-w-6xl mx-auto p-4 md:p-8 transition-colors duration-300">
    <PageHeader eyebrow="EĞİTMEN PANELİ" title="Kayıtlı Öğrenciler" :subtitle="`${students.length} öğrenci kayıtlı`">
      <template #actions>
        <i v-if="isLoading" class="pi pi-spin pi-spinner text-[#718161] text-xl"></i>
      </template>
    </PageHeader>

    <div class="bg-white rounded-3xl p-5 md:p-8 shadow-sm border border-gray-100 transition-all duration-300">
      <div class="mb-5 grid gap-3 md:grid-cols-[minmax(0,1fr)_220px]">
        <div class="relative">
          <i class="pi pi-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"></i>
          <InputText v-model="searchQuery" aria-label="Öğrencilerde ara"
            placeholder="İsim veya kullanıcı adına göre ara" class="studio-input !pl-10" />
        </div>
        <select v-model="balanceFilter" aria-label="Bakiyeye göre filtrele" class="studio-input text-sm">
          <option value="all">Tüm bakiyeler</option>
          <option value="low">Dersi az (0-2)</option>
          <option value="zero">Ders hakkı sıfır</option>
          <option value="negative">Bakiyesi ekside</option>
          <option value="available">3 veya daha fazla ders</option>
        </select>
      </div>

      <p class="mb-3 px-1 text-xs font-medium text-gray-500">
        {{ filteredStudents.length }} öğrenci gösteriliyor
      </p>

      <div v-if="pageStudents.length" class="space-y-3">
        <article v-for="student in pageStudents" :key="student.id"
          class="rounded-2xl border border-gray-100 bg-white p-4 transition-all hover:border-[#dce3d4] hover:shadow-sm">
          <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            <div class="flex min-w-0 items-center gap-3">
              <span
                class="flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-[#f4f7f0] text-base font-bold text-[#2d3627]">
                {{ student.firstName.charAt(0) }}{{ student.lastName.charAt(0) }}
              </span>
              <div class="min-w-0">
                <div class="cursor-pointer select-none truncate font-bold text-[#2d3627]" data-delete-trigger
                  @dblclick="showDeleteButton(student.id)" @touchstart="startDeleteButtonLongPress(student.id)"
                  @touchend="cancelDeleteButtonLongPress" @touchmove="cancelDeleteButtonLongPress"
                  @touchcancel="cancelDeleteButtonLongPress" @contextmenu.prevent>
                  {{ student.firstName }} {{ student.lastName }}
                </div>
                <p class="mt-0.5 truncate text-sm text-gray-500">@{{ student.username }}</p>
              </div>
            </div>

            <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-end">
              <BalanceBadge :balance="student.balance" :has-any-payment="student.hasAnyPayment"
                class="self-start sm:self-auto" />

              <div class="flex flex-wrap gap-2 sm:justify-end">
                <Button label="Paket Yükle" size="small" icon="pi pi-plus-circle"
                  class="!bg-[#f4f7f0] hover:!bg-[#e5ebe0] !border !border-[#dce3d4] !text-[#2d3627] !rounded-xl transition-all shadow-sm font-semibold"
                  @click="openPaymentModal(student)" />

                <Button
                  v-if="activeDeleteStudentId === student.id"
                  label="Ders Düş"
                  icon="pi pi-minus-circle"
                  size="small"
                  class="!bg-[#f3f4ef] hover:!bg-[#e5e8df] !border !border-[#cfd8c5] !text-[#2d3627] !rounded-xl transition-all shadow-sm font-semibold"
                  @click="openManualDeductModal(student)" />

                <Button v-if="activeDeleteStudentId === student.id || resettingStudentId === student.id"
                  label="Şifre Sıfırla" icon="pi pi-key" size="small" :loading="resettingStudentId === student.id"
                  class="!bg-amber-50 hover:!bg-amber-100 !border !border-amber-200 !text-amber-800 !rounded-xl transition-all shadow-sm font-semibold"
                  @click="confirmResetPassword(student)" />

                <Button v-if="activeDeleteStudentId === student.id" label="Öğrenciyi Kaldır" icon="pi pi-trash"
                  size="small"
                  class="!bg-red-50 hover:!bg-red-100 !border !border-red-200 !text-red-700 !rounded-xl transition-all shadow-sm font-semibold"
                  @click="confirmDeleteStudent(student.id, student.firstName)" />
              </div>
            </div>
          </div>
        </article>
      </div>

      <div v-else class="rounded-2xl bg-[#f8f9f5] px-4 py-8 text-center text-sm font-medium text-gray-500">
        {{ students.length ? 'Arama veya filtreye uygun öğrenci bulunamadı.' : 'Henüz kayıtlı öğrenci bulunmuyor.' }}
      </div>

      <Paginator v-if="filteredStudents.length > rows" v-model:first="first" :rows="rows"
        :totalRecords="filteredStudents.length" class="!bg-transparent mt-5" />
    </div>

    <StudentPaymentDialog v-model:visible="showPaymentModal" :student="selectedStudent"
      @payment-succeeded="alert.showSuccess('Paket başarıyla yüklendi.')" />


    <Dialog v-model:visible="showResetResultModal" modal dismissableMask header="Geçici Şifre Oluşturuldu"
      :style="{ width: '26rem', maxWidth: '92vw' }">
      <div v-if="resetResult" class="space-y-4">
        <p class="text-sm text-[#4a5542]">
          <strong class="text-[#2d3627]">{{ resetResult.studentName }}</strong> (@{{ resetResult.username }}) için tek
          seferlik geçici şifre oluşturuldu. Öğrenci bu şifreyle giriş yaptığında kendi yeni şifresini belirleyecektir.
        </p>

        <div class="flex items-center justify-between rounded-2xl border border-[#dce3d4] bg-[#f4f7f0] px-4 py-3">
          <div>
            <p class="text-[11px] font-semibold uppercase tracking-wide text-[#718161]">Geçici Şifre</p>
            <p class="text-lg font-bold tracking-wider text-[#2d3627] select-all">{{ resetResult.tempPassword }}</p>
          </div>
          <Button icon="pi pi-copy" size="small" text rounded aria-label="Şifreyi kopyala" class="!text-[#2d3627]"
            @click="copyTempPassword" />
        </div>
      </div>
    </Dialog>
    <Dialog
      v-model:visible="showManualDeductModal"
      modal
      dismissableMask
      header="Manuel Ders Düş"
      :style="{ width: '28rem', maxWidth: '92vw' }">
      <div v-if="deductStudent" class="space-y-4">
        <div class="rounded-2xl border border-[#dce3d4] bg-[#f4f7f0] px-4 py-3 text-sm text-[#4a5542]">
          <p class="font-bold text-[#2d3627]">{{ deductStudent.firstName }} {{ deductStudent.lastName }}</p>
          <p class="mt-0.5 text-xs">
            Mevcut bakiye: <strong>{{ deductStudent.balance }} ders</strong> → İşlem sonrası: <strong>{{ deductStudent.balance - 1 }} ders</strong>
          </p>
        </div>

        <div class="space-y-2">
          <p class="text-xs font-semibold text-[#4a5542]">İşlem Sebebi</p>
          <div class="grid grid-cols-1 gap-2">
            <button
              type="button"
              class="flex items-start gap-3 rounded-2xl border p-3 text-left transition-all"
              :class="deductForm.type === 2
                ? 'border-[#718161] bg-[#f4f7f0] text-[#2d3627]'
                : 'border-gray-200 bg-white text-[#4a5542] hover:border-[#dce3d4]'"
              @click="deductForm.type = 2">
              <i class="pi pi-clock mt-0.5 text-base" :class="deductForm.type === 2 ? 'text-[#718161]' : 'text-gray-400'"></i>
              <div>
                <p class="text-sm font-bold">Son Dakika İptali</p>
                <p class="text-xs text-gray-500">Geç iptal sebebiyle 1 ders hakkı düşülür.</p>
              </div>
            </button>

            <button
              type="button"
              class="flex items-start gap-3 rounded-2xl border p-3 text-left transition-all"
              :class="deductForm.type === 1
                ? 'border-[#718161] bg-[#f4f7f0] text-[#2d3627]'
                : 'border-gray-200 bg-white text-[#4a5542] hover:border-[#dce3d4]'"
              @click="deductForm.type = 1">
              <i class="pi pi-check-circle mt-0.5 text-base" :class="deductForm.type === 1 ? 'text-[#718161]' : 'text-gray-400'"></i>
              <div>
                <p class="text-sm font-bold">QR Okutulmadı (Manuel Yoklama)</p>
                <p class="text-xs text-gray-500">Öğrenci derse katıldı, yoklama manuel işlenir.</p>
              </div>
            </button>
          </div>
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="deductNote" class="text-xs font-semibold text-[#4a5542]">
            Kısa Not <span class="font-normal text-gray-400">(isteğe bağlı)</span>
          </label>
          <InputText
            id="deductNote"
            v-model="deductForm.note"
            maxlength="200"
            class="studio-input"
            placeholder="Örn: Derse 2 saat kala iptal edildi"
          />
        </div>

        <div class="flex justify-end gap-2 pt-1">
          <Button
            label="Vazgeç"
            size="small"
            text
            class="!text-[#4a5542]"
            @click="showManualDeductModal = false" />
          <Button
            label="1 Ders Düş"
            icon="pi pi-minus-circle"
            size="small"
            :loading="isDeducting"
            class="!bg-[#718161] hover:!bg-[#5c6a4e] !border-none !text-white !rounded-xl px-4 font-semibold"
            @click="submitManualDeduct" />
        </div>
      </div>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted, onUnmounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useStudentStore } from '../stores/student.store'
import { useAttendanceStore } from '../stores/attendance.store'
import { useAuthStore } from '../stores/auth.store'
import { useAlert } from '../composables/useAlert'
import type { StudentDto } from '../types/student.type'

import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Paginator from 'primevue/paginator'
import PageHeader from '../components/PageHeader.vue'
import BalanceBadge from '../components/BalanceBadge.vue'
import StudentPaymentDialog from '../components/StudentPaymentDialog.vue'

import Dialog from 'primevue/dialog'
import type { ResetPasswordResponse } from '../types/auth.type'

const studentStore = useStudentStore()
const alert = useAlert()
const authStore = useAuthStore()
const attendanceStore = useAttendanceStore()

const { students, isLoading } = storeToRefs(studentStore)

const searchQuery = ref('')
const balanceFilter = ref<'all' | 'low' | 'zero' | 'negative' | 'available'>('all')
const first = ref(0)
const rows = 10
const showPaymentModal = ref(false)
const selectedStudent = ref<StudentDto | null>(null)
const activeDeleteStudentId = ref<number | null>(null)
let deleteButtonLongPressTimer: ReturnType<typeof setTimeout> | null = null
const resettingStudentId = ref<number | null>(null)
const showResetResultModal = ref(false)
const resetResult = ref<ResetPasswordResponse | null>(null)
const showManualDeductModal = ref(false)
const deductStudent = ref<StudentDto | null>(null)
const isDeducting = ref(false)
const deductForm = ref<{ type: 1 | 2; note: string }>({
  type: 2,
  note: ''
})
let ignoreLongPressClick = false

const filteredStudents = computed(() => {
  const query = searchQuery.value.trim().toLocaleLowerCase('tr-TR')

  return students.value.filter(student => {
    const matchesSearch = !query ||
      `${student.firstName} ${student.lastName} ${student.username}`
        .toLocaleLowerCase('tr-TR')
        .includes(query)

    const matchesBalance = balanceFilter.value === 'all' ||
      (balanceFilter.value === 'low' && student.balance >= 0 && student.balance <= 2) ||
      (balanceFilter.value === 'zero' && student.balance === 0) ||
      (balanceFilter.value === 'negative' && student.balance < 0) ||
      (balanceFilter.value === 'available' && student.balance >= 3)

    return matchesSearch && matchesBalance
  })
})

const pageStudents = computed(() =>
  filteredStudents.value.slice(first.value, first.value + rows)
)

watch([searchQuery, balanceFilter], () => {
  first.value = 0
})

watch(filteredStudents, filtered => {
  if (first.value >= filtered.length && first.value > 0) {
    first.value = Math.max(0, Math.floor((filtered.length - 1) / rows) * rows)
  }
})

const showDeleteButton = (studentId: number) => {
  activeDeleteStudentId.value = studentId
}

const startDeleteButtonLongPress = (studentId: number) => {
  cancelDeleteButtonLongPress()
  deleteButtonLongPressTimer = setTimeout(() => {
    showDeleteButton(studentId)
    ignoreLongPressClick = true
    deleteButtonLongPressTimer = null
  }, 550)
}

const cancelDeleteButtonLongPress = () => {
  if (deleteButtonLongPressTimer) {
    clearTimeout(deleteButtonLongPressTimer)
    deleteButtonLongPressTimer = null
  }
}

const dismissDeleteButtonOnClick = (event: MouseEvent) => {
  const target = event.target
  if (ignoreLongPressClick && target instanceof Element && target.closest('[data-delete-trigger]')) {
    ignoreLongPressClick = false
    return
  }

  ignoreLongPressClick = false
  activeDeleteStudentId.value = null
}

const confirmDeleteStudent = (studentId: number, studentName: string) => {
  activeDeleteStudentId.value = null
  alert.requireDangerConfirm(
    `${studentName} adlı öğrenciyi silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`,
    async () => {
      await studentStore.deleteStudent(studentId)
      if (studentStore.error) {
        alert.showError(studentStore.error)
        return
      }
      alert.showSuccess('Öğrenci başarıyla silindi.')
    }
  )
}

const openPaymentModal = (student: StudentDto) => {
  selectedStudent.value = student
  showPaymentModal.value = true
}

const confirmResetPassword = (student: StudentDto) => {
  activeDeleteStudentId.value = null
  alert.requireDangerConfirm(
    `${student.firstName} ${student.lastName} adlı öğrencinin mevcut şifresi sıfırlanıp geçici şifre oluşturulacak. Devam etmek istiyor musunuz?`,
    async () => {
      resettingStudentId.value = student.id
      try {
        resetResult.value = await authStore.resetStudentPassword(student.id)
        showResetResultModal.value = true
      } catch (err: any) {
        alert.showError(err.response?.data?.message || 'Şifre sıfırlanırken bir hata oluştu.')
      } finally {
        resettingStudentId.value = null
      }
    }
  )
}

const copyTempPassword = async () => {
  if (!resetResult.value) return
  try {
    await navigator.clipboard.writeText(resetResult.value.tempPassword)
    alert.showSuccess('Geçici şifre kopyalandı.')
  } catch {
    alert.showWarn('Otomatik kopyalanamadı, şifreyi seçip manuel kopyalayabilirsiniz.')
  }
}

const openManualDeductModal = (student: StudentDto) => {
  activeDeleteStudentId.value = null
  deductStudent.value = student
  deductForm.value = { type: 2, note: '' }
  showManualDeductModal.value = true
}

const submitManualDeduct = async () => {
  if (!deductStudent.value) return

  isDeducting.value = true
  try {
    const res = await attendanceStore.manualDeduct({
      studentId: deductStudent.value.id,
      type: deductForm.value.type,
      note: deductForm.value.note.trim() || undefined
    })

    if (!res) {
      alert.showError(attendanceStore.error || 'Ders düşülürken bir hata oluştu.')
      return
    }

    deductStudent.value.balance = res.newBalance
    showManualDeductModal.value = false
    alert.showSuccess(res.message)
  } finally {
    isDeducting.value = false
  }
}

onMounted(async () => {
  document.addEventListener('click', dismissDeleteButtonOnClick)
  await studentStore.fetchStudents()
})

onUnmounted(() => {
  document.removeEventListener('click', dismissDeleteButtonOnClick)
  cancelDeleteButtonLongPress()
})
</script>
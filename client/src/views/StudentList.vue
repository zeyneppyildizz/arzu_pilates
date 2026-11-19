<template>
  <div class="max-w-6xl mx-auto p-4 md:p-8 transition-colors duration-300">
    <PageHeader
      eyebrow="EĞİTMEN PANELİ"
      title="Kayıtlı Öğrenciler"
      :subtitle="`${students.length} öğrenci kayıtlı`">
      <template #actions>
        <i v-if="isLoading" class="pi pi-spin pi-spinner text-[#718161] text-xl"></i>
      </template>
    </PageHeader>

    <div class="bg-white rounded-3xl p-5 md:p-8 shadow-sm border border-gray-100 transition-all duration-300">
      <div class="mb-5 grid gap-3 md:grid-cols-[minmax(0,1fr)_220px]">
        <div class="relative">
          <i class="pi pi-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"></i>
          <InputText v-model="searchQuery" aria-label="Öğrencilerde ara"
            placeholder="İsim veya kullanıcı adına göre ara"
            class="studio-input !pl-10" />
        </div>
        <select v-model="balanceFilter" aria-label="Bakiyeye göre filtrele"
          class="studio-input text-sm">
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
              <span class="flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-[#f4f7f0] text-base font-bold text-[#2d3627]">
                {{ student.firstName.charAt(0) }}{{ student.lastName.charAt(0) }}
              </span>
              <div class="min-w-0">
                <div
                  class="cursor-pointer select-none truncate font-bold text-[#2d3627]"
                  data-delete-trigger
                  title="Silme butonunu göstermek için çift tıklayın veya basılı tutun"
                  @dblclick="showDeleteButton(student.id)"
                  @touchstart="startDeleteButtonLongPress(student.id)"
                  @touchend="cancelDeleteButtonLongPress"
                  @touchmove="cancelDeleteButtonLongPress"
                  @touchcancel="cancelDeleteButtonLongPress"
                  @contextmenu.prevent>
                  {{ student.firstName }} {{ student.lastName }}
                </div>
                <p class="mt-0.5 truncate text-sm text-gray-500">@{{ student.username }}</p>
              </div>
            </div>

            <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-end">
              <BalanceBadge :balance="student.balance" :has-any-payment="student.hasAnyPayment" class="self-start sm:self-auto" />

              <div class="flex flex-wrap gap-2 sm:justify-end">
                <Button label="Paket Yükle" size="small" icon="pi pi-plus-circle"
                  class="!bg-[#f4f7f0] hover:!bg-[#e5ebe0] !border !border-[#dce3d4] !text-[#2d3627] !rounded-xl transition-all shadow-sm font-semibold"
                  @click="openPaymentModal(student)" />

                <Button v-if="activeDeleteStudentId === student.id"
                  label="Öğrenciyi Kaldır" icon="pi pi-trash" size="small"
                  class="!bg-red-50 hover:!bg-red-100 !border !border-red-200 !text-red-700 transition-all shadow-sm font-semibold"
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
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted, onUnmounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useStudentStore } from '../stores/student.store'
import { useAlert } from '../composables/useAlert'
import type { StudentDto } from '../types/student.type'

import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Paginator from 'primevue/paginator'
import PageHeader from '../components/PageHeader.vue'
import BalanceBadge from '../components/BalanceBadge.vue'
import StudentPaymentDialog from '../components/StudentPaymentDialog.vue'

const studentStore = useStudentStore()
const alert = useAlert()

const { students, isLoading } = storeToRefs(studentStore)

const searchQuery = ref('')
const balanceFilter = ref<'all' | 'low' | 'zero' | 'negative' | 'available'>('all')
const first = ref(0)
const rows = 10
const showPaymentModal = ref(false)
const selectedStudent = ref<StudentDto | null>(null)
const activeDeleteStudentId = ref<number | null>(null)
let deleteButtonLongPressTimer: ReturnType<typeof setTimeout> | null = null
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

onMounted(async () => {
  document.addEventListener('click', dismissDeleteButtonOnClick)
  await studentStore.fetchStudents()
})

onUnmounted(() => {
  document.removeEventListener('click', dismissDeleteButtonOnClick)
  cancelDeleteButtonLongPress()
})
</script>
<template>
  <div class="max-w-7xl mx-auto p-4 md:p-8 transition-colors duration-300">
    <PageHeader
      eyebrow="EĞİTMEN PANELİ"
      title="Ders Programı"
      subtitle="Haftalık grup saatlerini yönetin.">
      <template #actions>
        <Button
          label="Saat Ekle"
          icon="pi pi-plus"
          class="!bg-[#718161] hover:!bg-[#5c6a4e] !border-none !text-white !rounded-xl !px-4 !py-2.5 shadow-sm text-sm font-semibold whitespace-nowrap"
          @click="openCreateModal()"
        />
      </template>
    </PageHeader>

    <section class="mb-6 rounded-3xl border border-gray-100 bg-white p-4 shadow-sm md:p-5">
      <div class="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div class="relative flex-1">
          <i class="pi pi-search pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-sm text-[#718161]"></i>
          <InputText
            v-model="studentQuery"
            type="text"
            placeholder="Öğrenci ara (ad, soyad veya kullanıcı adı)..."
            class="studio-input !pl-10 !pr-10 w-full"
          />
          <button
            v-if="studentQuery"
            type="button"
            aria-label="Aramayı temizle"
            class="absolute right-2.5 top-1/2 flex h-7 w-7 -translate-y-1/2 items-center justify-center rounded-lg text-gray-400 hover:bg-gray-100 hover:text-[#2d3627]"
            @click="studentQuery = ''">
            <i class="pi pi-times text-xs"></i>
          </button>
        </div>

        <p class="text-xs text-gray-500 md:text-right">
          Öğrenciyi bir ders kartına <strong>sürükleyin</strong> veya seçip karta dokunun.
        </p>
      </div>

      <div
        v-if="selectedStudent"
        class="mt-3 flex items-center justify-between gap-2 rounded-2xl border border-[#718161] bg-[#f4f7f0] px-3.5 py-2 text-xs text-[#2d3627]">
        <span>
          <strong>{{ selectedStudent.firstName }} {{ selectedStudent.lastName }}</strong> seçili — eklemek istediğiniz ders kartına dokunun.
        </span>
        <button
          type="button"
          class="font-bold text-[#718161] hover:text-[#2d3627]"
          @click="selectedStudent = null">
          İptal
        </button>
      </div>

      <div v-if="studentQuery.trim()" class="mt-3">
        <div v-if="filteredStudents.length" class="flex flex-wrap gap-2">
          <div
            v-for="student in filteredStudents"
            :key="student.id"
            class="flex cursor-grab touch-none select-none items-center gap-2 rounded-xl border px-3 py-2 text-xs font-semibold transition-all active:cursor-grabbing"
            :class="[
              isDragging && draggingStudent?.id === student.id ? 'opacity-40' : '',
              selectedStudent?.id === student.id
                ? 'border-[#2d3627] bg-[#2d3627] text-[#f3f4ef]'
                : 'border-[#dce3d4] bg-[#f8f9f5] text-[#2d3627] hover:border-[#718161] hover:bg-[#f4f7f0]'
            ]"
            @pointerdown="onStudentPointerDown($event, student)">
            <i class="pi pi-bars text-[10px] opacity-60"></i>
            <span>{{ student.firstName }} {{ student.lastName }}</span>
            <span
              class="rounded-full px-1.5 py-0.5 text-[10px] font-bold"
              :class="selectedStudent?.id === student.id
                ? 'bg-white/20 text-white'
                : student.balance <= 0
                  ? 'bg-red-50 text-red-600'
                  : 'bg-white text-[#4a5542]'">
              {{ student.balance }} ders
            </span>
          </div>
        </div>
        <p v-else class="py-2 text-xs font-medium text-gray-400">
          "{{ studentQuery }}" ile eşleşen öğrenci bulunamadı.
        </p>
      </div>
    </section>

    <div class="mb-6 flex gap-2 overflow-x-auto pb-1">
      <button
        type="button"
        class="shrink-0 rounded-xl border px-3.5 py-2 text-xs font-bold transition-colors"
        :class="selectedDay === 0
          ? 'border-[#2d3627] bg-[#2d3627] text-[#f3f4ef]'
          : 'border-gray-200 bg-white text-[#4a5542] hover:border-[#718161]'"
        @click="selectedDay = 0">
        Tüm Hafta ({{ slots.length }})
      </button>

      <button
        v-for="day in DAYS"
        :key="day.value"
        type="button"
        class="shrink-0 rounded-xl border px-3.5 py-2 text-xs font-bold transition-colors"
        :class="selectedDay === day.value
          ? 'border-[#2d3627] bg-[#2d3627] text-[#f3f4ef]'
          : 'border-gray-200 bg-white text-[#4a5542] hover:border-[#718161]'"
        @click="selectedDay = day.value">
        {{ day.short }}
        <span
          v-if="slotCountByDay(day.value) > 0"
          class="ml-1 opacity-75">
          ({{ slotCountByDay(day.value) }})
        </span>
      </button>
    </div>

    <div v-if="isLoading" class="flex justify-center py-16">
      <i class="pi pi-spin pi-spinner text-4xl text-[#718161]"></i>
    </div>

    <div v-else-if="visibleDayGroups.length" class="flex flex-col gap-6">
      <section v-for="group in visibleDayGroups" :key="group.day.value">
        <div class="mb-3 flex items-center justify-between">
          <div class="flex items-center gap-2">
            <h2 class="text-base font-bold text-[#2d3627]">{{ group.day.label }}</h2>
            <span class="rounded-full bg-[#f4f7f0] px-2.5 py-0.5 text-xs font-semibold text-[#4a5542]">
              {{ group.slots.length }} ders
            </span>
          </div>
          <button
            type="button"
            class="text-xs font-semibold text-[#718161] hover:text-[#2d3627]"
            @click="openCreateModal(group.day.value)">
            + Saat ekle
          </button>
        </div>

        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <article
            v-for="slot in group.slots"
            :key="slot.id"
            :data-slot-id="slot.id"
            class="flex flex-col justify-between rounded-3xl border bg-white p-5 shadow-sm transition-all"
            :class="getSlotCardClass(slot)"
            @click="handleSlotTap(slot)">

            <div>
              <div class="mb-4 flex items-center justify-between gap-2">
                <div class="flex items-center gap-2.5">
                  <span class="flex h-10 w-10 items-center justify-center rounded-2xl bg-[#f4f7f0] text-[#2d3627]">
                    <i class="pi pi-clock text-sm"></i>
                  </span>
                  <div>
                    <p class="text-lg font-extrabold tracking-tight text-[#2d3627]">{{ slot.startTime }}</p>
                    <p class="text-[11px] font-medium text-gray-500">{{ group.day.label }}</p>
                  </div>
                </div>

                <div class="flex items-center gap-2">
                  <span
                    class="rounded-full border px-2.5 py-1 text-xs font-bold"
                    :class="slot.students.length >= slot.capacity
                      ? 'border-amber-200 bg-amber-50 text-amber-800'
                      : 'border-[#dce3d4] bg-[#f4f7f0] text-[#2d3627]'">
                    {{ slot.students.length }}/{{ slot.capacity }}
                  </span>
                  <button
                    type="button"
                    title="Ders saatini sil"
                    class="flex h-8 w-8 items-center justify-center rounded-xl text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600"
                    @click.stop="confirmDeleteSlot(slot)">
                    <i class="pi pi-trash text-xs"></i>
                  </button>
                </div>
              </div>

              <ul v-if="slot.students.length" class="space-y-2">
                <li
                  v-for="(student, index) in slot.students"
                  :key="student.id"
                  class="flex items-center justify-between gap-2 rounded-2xl border border-gray-100 bg-[#fdfdfb] px-3 py-2">
                  <div class="flex min-w-0 items-center gap-2.5">
                    <span class="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-[#eef2e9] text-[11px] font-bold text-[#4a5542]">
                      {{ index + 1 }}
                    </span>
                    <div class="min-w-0">
                      <p class="truncate text-sm font-semibold text-[#2d3627]">
                        {{ student.firstName }} {{ student.lastName }}
                      </p>
                      <p
                        class="text-[11px]"
                        :class="student.balance <= 0 ? 'font-semibold text-red-600' : 'text-gray-400'">
                        {{ student.balance }} ders hakkı
                      </p>
                    </div>
                  </div>

                  <button
                    type="button"
                    title="Öğrenciyi bu saatten çıkar"
                    class="flex h-7 w-7 shrink-0 items-center justify-center rounded-lg text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600"
                    @click.stop="handleRemoveStudent(slot.id, student.id, `${student.firstName} ${student.lastName}`)">
                    <i class="pi pi-times text-xs"></i>
                  </button>
                </li>
              </ul>

              <div
                v-else
                class="rounded-2xl border border-dashed border-[#dce3d4] bg-[#f8f9f5] px-3 py-5 text-center text-xs text-gray-500">
                Henüz öğrenci eklenmedi
              </div>
            </div>

            <div class="mt-4">
              <div
                v-if="slot.students.length < slot.capacity"
                class="rounded-xl border border-dashed px-3 py-2 text-center text-[11px] font-semibold transition-colors"
                :class="dragOverSlotId === slot.id || selectedStudent
                  ? 'border-[#718161] bg-[#f4f7f0] text-[#2d3627]'
                  : 'border-gray-200 text-gray-400'">
                <template v-if="selectedStudent">
                  + {{ selectedStudent.firstName }} eklemek için dokunun
                </template>
                <template v-else>
                  Öğrenciyi buraya sürükleyin ({{ slot.capacity - slot.students.length }} boş yer)
                </template>
              </div>
              <div
                v-else
                class="rounded-xl bg-amber-50/80 px-3 py-2 text-center text-[11px] font-semibold text-amber-800">
                Kontenjan dolu (4/4)
              </div>
            </div>
          </article>
        </div>
      </section>
    </div>

    <div v-else class="rounded-3xl border border-gray-100 bg-white px-6 py-12 text-center shadow-sm">
      <div class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-2xl bg-[#f4f7f0] text-[#718161]">
        <i class="pi pi-calendar text-xl"></i>
      </div>
      <p class="font-bold text-[#2d3627]">
        {{ selectedDay === 0 ? 'Henüz ders saati eklenmemiş' : `${getDayLabel(selectedDay)} için ders saati yok` }}
      </p>
      <p class="mt-1 text-xs text-gray-500">
        Yeni bir gün ve saat kartı oluşturup öğrencilerinizi gruplayabilirsiniz.
      </p>
      <Button
        label="Ders Saati Ekle"
        icon="pi pi-plus"
        class="mt-4 !bg-[#718161] hover:!bg-[#5c6a4e] !border-none !text-white !rounded-xl !px-4 !py-2 text-sm font-semibold"
        @click="openCreateModal(selectedDay || 1)"
      />
    </div>

    <Dialog
      v-model:visible="showCreateModal"
      modal
      dismissableMask
      header="Yeni Ders Saati Ekle"
      :style="{ width: '26rem', maxWidth: '92vw' }">
      <form class="space-y-4" @submit.prevent="submitCreateSlot">
        <div>
          <p class="mb-2 text-xs font-semibold text-[#4a5542]">Gün</p>
          <div class="grid grid-cols-4 gap-2">
            <button
              v-for="day in DAYS"
              :key="day.value"
              type="button"
              class="rounded-xl border px-2 py-2 text-xs font-bold transition-colors"
              :class="createForm.dayOfWeek === day.value
                ? 'border-[#2d3627] bg-[#2d3627] text-[#f3f4ef]'
                : 'border-gray-200 bg-white text-[#4a5542] hover:border-[#718161]'"
              @click="createForm.dayOfWeek = day.value">
              {{ day.short }}
            </button>
          </div>
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="slotStartTime" class="text-xs font-semibold text-[#4a5542]">Ders Saati</label>
          <input
            id="slotStartTime"
            v-model="createForm.startTime"
            type="time"
            required
            class="studio-input"
          />
        </div>

        <div class="flex justify-end gap-2 pt-1">
          <Button
            type="button"
            label="Vazgeç"
            size="small"
            text
            class="!text-[#4a5542]"
            @click="showCreateModal = false"
          />
          <Button
            type="submit"
            label="Oluştur"
            icon="pi pi-check"
            size="small"
            :loading="isSubmittingSlot"
            class="!bg-[#718161] hover:!bg-[#5c6a4e] !border-none !text-white !rounded-xl px-4 font-semibold"
          />
        </div>
      </form>
    </Dialog>

    <Dialog
      v-model:visible="showDeleteModal"
      modal
      dismissableMask
      header="Ders Saatini Sil"
      :style="{ width: '24rem', maxWidth: '92vw' }">
      <div v-if="slotToDelete" class="space-y-4">
        <p class="text-sm text-[#4a5542]">
          <strong>{{ getDayLabel(slotToDelete.dayOfWeek) }} {{ slotToDelete.startTime }}</strong> ders kartını silmek istediğinize emin misiniz?
          <span v-if="slotToDelete.students.length" class="mt-1 block text-xs text-gray-500">
            İçindeki {{ slotToDelete.students.length }} öğrencinin bu saatteki kaydı kaldırılacak (ders hakları etkilenmez).
          </span>
        </p>

        <div class="flex justify-end gap-2">
          <Button
            label="Vazgeç"
            size="small"
            text
            class="!text-[#4a5542]"
            @click="showDeleteModal = false"
          />
          <Button
            label="Sil"
            icon="pi pi-trash"
            size="small"
            :loading="isDeletingSlot"
            class="!bg-red-600 hover:!bg-red-700 !border-none !text-white !rounded-xl px-4 font-semibold"
            @click="executeDeleteSlot"
          />
        </div>
      </div>
    </Dialog>

    <Teleport to="body">
      <div
        v-if="isDragging && draggingStudent"
        class="pointer-events-none fixed flex items-center gap-2 rounded-xl border border-[#2d3627] bg-[#2d3627] px-3.5 py-2 text-xs font-semibold text-[#f3f4ef] shadow-xl"
        :style="dragGhostStyle">
        <i class="pi pi-arrows-alt text-[10px] opacity-80"></i>
        <span>{{ draggingStudent.firstName }} {{ draggingStudent.lastName }}</span>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useScheduleStore } from '../stores/schedule.store'
import { useStudentStore } from '../stores/student.store'
import { useAlert } from '../composables/useAlert'
import type { ScheduleSlotDto } from '../types/schedule.type'
import type { StudentDto } from '../types/student.type'

import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import PageHeader from '../components/PageHeader.vue'

const DAYS = [
  { value: 1, label: 'Pazartesi', short: 'Pzt' },
  { value: 2, label: 'Salı', short: 'Sal' },
  { value: 3, label: 'Çarşamba', short: 'Çar' },
  { value: 4, label: 'Perşembe', short: 'Per' },
  { value: 5, label: 'Cuma', short: 'Cum' },
  { value: 6, label: 'Cumartesi', short: 'Cmt' },
  { value: 7, label: 'Pazar', short: 'Paz' }
] as const

const scheduleStore = useScheduleStore()
const studentStore = useStudentStore()
const alert = useAlert()

const { slots, isLoading } = storeToRefs(scheduleStore)
const { students } = storeToRefs(studentStore)

const studentQuery = ref('')
const selectedDay = ref<number>(0) // 0 = Tüm Hafta
const draggingStudent = ref<StudentDto | null>(null)
const isDragging = ref(false)
const dragPos = ref({ x: 0, y: 0 })
const isTouchDrag = ref(false)
const dragOverSlotId = ref<number | null>(null)
const selectedStudent = ref<StudentDto | null>(null)

let dragStart = { x: 0, y: 0 }
let scrollRafId: number | null = null

const showCreateModal = ref(false)
const isSubmittingSlot = ref(false)
const createForm = ref({
  dayOfWeek: 1,
  startTime: '08:00'
})

const showDeleteModal = ref(false)
const isDeletingSlot = ref(false)
const slotToDelete = ref<ScheduleSlotDto | null>(null)

const dragGhostStyle = computed(() => ({
  left: `${dragPos.value.x}px`,
  top: `${dragPos.value.y}px`,
  zIndex: 9999,
  transform: isTouchDrag.value
    ? 'translate(-50%, -140%)'
    : 'translate(-50%, -50%)'
}))

const filteredStudents = computed(() => {
  const q = studentQuery.value.trim().toLocaleLowerCase('tr-TR')
  if (!q) return []

  return students.value
    .filter(student => {
      const fullName = `${student.firstName} ${student.lastName}`.toLocaleLowerCase('tr-TR')
      const username = (student.username ?? '').toLocaleLowerCase('tr-TR')
      return fullName.includes(q) || username.includes(q)
    })
    .slice(0, 12)
})

const visibleDayGroups = computed(() => {
  const daysToShow = selectedDay.value === 0
    ? DAYS
    : DAYS.filter(d => d.value === selectedDay.value)

  return daysToShow
    .map(day => ({
      day,
      slots: slots.value.filter(s => s.dayOfWeek === day.value)
    }))
    .filter(group => group.slots.length > 0)
})

const getDayLabel = (dayValue: number) =>
  DAYS.find(d => d.value === dayValue)?.label ?? ''

const slotCountByDay = (dayValue: number) =>
  slots.value.filter(s => s.dayOfWeek === dayValue).length

const getSlotCardClass = (slot: ScheduleSlotDto) => {
  const isFull = slot.students.length >= slot.capacity

  if (dragOverSlotId.value === slot.id) {
    return isFull
      ? 'border-red-400 bg-red-50/40 ring-2 ring-red-200'
      : 'border-[#718161] bg-[#f4f7f0]/60 ring-2 ring-[#718161]/30'
  }

  if (selectedStudent.value && !isFull) {
    return 'border-[#718161]/60 hover:border-[#2d3627] cursor-pointer'
  }

  return 'border-gray-100'
}

const updateHoveredSlot = (x: number, y: number) => {
  const el = document.elementFromPoint(x, y)
  const slotEl = el?.closest<HTMLElement>('[data-slot-id]')
  dragOverSlotId.value = slotEl ? Number(slotEl.dataset.slotId) : null
}

const stopAutoScroll = () => {
  if (scrollRafId !== null) {
    cancelAnimationFrame(scrollRafId)
    scrollRafId = null
  }
}

const runAutoScroll = () => {
  if (!isDragging.value) {
    stopAutoScroll()
    return
  }

  const edgeSize = 85
  const y = dragPos.value.y

  if (y < edgeSize) {
    window.scrollBy(0, -10)
    updateHoveredSlot(dragPos.value.x, dragPos.value.y)
  } else if (y > window.innerHeight - edgeSize) {
    window.scrollBy(0, 10)
    updateHoveredSlot(dragPos.value.x, dragPos.value.y)
  }

  scrollRafId = requestAnimationFrame(runAutoScroll)
}

const cleanupDrag = () => {
  window.removeEventListener('pointermove', onWindowPointerMove)
  window.removeEventListener('pointerup', onWindowPointerUp)
  window.removeEventListener('pointercancel', onWindowPointerCancel)
  stopAutoScroll()
  draggingStudent.value = null
  isDragging.value = false
  dragOverSlotId.value = null
}

const onStudentPointerDown = (event: PointerEvent, student: StudentDto) => {
  if (event.pointerType === 'mouse' && event.button !== 0) return

  draggingStudent.value = student
  isTouchDrag.value = event.pointerType === 'touch'
  dragStart = { x: event.clientX, y: event.clientY }
  dragPos.value = { x: event.clientX, y: event.clientY }
  isDragging.value = false

  window.addEventListener('pointermove', onWindowPointerMove, { passive: false })
  window.addEventListener('pointerup', onWindowPointerUp)
  window.addEventListener('pointercancel', onWindowPointerCancel)
}

const onWindowPointerMove = (event: PointerEvent) => {
  if (!draggingStudent.value) return

  const dx = event.clientX - dragStart.x
  const dy = event.clientY - dragStart.y

  if (!isDragging.value && Math.hypot(dx, dy) > 6) {
    isDragging.value = true
    selectedStudent.value = null
    scrollRafId = requestAnimationFrame(runAutoScroll)
  }

  if (isDragging.value) {
    event.preventDefault()
    dragPos.value = { x: event.clientX, y: event.clientY }
    updateHoveredSlot(event.clientX, event.clientY)
  }
}

const onWindowPointerUp = async (event: PointerEvent) => {
  const student = draggingStudent.value
  const wasDragging = isDragging.value

  if (wasDragging) {
    updateHoveredSlot(event.clientX, event.clientY)
  }

  const targetSlotId = dragOverSlotId.value
  cleanupDrag()

  if (!student) return

  if (!wasDragging) {
    toggleSelectStudent(student)
    return
  }

  if (targetSlotId) {
    const targetSlot = slots.value.find(s => s.id === targetSlotId)
    if (targetSlot) {
      await assignStudent(targetSlot, student.id)
    }
  }
}

const onWindowPointerCancel = () => {
  cleanupDrag()
}

const assignStudent = async (slot: ScheduleSlotDto, studentId: number) => {
  if (slot.students.some(s => s.id === studentId)) {
    alert.showWarn('Bu öğrenci bu derse zaten ekli.')
    return
  }

  if (slot.students.length >= slot.capacity) {
    alert.showWarn(`Bu ders saati dolu (${slot.capacity}/${slot.capacity}).`)
    return
  }

  const ok = await scheduleStore.addStudentToSlot(slot.id, studentId)
  if (!ok) {
    alert.showError(scheduleStore.error || 'Öğrenci eklenemedi.')
    return
  }

  selectedStudent.value = null
  alert.showSuccess('Öğrenci ders programına eklendi.')
}

const toggleSelectStudent = (student: StudentDto) => {
  selectedStudent.value = selectedStudent.value?.id === student.id ? null : student
}

const handleSlotTap = async (slot: ScheduleSlotDto) => {
  if (!selectedStudent.value) return
  await assignStudent(slot, selectedStudent.value.id)
}

const handleRemoveStudent = async (slotId: number, studentId: number, fullName: string) => {
  const ok = await scheduleStore.removeStudentFromSlot(slotId, studentId)
  if (!ok) {
    alert.showError(scheduleStore.error || 'Öğrenci çıkarılamadı.')
    return
  }
  alert.showSuccess(`${fullName} dersten çıkarıldı.`)
}

const openCreateModal = (dayOfWeek?: number) => {
  createForm.value = {
    dayOfWeek: dayOfWeek || (selectedDay.value || 1),
    startTime: '08:00'
  }
  showCreateModal.value = true
}

const submitCreateSlot = async () => {
  if (!createForm.value.startTime) return

  isSubmittingSlot.value = true
  try {
    const created = await scheduleStore.createSlot({
      dayOfWeek: createForm.value.dayOfWeek,
      startTime: createForm.value.startTime
    })

    if (!created) {
      alert.showError(scheduleStore.error || 'Ders saati eklenemedi.')
      return
    }

    showCreateModal.value = false
    alert.showSuccess(`${getDayLabel(created.dayOfWeek)} ${created.startTime} kartı oluşturuldu.`)
  } finally {
    isSubmittingSlot.value = false
  }
}

const confirmDeleteSlot = (slot: ScheduleSlotDto) => {
  slotToDelete.value = slot
  showDeleteModal.value = true
}

const executeDeleteSlot = async () => {
  if (!slotToDelete.value) return

  isDeletingSlot.value = true
  try {
    const ok = await scheduleStore.deleteSlot(slotToDelete.value.id)
    if (!ok) {
      alert.showError(scheduleStore.error || 'Ders saati silinemedi.')
      return
    }
    showDeleteModal.value = false
    slotToDelete.value = null
    alert.showSuccess('Ders saati silindi.')
  } finally {
    isDeletingSlot.value = false
  }
}

onMounted(async () => {
  await Promise.all([
    scheduleStore.fetchSchedule(),
    studentStore.fetchStudents()
  ])
})

onUnmounted(() => {
  cleanupDrag()
})
</script>
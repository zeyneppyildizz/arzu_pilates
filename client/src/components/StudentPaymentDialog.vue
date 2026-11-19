<template>
  <Dialog v-model:visible="dialogVisible" modal header="Paket Yükle" :style="{ width: '25rem' }">
    <p class="text-gray-600 text-sm mb-6 pt-2">
      <span class="font-bold text-[#2d3627]">{{ student?.firstName }}</span> adlı öğrenciye
      ders hakkı ekliyorsunuz.
    </p>

    <div class="space-y-4 mb-4">
      <div class="flex flex-col gap-1.5">
        <label class="text-sm font-bold text-[#4a5542]">Eklenecek Ders Sayısı</label>
        <InputNumber v-model="paymentForm.sessions" :min="1" class="w-full"
          inputClass="w-full !bg-[#f8f9f5] !border !border-gray-200/80 !text-[#2d3627] !rounded-xl !p-3 focus:!border-[#718161] focus:!bg-white transition-colors" />
      </div>

      <div class="flex flex-col gap-1.5">
        <label class="text-sm font-bold text-[#4a5542]">Alınan Nakit Ücret (TL)</label>
        <InputNumber v-model="paymentForm.amount" :min="0" mode="currency" currency="TRY" locale="tr-TR" class="w-full"
          inputClass="w-full !bg-[#f8f9f5] !border !border-gray-200/80 !text-[#2d3627] !rounded-xl !p-3 focus:!border-[#718161] focus:!bg-white transition-colors" />
      </div>
    </div>

    <template #footer>
      <Button label="İptal" text :disabled="isSubmitting" @click="dialogVisible = false"
        class="!text-gray-500 hover:!text-gray-700 font-medium" />
      <Button label="Ödemeyi Onayla" icon="pi pi-check" :loading="isSubmitting" @click="submitPayment"
        class="!bg-[#718161] hover:!bg-[#5c6a4e] !border-none !text-white !rounded-xl shadow-md transition-colors font-semibold px-4" />
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useStudentStore } from '../stores/student.store'
import { useAlert } from '../composables/useAlert'
import type { AddPaymentRequest, StudentDto } from '../types/student.type'

import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputNumber from 'primevue/inputnumber'

const props = defineProps<{
  visible: boolean
  student: StudentDto | null
}>()

const emit = defineEmits<{
  'update:visible': [visible: boolean]
  'payment-succeeded': []
}>()

const studentStore = useStudentStore()
const alert = useAlert()

const isSubmitting = ref(false)

const paymentForm = reactive<{ sessions: number | null; amount: number | null }>({
  sessions: 8,
  amount: 3500
})

const dialogVisible = computed({
  get: () => props.visible,
  set: (visible: boolean) => emit('update:visible', visible)
})

watch(() => props.visible, visible => {
  if (visible) {
    paymentForm.sessions = 8
    paymentForm.amount = 3500
  }
})

const submitPayment = async () => {
  if (!props.student || isSubmitting.value) return

  if (!paymentForm.sessions || paymentForm.sessions <= 0) {
    alert.showError('Lütfen geçerli bir ders sayısı girin.')
    return
  }

  if (paymentForm.amount == null || paymentForm.amount < 0) {
    alert.showError('Lütfen geçerli bir ücret girin.')
    return
  }

  const payload: AddPaymentRequest = {
    studentId: props.student.id,
    addedSessions: paymentForm.sessions,
    amount: paymentForm.amount
  }

  isSubmitting.value = true
  try {
    await studentStore.addPayment(payload)
    if (studentStore.error) {
      alert.showError(studentStore.error)
      return
    }

    dialogVisible.value = false
    emit('payment-succeeded')
  } finally {
    isSubmitting.value = false
  }
}
</script>
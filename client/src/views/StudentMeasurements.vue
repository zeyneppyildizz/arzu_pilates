<template>
  <div class="max-w-6xl mx-auto p-4 md:p-8 transition-colors duration-300">
    <PageHeader eyebrow="ÖĞRENCİ PANELİ" title="Vücut Ölçülerim" subtitle="Ölçülerinizi güncel tutun.">
      <template #actions>
        <Button label="Kaydet" icon="pi pi-save" @click="submitMeasurements" :loading="isSaving"
          class="!bg-[#718161] hover:!bg-[#5c6a4e] !border-none !text-white !rounded-xl !px-3 md:!px-6 !py-2 md:!py-3 shadow-md transition-all duration-300 text-sm md:text-base font-semibold whitespace-nowrap" />
      </template>
    </PageHeader>

    <div class="bg-white rounded-3xl p-5 md:p-8 shadow-sm border border-gray-100">
      <div class="space-y-7">
        <section v-for="section in measurementSections" :key="section.title">
          <div class="mb-3">
            <h2 class="font-bold text-[#2d3627]">{{ section.title }}</h2>
            <p class="mt-1 text-xs text-gray-500">{{ section.description }}</p>
          </div>

          <div class="grid grid-cols-2 gap-3 md:gap-6" :class="section.gridClass">
            <div v-for="field in section.fields" :key="field.key" class="flex flex-col gap-1 md:gap-2">
              <label :for="field.key" class="text-xs md:text-sm font-bold text-[#4a5542]">
                {{ field.label }}
              </label>
              <InputNumber v-model="measurements[field.key]" :inputId="field.key" :min="0" :useGrouping="false"
                :minFractionDigits="0" :maxFractionDigits="1" class="w-full" :inputClass="fieldErrors[field.key]
                  ? 'studio-input text-sm md:text-base !border-red-400 !bg-red-50/40'
                  : 'studio-input text-sm md:text-base'" @input="clearFieldError(field.key)" />
              <p v-if="fieldErrors[field.key]" class="text-[11px] font-medium text-red-600">
                {{ fieldErrors[field.key] }}
              </p>
            </div>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '../stores/auth.store'
import { useMeasurementStore } from '../stores/measurement.store'
import { useAlert } from '../composables/useAlert'
import type { MeasurementDto } from '../types/measurement.type'

import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'
import PageHeader from '../components/PageHeader.vue'

interface MeasurementField {
  key: keyof MeasurementDto
  label: string
  name: string
  unit: string
  min: number
  max: number
}

interface MeasurementSection {
  title: string
  description: string
  gridClass: string
  fields: MeasurementField[]
}

const measurementSections: MeasurementSection[] = [
  {
    title: 'Temel ölçüler',
    description: 'Boy ve kilo bilgilerinizi girin.',
    gridClass: 'md:grid-cols-2',
    fields: [
{ key: 'height', label: 'Boy (cm)', name: 'Boy', unit: 'cm', min: 50, max: 250 },
      { key: 'weight', label: 'Kilo (kg)', name: 'Kilo', unit: 'kg', min: 20, max: 300 }
    ]
  },
  {
    title: 'Gövde ölçüleri',
    description: 'Ölçüleri santimetre cinsinden girin.',
    gridClass: 'md:grid-cols-3',
    fields: [
{ key: 'waist', label: 'Bel (cm)', name: 'Bel', unit: 'cm', min: 30, max: 250 },
      { key: 'abdomen', label: 'Karın (cm)', name: 'Karın', unit: 'cm', min: 30, max: 250 },
      { key: 'hips', label: 'Kalça (cm)', name: 'Kalça', unit: 'cm', min: 30, max: 250 }
    ]
  },
  {
    title: 'Kol ve bacak ölçüleri',
    description: 'Tüm ölçüleri santimetre cinsinden girin.',
    gridClass: 'md:grid-cols-4',
    fields: [
{ key: 'rightArm', label: 'Sağ Kol (cm)', name: 'Sağ kol', unit: 'cm', min: 10, max: 100 },
      { key: 'leftArm', label: 'Sol Kol (cm)', name: 'Sol kol', unit: 'cm', min: 10, max: 100 },
      { key: 'rightLeg', label: 'Sağ Bacak (cm)', name: 'Sağ bacak', unit: 'cm', min: 20, max: 150 },
      { key: 'leftLeg', label: 'Sol Bacak (cm)', name: 'Sol bacak', unit: 'cm', min: 20, max: 150 }
    ]
  }
]

const authStore = useAuthStore()
const measurementStore = useMeasurementStore()
const alert = useAlert()

const { measurements, isSaving } = storeToRefs(measurementStore)

const fieldErrors = ref<Partial<Record<keyof MeasurementDto, string>>>({})

const clearFieldError = (key: keyof MeasurementDto) => {
  if (fieldErrors.value[key]) {
    delete fieldErrors.value[key]
  }
}

const validateMeasurements = (): boolean => {
  const errors: Partial<Record<keyof MeasurementDto, string>> = {}

  for (const section of measurementSections) {
    for (const field of section.fields) {
      const value = measurements.value[field.key]
      if (value === null || value === undefined) continue

      if (value < field.min || value > field.max) {
        errors[field.key] = `${field.name} ${field.min} ile ${field.max} ${field.unit} arasında olmalıdır.`
      }
    }
  }

  fieldErrors.value = errors
  return Object.keys(errors).length === 0
}

onMounted(async () => {
  if (authStore.user?.id) {
    await measurementStore.fetchMeasurements(authStore.user.id)
  }
})

const submitMeasurements = async () => {
  if (!authStore.user?.id) return
  if (!validateMeasurements()) {
    alert.showWarn('Lütfen girdiğiniz ölçü değerlerini kontrol edin.')
    return
  }

  try {
    await measurementStore.saveMeasurements(authStore.user.id)
    alert.showSuccess('Ölçüleriniz başarıyla güncellendi!')
  } catch {
    alert.showError(measurementStore.error || 'Bilinmeyen bir hata oluştu.')
  }
}
</script>
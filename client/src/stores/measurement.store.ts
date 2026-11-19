import { defineStore } from 'pinia'
import { ref } from 'vue'
import { measurementService } from '../services/measurement.service'
import type { MeasurementDto } from '../types/measurement.type'

export const useMeasurementStore = defineStore('measurement', () => {
  const measurements = ref<MeasurementDto>({
    height: null, weight: null, waist: null, abdomen: null, hips: null,
    rightLeg: null, leftLeg: null, rightArm: null, leftArm: null
  })

  const isLoading = ref(false)
  const isSaving = ref(false)
  const error = ref<string | null>(null)

  const fetchMeasurements = async (studentId: number) => {
    isLoading.value = true
    error.value = null
    try {
      const data = await measurementService.getMeasurements(studentId)
      measurements.value = data
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ölçüler yüklenirken bir hata oluştu.'
      console.error(err)
    } finally {
      isLoading.value = false
    }
  }

  const saveMeasurements = async (studentId: number) => {
    isSaving.value = true
    error.value = null
    try {
      await measurementService.saveMeasurements(studentId, measurements.value)
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ölçüler kaydedilemedi.'
      throw err
    } finally {
      isSaving.value = false
    }
  }

  return {
    measurements,
    isLoading,
    isSaving,
    error,
    fetchMeasurements,
    saveMeasurements
  }
})
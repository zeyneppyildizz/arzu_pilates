<template>
  <div class="min-h-[calc(100vh-8rem)] flex flex-col items-center pt-8 px-4 pb-8 transition-colors duration-300">
    <section class="w-full max-w-md rounded-3xl border border-gray-100 bg-white p-5 md:p-6 shadow-sm transition-all duration-300">
      <div class="text-center mb-5">
        <p class="text-xs font-semibold uppercase tracking-wide text-[#718161]">EĞİTMEN PANELİ</p>
        <h1 class="mt-1 text-xl font-bold tracking-tight text-[#2d3627]">Yoklama Tarayıcısı</h1>
        <p class="mt-1 text-sm font-medium text-[#4a5542]">
          Yoklamayı almak için öğrencinin telefonundaki karekodu okutun.
        </p>
      </div>

      <div v-if="balance !== null" class="bg-[#f8f9f5] border border-gray-200/80 rounded-2xl p-5 mb-5 w-full text-center shadow-sm">
        <div class="text-sm font-bold text-[#4a5542] mb-1">
          Öğrencinin Güncel Bakiyesi
        </div>
        <div class="text-4xl font-black text-[#2d3627]">
          {{ balance }}
        </div>
        <div v-if="balance < 0" class="text-xs mt-3 font-bold bg-red-50 text-red-700 border border-red-200 inline-block px-3 py-1 rounded-full">
          Uyarı: Ödeme alınması gerekiyor!
        </div>
      </div>

      <div class="relative w-full aspect-square bg-[#f8f9f5] rounded-2xl overflow-hidden border border-gray-200/80 shadow-sm transition-all duration-300">
        <qrcode-stream
          v-if="!isScanningSuccess && !cameraError"
          @detect="onDetect"
          @camera-on="onCameraOn"
          @error="onCameraError"
        />

        <div v-if="cameraError" class="absolute inset-0 flex flex-col items-center justify-center bg-[#f8f9f5] p-6 text-center text-[#2d3627]">
          <i class="pi pi-camera text-4xl text-[#718161]" aria-hidden="true"></i>
          <p class="mt-4 font-bold">Kamera açılamadı</p>
          <p class="mt-2 text-sm text-[#4a5542]">{{ cameraError }}</p>
          <button
            type="button"
            class="mt-5 rounded-xl bg-[#718161] px-4 py-2 text-sm font-semibold text-white transition-colors hover:bg-[#5c6a4e]"
            @click="retryCamera">
            Tekrar dene
          </button>
        </div>

        <div v-else-if="isScanningSuccess" class="absolute inset-0 flex flex-col items-center justify-center bg-[#f4f7f0] text-[#2d3627] transition-all duration-500">
          <i class="pi pi-check-circle text-7xl mb-2 text-[#718161]" aria-hidden="true"></i>
          <span class="font-bold text-lg">Başarılı!</span>
        </div>

        <div v-else-if="!isCameraReady" class="absolute inset-0 flex flex-col items-center justify-center bg-[#f8f9f5] text-[#4a5542]">
          <i class="pi pi-spin pi-spinner text-3xl text-[#718161]" aria-hidden="true"></i>
          <span class="mt-3 text-sm font-semibold">Kamera açılıyor...</span>
        </div>
      </div>

      <div
        v-if="statusMessage && !cameraError"
        role="status"
        aria-live="polite"
        class="mt-4 w-full flex items-center gap-3 p-4 rounded-xl shadow-sm border transition-all duration-300"
        :class="statusMeta.classes">
        <i :class="statusMeta.icon" aria-hidden="true"></i>
        <span class="font-bold text-sm">{{ statusMessage }}</span>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onUnmounted, ref } from 'vue'
import { QrcodeStream, type DetectedBarcode, type EmittedError } from 'vue-qrcode-reader'
import { useAttendanceStore } from '../stores/attendance.store'

const attendanceStore = useAttendanceStore()

const balance = ref<number | null>(null)
const isScanningSuccess = ref(false)
const isCameraReady = ref(false)
const cameraError = ref('')
const isProcessing = ref(false)

const statusMessage = ref('Kamera açılıyor, lütfen karekodu hizalayın...')
const messageSeverity = ref<'info' | 'success' | 'error'>('info')
let resetTimer: ReturnType<typeof setTimeout> | null = null

const STATUS_META_MAP = {
  success: {
    classes: 'border-[#dce3d4] bg-[#f4f7f0] text-[#2d3627]',
    icon: 'pi pi-check-circle text-[#718161]'
  },
  error: {
    classes: 'border-red-200 bg-red-50 text-red-700',
    icon: 'pi pi-exclamation-circle'
  },
  info: {
    classes: 'border-gray-200/80 bg-[#f8f9f5] text-[#4a5542]',
    icon: 'pi pi-info-circle text-[#718161]'
  }
} as const

const statusMeta = computed(() => STATUS_META_MAP[messageSeverity.value])

const onCameraOn = () => {
  isCameraReady.value = true
  statusMessage.value = 'Kamera hazır. Öğrenci karekodunu hizalayın.'
  messageSeverity.value = 'info'
}

const onCameraError = (error: EmittedError) => {
  isCameraReady.value = false
  cameraError.value = getCameraErrorMessage(error)
}

const getCameraErrorMessage = (error: EmittedError) => {
  switch (error.name) {
    case 'NotAllowedError':
      return 'Tarayıcı ayarlarından kamera erişimine izin verin.'
    case 'NotFoundError':
      return 'Bu cihazda kullanılabilir bir kamera bulunamadı.'
    case 'NotReadableError':
      return 'Kamera başka bir uygulama tarafından kullanılıyor olabilir.'
    case 'InsecureContextError':
      return 'Kamera için HTTPS veya localhost bağlantısı gerekir.'
    case 'StreamApiNotSupportedError':
      return 'Bu tarayıcı kamera kullanımını desteklemiyor.'
    default:
      return 'Kamera iznini ve bağlantınızı kontrol edip tekrar deneyin.'
  }
}

const retryCamera = () => {
  cameraError.value = ''
  isCameraReady.value = false
  statusMessage.value = 'Kamera açılıyor, lütfen karekodu hizalayın...'
  messageSeverity.value = 'info'
}

const onDetect = async (detectedCodes: DetectedBarcode[]) => {
  const scannedToken = detectedCodes[0]?.rawValue
  if (!scannedToken || isProcessing.value) return

  isProcessing.value = true
  statusMessage.value = 'Yoklama işleniyor...'
  messageSeverity.value = 'info'

  try {
    const data = await attendanceStore.scanQrCode(scannedToken)

    balance.value = data.newBalance
    isScanningSuccess.value = true
    statusMessage.value = 'Yoklama işlendi. Öğrenci derse girebilir.'
    messageSeverity.value = 'success'

    if (resetTimer) clearTimeout(resetTimer)
    resetTimer = setTimeout(() => {
      isScanningSuccess.value = false
      isCameraReady.value = false
      statusMessage.value = 'Kamera açılıyor, lütfen karekodu hizalayın...'
      messageSeverity.value = 'info'
      balance.value = null
      resetTimer = null
    }, 3500)
  } catch {
    statusMessage.value = attendanceStore.error || 'Yoklama işlenirken bir hata oluştu.'
    messageSeverity.value = 'error'

    if (resetTimer) clearTimeout(resetTimer)
    resetTimer = setTimeout(() => {
      statusMessage.value = 'Yeni barkod bekleniyor...'
      messageSeverity.value = 'info'
      resetTimer = null
    }, 3000)
  } finally {
    isProcessing.value = false
  }
}

onUnmounted(() => {
  if (resetTimer) clearTimeout(resetTimer)
})
</script>
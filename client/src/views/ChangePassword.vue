<template>
  <div class="min-h-screen flex items-center justify-center px-4 py-8 bg-[#f3f4ef]">
    <div class="w-full max-w-md rounded-3xl border border-[#e2e6dc] bg-white p-6 sm:p-8 shadow-sm">
      <div class="mb-6 flex items-center gap-3">
        <div class="flex h-11 w-11 items-center justify-center rounded-xl bg-[#f4f7f0] text-[#2d3627] border border-[#dce3d4]">
          <i class="pi pi-lock text-lg"></i>
        </div>
        <div>
          <p class="text-xs font-semibold uppercase tracking-wide text-[#718161]">
            {{ isForced ? 'GÜVENLİK ADIMI' : 'HESAP GÜVENLİĞİ' }}
          </p>
          <h1 class="text-xl font-bold text-[#2d3627]">
            {{ isForced ? 'Yeni Şifrenizi Belirleyin' : 'Şifre Değiştir' }}
          </h1>
        </div>
      </div>

      <p
        v-if="isForced"
        class="mb-5 rounded-2xl border border-[#dce3d4] bg-[#f4f7f0] px-4 py-3 text-sm text-[#4a5542]">
        Geçici şifreyle giriş yaptınız. Devam edebilmek için kendinize özel yeni bir şifre belirleyin.
      </p>

      <form @submit.prevent="handleSubmit" novalidate class="space-y-4">
        <div v-if="!isForced" class="flex flex-col gap-1.5">
          <label for="currentPassword" class="text-xs font-semibold text-[#4a5542]">Mevcut Şifre</label>
          <div class="relative">
            <InputText
              id="currentPassword"
              v-model="form.currentPassword"
              :type="showCurrent ? 'text' : 'password'"
              autocomplete="current-password"
              class="studio-input !pr-12"
              placeholder="****"
            />
            <button
              type="button"
              class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] hover:text-[#2d3627]"
              @click="showCurrent = !showCurrent">
              <i :class="showCurrent ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
            </button>
          </div>
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="newPassword" class="text-xs font-semibold text-[#4a5542]">Yeni Şifre</label>
          <div class="relative">
            <InputText
              id="newPassword"
              v-model="form.newPassword"
              :type="showNew ? 'text' : 'password'"
              autocomplete="new-password"
              class="studio-input !pr-12"
              placeholder="En az 3 karakter"
            />
            <button
              type="button"
              class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] hover:text-[#2d3627]"
              @click="showNew = !showNew">
              <i :class="showNew ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
            </button>
          </div>
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="confirmPassword" class="text-xs font-semibold text-[#4a5542]">Yeni Şifre (Tekrar)</label>
          <div class="relative">
            <InputText
              id="confirmPassword"
              v-model="form.confirmPassword"
              :type="showConfirm ? 'text' : 'password'"
              autocomplete="new-password"
              class="studio-input !pr-12"
              placeholder="Yeni şifrenizi tekrar girin"
            />
            <button
              type="button"
              class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] hover:text-[#2d3627]"
              @click="showConfirm = !showConfirm">
              <i :class="showConfirm ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
            </button>
          </div>
        </div>

        <p
          v-if="displayError"
          role="alert"
          class="rounded-xl border border-red-200 bg-red-50 px-3.5 py-2.5 text-center text-sm font-semibold text-red-700">
          {{ displayError }}
        </p>

        <Button
          type="submit"
          label="Şifreyi Güncelle"
          :loading="authStore.loading"
          class="w-full !mt-2 !rounded-xl !border-none !bg-[#718161] !py-3 !text-base !font-semibold !text-white shadow-md transition-colors hover:!bg-[#5c6a4e]"
        />

        <div class="pt-1 text-center">
          <button
            v-if="isForced"
            type="button"
            class="text-xs font-semibold text-[#718161] hover:text-[#2d3627] transition-colors"
            @click="handleLogout">
            Farklı hesapla giriş yap (Çıkış Yap)
          </button>
          <button
            v-else
            type="button"
            class="text-xs font-semibold text-[#718161] hover:text-[#2d3627] transition-colors"
            @click="router.back()">
            Geri Dön
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'
import { useAlert } from '../composables/useAlert'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const router = useRouter()
const authStore = useAuthStore()
const alert = useAlert()

const isForced = computed(() => !!authStore.user?.mustChangePassword)

const form = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const showCurrent = ref(false)
const showNew = ref(false)
const showConfirm = ref(false)
const localError = ref('')

const displayError = computed(() => localError.value || authStore.error)

onMounted(() => {
  authStore.error = ''
})

const handleSubmit = async () => {
  localError.value = ''
  authStore.error = ''

  if (!isForced.value && !form.value.currentPassword) {
    localError.value = 'Lütfen mevcut şifrenizi girin.'
    return
  }

  if (!form.value.newPassword || !form.value.confirmPassword) {
    localError.value = 'Lütfen yeni şifre alanlarını doldurun.'
    return
  }

  if (form.value.newPassword.length < 3) {
    localError.value = 'Yeni şifre en az 3 karakter olmalıdır.'
    return
  }

  if (form.value.newPassword !== form.value.confirmPassword) {
    localError.value = 'Yeni şifreler birbiriyle eşleşmiyor.'
    return
  }

  const success = await authStore.changePassword({
    currentPassword: isForced.value ? undefined : form.value.currentPassword,
    newPassword: form.value.newPassword
  })

  if (success) {
    alert.showSuccess('Şifreniz başarıyla güncellendi.')
    router.push(authStore.user?.role === 1 ? '/instructor' : '/profile')
  }
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
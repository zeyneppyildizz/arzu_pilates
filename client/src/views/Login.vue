<template>
  <div class="relative min-h-screen flex items-center justify-center px-4 py-8 md:px-8 overflow-hidden bg-black">
    <div class="fixed inset-0 z-0 pointer-events-none">
      <img
        :src="studioMobileImage"
        alt=""
        class="block md:hidden w-full h-full object-cover blur-[2px] scale-105"
      />
      <img
        :src="studioImage"
        alt=""
        class="hidden md:block w-full h-full object-cover object-[80%_15%] blur-[2px] scale-105"
      />
      <div class="absolute inset-0 bg-gradient-to-b from-black/30 via-black/20 to-black/35"></div>
    </div>

    <div class="relative z-10 grid w-full max-w-5xl overflow-hidden rounded-3xl border border-white/30 shadow-2xl md:grid-cols-2">
      <section
        class="relative hidden min-h-[620px] flex-col justify-between bg-cover bg-center p-10 text-white md:flex"
        :style="{ backgroundImage: `linear-gradient(180deg, rgba(28, 36, 25, 0.18), rgba(28, 36, 25, 0.58)), url(${studioImage})` }">
        <div class="absolute inset-0 bg-gradient-to-br from-[#2d3627]/10 to-transparent"></div>
        <div class="relative z-10">
          <span class="inline-flex items-center gap-2 rounded-full border border-white/30 bg-black/15 px-3 py-1.5 text-xs font-semibold tracking-wide backdrop-blur-sm">
            <i class="pi pi-sparkles"></i>
            REFORMER PILATES STÜDYOSU
          </span>
        </div>
        <div class="relative z-10">
          <h1 class="text-4xl font-bold leading-tight tracking-tight">
            Güçlü hisset.<br />Dengede kal.
          </h1>
          <p class="mt-4 max-w-sm text-base leading-relaxed text-white/85">
            Hareket, denge ve kendinize ayırdığınız iyi bir zaman.
          </p>
        </div>
        <p class="relative z-10 text-sm font-medium text-white/75">Arzu Pilates · Bedeninize iyi bakın.</p>
      </section>

      <section class="flex min-h-[560px] flex-col justify-center bg-[#f7f8f4]/85 p-6 backdrop-blur-md sm:p-9 md:min-h-[620px] md:p-10">
        <div class="mb-7 flex items-center gap-3">
          <div class="flex h-11 w-11 items-center justify-center rounded-xl border border-[#dce3d4] bg-[#2d3627] text-xl font-black text-[#f3f4ef]">
            A
          </div>
          <div>
            <p class="font-bold tracking-tight text-[#2d3627]">Arzu Pilates</p>
            <p class="text-xs font-medium text-[#718161]">Hareket ve denge</p>
          </div>
        </div>

        <p class="mb-1 text-sm font-semibold text-[#718161]">{{ isLoginMode ? 'TEKRAR HOŞ GELDİNİZ' : 'STÜDYOMUZA KATILIN' }}</p>
        <h2 class="text-2xl font-bold tracking-tight text-[#2d3627]">
          {{ isLoginMode ? 'Sisteme Giriş' : 'Yeni Öğrenci Kaydı' }}
        </h2>
        <p class="mt-2 text-sm text-[#4a5542]">
          {{ isLoginMode ? 'Devam etmek için hesabınıza giriş yapın.' : 'Hesabınızı oluşturmak için bilgilerinizi girin.' }}
        </p>

        <div class="mt-6 grid grid-cols-2 gap-1 rounded-xl border border-[#e2e6dc] bg-[#edf0e9] p-1" aria-label="Giriş veya kayıt seçin">
          <button
            type="button"
            :aria-pressed="isLoginMode"
            @click="setMode(true)"
            class="rounded-lg px-3 py-2.5 text-sm font-semibold transition-colors"
            :class="isLoginMode ? 'bg-white text-[#2d3627] shadow-sm' : 'text-[#4a5542] hover:text-[#2d3627]'">
            Giriş Yap
          </button>
          <button
            type="button"
            :aria-pressed="!isLoginMode"
            @click="setMode(false)"
            class="rounded-lg px-3 py-2.5 text-sm font-semibold transition-colors"
            :class="!isLoginMode ? 'bg-white text-[#2d3627] shadow-sm' : 'text-[#4a5542] hover:text-[#2d3627]'">
            Kayıt Ol
          </button>
        </div>

        <form @submit.prevent="handleSubmit" novalidate class="mt-5 space-y-4">
          <div v-if="!isLoginMode" class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div class="flex flex-col gap-1.5">
              <label for="firstName" class="text-xs font-semibold text-[#4a5542]">Ad</label>
              <InputText id="firstName" v-model="form.firstName" type="text" class="studio-input" />
            </div>
            <div class="flex flex-col gap-1.5">
              <label for="lastName" class="text-xs font-semibold text-[#4a5542]">Soyad</label>
              <InputText id="lastName" v-model="form.lastName" type="text" class="studio-input" />
            </div>
          </div>

          <div class="flex flex-col gap-1.5">
            <label for="username" class="text-xs font-semibold text-[#4a5542]">Kullanıcı Adı</label>
            <InputText
              id="username"
              v-model="form.username"
              type="text"
              autocomplete="username"
              class="studio-input"
              placeholder="Örn: zeynep"
            />
          </div>

          <div class="flex flex-col gap-1.5">
            <label for="password" class="text-xs font-semibold text-[#4a5542]">Şifre</label>
            <div class="relative">
              <InputText
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                :autocomplete="isLoginMode ? 'current-password' : 'new-password'"
                class="studio-input !pr-12"
                placeholder="****"
              />
              <button
                type="button"
                :aria-label="showPassword ? 'Şifreyi gizle' : 'Şifreyi göster'"
                class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] transition-colors hover:text-[#2d3627]"
                @click.prevent="showPassword = !showPassword">
                <i :class="showPassword ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
              </button>
            </div>
          </div>

          <p
            v-if="authStore.error"
            role="alert"
            class="rounded-xl border border-red-200 bg-red-50 px-3.5 py-2.5 text-center text-sm font-semibold text-red-700">
            {{ authStore.error }}
          </p>

          <Button
            type="submit"
            :label="isLoginMode ? 'Giriş Yap' : 'Kayıt Ol'"
            :loading="authStore.loading"
            class="w-full !mt-2 !rounded-xl !border-none !bg-[#718161] !py-3 !text-base !font-semibold !text-white shadow-md transition-colors hover:!bg-[#5c6a4e]"
          />
        </form>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import studioImage from '../assets/studio.webp'
import studioMobileImage from '../assets/studio-mobile.webp'
import { useAuthStore } from '../stores/auth.store'
import { useAlert } from '../composables/useAlert'

import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const router = useRouter()
const authStore = useAuthStore()
const alert = useAlert()

const isLoginMode = ref(true)
const showPassword = ref(false)

const form = ref({
  firstName: '',
  lastName: '',
  username: '',
  password: ''
})

const setMode = (loginMode: boolean) => {
  if (isLoginMode.value === loginMode) return

  isLoginMode.value = loginMode
  showPassword.value = false
  authStore.error = ''
  form.value = { firstName: '', lastName: '', username: '', password: '' }
}

const handleSubmit = async () => {
  authStore.error = ''

  const registrationData = {
    firstName: form.value.firstName.trim(),
    lastName: form.value.lastName.trim(),
    username: form.value.username.trim(),
    password: form.value.password
  }

  if (isLoginMode.value) {
    if (!registrationData.username || !registrationData.password) {
      authStore.error = 'Lütfen kullanıcı adı ve şifre alanlarını doldurun.'
      return
    }

    const success = await authStore.login({
      username: registrationData.username,
      password: registrationData.password
    })

    if (success && authStore.user) {
      router.push(authStore.user.role === 0 ? '/profile' : '/instructor')
    }
  } else {
    if (!registrationData.firstName || !registrationData.lastName || !registrationData.username || !registrationData.password) {
      authStore.error = 'Lütfen tüm alanları doldurun.'
      return
    }

    const success = await authStore.register(registrationData)

    if (success) {
      setMode(true)
      alert.showSuccess('Kayıt başarılı! Lütfen yeni hesabınızla giriş yapın.')
    }
  }
}
</script>
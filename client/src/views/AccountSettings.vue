<template>
    <div class="max-w-3xl mx-auto p-4 md:p-8 transition-colors duration-300">
        <PageHeader eyebrow="HESAP AYARLARI" title="Hesap Bilgilerim"
            subtitle="Kişisel bilgilerinizi ve şifrenizi buradan güncelleyebilirsiniz." />

        <div class="flex flex-col gap-6">
            <section class="rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-8">
                <div class="mb-6 flex items-center gap-4">
                    <div
                        class="flex h-12 w-12 min-w-12 shrink-0 items-center justify-center rounded-full border border-[#dce3d4] bg-[#f4f7f0] text-sm font-bold text-[#2d3627]">
                        {{ initials }}
                    </div>
                    <div class="min-w-0">
                        <h2 class="text-lg font-bold text-[#2d3627]">Kişisel Bilgiler</h2>
                        <p class="text-xs text-gray-500">Ad, soyad ve girişte kullandığınız kullanıcı adınızı
                            düzenleyin.</p>
                    </div>
                </div>

                <form @submit.prevent="handleProfileSubmit" novalidate class="space-y-4">
                    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                        <div class="flex flex-col gap-1.5">
                            <label for="accountFirstName" class="text-xs font-semibold text-[#4a5542]">Ad</label>
                            <InputText id="accountFirstName" v-model="profileForm.firstName" type="text"
                                class="studio-input" />
                        </div>

                        <div class="flex flex-col gap-1.5">
                            <label for="accountLastName" class="text-xs font-semibold text-[#4a5542]">Soyad</label>
                            <InputText id="accountLastName" v-model="profileForm.lastName" type="text"
                                class="studio-input" />
                        </div>
                    </div>

                    <div class="flex flex-col gap-1.5">
                        <label for="accountUsername" class="text-xs font-semibold text-[#4a5542]">Kullanıcı Adı</label>
                        <InputText id="accountUsername" v-model="profileForm.username" type="text"
                            autocomplete="username" class="studio-input" />
                    </div>

                    <p v-if="profileError" role="alert"
                        class="rounded-xl border border-red-200 bg-red-50 px-3.5 py-2.5 text-center text-sm font-semibold text-red-700">
                        {{ profileError }}
                    </p>

                    <div class="flex justify-end pt-1">
                        <Button type="submit" label="Bilgileri Güncelle" icon="pi pi-check" :loading="isUpdatingProfile"
                            class="w-full sm:w-auto !rounded-xl !border-none !bg-[#718161] !px-6 !py-2.5 !text-sm !font-semibold !text-white shadow-sm transition-colors hover:!bg-[#5c6a4e]" />
                    </div>
                </form>
            </section>

            <section class="rounded-3xl border border-gray-100 bg-white p-5 shadow-sm md:p-8">
                <div class="mb-6 flex items-center gap-3">
                    <div
                        class="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl border border-[#dce3d4] bg-[#f4f7f0] text-[#2d3627]">
                        <i class="pi pi-lock text-base"></i>
                    </div>
                    <div>
                        <h2 class="text-lg font-bold text-[#2d3627]">Şifre Değiştir</h2>
                        <p class="text-xs text-gray-500">Hesabınızın güvenliği için mevcut şifrenizi girerek yenileyin.
                        </p>
                    </div>
                </div>

                <form @submit.prevent="handlePasswordSubmit" novalidate class="space-y-4">
                    <div class="flex flex-col gap-1.5">
                        <label for="currentPassword" class="text-xs font-semibold text-[#4a5542]">Mevcut Şifre</label>
                        <div class="relative">
                            <InputText id="currentPassword" v-model="passwordForm.currentPassword"
                                :type="showCurrent ? 'text' : 'password'" autocomplete="current-password"
                                class="studio-input !pr-12" placeholder="****" />
                            <button type="button"
                                class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] hover:text-[#2d3627]"
                                @click="showCurrent = !showCurrent">
                                <i :class="showCurrent ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
                            </button>
                        </div>
                    </div>

                    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                        <div class="flex flex-col gap-1.5">
                            <label for="newPassword" class="text-xs font-semibold text-[#4a5542]">Yeni Şifre</label>
                            <div class="relative">
                                <InputText id="newPassword" v-model="passwordForm.newPassword"
                                    :type="showNew ? 'text' : 'password'" autocomplete="new-password"
                                    class="studio-input !pr-12" placeholder="En az 3 karakter" />
                                <button type="button"
                                    class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] hover:text-[#2d3627]"
                                    @click="showNew = !showNew">
                                    <i :class="showNew ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
                                </button>
                            </div>
                        </div>

                        <div class="flex flex-col gap-1.5">
                            <label for="confirmPassword" class="text-xs font-semibold text-[#4a5542]">Yeni Şifre
                                (Tekrar)</label>
                            <div class="relative">
                                <InputText id="confirmPassword" v-model="passwordForm.confirmPassword"
                                    :type="showConfirm ? 'text' : 'password'" autocomplete="new-password"
                                    class="studio-input !pr-12" placeholder="Tekrar girin" />
                                <button type="button"
                                    class="absolute inset-y-0 right-0 flex w-11 items-center justify-center text-[#718161] hover:text-[#2d3627]"
                                    @click="showConfirm = !showConfirm">
                                    <i :class="showConfirm ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <p v-if="passwordError" role="alert"
                        class="rounded-xl border border-red-200 bg-red-50 px-3.5 py-2.5 text-center text-sm font-semibold text-red-700">
                        {{ passwordError }}
                    </p>

                    <div class="flex justify-end pt-1">
                        <Button type="submit" label="Şifreyi Güncelle" icon="pi pi-key" :loading="isChangingPassword"
                            class="w-full sm:w-auto !rounded-xl !border-none !bg-[#718161] !px-6 !py-2.5 !text-sm !font-semibold !text-white shadow-sm transition-colors hover:!bg-[#5c6a4e]" />
                    </div>
                </form>
            </section>
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
import PageHeader from '../components/PageHeader.vue'

const router = useRouter()
const authStore = useAuthStore()
const alert = useAlert()

const profileForm = ref({
    firstName: authStore.user?.firstName ?? '',
    lastName: authStore.user?.lastName ?? '',
    username: authStore.user?.username ?? ''
})

const passwordForm = ref({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
})

const showCurrent = ref(false)
const showNew = ref(false)
const showConfirm = ref(false)

const isUpdatingProfile = ref(false)
const isChangingPassword = ref(false)
const profileError = ref('')
const passwordError = ref('')

const initials = computed(() => {
    const first = profileForm.value.firstName.trim().charAt(0)
    const last = profileForm.value.lastName.trim().charAt(0)
    return `${first}${last}`.toUpperCase() || '?'
})

onMounted(async () => {
    authStore.error = ''
    await authStore.fetchMyProfile()
    if (authStore.user) {
        profileForm.value = {
            firstName: authStore.user.firstName ?? '',
            lastName: authStore.user.lastName ?? '',
            username: authStore.user.username ?? ''
        }
    }
})

const handleProfileSubmit = async () => {
    profileError.value = ''

    const firstName = profileForm.value.firstName.trim()
    const lastName = profileForm.value.lastName.trim()
    const username = profileForm.value.username.trim()

    if (firstName.length < 2 || lastName.length < 2) {
        profileError.value = 'Ad ve soyad en az 2 karakter olmalıdır.'
        return
    }

    if (username.length < 3) {
        profileError.value = 'Kullanıcı adı en az 3 karakter olmalıdır.'
        return
    }

    isUpdatingProfile.value = true
    try {
        const success = await authStore.updateProfile({ firstName, lastName, username })
        if (!success) {
            profileError.value = authStore.error || 'Bilgiler güncellenemedi.'
            return
        }
        profileForm.value = { firstName, lastName, username }
        alert.showSuccess('Hesap bilgileriniz güncellendi.')
    } finally {
        isUpdatingProfile.value = false
    }
}

const handlePasswordSubmit = async () => {
    passwordError.value = ''

    if (!passwordForm.value.currentPassword) {
        passwordError.value = 'Lütfen mevcut şifrenizi girin.'
        return
    }

    if (!passwordForm.value.newPassword || !passwordForm.value.confirmPassword) {
        passwordError.value = 'Lütfen yeni şifre alanlarını doldurun.'
        return
    }

    if (passwordForm.value.newPassword.length < 3) {
        passwordError.value = 'Yeni şifre en az 3 karakter olmalıdır.'
        return
    }

    if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
        passwordError.value = 'Yeni şifreler birbiriyle eşleşmiyor.'
        return
    }

    isChangingPassword.value = true
    try {
        const success = await authStore.changePassword({
            currentPassword: passwordForm.value.currentPassword,
            newPassword: passwordForm.value.newPassword
        })

        if (!success) {
            passwordError.value = authStore.error || 'Şifre güncellenemedi.'
            return
        }

        alert.showSuccess('Şifreniz güncellendi. Lütfen yeni şifrenizle tekrar giriş yapın.')
        authStore.logout()
        router.push('/login')
    } finally {
        isChangingPassword.value = false
    }
}
</script>
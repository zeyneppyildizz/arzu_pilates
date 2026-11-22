<template>
  <header class="bg-white shadow-sm sticky top-0 z-40 border-b border-gray-100 pt-[env(safe-area-inset-top)] transition-all duration-300">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">

      <div class="flex items-center gap-2">
        <div
          class="w-8 h-8 bg-[#2d3627] border border-[#dce3d4] rounded-lg flex items-center justify-center text-[#f3f4ef] font-black text-xl">
          A
        </div>
        <span class="text-xl font-bold text-[#2d3627] tracking-tight">
          Arzu Pilates
        </span>
      </div>

      <div class="flex-1 flex justify-end mr-3 lg:mr-6">
        <nav v-if="navItems.length" class="hidden md:flex items-center gap-1 h-full">
          <router-link v-for="item in navItems" :key="item.to" :to="item.to" :aria-label="item.label"
            :title="item.label"
            class="nav-link flex h-11 w-11 items-center justify-center rounded-xl text-lg text-[#4a5542] font-medium hover:bg-[#f8f9f5] hover:text-[#2d3627] transition-colors">
            <i :class="item.icon" aria-hidden="true"></i>
          </router-link>
        </nav>
      </div>

      <div class="flex items-center border-l border-gray-200 pl-4 transition-colors">
        <router-link to="/account" aria-label="Hesap Bilgilerim"
          :title="`${authStore.user?.firstName} ${authStore.user?.lastName} · Hesap Bilgilerim`"
          class="mr-2 flex h-9 w-9 items-center justify-center rounded-full border border-[#dce3d4] bg-[#f4f7f0] text-xs font-bold text-[#2d3627] transition-all hover:border-[#718161] hover:bg-[#e5ebe0]"
          active-class="!bg-[#2d3627] !text-[#f3f4ef] !border-[#2d3627]">
          {{ userInitials }}
        </router-link>
        <Button text @click="handleLogout"
          class="!px-3 !py-2 !text-gray-500 hover:!bg-red-50 hover:!text-red-600 transition-all rounded-xl">
          <i class="pi pi-sign-out"></i>
          <span class="hidden sm:inline ml-2 font-bold">Çıkış</span>
        </Button>
      </div>

    </div>

    <div v-if="navItems.length"
      class="fixed inset-x-0 bottom-0 z-50 flex items-center border-t border-gray-100 bg-white/95 px-1 pt-1 pb-[max(0.25rem,env(safe-area-inset-bottom))] shadow-[0_-4px_16px_rgba(45,54,39,0.08)] backdrop-blur md:hidden">
      <router-link v-for="item in navItems" :key="item.to" :to="item.to" :aria-label="item.label" :title="item.label"
        class="nav-link flex h-14 min-w-0 flex-1 items-center justify-center rounded-xl text-xl text-[#4a5542] font-medium transition-colors">
        <i :class="item.icon" aria-hidden="true"></i>
      </router-link>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'
import Button from 'primevue/button'

const router = useRouter()
const authStore = useAuthStore()

const navItems = computed(() => {
  return router.options.routes
    .filter(r => r.meta?.requiredRole === authStore.user?.role && r.meta?.title)
    .map(r => ({
      to: r.path,
      label: String(r.meta?.title),
      icon: String(r.meta?.icon ?? '')
    }))
})

const userInitials = computed(() => {
  const first = authStore.user?.firstName?.charAt(0) ?? ''
  const last = authStore.user?.lastName?.charAt(0) ?? ''
  return `${first}${last}`.toUpperCase()
})

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
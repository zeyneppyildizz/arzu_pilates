import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '../stores/auth.store' 

import Login from '../views/Login.vue'
import ChangePassword from '../views/ChangePassword.vue'
import StudentScanner from '../views/StudentScanner.vue'
import StudentList from '../views/StudentList.vue'
import StudentProfile from '../views/StudentProfile.vue'
import InstructorDashboard from '../views/InstructorDashboard.vue'
import StudentHistory from '../views/StudentHistory.vue'
import StudentMeasurements from '../views/StudentMeasurements.vue'
import AccountSettings from '../views/AccountSettings.vue'

const routes: Array<RouteRecordRaw> = [
  { path: '/', redirect: '/login' },
  { path: '/login', name: 'Login', component: Login },  
  {
    path: '/change-password',
    name: 'ChangePassword',
    component: ChangePassword,
    meta: { requiresAuth: true, title: 'Şifre Değiştir' }
  },
  {
    path: '/account',
    name: 'AccountSettings',
    component: AccountSettings,
    meta: { requiresAuth: true, title: 'Hesap Bilgilerim' }
  },
  { 
    path: '/profile', 
    name: 'StudentProfile', 
    component: StudentProfile,
    meta: { requiresAuth: true, requiredRole: 0, title: 'Ana Ekran', icon: 'pi pi-home' }
  },
  { 
    path: '/history', 
    name: 'StudentHistory', 
    component: StudentHistory,
    meta: { requiresAuth: true, requiredRole: 0, title: 'İşlemlerim', icon: 'pi pi-history' }
  },
  { 
    path: '/measurements', 
    name: 'StudentMeasurements', 
    component: StudentMeasurements,
    meta: { requiresAuth: true, requiredRole: 0, title: 'Bilgilerim', icon: 'pi pi-chart-line' }
  },

  { 
    path: '/instructor',
    name: 'InstructorDashboard',
    component: InstructorDashboard,
    meta: { requiresAuth: true, requiredRole: 1, title: 'Genel Bakış', icon: 'pi pi-home' }
  },
  { 
    path: '/list', 
    name: 'StudentList', 
    component: StudentList,
    meta: { requiresAuth: true, requiredRole: 1, title: 'Öğrenci Listesi', icon: 'pi pi-users' }
  },
  { 
    path: '/scanner', 
    name: 'StudentScanner', 
    component: StudentScanner,
    meta: { requiresAuth: true, requiredRole: 1, title: 'Yoklama Tarayıcısı', icon: 'pi pi-qrcode' }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()

  const isAuthenticated = !!authStore.user
  const userRole = authStore.user?.role
  const mustChangePassword = !!authStore.user?.mustChangePassword

  if (isAuthenticated && mustChangePassword) {
    if (to.name !== 'ChangePassword') {
      next({ name: 'ChangePassword' })
    } else {
      next()
    }
    return
  }

  if (to.meta.requiresAuth) {
    if (!isAuthenticated) {
      next({ name: 'Login' })
    } else {
      if (to.meta.requiredRole !== undefined && to.meta.requiredRole !== userRole) {
        next(userRole === 1 ? '/instructor' : '/profile')
      } else {
        next()
      }
    }
  } else {
    if (to.name === 'Login' && isAuthenticated) {
      next(userRole === 1 ? '/instructor' : '/profile')
    } else {
      next()
    }
  }
})

export default router
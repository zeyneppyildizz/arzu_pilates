import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      // /api ile başlayan tüm istekleri .NET backend'e yönlendir
      '/api': {
        target: 'http://localhost:5267',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
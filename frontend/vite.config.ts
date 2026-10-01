import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    // Redirige las llamadas /api al backend: así el navegador no ve dos orígenes distintos
    proxy: {
      '/api': 'http://localhost:5117',
    },
  },
})
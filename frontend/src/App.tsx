import { Navigate, Route, Routes } from 'react-router'
import { ProtectedRoute } from './auth/ProtectedRoute'
import { Layout } from './components/Layout'
import { DashboardPage } from './pages/DashboardPage'
import { LoginPage } from './pages/LoginPage'
import { ProximamentePage } from './pages/ProximamentePage'
import { EquiposPage } from './pages/equipos/EquiposPage'
import { EmpleadosPage } from './pages/empleados/EmpleadosPage'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      {/* Todo lo que está aquí dentro requiere sesión */}
      <Route element={<ProtectedRoute />}>
        <Route path="/" element={<Layout />}>
          <Route index element={<DashboardPage />} />
          <Route path="equipos" element={<EquiposPage />} />          
          <Route path="empleados" element={<EmpleadosPage />} />
          <Route path="asignaciones" element={<ProximamentePage titulo="Asignaciones" />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
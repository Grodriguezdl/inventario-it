import { Navigate, Route, Routes } from 'react-router'
import { ProtectedRoute } from './auth/ProtectedRoute'
import { Layout } from './components/Layout'
import { AsignacionesPage } from './pages/asignaciones/AsignacionesPage'
import { DashboardPage } from './pages/DashboardPage'
import { EmpleadosPage } from './pages/empleados/EmpleadosPage'
import { EquiposPage } from './pages/equipos/EquiposPage'
import { LoginPage } from './pages/LoginPage'

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
          <Route path="asignaciones" element={<AsignacionesPage />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
import { Navigate, Outlet, useLocation } from 'react-router'
import { useAuth } from './AuthContext'

// Envuelve las páginas privadas: sin sesión, redirige al login
export function ProtectedRoute() {
  const { usuario, cargando } = useAuth()
  const location = useLocation()

  if (cargando) {
    return <div className="grid min-h-screen place-items-center text-slate-500">Cargando…</div>
  }

  if (!usuario) {
    // Guardamos a dónde quería ir, para llevarlo ahí después del login
    return <Navigate to="/login" replace state={{ desde: location.pathname }} />
  }

  return <Outlet />
}
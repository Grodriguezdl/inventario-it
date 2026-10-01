import { NavLink, Outlet } from 'react-router'
import { useAuth } from '../auth/AuthContext'

const enlaces = [
  { to: '/', texto: 'Dashboard', exacto: true },
  { to: '/equipos', texto: 'Equipos' },
  { to: '/empleados', texto: 'Empleados' },
  { to: '/asignaciones', texto: 'Asignaciones' },
]

export function Layout() {
  const { usuario, logout } = useAuth()

  return (
    <div className="flex min-h-screen bg-slate-50">
      <aside className="flex w-60 shrink-0 flex-col bg-slate-900 text-slate-100">
        <div className="px-6 py-5 text-lg font-semibold">Inventario IT</div>

        <nav className="flex-1 space-y-1 px-3">
          {enlaces.map((enlace) => (
            <NavLink
              key={enlace.to}
              to={enlace.to}
              end={enlace.exacto}
              className={({ isActive }) =>
                `block rounded-md px-3 py-2 text-sm transition-colors ${
                  isActive ? 'bg-slate-700 text-white' : 'text-slate-300 hover:bg-slate-800'
                }`
              }
            >
              {enlace.texto}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-slate-800 p-4 text-sm">
          <p className="font-medium">{usuario?.nombre}</p>
          <p className="text-slate-400">{usuario?.rol === 'Admin' ? 'Administrador' : 'Técnico'}</p>
          <button onClick={logout} className="mt-3 text-slate-300 hover:text-white">
            Cerrar sesión
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-x-auto p-8">
        <Outlet />
      </main>
    </div>
  )
}
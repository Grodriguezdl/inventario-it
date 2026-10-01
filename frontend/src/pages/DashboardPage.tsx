import { useQuery } from '@tanstack/react-query'
import { api } from '../api/client'
import type { Dashboard, EstadoEquipo } from '../api/types'

const etiquetasEstado: Record<EstadoEquipo, string> = {
  Disponible: 'Disponibles',
  Asignado: 'Asignados',
  EnReparacion: 'En reparación',
  DeBaja: 'De baja',
}

function Tarjeta({ titulo, valor }: { titulo: string; valor: number }) {
  return (
    <div className="rounded-xl bg-white p-5 shadow-sm">
      <p className="text-sm text-slate-500">{titulo}</p>
      <p className="mt-1 text-3xl font-semibold text-slate-900">{valor}</p>
    </div>
  )
}

export function DashboardPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['dashboard'],
    queryFn: () => api.get<Dashboard>('/dashboard'),
  })

  if (isLoading) return <p className="text-slate-500">Cargando…</p>
  if (error) return <p className="text-red-600">Error: {error.message}</p>
  if (!data) return null

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold text-slate-900">Dashboard</h1>

      <div className="grid gap-4 sm:grid-cols-3">
        <Tarjeta titulo="Equipos activos" valor={data.totalEquipos} />
        <Tarjeta titulo="Empleados activos" valor={data.empleadosActivos} />
        <Tarjeta titulo="Asignaciones activas" valor={data.asignacionesActivas} />
      </div>

      <div className="grid gap-4 sm:grid-cols-4">
        {data.equiposPorEstado.map((item) => (
          <Tarjeta key={item.estado} titulo={etiquetasEstado[item.estado]} valor={item.total} />
        ))}
      </div>

      <div className="rounded-xl bg-white p-5 shadow-sm">
        <h2 className="mb-4 font-semibold text-slate-900">Últimas asignaciones</h2>
        {data.ultimasAsignaciones.length === 0 ? (
          <p className="text-sm text-slate-500">Todavía no hay asignaciones.</p>
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="text-slate-500">
              <tr>
                <th className="pb-2">Equipo</th>
                <th className="pb-2">Empleado</th>
                <th className="pb-2">Fecha</th>
                <th className="pb-2">Estado</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {data.ultimasAsignaciones.map((a) => (
                <tr key={a.id}>
                  <td className="py-2">
                    <span className="font-medium">{a.codigoInventario}</span> · {a.equipo}
                  </td>
                  <td className="py-2">{a.empleado}</td>
                  <td className="py-2">{new Date(a.fechaAsignacion).toLocaleDateString()}</td>
                  <td className="py-2">{a.activa ? 'Activa' : 'Devuelta'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
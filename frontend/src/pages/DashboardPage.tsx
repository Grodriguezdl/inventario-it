import { Link } from 'react-router'
import { useQuery } from '@tanstack/react-query'
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { api } from '../api/client'
import type { Dashboard, EstadoEquipo } from '../api/types'
import { EstadoBadge } from '../components/ui/EstadoBadge'
import { formatearFecha } from '../utils/formato'

// Los mismos colores que las etiquetas de estado, para que todo sea coherente
const coloresEstado: Record<EstadoEquipo, { etiqueta: string; color: string }> = {
  Disponible: { etiqueta: 'Disponibles', color: '#16a34a' },
  Asignado: { etiqueta: 'Asignados', color: '#2563eb' },
  EnReparacion: { etiqueta: 'En reparación', color: '#d97706' },
  DeBaja: { etiqueta: 'De baja', color: '#94a3b8' },
}

function Tarjeta({ titulo, valor, detalle }: { titulo: string; valor: string | number; detalle?: string }) {
  return (
    <div className="rounded-xl bg-white p-5 shadow-sm">
      <p className="text-sm text-slate-500">{titulo}</p>
      <p className="mt-1 text-3xl font-semibold text-slate-900">{valor}</p>
      {detalle && <p className="mt-1 text-xs text-slate-400">{detalle}</p>}
    </div>
  )
}

function Panel({ titulo, children }: { titulo: string; children: React.ReactNode }) {
  return (
    <div className="rounded-xl bg-white p-5 shadow-sm">
      <h2 className="mb-4 font-semibold text-slate-900">{titulo}</h2>
      {children}
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

  // Datos para la dona: solo los estados con al menos un equipo
  const datosEstado = data.equiposPorEstado
    .filter((item) => item.total > 0)
    .map((item) => ({
      estado: item.estado,
      nombre: coloresEstado[item.estado].etiqueta,
      total: item.total,
    }))

  // Utilización: qué porcentaje de los equipos activos está asignado
  const asignados = data.equiposPorEstado.find((item) => item.estado === 'Asignado')?.total ?? 0
  const utilizacion = data.totalEquipos > 0 ? Math.round((asignados / data.totalEquipos) * 100) : 0

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold text-slate-900">Dashboard</h1>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <Tarjeta titulo="Equipos activos" valor={data.totalEquipos} detalle="Sin contar los dados de baja" />
        <Tarjeta titulo="Asignaciones activas" valor={data.asignacionesActivas} />
        <Tarjeta titulo="Empleados activos" valor={data.empleadosActivos} />
        <Tarjeta
          titulo="Utilización"
          valor={`${utilizacion}%`}
          detalle={`${asignados} de ${data.totalEquipos} equipos en uso`}
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <Panel titulo="Equipos por estado">
          {datosEstado.length === 0 ? (
            <p className="text-sm text-slate-500">Todavía no hay equipos registrados.</p>
          ) : (
            <div className="h-72">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={datosEstado}
                    dataKey="total"
                    nameKey="nombre"
                    innerRadius="55%"
                    outerRadius="85%"
                    paddingAngle={2}
                  >
                    {datosEstado.map((item) => (
                      <Cell key={item.estado} fill={coloresEstado[item.estado].color} />
                    ))}
                  </Pie>
                  <Tooltip />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </div>
          )}
        </Panel>

        <Panel titulo="Equipos activos por categoría">
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={data.equiposPorCategoria} margin={{ top: 5, right: 10, left: -20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis dataKey="categoria" tick={{ fontSize: 12 }} interval={0} />
                <YAxis allowDecimals={false} tick={{ fontSize: 12 }} />
                <Tooltip cursor={{ fill: '#f1f5f9' }} />
                <Bar dataKey="total" name="Equipos" fill="#0f172a" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Panel>
      </div>

      <Panel titulo="Últimas asignaciones">
        {data.ultimasAsignaciones.length === 0 ? (
          <p className="text-sm text-slate-500">Todavía no hay asignaciones.</p>
        ) : (
          <>
            <div className="overflow-x-auto">
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
                        <span className="font-medium">{a.codigoInventario}</span>
                        <span className="text-slate-500"> · {a.equipo}</span>
                      </td>
                      <td className="py-2">{a.empleado}</td>
                      <td className="py-2">{formatearFecha(a.fechaAsignacion)}</td>
                      <td className="py-2">
                        {a.activa ? (
                          <EstadoBadge estado="Asignado" />
                        ) : (
                          <span className="text-slate-500">Devuelta</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <Link to="/asignaciones" className="mt-4 inline-block text-sm font-medium text-slate-700 hover:underline">
              Ver todas las asignaciones →
            </Link>
          </>
        )}
      </Panel>
    </div>
  )
}
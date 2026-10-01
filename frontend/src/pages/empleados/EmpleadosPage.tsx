import { useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { empleadosApi, type FiltroEmpleados } from '../../api/empleados'
import type { Empleado } from '../../api/types'
import { useAuth } from '../../auth/AuthContext'
import { useNotificar } from '../../components/Notificaciones'
import { ConfirmarModal } from '../../components/ui/ConfirmarModal'
import { Paginacion } from '../../components/ui/Paginacion'
import { claseBotonPrimario, claseInput } from '../../components/ui/estilos'
import { useDebounce } from '../../hooks/useDebounce'
import { EmpleadoFormModal } from './EmpleadoFormModal'

const claseAccion = 'text-sm font-medium hover:underline disabled:cursor-not-allowed disabled:opacity-40 disabled:no-underline'

export function EmpleadosPage() {
  const { usuario } = useAuth()
  const esAdmin = usuario?.rol === 'Admin'
  const queryClient = useQueryClient()
  const notificar = useNotificar()

  const [busqueda, setBusqueda] = useState('')
  const [departamento, setDepartamento] = useState('')
  const [activo, setActivo] = useState<FiltroEmpleados['activo']>('true')
  const [pagina, setPagina] = useState(1)
  const busquedaRetrasada = useDebounce(busqueda)
  const departamentoRetrasado = useDebounce(departamento)

  const [formulario, setFormulario] = useState<{ empleado: Empleado | null } | null>(null)
  const [empleadoDesactivar, setEmpleadoDesactivar] = useState<Empleado | null>(null)

  const filtro: FiltroEmpleados = {
    busqueda: busquedaRetrasada,
    departamento: departamentoRetrasado,
    activo,
    pagina,
    tamanoPagina: 10,
  }

  const { data, isLoading, isError, error, isFetching } = useQuery({
    queryKey: ['empleados', filtro],
    queryFn: () => empleadosApi.listar(filtro),
    placeholderData: keepPreviousData,
  })

  function refrescar() {
    queryClient.invalidateQueries({ queryKey: ['empleados'] })
    queryClient.invalidateQueries({ queryKey: ['dashboard'] })
  }

  const desactivar = useMutation({
    mutationFn: (empleado: Empleado) => empleadosApi.desactivar(empleado.id),
    onSuccess: (_, empleado) => {
      refrescar()
      notificar('exito', `${empleado.nombre} ${empleado.apellido} fue desactivado.`)
      setEmpleadoDesactivar(null)
    },
    onError: (err) => {
      notificar('error', err.message)
      setEmpleadoDesactivar(null)
    },
  })

  const activar = useMutation({
    mutationFn: (empleado: Empleado) => empleadosApi.activar(empleado.id),
    onSuccess: (_, empleado) => {
      refrescar()
      notificar('exito', `${empleado.nombre} ${empleado.apellido} fue reactivado.`)
    },
    onError: (err) => notificar('error', err.message),
  })

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900">Empleados</h1>
        <button onClick={() => setFormulario({ empleado: null })} className={claseBotonPrimario}>
          + Nuevo empleado
        </button>
      </div>

      <div className="grid gap-3 sm:grid-cols-3">
        <input
          value={busqueda}
          onChange={(e) => {
            setBusqueda(e.target.value)
            setPagina(1)
          }}
          placeholder="Buscar por nombre, apellido o correo…"
          className={claseInput}
        />
        <input
          value={departamento}
          onChange={(e) => {
            setDepartamento(e.target.value)
            setPagina(1)
          }}
          placeholder="Departamento exacto (ej. Finanzas)"
          className={claseInput}
        />
        <select
          value={activo}
          onChange={(e) => {
            setActivo(e.target.value as FiltroEmpleados['activo'])
            setPagina(1)
          }}
          className={claseInput}
        >
          <option value="true">Solo activos</option>
          <option value="false">Solo inactivos</option>
          <option value="">Todos</option>
        </select>
      </div>

      <div className={`overflow-x-auto rounded-xl bg-white shadow-sm ${isFetching ? 'opacity-70' : ''}`}>
        {isLoading ? (
          <p className="p-6 text-slate-500">Cargando…</p>
        ) : isError ? (
          <p className="p-6 text-red-600">Error: {error.message}</p>
        ) : data && data.items.length === 0 ? (
          <p className="p-6 text-slate-500">No hay empleados que coincidan con los filtros.</p>
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-200 text-slate-500">
              <tr>
                <th className="px-4 py-3">Empleado</th>
                <th className="px-4 py-3">Departamento</th>
                <th className="px-4 py-3">Puesto</th>
                <th className="px-4 py-3 text-center">Equipos</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {data?.items.map((empleado) => (
                <tr key={empleado.id} className="hover:bg-slate-50">
                  <td className="px-4 py-3">
                    <span className="font-medium text-slate-900">
                      {empleado.nombre} {empleado.apellido}
                    </span>
                    <span className="block text-xs text-slate-400">{empleado.email}</span>
                  </td>
                  <td className="px-4 py-3">{empleado.departamento}</td>
                  <td className="px-4 py-3">{empleado.puesto}</td>
                  <td className="px-4 py-3 text-center">{empleado.equiposAsignados}</td>
                  <td className="px-4 py-3">
                    <span
                      className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
                        empleado.activo ? 'bg-green-100 text-green-700' : 'bg-slate-200 text-slate-600'
                      }`}
                    >
                      {empleado.activo ? 'Activo' : 'Inactivo'}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-3">
                      <button
                        onClick={() => setFormulario({ empleado })}
                        className={`${claseAccion} text-slate-700`}
                      >
                        Editar
                      </button>
                      {esAdmin && empleado.activo && (
                        <button
                          onClick={() => setEmpleadoDesactivar(empleado)}
                          disabled={empleado.equiposAsignados > 0}
                          title={
                            empleado.equiposAsignados > 0
                              ? 'Primero registra la devolución de sus equipos'
                              : undefined
                          }
                          className={`${claseAccion} text-red-600`}
                        >
                          Desactivar
                        </button>
                      )}
                      {esAdmin && !empleado.activo && (
                        <button
                          onClick={() => activar.mutate(empleado)}
                          disabled={activar.isPending}
                          className={`${claseAccion} text-green-700`}
                        >
                          Reactivar
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {data && (
        <Paginacion
          pagina={data.pagina}
          totalPaginas={data.totalPaginas}
          totalRegistros={data.totalRegistros}
          onCambiar={setPagina}
        />
      )}

      {formulario && (
        <EmpleadoFormModal empleado={formulario.empleado} onCerrar={() => setFormulario(null)} />
      )}

      {empleadoDesactivar && (
        <ConfirmarModal
          titulo="Desactivar empleado"
          mensaje={`¿Seguro que quieres desactivar a ${empleadoDesactivar.nombre} ${empleadoDesactivar.apellido}? No se le podrán asignar equipos, pero conservará su historial y podrás reactivarlo después.`}
          textoConfirmar="Desactivar"
          procesando={desactivar.isPending}
          onConfirmar={() => desactivar.mutate(empleadoDesactivar)}
          onCerrar={() => setEmpleadoDesactivar(null)}
        />
      )}
    </div>
  )
}
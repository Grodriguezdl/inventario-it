import { useState } from 'react'
import { useSearchParams } from 'react-router'
import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { asignacionesApi, type FiltroAsignaciones } from '../../api/asignaciones'
import type { Asignacion } from '../../api/types'
import { Paginacion } from '../../components/ui/Paginacion'
import { claseBotonPrimario, claseInput } from '../../components/ui/estilos'
import { formatearFecha } from '../../utils/formato'
import { AsignarModal } from './AsignarModal'
import { DevolverModal } from './DevolverModal'

export function AsignacionesPage() {
  // El historial se abre desde Equipos o Empleados con ?equipoId= o ?empleadoId= en la URL
  const [parametros, setParametros] = useSearchParams()
  const equipoId = parametros.get('equipoId') ?? ''
  const empleadoId = parametros.get('empleadoId') ?? ''
  const esHistorial = equipoId !== '' || empleadoId !== ''

  // En un historial mostramos todo; en la vista general, solo las activas
  const [activas, setActivas] = useState<FiltroAsignaciones['activas']>(esHistorial ? '' : 'true')
  const [pagina, setPagina] = useState(1)

  const [asignando, setAsignando] = useState(false)
  const [asignacionDevolver, setAsignacionDevolver] = useState<Asignacion | null>(null)

  const filtro: FiltroAsignaciones = { equipoId, empleadoId, activas, pagina, tamanoPagina: 10 }

  const { data, isLoading, isError, error, isFetching } = useQuery({
    queryKey: ['asignaciones', filtro],
    queryFn: () => asignacionesApi.listar(filtro),
    placeholderData: keepPreviousData,
  })

  // Nombre legible del equipo o empleado del historial, tomado del primer resultado
  const primera = data?.items[0]
  const tituloHistorial = equipoId
    ? `Historial del equipo ${primera?.codigoInventario ?? `#${equipoId}`}`
    : `Historial de ${primera?.empleado ?? `empleado #${empleadoId}`}`

  function quitarFiltro() {
    setParametros({})
    setActivas('true')
    setPagina(1)
  }

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900">Asignaciones</h1>
        <button onClick={() => setAsignando(true)} className={claseBotonPrimario}>
          + Nueva asignación
        </button>
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <select
          value={activas}
          onChange={(e) => {
            setActivas(e.target.value as FiltroAsignaciones['activas'])
            setPagina(1)
          }}
          className={`${claseInput} sm:w-60`}
        >
          <option value="true">Solo activas</option>
          <option value="false">Solo devueltas</option>
          <option value="">Todas</option>
        </select>

        {esHistorial && (
          <span className="flex items-center gap-2 rounded-full bg-slate-200 px-3 py-1 text-sm text-slate-700">
            {tituloHistorial}
            <button onClick={quitarFiltro} aria-label="Quitar filtro" className="text-slate-500 hover:text-slate-900">
              ✕
            </button>
          </span>
        )}
      </div>

      <div className={`overflow-x-auto rounded-xl bg-white shadow-sm ${isFetching ? 'opacity-70' : ''}`}>
        {isLoading ? (
          <p className="p-6 text-slate-500">Cargando…</p>
        ) : isError ? (
          <p className="p-6 text-red-600">Error: {error.message}</p>
        ) : data && data.items.length === 0 ? (
          <p className="p-6 text-slate-500">No hay asignaciones que coincidan con los filtros.</p>
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-200 text-slate-500">
              <tr>
                <th className="px-4 py-3">Equipo</th>
                <th className="px-4 py-3">Empleado</th>
                <th className="px-4 py-3">Asignación</th>
                <th className="px-4 py-3">Devolución</th>
                <th className="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {data?.items.map((a) => (
                <tr key={a.id} className="align-top hover:bg-slate-50">
                  <td className="px-4 py-3">
                    <span className="font-medium text-slate-900">{a.codigoInventario}</span>
                    <span className="block text-xs text-slate-400">{a.equipo}</span>
                  </td>
                  <td className="px-4 py-3">
                    {a.empleado}
                    <span className="block text-xs text-slate-400">{a.departamento}</span>
                  </td>
                  <td className="px-4 py-3">
                    {formatearFecha(a.fechaAsignacion)}
                    <span className="block text-xs text-slate-400">por {a.asignadoPor}</span>
                    {a.observaciones && (
                      <span className="mt-1 block text-xs text-slate-500 italic">"{a.observaciones}"</span>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    {a.activa ? (
                      <span className="rounded-full bg-blue-100 px-2.5 py-0.5 text-xs font-medium text-blue-700">
                        Activa
                      </span>
                    ) : (
                      <>
                        {a.fechaDevolucion && formatearFecha(a.fechaDevolucion)}
                        {a.observacionesDevolucion && (
                          <span className="mt-1 block text-xs text-slate-500 italic">
                            "{a.observacionesDevolucion}"
                          </span>
                        )}
                      </>
                    )}
                  </td>
                  <td className="px-4 py-3 text-right">
                    {a.activa && (
                      <button
                        onClick={() => setAsignacionDevolver(a)}
                        className="text-sm font-medium text-slate-700 hover:underline"
                      >
                        Registrar devolución
                      </button>
                    )}
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

      {asignando && <AsignarModal onCerrar={() => setAsignando(false)} />}

      {asignacionDevolver && (
        <DevolverModal asignacion={asignacionDevolver} onCerrar={() => setAsignacionDevolver(null)} />
      )}
    </div>
  )
}
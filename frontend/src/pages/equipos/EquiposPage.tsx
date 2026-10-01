import { useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { categoriasApi, equiposApi, type FiltroEquipos } from '../../api/equipos'
import type { Equipo, EstadoEquipo } from '../../api/types'
import { useAuth } from '../../auth/AuthContext'
import { useNotificar } from '../../components/Notificaciones'
import { ConfirmarModal } from '../../components/ui/ConfirmarModal'
import { EstadoBadge } from '../../components/ui/EstadoBadge'
import { Paginacion } from '../../components/ui/Paginacion'
import { claseBotonPrimario, claseInput } from '../../components/ui/estilos'
import { useDebounce } from '../../hooks/useDebounce'
import { EquipoFormModal } from './EquipoFormModal'

const claseAccion = 'text-sm font-medium hover:underline disabled:opacity-50'

export function EquiposPage() {
  const { usuario } = useAuth()
  const esAdmin = usuario?.rol === 'Admin'
  const queryClient = useQueryClient()
  const notificar = useNotificar()

  // Filtros
  const [busqueda, setBusqueda] = useState('')
  const [estado, setEstado] = useState<EstadoEquipo | ''>('')
  const [categoriaId, setCategoriaId] = useState('')
  const [pagina, setPagina] = useState(1)
  const busquedaRetrasada = useDebounce(busqueda)

  // Ventanas abiertas
  const [formulario, setFormulario] = useState<{ equipo: Equipo | null } | null>(null)
  const [equipoBaja, setEquipoBaja] = useState<Equipo | null>(null)

  const filtro: FiltroEquipos = {
    busqueda: busquedaRetrasada,
    estado,
    categoriaId,
    pagina,
    tamanoPagina: 10,
  }

  const { data, isLoading, isError, error, isFetching } = useQuery({
    queryKey: ['equipos', filtro],
    queryFn: () => equiposApi.listar(filtro),
    placeholderData: keepPreviousData, // mantiene la tabla visible mientras carga la siguiente página
  })

  const { data: categorias = [] } = useQuery({
    queryKey: ['categorias'],
    queryFn: categoriasApi.listar,
  })

  function refrescar() {
    queryClient.invalidateQueries({ queryKey: ['equipos'] })
    queryClient.invalidateQueries({ queryKey: ['dashboard'] })
  }

  const cambiarEstado = useMutation({
    mutationFn: ({ id, nuevo }: { id: number; nuevo: EstadoEquipo }) =>
      equiposApi.cambiarEstado(id, nuevo),
    onSuccess: (equipo) => {
      refrescar()
      notificar('exito', `${equipo.codigoInventario} ahora está ${equipo.estado === 'EnReparacion' ? 'en reparación' : 'disponible'}.`)
    },
    onError: (err) => notificar('error', err.message),
  })

  const darDeBaja = useMutation({
    mutationFn: (equipo: Equipo) => equiposApi.darDeBaja(equipo.id),
    onSuccess: (_, equipo) => {
      refrescar()
      notificar('exito', `${equipo.codigoInventario} fue dado de baja.`)
      setEquipoBaja(null)
    },
    onError: (err) => {
      notificar('error', err.message)
      setEquipoBaja(null)
    },
  })

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900">Equipos</h1>
        <button onClick={() => setFormulario({ equipo: null })} className={claseBotonPrimario}>
          + Nuevo equipo
        </button>
      </div>

      {/* Filtros: al cambiar cualquiera, volvemos a la página 1 */}
      <div className="grid gap-3 sm:grid-cols-3">
        <input
          value={busqueda}
          onChange={(e) => {
            setBusqueda(e.target.value)
            setPagina(1)
          }}
          placeholder="Buscar por código, marca, modelo o serie…"
          className={claseInput}
        />
        <select
          value={estado}
          onChange={(e) => {
            setEstado(e.target.value as EstadoEquipo | '')
            setPagina(1)
          }}
          className={claseInput}
        >
          <option value="">Todos los estados</option>
          <option value="Disponible">Disponible</option>
          <option value="Asignado">Asignado</option>
          <option value="EnReparacion">En reparación</option>
          <option value="DeBaja">De baja</option>
        </select>
        <select
          value={categoriaId}
          onChange={(e) => {
            setCategoriaId(e.target.value)
            setPagina(1)
          }}
          className={claseInput}
        >
          <option value="">Todas las categorías</option>
          {categorias.map((c) => (
            <option key={c.id} value={c.id}>
              {c.nombre}
            </option>
          ))}
        </select>
      </div>

      <div className={`overflow-x-auto rounded-xl bg-white shadow-sm ${isFetching ? 'opacity-70' : ''}`}>
        {isLoading ? (
          <p className="p-6 text-slate-500">Cargando…</p>
        ) : isError ? (
          <p className="p-6 text-red-600">Error: {error.message}</p>
        ) : data && data.items.length === 0 ? (
          <p className="p-6 text-slate-500">No hay equipos que coincidan con los filtros.</p>
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-200 text-slate-500">
              <tr>
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Equipo</th>
                <th className="px-4 py-3">Categoría</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {data?.items.map((equipo) => (
                <tr key={equipo.id} className="hover:bg-slate-50">
                  <td className="px-4 py-3 font-medium text-slate-900">{equipo.codigoInventario}</td>
                  <td className="px-4 py-3">
                    {equipo.marca} {equipo.modelo}
                    {equipo.numeroSerie && (
                      <span className="block text-xs text-slate-400">S/N {equipo.numeroSerie}</span>
                    )}
                  </td>
                  <td className="px-4 py-3">{equipo.categoria}</td>
                  <td className="px-4 py-3">
                    <EstadoBadge estado={equipo.estado} />
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-3">
                      {equipo.estado !== 'DeBaja' && (
                        <button
                          onClick={() => setFormulario({ equipo })}
                          className={`${claseAccion} text-slate-700`}
                        >
                          Editar
                        </button>
                      )}
                      {equipo.estado === 'Disponible' && (
                        <button
                          onClick={() => cambiarEstado.mutate({ id: equipo.id, nuevo: 'EnReparacion' })}
                          disabled={cambiarEstado.isPending}
                          className={`${claseAccion} text-amber-700`}
                        >
                          A reparación
                        </button>
                      )}
                      {equipo.estado === 'EnReparacion' && (
                        <button
                          onClick={() => cambiarEstado.mutate({ id: equipo.id, nuevo: 'Disponible' })}
                          disabled={cambiarEstado.isPending}
                          className={`${claseAccion} text-green-700`}
                        >
                          Marcar disponible
                        </button>
                      )}
                      {/* Solo el administrador ve la opción de baja */}
                      {esAdmin && (equipo.estado === 'Disponible' || equipo.estado === 'EnReparacion') && (
                        <button
                          onClick={() => setEquipoBaja(equipo)}
                          className={`${claseAccion} text-red-600`}
                        >
                          Dar de baja
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
        <EquipoFormModal equipo={formulario.equipo} onCerrar={() => setFormulario(null)} />
      )}

      {equipoBaja && (
        <ConfirmarModal
          titulo="Dar de baja equipo"
          mensaje={`¿Seguro que quieres dar de baja ${equipoBaja.codigoInventario} (${equipoBaja.marca} ${equipoBaja.modelo})? El equipo conservará su historial, pero ya no podrá asignarse.`}
          textoConfirmar="Dar de baja"
          procesando={darDeBaja.isPending}
          onConfirmar={() => darDeBaja.mutate(equipoBaja)}
          onCerrar={() => setEquipoBaja(null)}
        />
      )}
    </div>
  )
}
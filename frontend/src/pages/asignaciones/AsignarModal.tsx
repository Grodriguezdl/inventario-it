import { useState, type FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { asignacionesApi } from '../../api/asignaciones'
import { ApiError } from '../../api/client'
import { empleadosApi } from '../../api/empleados'
import { equiposApi } from '../../api/equipos'
import type { AsignarRequest } from '../../api/types'
import { useNotificar } from '../../components/Notificaciones'
import { Campo } from '../../components/ui/Campo'
import { ErrorFormulario } from '../../components/ui/ErrorFormulario'
import { Modal } from '../../components/ui/Modal'
import { claseBotonPrimario, claseBotonSecundario, claseInput } from '../../components/ui/estilos'

export function AsignarModal({ onCerrar }: { onCerrar: () => void }) {
  const queryClient = useQueryClient()
  const notificar = useNotificar()
  const [error, setError] = useState<ApiError | null>(null)

  const [equipoId, setEquipoId] = useState('')
  const [empleadoId, setEmpleadoId] = useState('')
  const [observaciones, setObservaciones] = useState('')

  // Solo se pueden asignar equipos disponibles a empleados activos
  const { data: equipos, isLoading: cargandoEquipos } = useQuery({
    queryKey: ['equipos', 'disponibles'],
    queryFn: () =>
      equiposApi.listar({ busqueda: '', estado: 'Disponible', categoriaId: '', pagina: 1, tamanoPagina: 100 }),
  })

  const { data: empleados, isLoading: cargandoEmpleados } = useQuery({
    queryKey: ['empleados', 'activos'],
    queryFn: () =>
      empleadosApi.listar({ busqueda: '', departamento: '', activo: 'true', pagina: 1, tamanoPagina: 100 }),
  })

  const asignar = useMutation({
    mutationFn: (request: AsignarRequest) => asignacionesApi.asignar(request),
    onSuccess: (resultado) => {
      // Una asignación cambia equipos, empleados, asignaciones y el dashboard
      for (const clave of ['asignaciones', 'equipos', 'empleados', 'dashboard']) {
        queryClient.invalidateQueries({ queryKey: [clave] })
      }
      notificar('exito', `${resultado.codigoInventario} asignado a ${resultado.empleado}.`)
      onCerrar()
    },
    onError: (err) => {
      setError(err instanceof ApiError ? err : new ApiError(0, 'No se pudo conectar con el servidor.'))
    },
  })

  function handleSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault()
    setError(null)
    asignar.mutate({
      equipoId: Number(equipoId),
      empleadoId: Number(empleadoId),
      observaciones: observaciones || null,
    })
  }

  const sinEquipos = !cargandoEquipos && equipos?.items.length === 0

  return (
    <Modal titulo="Nueva asignación" onCerrar={onCerrar}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <ErrorFormulario error={error} />

        {sinEquipos && (
          <div className="rounded-md bg-amber-50 px-3 py-2 text-sm text-amber-800">
            No hay equipos disponibles para asignar en este momento.
          </div>
        )}

        <Campo etiqueta="Equipo disponible *">
          <select
            required
            value={equipoId}
            onChange={(e) => setEquipoId(e.target.value)}
            disabled={cargandoEquipos}
            className={claseInput}
          >
            <option value="">{cargandoEquipos ? 'Cargando…' : 'Selecciona un equipo…'}</option>
            {equipos?.items.map((equipo) => (
              <option key={equipo.id} value={equipo.id}>
                {equipo.codigoInventario} · {equipo.marca} {equipo.modelo} ({equipo.categoria})
              </option>
            ))}
          </select>
        </Campo>

        <Campo etiqueta="Empleado *">
          <select
            required
            value={empleadoId}
            onChange={(e) => setEmpleadoId(e.target.value)}
            disabled={cargandoEmpleados}
            className={claseInput}
          >
            <option value="">{cargandoEmpleados ? 'Cargando…' : 'Selecciona un empleado…'}</option>
            {empleados?.items.map((empleado) => (
              <option key={empleado.id} value={empleado.id}>
                {empleado.nombre} {empleado.apellido} · {empleado.departamento}
              </option>
            ))}
          </select>
        </Campo>

        <Campo etiqueta="Observaciones">
          <textarea
            rows={3}
            value={observaciones}
            onChange={(e) => setObservaciones(e.target.value)}
            placeholder="Ej. Entrega con cargador y mochila"
            className={claseInput}
          />
        </Campo>

        <div className="flex justify-end gap-2 pt-2">
          <button type="button" onClick={onCerrar} className={claseBotonSecundario}>
            Cancelar
          </button>
          <button
            type="submit"
            disabled={asignar.isPending || sinEquipos}
            className={claseBotonPrimario}
          >
            {asignar.isPending ? 'Asignando…' : 'Asignar'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
import { useState, type FormEvent } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { asignacionesApi } from '../../api/asignaciones'
import { ApiError } from '../../api/client'
import type { Asignacion, DevolucionRequest } from '../../api/types'
import { useNotificar } from '../../components/Notificaciones'
import { Campo } from '../../components/ui/Campo'
import { ErrorFormulario } from '../../components/ui/ErrorFormulario'
import { Modal } from '../../components/ui/Modal'
import { claseBotonPrimario, claseBotonSecundario, claseInput } from '../../components/ui/estilos'

interface DevolverModalProps {
  asignacion: Asignacion
  onCerrar: () => void
}

const opciones: { valor: DevolucionRequest['estadoEquipo']; titulo: string; descripcion: string }[] = [
  { valor: 'Disponible', titulo: 'En buen estado', descripcion: 'Queda disponible para asignarse de nuevo.' },
  { valor: 'EnReparacion', titulo: 'Necesita reparación', descripcion: 'Queda en reparación hasta que se marque disponible.' },
]

export function DevolverModal({ asignacion, onCerrar }: DevolverModalProps) {
  const queryClient = useQueryClient()
  const notificar = useNotificar()
  const [error, setError] = useState<ApiError | null>(null)
  const [estadoEquipo, setEstadoEquipo] = useState<DevolucionRequest['estadoEquipo']>('Disponible')
  const [observaciones, setObservaciones] = useState('')

  const devolver = useMutation({
    mutationFn: (request: DevolucionRequest) => asignacionesApi.devolver(asignacion.id, request),
    onSuccess: (resultado) => {
      for (const clave of ['asignaciones', 'equipos', 'empleados', 'dashboard']) {
        queryClient.invalidateQueries({ queryKey: [clave] })
      }
      notificar('exito', `Devolución de ${resultado.codigoInventario} registrada.`)
      onCerrar()
    },
    onError: (err) => {
      setError(err instanceof ApiError ? err : new ApiError(0, 'No se pudo conectar con el servidor.'))
    },
  })

  function handleSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault()
    setError(null)
    devolver.mutate({ estadoEquipo, observaciones: observaciones || null })
  }

  return (
    <Modal titulo="Registrar devolución" onCerrar={onCerrar}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <ErrorFormulario error={error} />

        <div className="rounded-md bg-slate-50 px-3 py-2 text-sm text-slate-700">
          <span className="font-medium">{asignacion.codigoInventario}</span> · {asignacion.equipo}
          <span className="block text-slate-500">Asignado a {asignacion.empleado}</span>
        </div>

        <fieldset className="space-y-2">
          <legend className="text-sm font-medium text-slate-700">¿En qué estado se recibe el equipo?</legend>
          {opciones.map((opcion) => (
            <label
              key={opcion.valor}
              className={`flex cursor-pointer gap-3 rounded-md border px-3 py-2 ${
                estadoEquipo === opcion.valor ? 'border-slate-900 bg-slate-50' : 'border-slate-200'
              }`}
            >
              <input
                type="radio"
                name="estadoEquipo"
                value={opcion.valor}
                checked={estadoEquipo === opcion.valor}
                onChange={() => setEstadoEquipo(opcion.valor)}
                className="mt-1"
              />
              <span className="text-sm">
                <span className="font-medium text-slate-900">{opcion.titulo}</span>
                <span className="block text-slate-500">{opcion.descripcion}</span>
              </span>
            </label>
          ))}
        </fieldset>

        <Campo etiqueta="Observaciones">
          <textarea
            rows={3}
            value={observaciones}
            onChange={(e) => setObservaciones(e.target.value)}
            placeholder={estadoEquipo === 'EnReparacion' ? 'Describe la falla…' : 'Opcional'}
            className={claseInput}
          />
        </Campo>

        <div className="flex justify-end gap-2 pt-2">
          <button type="button" onClick={onCerrar} className={claseBotonSecundario}>
            Cancelar
          </button>
          <button type="submit" disabled={devolver.isPending} className={claseBotonPrimario}>
            {devolver.isPending ? 'Registrando…' : 'Registrar devolución'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
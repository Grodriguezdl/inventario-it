import { useState, type FormEvent } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { ApiError } from '../../api/client'
import { empleadosApi } from '../../api/empleados'
import type { Empleado, EmpleadoRequest } from '../../api/types'
import { useNotificar } from '../../components/Notificaciones'
import { Campo } from '../../components/ui/Campo'
import { ErrorFormulario } from '../../components/ui/ErrorFormulario'
import { Modal } from '../../components/ui/Modal'
import { claseBotonPrimario, claseBotonSecundario, claseInput } from '../../components/ui/estilos'

interface EmpleadoFormModalProps {
  empleado: Empleado | null // null = crear uno nuevo
  onCerrar: () => void
}

export function EmpleadoFormModal({ empleado, onCerrar }: EmpleadoFormModalProps) {
  const queryClient = useQueryClient()
  const notificar = useNotificar()
  const [error, setError] = useState<ApiError | null>(null)

  const [datos, setDatos] = useState<EmpleadoRequest>(() => ({
    nombre: empleado?.nombre ?? '',
    apellido: empleado?.apellido ?? '',
    email: empleado?.email ?? '',
    departamento: empleado?.departamento ?? '',
    puesto: empleado?.puesto ?? '',
  }))

  const guardar = useMutation({
    mutationFn: (request: EmpleadoRequest) =>
      empleado ? empleadosApi.actualizar(empleado.id, request) : empleadosApi.crear(request),
    onSuccess: (resultado) => {
      queryClient.invalidateQueries({ queryKey: ['empleados'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
      const nombre = `${resultado.nombre} ${resultado.apellido}`
      notificar('exito', empleado ? `${nombre} actualizado.` : `${nombre} registrado.`)
      onCerrar()
    },
    onError: (err) => {
      setError(err instanceof ApiError ? err : new ApiError(0, 'No se pudo conectar con el servidor.'))
    },
  })

  function cambiar(campo: keyof EmpleadoRequest, valor: string) {
    setDatos((actuales) => ({ ...actuales, [campo]: valor }))
  }

  function handleSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault()
    setError(null)
    guardar.mutate(datos)
  }

  return (
    <Modal
      titulo={empleado ? `Editar a ${empleado.nombre} ${empleado.apellido}` : 'Nuevo empleado'}
      onCerrar={onCerrar}
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        <ErrorFormulario error={error} />

        <div className="grid gap-4 sm:grid-cols-2">
          <Campo etiqueta="Nombre *">
            <input
              required
              value={datos.nombre}
              onChange={(e) => cambiar('nombre', e.target.value)}
              className={claseInput}
            />
          </Campo>
          <Campo etiqueta="Apellido *">
            <input
              required
              value={datos.apellido}
              onChange={(e) => cambiar('apellido', e.target.value)}
              className={claseInput}
            />
          </Campo>
        </div>

        <Campo etiqueta="Correo *">
          <input
            type="email"
            required
            value={datos.email}
            onChange={(e) => cambiar('email', e.target.value)}
            className={claseInput}
          />
        </Campo>

        <div className="grid gap-4 sm:grid-cols-2">
          <Campo etiqueta="Departamento *">
            <input
              required
              value={datos.departamento}
              onChange={(e) => cambiar('departamento', e.target.value)}
              placeholder="Finanzas"
              className={claseInput}
            />
          </Campo>
          <Campo etiqueta="Puesto *">
            <input
              required
              value={datos.puesto}
              onChange={(e) => cambiar('puesto', e.target.value)}
              placeholder="Analista"
              className={claseInput}
            />
          </Campo>
        </div>

        <div className="flex justify-end gap-2 pt-2">
          <button type="button" onClick={onCerrar} className={claseBotonSecundario}>
            Cancelar
          </button>
          <button type="submit" disabled={guardar.isPending} className={claseBotonPrimario}>
            {guardar.isPending ? 'Guardando…' : 'Guardar'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
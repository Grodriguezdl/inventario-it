import { useState, type FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ApiError } from '../../api/client'
import { categoriasApi, equiposApi } from '../../api/equipos'
import type { Equipo, EquipoRequest } from '../../api/types'
import { useNotificar } from '../../components/Notificaciones'
import { Campo } from '../../components/ui/Campo'
import { ErrorFormulario } from '../../components/ui/ErrorFormulario'
import { Modal } from '../../components/ui/Modal'
import { claseBotonPrimario, claseBotonSecundario, claseInput } from '../../components/ui/estilos'

interface EquipoFormModalProps {
  equipo: Equipo | null // null = crear uno nuevo
  onCerrar: () => void
}

export function EquipoFormModal({ equipo, onCerrar }: EquipoFormModalProps) {
  const queryClient = useQueryClient()
  const notificar = useNotificar()
  const [error, setError] = useState<ApiError | null>(null)

  const [datos, setDatos] = useState(() => ({
    codigoInventario: equipo?.codigoInventario ?? '',
    numeroSerie: equipo?.numeroSerie ?? '',
    marca: equipo?.marca ?? '',
    modelo: equipo?.modelo ?? '',
    categoriaId: equipo ? String(equipo.categoriaId) : '',
    fechaAdquisicion: equipo?.fechaAdquisicion ?? '',
    notas: equipo?.notas ?? '',
  }))

  const { data: categorias = [] } = useQuery({
    queryKey: ['categorias'],
    queryFn: categoriasApi.listar,
  })

  const guardar = useMutation({
    mutationFn: (request: EquipoRequest) =>
      equipo ? equiposApi.actualizar(equipo.id, request) : equiposApi.crear(request),
    onSuccess: (resultado) => {
      queryClient.invalidateQueries({ queryKey: ['equipos'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
      notificar(
        'exito',
        equipo
          ? `Equipo ${resultado.codigoInventario} actualizado.`
          : `Equipo ${resultado.codigoInventario} registrado.`,
      )
      onCerrar()
    },
    onError: (err) => {
      setError(err instanceof ApiError ? err : new ApiError(0, 'No se pudo conectar con el servidor.'))
    },
  })

  function cambiar(campo: keyof typeof datos, valor: string) {
    setDatos((actuales) => ({ ...actuales, [campo]: valor }))
  }

  function handleSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault()
    setError(null)
    guardar.mutate({
      codigoInventario: datos.codigoInventario,
      numeroSerie: datos.numeroSerie || null,
      marca: datos.marca,
      modelo: datos.modelo,
      categoriaId: Number(datos.categoriaId),
      fechaAdquisicion: datos.fechaAdquisicion || null,
      notas: datos.notas || null,
    })
  }

  return (
    <Modal titulo={equipo ? `Editar ${equipo.codigoInventario}` : 'Nuevo equipo'} onCerrar={onCerrar}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <ErrorFormulario error={error} />

        <div className="grid gap-4 sm:grid-cols-2">
          <Campo etiqueta="Código de inventario *">
            <input
              required
              value={datos.codigoInventario}
              onChange={(e) => cambiar('codigoInventario', e.target.value)}
              placeholder="LAP-001"
              className={claseInput}
            />
          </Campo>
          <Campo etiqueta="Número de serie">
            <input
              value={datos.numeroSerie}
              onChange={(e) => cambiar('numeroSerie', e.target.value)}
              className={claseInput}
            />
          </Campo>
          <Campo etiqueta="Marca *">
            <input
              required
              value={datos.marca}
              onChange={(e) => cambiar('marca', e.target.value)}
              className={claseInput}
            />
          </Campo>
          <Campo etiqueta="Modelo *">
            <input
              required
              value={datos.modelo}
              onChange={(e) => cambiar('modelo', e.target.value)}
              className={claseInput}
            />
          </Campo>
          <Campo etiqueta="Categoría *">
            <select
              required
              value={datos.categoriaId}
              onChange={(e) => cambiar('categoriaId', e.target.value)}
              className={claseInput}
            >
              <option value="">Selecciona…</option>
              {categorias.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.nombre}
                </option>
              ))}
            </select>
          </Campo>
          <Campo etiqueta="Fecha de adquisición">
            <input
              type="date"
              value={datos.fechaAdquisicion}
              onChange={(e) => cambiar('fechaAdquisicion', e.target.value)}
              className={claseInput}
            />
          </Campo>
        </div>

        <Campo etiqueta="Notas">
          <textarea
            rows={3}
            value={datos.notas}
            onChange={(e) => cambiar('notas', e.target.value)}
            className={claseInput}
          />
        </Campo>

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
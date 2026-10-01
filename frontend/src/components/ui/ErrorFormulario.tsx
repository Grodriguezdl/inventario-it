import type { ApiError } from '../../api/client'

// Muestra los errores de validación por campo, o el mensaje general si no hay
export function ErrorFormulario({ error }: { error: ApiError | null }) {
  if (!error) return null

  const mensajes = error.errores ? Object.values(error.errores).flat() : []

  return (
    <div className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">
      {mensajes.length > 0 ? (
        <ul className="list-inside list-disc">
          {mensajes.map((mensaje) => (
            <li key={mensaje}>{mensaje}</li>
          ))}
        </ul>
      ) : (
        error.message
      )}
    </div>
  )
}
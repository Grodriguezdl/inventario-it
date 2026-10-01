import type { ProblemDetails } from './types'

const TOKEN_KEY = 'inventario.token'

export const tokenStorage = {
  get: () => localStorage.getItem(TOKEN_KEY),
  set: (token: string) => localStorage.setItem(TOKEN_KEY, token),
  clear: () => localStorage.removeItem(TOKEN_KEY),
}

// Error con el código HTTP y el mensaje que envió la API
export class ApiError extends Error {
  status: number
  errores?: Record<string, string[]>

  constructor(status: number, mensaje: string, errores?: Record<string, string[]>) {
    super(mensaje)
    this.status = status
    this.errores = errores
  }
}

type Metodo = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

async function request<T>(metodo: Metodo, ruta: string, cuerpo?: unknown): Promise<T> {
  const headers: Record<string, string> = {}

  const token = tokenStorage.get()
  if (token) {
    headers.Authorization = `Bearer ${token}`
  }
  if (cuerpo !== undefined) {
    headers['Content-Type'] = 'application/json'
  }

  const respuesta = await fetch(`/api${ruta}`, {
    method: metodo,
    headers,
    body: cuerpo !== undefined ? JSON.stringify(cuerpo) : undefined,
  })

  // Token vencido o inválido: avisamos a la app para cerrar la sesión
  if (respuesta.status === 401) {
    tokenStorage.clear()
    window.dispatchEvent(new Event('sesion-expirada'))
  }

  if (!respuesta.ok) {
    let problema: ProblemDetails | undefined
    try {
      problema = await respuesta.json()
    } catch {
      // La respuesta no traía cuerpo
    }
    const mensaje = problema?.detail ?? problema?.title ?? `Error ${respuesta.status}`
    throw new ApiError(respuesta.status, mensaje, problema?.errors)
  }

  if (respuesta.status === 204) {
    return undefined as T
  }

  return respuesta.json() as Promise<T>
}

export const api = {
  get: <T>(ruta: string) => request<T>('GET', ruta),
  post: <T>(ruta: string, cuerpo?: unknown) => request<T>('POST', ruta, cuerpo),
  put: <T>(ruta: string, cuerpo?: unknown) => request<T>('PUT', ruta, cuerpo),
  patch: <T>(ruta: string, cuerpo?: unknown) => request<T>('PATCH', ruta, cuerpo),
  delete: <T = void>(ruta: string) => request<T>('DELETE', ruta),
}
// Convierte un objeto en query string, omitiendo los valores vacíos
export function construirQuery(params: Record<string, string | number | null | undefined>) {
  const query = new URLSearchParams()
  for (const [clave, valor] of Object.entries(params)) {
    if (valor !== undefined && valor !== null && valor !== '') {
      query.set(clave, String(valor))
    }
  }
  return query.toString()
}
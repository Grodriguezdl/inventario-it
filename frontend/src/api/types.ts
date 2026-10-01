// Reflejan los DTOs del backend (ASP.NET los envía en camelCase)

export type RolUsuario = 'Admin' | 'Tecnico'
export type EstadoEquipo = 'Disponible' | 'Asignado' | 'EnReparacion' | 'DeBaja'

export interface Usuario {
  id: number
  nombre: string
  email: string
  rol: RolUsuario
  activo: boolean
}

export interface LoginResponse {
  token: string
  expiraEn: string
  usuario: Usuario
}

export interface Asignacion {
  id: number
  equipoId: number
  codigoInventario: string
  equipo: string
  empleadoId: number
  empleado: string
  departamento: string
  asignadoPor: string
  fechaAsignacion: string
  fechaDevolucion: string | null
  observaciones: string | null
  observacionesDevolucion: string | null
  activa: boolean
}

export interface Dashboard {
  totalEquipos: number
  empleadosActivos: number
  asignacionesActivas: number
  equiposPorEstado: { estado: EstadoEquipo; total: number }[]
  equiposPorCategoria: { categoria: string; total: number }[]
  ultimasAsignaciones: Asignacion[]
}

// Formato de error que devuelve nuestra API (Problem Details)
export interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
  errors?: Record<string, string[]>
}
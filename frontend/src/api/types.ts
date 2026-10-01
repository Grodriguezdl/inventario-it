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
export interface PaginaResultado<T> {
  items: T[]
  pagina: number
  tamanoPagina: number
  totalRegistros: number
  totalPaginas: number
}

export interface Categoria {
  id: number
  nombre: string
  descripcion: string | null
  totalEquipos: number
}

export interface Equipo {
  id: number
  codigoInventario: string
  numeroSerie: string | null
  marca: string
  modelo: string
  estado: EstadoEquipo
  categoriaId: number
  categoria: string
  fechaAdquisicion: string | null
  notas: string | null
  creadoEn: string
  actualizadoEn: string
}

export interface EquipoRequest {
  codigoInventario: string
  numeroSerie: string | null
  marca: string
  modelo: string
  categoriaId: number
  fechaAdquisicion: string | null
  notas: string | null
}
import { api, construirQuery } from './client'
import type { Empleado, EmpleadoRequest, PaginaResultado } from './types'

export type FiltroEmpleados = {
  busqueda: string
  departamento: string
  activo: '' | 'true' | 'false'
  pagina: number
  tamanoPagina: number
}

export const empleadosApi = {
  listar: (filtro: FiltroEmpleados) =>
    api.get<PaginaResultado<Empleado>>(`/empleados?${construirQuery(filtro)}`),
  crear: (datos: EmpleadoRequest) => api.post<Empleado>('/empleados', datos),
  actualizar: (id: number, datos: EmpleadoRequest) => api.put<Empleado>(`/empleados/${id}`, datos),
  desactivar: (id: number) => api.delete(`/empleados/${id}`),
  activar: (id: number) => api.post<void>(`/empleados/${id}/activar`),
}
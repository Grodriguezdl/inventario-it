import { api, construirQuery } from './client'
import type { AsignarRequest, Asignacion, DevolucionRequest, PaginaResultado } from './types'

export type FiltroAsignaciones = {
  equipoId: string
  empleadoId: string
  activas: '' | 'true' | 'false'
  pagina: number
  tamanoPagina: number
}

export const asignacionesApi = {
  listar: (filtro: FiltroAsignaciones) =>
    api.get<PaginaResultado<Asignacion>>(`/asignaciones?${construirQuery(filtro)}`),
  asignar: (datos: AsignarRequest) => api.post<Asignacion>('/asignaciones', datos),
  devolver: (id: number, datos: DevolucionRequest) =>
    api.post<Asignacion>(`/asignaciones/${id}/devolucion`, datos),
}
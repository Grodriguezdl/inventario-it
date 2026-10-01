import { api, construirQuery } from './client'
import type { Categoria, Equipo, EquipoRequest, EstadoEquipo, PaginaResultado } from './types'

export type FiltroEquipos = {
  busqueda: string
  estado: EstadoEquipo | ''
  categoriaId: string
  pagina: number
  tamanoPagina: number
}

export const equiposApi = {
  listar: (filtro: FiltroEquipos) =>
    api.get<PaginaResultado<Equipo>>(`/equipos?${construirQuery(filtro)}`),
  crear: (datos: EquipoRequest) => api.post<Equipo>('/equipos', datos),
  actualizar: (id: number, datos: EquipoRequest) => api.put<Equipo>(`/equipos/${id}`, datos),
  cambiarEstado: (id: number, estado: EstadoEquipo) =>
    api.patch<Equipo>(`/equipos/${id}/estado`, { estado }),
  darDeBaja: (id: number) => api.delete(`/equipos/${id}`),
}

export const categoriasApi = {
  listar: () => api.get<Categoria[]>('/categorias'),
}
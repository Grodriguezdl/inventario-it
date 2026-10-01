import type { EstadoEquipo } from '../../api/types'

const estilos: Record<EstadoEquipo, { texto: string; clase: string }> = {
  Disponible: { texto: 'Disponible', clase: 'bg-green-100 text-green-700' },
  Asignado: { texto: 'Asignado', clase: 'bg-blue-100 text-blue-700' },
  EnReparacion: { texto: 'En reparación', clase: 'bg-amber-100 text-amber-700' },
  DeBaja: { texto: 'De baja', clase: 'bg-slate-200 text-slate-600' },
}

export function EstadoBadge({ estado }: { estado: EstadoEquipo }) {
  const { texto, clase } = estilos[estado]
  return <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${clase}`}>{texto}</span>
}
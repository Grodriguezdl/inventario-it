import { claseBotonSecundario } from './estilos'

interface PaginacionProps {
  pagina: number
  totalPaginas: number
  totalRegistros: number
  onCambiar: (pagina: number) => void
}

export function Paginacion({ pagina, totalPaginas, totalRegistros, onCambiar }: PaginacionProps) {
  if (totalRegistros === 0) return null

  return (
    <div className="flex items-center justify-between text-sm text-slate-600">
      <span>
        {totalRegistros} registros · Página {pagina} de {totalPaginas}
      </span>
      <div className="flex gap-2">
        <button
          onClick={() => onCambiar(pagina - 1)}
          disabled={pagina <= 1}
          className={claseBotonSecundario}
        >
          Anterior
        </button>
        <button
          onClick={() => onCambiar(pagina + 1)}
          disabled={pagina >= totalPaginas}
          className={claseBotonSecundario}
        >
          Siguiente
        </button>
      </div>
    </div>
  )
}
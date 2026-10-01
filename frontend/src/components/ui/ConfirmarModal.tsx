import { Modal } from './Modal'
import { claseBotonSecundario } from './estilos'

interface ConfirmarModalProps {
  titulo: string
  mensaje: string
  textoConfirmar: string
  procesando: boolean
  onConfirmar: () => void
  onCerrar: () => void
}

export function ConfirmarModal({
  titulo,
  mensaje,
  textoConfirmar,
  procesando,
  onConfirmar,
  onCerrar,
}: ConfirmarModalProps) {
  return (
    <Modal titulo={titulo} onCerrar={onCerrar}>
      <p className="text-sm text-slate-600">{mensaje}</p>
      <div className="mt-6 flex justify-end gap-2">
        <button onClick={onCerrar} className={claseBotonSecundario}>
          Cancelar
        </button>
        <button
          onClick={onConfirmar}
          disabled={procesando}
          className="rounded-md bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 disabled:opacity-60"
        >
          {procesando ? 'Procesando…' : textoConfirmar}
        </button>
      </div>
    </Modal>
  )
}
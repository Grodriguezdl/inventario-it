import { useEffect, type ReactNode } from 'react'

interface ModalProps {
  titulo: string
  onCerrar: () => void
  children: ReactNode
}

export function Modal({ titulo, onCerrar, children }: ModalProps) {
  // Cerrar con la tecla Escape
  useEffect(() => {
    function alPresionar(evento: KeyboardEvent) {
      if (evento.key === 'Escape') onCerrar()
    }
    window.addEventListener('keydown', alPresionar)
    return () => window.removeEventListener('keydown', alPresionar)
  }, [onCerrar])

  return (
    <div
      className="fixed inset-0 z-40 flex items-center justify-center bg-black/40 p-4"
      onClick={onCerrar}
    >
      <div
        role="dialog"
        aria-modal="true"
        className="max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-xl bg-white shadow-xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between border-b border-slate-200 px-5 py-3">
          <h2 className="font-semibold text-slate-900">{titulo}</h2>
          <button onClick={onCerrar} aria-label="Cerrar" className="text-slate-400 hover:text-slate-600">
            ✕
          </button>
        </div>
        <div className="p-5">{children}</div>
      </div>
    </div>
  )
}
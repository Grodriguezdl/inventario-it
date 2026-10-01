import { createContext, useCallback, useContext, useState, type ReactNode } from 'react'

type TipoNotificacion = 'exito' | 'error'

interface Notificacion {
  id: number
  tipo: TipoNotificacion
  mensaje: string
}

type Notificar = (tipo: TipoNotificacion, mensaje: string) => void

const NotificacionesContext = createContext<Notificar>(() => {})

export function NotificacionesProvider({ children }: { children: ReactNode }) {
  const [notificaciones, setNotificaciones] = useState<Notificacion[]>([])

  const notificar = useCallback<Notificar>((tipo, mensaje) => {
    const id = Date.now() + Math.random()
    setNotificaciones((actuales) => [...actuales, { id, tipo, mensaje }])
    setTimeout(() => {
      setNotificaciones((actuales) => actuales.filter((n) => n.id !== id))
    }, 4000)
  }, [])

  return (
    <NotificacionesContext.Provider value={notificar}>
      {children}
      <div className="fixed right-4 bottom-4 z-50 space-y-2">
        {notificaciones.map((n) => (
          <div
            key={n.id}
            className={`rounded-lg px-4 py-3 text-sm text-white shadow-lg ${
              n.tipo === 'exito' ? 'bg-green-600' : 'bg-red-600'
            }`}
          >
            {n.mensaje}
          </div>
        ))}
      </div>
    </NotificacionesContext.Provider>
  )
}

export function useNotificar() {
  return useContext(NotificacionesContext)
}
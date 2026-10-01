import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from 'react'
import { api, tokenStorage } from '../api/client'
import type { LoginResponse, Usuario } from '../api/types'

interface AuthContextValue {
  usuario: Usuario | null
  cargando: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(null)
  // Si hay un token guardado, empezamos "cargando" mientras lo validamos
  const [cargando, setCargando] = useState(() => tokenStorage.get() !== null)

  const logout = useCallback(() => {
    tokenStorage.clear()
    setUsuario(null)
  }, [])

  // Al abrir la app con un token guardado, recuperamos los datos del usuario
  useEffect(() => {
    if (!tokenStorage.get()) return

    api
      .get<Usuario>('/auth/me')
      .then(setUsuario)
      .catch(logout)
      .finally(() => setCargando(false))
  }, [logout])

  // El cliente de la API avisa cuando el token expira
  useEffect(() => {
    window.addEventListener('sesion-expirada', logout)
    return () => window.removeEventListener('sesion-expirada', logout)
  }, [logout])

  const login = useCallback(async (email: string, password: string) => {
    const respuesta = await api.post<LoginResponse>('/auth/login', { email, password })
    tokenStorage.set(respuesta.token)
    setUsuario(respuesta.usuario)
  }, [])

  return (
    <AuthContext.Provider value={{ usuario, cargando, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const contexto = useContext(AuthContext)
  if (!contexto) {
    throw new Error('useAuth debe usarse dentro de AuthProvider')
  }
  return contexto
}
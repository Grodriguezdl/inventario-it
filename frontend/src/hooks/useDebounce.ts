import { useEffect, useState } from 'react'

export function useDebounce<T>(valor: T, milisegundos = 400): T {
  const [valorRetrasado, setValorRetrasado] = useState(valor)

  useEffect(() => {
    const temporizador = setTimeout(() => setValorRetrasado(valor), milisegundos)
    return () => clearTimeout(temporizador)
  }, [valor, milisegundos])

  return valorRetrasado
}
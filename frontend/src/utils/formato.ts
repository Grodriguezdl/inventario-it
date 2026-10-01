// Fechas legibles en español, por ejemplo: "15 oct 2026"
export function formatearFecha(iso: string) {
  return new Date(iso).toLocaleDateString('es', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  })
}
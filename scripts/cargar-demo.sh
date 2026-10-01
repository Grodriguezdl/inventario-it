#!/usr/bin/env bash
# Carga datos de demostración en una instalación nueva de Inventario IT.
# Uso: bash scripts/cargar-demo.sh https://TU-URL.onrender.com
set -euo pipefail

API="${1:?Indica la URL de la API, por ejemplo https://mi-api.onrender.com}/api"

echo "Despertando la API (puede tardar hasta un minuto)..."
curl -s --max-time 120 "${API%/api}/health" > /dev/null || true

read -r -p "Correo del administrador: " EMAIL
read -r -s -p "Contraseña: " PASSWORD; echo

TOKEN=$(curl -s --max-time 60 -X POST "$API/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}" \
  | sed -n 's/.*"token":"\([^"]*\)".*/\1/p')

if [ -z "$TOKEN" ]; then
  echo "No se pudo iniciar sesión. Revisa el correo y la contraseña."
  exit 1
fi

# Envía una petición POST y devuelve el Id del registro creado
crear() {
  curl -s --max-time 60 -X POST "$API$1" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "$2" | sed -n 's/^{"id":\([0-9]*\).*/\1/p'
}

echo "Creando equipos..."
LAP1=$(crear /equipos '{"codigoInventario":"LAP-001","numeroSerie":"DL5440-8821","marca":"Dell","modelo":"Latitude 5440","categoriaId":1,"fechaAdquisicion":"2025-02-10"}')
LAP2=$(crear /equipos '{"codigoInventario":"LAP-002","numeroSerie":"LNT14-3307","marca":"Lenovo","modelo":"ThinkPad T14","categoriaId":1,"fechaAdquisicion":"2025-03-05"}')
LAP3=$(crear /equipos '{"codigoInventario":"LAP-003","numeroSerie":"HP840-1194","marca":"HP","modelo":"EliteBook 840","categoriaId":1,"fechaAdquisicion":"2024-11-20"}')
LAP4=$(crear /equipos '{"codigoInventario":"LAP-004","numeroSerie":"MBA-M3-5520","marca":"Apple","modelo":"MacBook Air M3","categoriaId":1,"fechaAdquisicion":"2025-06-01"}')
DSK1=$(crear /equipos '{"codigoInventario":"DSK-001","numeroSerie":"OPX7010-4410","marca":"Dell","modelo":"OptiPlex 7010","categoriaId":2,"fechaAdquisicion":"2024-08-15"}')
crear /equipos '{"codigoInventario":"DSK-002","numeroSerie":"PD400-7731","marca":"HP","modelo":"ProDesk 400","categoriaId":2,"fechaAdquisicion":"2024-09-02"}' > /dev/null
MON1=$(crear /equipos '{"codigoInventario":"MON-001","numeroSerie":"P2423-0091","marca":"Dell","modelo":"P2423","categoriaId":3,"fechaAdquisicion":"2025-01-12"}')
crear /equipos '{"codigoInventario":"MON-002","numeroSerie":"LG27-6614","marca":"LG","modelo":"27UK850","categoriaId":3,"fechaAdquisicion":"2025-01-12"}' > /dev/null
crear /equipos '{"codigoInventario":"IMP-001","numeroSerie":"LJM404-2208","marca":"HP","modelo":"LaserJet Pro M404","categoriaId":4,"fechaAdquisicion":"2023-12-01"}' > /dev/null
crear /equipos '{"codigoInventario":"PER-001","marca":"Logitech","modelo":"MX Keys","categoriaId":5}' > /dev/null
crear /equipos '{"codigoInventario":"RED-001","numeroSerie":"U6L-9902","marca":"Ubiquiti","modelo":"UniFi U6 Lite","categoriaId":6,"fechaAdquisicion":"2024-05-20"}' > /dev/null
crear /equipos '{"codigoInventario":"RED-002","numeroSerie":"CBS250-3381","marca":"Cisco","modelo":"CBS250-24T","categoriaId":6,"fechaAdquisicion":"2024-05-20"}' > /dev/null

echo "Creando empleados..."
EMP1=$(crear /empleados '{"nombre":"Lucía","apellido":"Méndez","email":"lucia.mendez@empresa-demo.com","departamento":"Finanzas","puesto":"Contadora"}')
EMP2=$(crear /empleados '{"nombre":"Andrés","apellido":"Castillo","email":"andres.castillo@empresa-demo.com","departamento":"Ventas","puesto":"Ejecutivo comercial"}')
EMP3=$(crear /empleados '{"nombre":"Sofía","apellido":"Herrera","email":"sofia.herrera@empresa-demo.com","departamento":"Recursos Humanos","puesto":"Coordinadora"}')
EMP4=$(crear /empleados '{"nombre":"Diego","apellido":"Ramírez","email":"diego.ramirez@empresa-demo.com","departamento":"Operaciones","puesto":"Supervisor"}')
crear /empleados '{"nombre":"Valeria","apellido":"Ortiz","email":"valeria.ortiz@empresa-demo.com","departamento":"Marketing","puesto":"Diseñadora"}' > /dev/null

echo "Creando asignaciones..."
crear /asignaciones "{\"equipoId\":$LAP1,\"empleadoId\":$EMP1,\"observaciones\":\"Entrega con cargador y mochila\"}" > /dev/null
crear /asignaciones "{\"equipoId\":$MON1,\"empleadoId\":$EMP1}" > /dev/null
crear /asignaciones "{\"equipoId\":$LAP2,\"empleadoId\":$EMP2}" > /dev/null
crear /asignaciones "{\"equipoId\":$DSK1,\"empleadoId\":$EMP4}" > /dev/null
ASIG_REP=$(crear /asignaciones "{\"equipoId\":$LAP3,\"empleadoId\":$EMP3}")
ASIG_DEV=$(crear /asignaciones "{\"equipoId\":$LAP4,\"empleadoId\":$EMP2,\"observaciones\":\"Préstamo temporal\"}")

echo "Registrando devoluciones..."
crear "/asignaciones/$ASIG_REP/devolucion" '{"estadoEquipo":"EnReparacion","observaciones":"Teclado con teclas dañadas"}' > /dev/null
crear "/asignaciones/$ASIG_DEV/devolucion" '{"estadoEquipo":"Disponible","observaciones":"Devuelta en buen estado"}' > /dev/null

echo "Creando usuario de demostración..."
crear /usuarios '{"nombre":"Usuario Demo","email":"demo@inventario-it.dev","password":"Demo2026","rol":"Admin"}' > /dev/null

echo "Listo. Usuario de demo: demo@inventario-it.dev / Demo2026"

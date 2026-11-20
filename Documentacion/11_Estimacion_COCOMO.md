# Estimación COCOMO (Modelo Básico) — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Fecha:** 2026-11-20

## 1. Tamaño estimado del software (KLOC)

### Medición/estimación realizada
Conteo sobre el código fuente del proyecto (excluyendo `bin/`, `obj/`, `node_modules/`, `.vs/`):

- **Archivos fuente considerados:** 72 (`.cs`, `.cshtml`, `.csproj`, `.json` relevantes del repositorio de solución)
- **Líneas totales en `.cs` y `.cshtml`:** **4.151 líneas** (medición con PowerShell `Measure-Object -Line`)

Conversión:
- **KLOC = 4.151 / 1000 = 4,151 KLOC** (≈ **4,15 KLOC**)

> **Supuesto:** Se considera el tamaño del software desarrollado/configurado en la solución (capas de presentación, lógica, datos, vistas y configuración). No se incluyen líneas generadas en compilación (`bin/obj`) ni dependencias externas (`node_modules`). El valor es una **medida del estado actual** del repositorio, utilizada como base de estimación.

## 2. Modo de desarrollo seleccionado

**Modo: Orgánico**

**Justificación:**
- Equipo pequeño con experiencia en el dominio/tecnologías del proyecto académico.
- Requisitos relativamente estables y bien definidos por el SRS (RF/RN/CP).
- Tamaño pequeño (≈4 KLOC) y entorno de desarrollo flexible.
- Aplicación de gestión con complejidad moderada, sin restricciones extremas de tiempo real o hardware especializado.

## 3. Coeficientes del modelo básico COCOMO (Orgánico)

| Parámetro | Valor |
|---|---|
| a | 2,4 |
| b | 1,05 |
| c | 2,5 |
| d | 0,38 |

## 4. Cálculos

### 4.1 Esfuerzo (E)
Fórmula: **E = a × (KLOC)^b**

- KLOC^b = (4,151)^1,05
- ln: 1,05 × ln(4,151) = 1,05 × 1,4233 ≈ 1,4945
- (4,151)^1,05 ≈ e^1,4945 ≈ **4,457**
- **E = 2,4 × 4,457 ≈ 10,70 personas-mes**

### 4.2 Tiempo de desarrollo (T)
Fórmula: **T = c × E^d**

- E^d = (10,70)^0,38
- ln: 0,38 × ln(10,70) = 0,38 × 2,3702 ≈ 0,9007
- (10,70)^0,38 ≈ e^0,9007 ≈ **2,462**
- **T = 2,5 × 2,462 ≈ 6,15 meses**

### 4.3 Tamaño promedio del equipo (P)
Fórmula: **P = E / T**

- **P = 10,70 / 6,15 ≈ 1,74 personas** (≈ **2 personas** en la práctica)

### 4.4 Costo estimado
Fórmula: **Costo = E × costo mensual por persona**

**Supuesto de costo:** 2.500 USD por persona-mes (valor referencial académico; no contractual).

- **Costo = 10,70 × 2.500 ≈ 26.750 USD**

> Si se utiliza otro costo mensual (p. ej. 3.000 USD/PM), el costo sería 10,70 × 3.000 ≈ **32.100 USD**. El documento conserva el supuesto declarado.

## 5. Resumen de resultados

| Métrica | Resultado |
|---|---|
| Tamaño | 4,151 KLOC (≈ 4.151 líneas en .cs/.cshtml) |
| Modo | Orgánico (a=2,4; b=1,05; c=2,5; d=0,38) |
| Esfuerzo (E) | ≈ **10,70 personas-mes** |
| Tiempo (T) | ≈ **6,15 meses** |
| Personal (P) | ≈ **1,74 personas** (≈ 2) |
| Costo estimado | ≈ **26.750 USD** (supuesto 2.500 USD/PM) |

## 6. Consideraciones
- COCOMO básico proporciona una estimación de orden de magnitud; no sustituye una planificación detallada.
- El esfuerzo real en un contexto académico puede diferir por alcance, reutilización, curva de aprendizaje y tiempo parcial dedicado.
- La estimación se basa en el tamaño medido del producto software entregado y en supuestos explícitos arriba indicados.

# Resultados de Pruebas — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Fecha de ejecución:** 2026-11-20  
**Entorno:** Windows, .NET 10.0.403, SQL Server Express 2022 `PAOLA\SQLEXPRESS`, BD `CitasMedicasDB`, app en `http://localhost:5199`.

## 1. Resumen ejecutivo
- **Compilación de solución (`dotnet build SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx`):** Correcta — **0 errores, 0 advertencias**.
- **Pruebas automatizadas (`dotnet test`):** No existen proyectos de pruebas en la solución; no se ejecutaron suites automatizadas (PENDIENTE).
- **Pruebas funcionales/sistema ejecutadas (HTTP sobre app real + SQL Server):** 21 verificaciones automatizadas por script, **21/21 aprobadas**.

## 2. Casos CP-01 a CP-10

| Código | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado | Observaciones |
|---|---|---|---|---|---|---|
| CP-01 | Iniciar sesión con datos correctos | Usuario `admin`, contraseña válida (cuenta activa) | Permite ingresar | Redirige al panel principal/Dashboard autenticado | **APROBADO** | Ejecutado tras creación de admin inicial. |
| CP-02 | Iniciar sesión con contraseña incorrecta | Usuario `admin`, contraseña incorrecta | Muestra error | Se muestra mensaje de credenciales incorrectas/cuenta inactiva; no ingresa | **APROBADO** | Validación en servidor. |
| CP-03 | Registrar un paciente | Documento `87654321`, Ana López, datos válidos | Guarda el paciente | Paciente registrado y visible en listado | **APROBADO** | Persistido en SQL Server. |
| CP-04 | Buscar un paciente por documento | Búsqueda `87654321` | Muestra los datos | Se muestran datos del paciente correspondiente | **APROBADO** | Búsqueda funcional. |
| CP-05 | Registrar una cita disponible | Paciente/medico/especialidad válidos, fecha futura, hora disponible `09:00` | Programa la cita | Cita programada correctamente (estado Programada) | **APROBADO** | Disponibilidad verificada contra BD. |
| CP-06 | Registrar una cita ocupada del mismo médico | Mismo médico y misma fecha/hora que cita activa existente | Rechaza el registro | Rechazo con mensaje de cita activa/duplicada (RF-11) | **APROBADO** | Citas canceladas no bloquean. |
| CP-07 | Modificar una cita | Cambio de fecha/hora/observaciones de cita no cancelada | Actualiza la información | Cita actualizada correctamente y reflejada en consulta | **APROBADO** | Revalida disponibilidad. |
| CP-08 | Cancelar una cita | Cita activa existente | Cambia el estado a Cancelada | Estado cambia a Cancelada; historial conservado | **APROBADO** | Cancelación lógica (sin borrado físico). |
| CP-09 | Consultar citas por fecha | Fecha de cita registrada | Muestra las correspondientes | Se muestran las citas de la fecha filtrada | **APROBADO** | Filtro por fecha operativo. |
| CP-10 | Acceder sin autorización | Usuario `recep1` (Recepcionista) accede a `/Usuarios` | Impide el acceso | Redirigido a Acceso denegado (sin acceso al módulo) | **APROBADO** | Autorización por rol aplicada. |

## 3. Pruebas adicionales ejecutadas

| Código | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado | Observaciones |
|---|---|---|---|---|---|---|
| AD-01 | Página de login con sistema sin usuarios | BD sin usuarios | Indica configuración inicial requerida | Muestra opción de crear administrador inicial | **APROBADO** | Flujo seguro de primer acceso. |
| AD-02 | Creación de administrador inicial | Datos válidos en `/Cuenta/Instalacion` | Crea admin y permite login | Administrador creado y redirigido a login | **APROBADO** | Contraseña almacenada con hash bcrypt. |
| AD-03 | Acceso de Administrador a Usuarios | Sesión admin | Permite acceso | Acceso correcto al módulo Usuarios | **APROBADO** | Control por rol. |
| AD-04 | CRUD Especialidad | Alta de "Medicina General" | Registro exitoso | Especialidad registrada y listada | **APROBADO** | Datos reales en BD. |
| AD-05 | CRUD Médico | Alta médico con matrícula y especialidad | Registro exitoso | Médico registrado y listado | **APROBADO** | Vinculado a especialidad existente. |
| AD-06 | Creación de usuario Recepcionista | Datos válidos, rol Recepcionista | Usuario creado | Usuario `recep1` creado y visible | **APROBADO** | Gestión desde admin. |
| AD-07 | Acceso Recepcionista a Pacientes | Sesión `recep1` | Permite acceso | Acceso correcto | **APROBADO** | Según perfil. |
| AD-08 | Acceso Recepcionista a Citas | Sesión `recep1` | Permite acceso | Acceso correcto | **APROBADO** | Según perfil. |
| RN-04 | Rechazo de cita con fecha/hora pasada | Fecha anterior a la actual | Rechaza registro | Mensaje de validación RN-04/fechas pasadas | **APROBADO** | Regla aplicada en lógica de negocio. |

## 4. Compilación y verificación técnica

| Verificación | Comando / método | Resultado | Estado |
|---|---|---|---|
| Restauración/compilación | `dotnet build SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx -v minimal` | Compilación correcta, 0 advertencias, 0 errores | **COMPLETADO Y VERIFICADO** |
| Pruebas automatizadas | `dotnet test` | Sin proyectos de test en la solución | **PENDIENTE** |
| Migraciones | `dotnet ef migrations list` y consulta a `__EFMigrationsHistory` | `20261117193543_Inicial`, `20261119162509_ActualizacionModeloSeguridad` aplicadas | **COMPLETADO Y VERIFICADO** |
| Estado BD | Consultas a tablas Usuarios/Roles/Pacientes/Especialidades/Medicos/Citas | Esquema y datos de demostración presentes | **COMPLETADO Y VERIFICADO** |

## 5. Datos de demostración preparados (ficticios)
- Usuarios: `admin` (Administrador), `recep1` (Recepcionista). Contraseñas definidas durante pruebas/instalación (hash bcrypt en BD, no almacenadas en texto plano).
- Especialidades: Medicina General, Pediatría, Odontología.
- Médicos: Carlos Pérez (MAT-001, Medicina General), María Quispe (MAT-002, Pediatría), Luis Fernández (MAT-003, Odontología).
- Pacientes: registro existente previo (documento 12345678) y pacientes ficticios adicionales (Ana López, Pedro Gómez, Lucía Mamani).
- Citas: ejemplos en estados Programada, Atendida y Cancelada para dashboard y reportes.

## 6. Limitaciones y pendientes de verificación
- No se ejecutaron pruebas unitarias/integración automatizadas por falta de proyecto de pruebas (deuda técnica documentada).
- No se realizó prueba manual interactiva en navegador por un usuario humano dentro de esta ejecución automatizada; las verificaciones funcionales se realizaron por HTTP automatizado contra la app real.
- Exportación "Excel" implementada como CSV compatible; exportación PDF mediante vista imprimible/navegador (sin librería PDF dedicada). Requiere verificación visual en navegador/Excel según entorno del usuario (**IMPLEMENTADO, PENDIENTE DE VERIFICACIÓN** visual final).
- Verificación UI responsive/tema claro-oscuro en múltiples dispositivos/navegadores: **IMPLEMENTADO, PENDIENTE DE VERIFICACIÓN** visual completa.

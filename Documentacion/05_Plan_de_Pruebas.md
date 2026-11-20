# Plan de Pruebas — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas para un Centro de Salud  
**Versión:** v1.0  
**Fecha:** 2026-11-20  
**Alcance:** Sistema web ASP.NET Core MVC (.NET 10) con SQL Server (CitasMedicasDB).

## 1. Objetivo
Verificar que las funcionalidades implementadas cumplen los requisitos funcionales (RF-01 a RF-17), reglas de negocio (RN), requisitos no funcionales aplicables y los casos de prueba definidos en el SRS (CP-01 a CP-10).

## 2. Estrategia de pruebas
- **Pruebas unitarias/de integración automatizadas:** No se implementó un proyecto de pruebas automatizadas (`dotnet test` no ejecuta casos específicos del dominio por ausencia de proyecto de tests en la solución). Estado: **PENDIENTE** de ampliación futura.
- **Pruebas del sistema (funcionales):** Ejecución manual/automatizada vía HTTP sobre la aplicación en ejecución contra SQL Server real (`PAOLA\SQLEXPRESS`, base `CitasMedicasDB`), validando formularios, reglas de negocio, persistencia y respuestas.
- **Pruebas de aceptación:** Verificación de flujos clave de demostración (instalación de administrador, login, gestión de catálogos, programación/control de citas, reportes y control de acceso por roles).

## 3. Entorno de pruebas
- SO: Windows
- .NET SDK: 10.0.403
- Servidor BD: Microsoft SQL Server Express 2022 (`PAOLA\SQLEXPRESS`)
- Base de datos: `CitasMedicasDB` (migraciones `Inicial` y `ActualizacionModeloSeguridad` aplicadas)
- Navegación/HTTP: PowerShell `Invoke-WebRequest` con sesión y tokens antiforgery
- URL de pruebas: `http://localhost:5199`

## 4. Casos de prueba del SRS (CP-01 a CP-10)

| Código | Descripción | Datos de entrada | Resultado esperado | Tipo |
|---|---|---|---|---|
| CP-01 | Iniciar sesión con datos correctos | Usuario y contraseña válidos de cuenta activa | Permite ingresar y muestra el panel principal | Sistema/Seguridad |
| CP-02 | Iniciar sesión con contraseña incorrecta | Usuario válido, contraseña incorrecta | Muestra mensaje de error y no permite acceso | Sistema/Seguridad |
| CP-03 | Registrar un paciente | Documento, nombres, apellidos, teléfono, fecha nacimiento válidos | Guarda el paciente y lo muestra en el listado | Funcional |
| CP-04 | Buscar un paciente por documento | Documento existente | Muestra los datos del paciente correspondiente | Funcional |
| CP-05 | Registrar una cita disponible | Paciente, médico, especialidad, fecha/hora futura disponible | Programa la cita en estado Programada | Funcional/Negocio |
| CP-06 | Registrar una cita ocupada del mismo médico | Mismo médico, misma fecha y hora con cita activa existente | Rechaza el registro (no duplica) | Funcional/Negocio |
| CP-07 | Modificar una cita | Cambio de fecha/hora/observaciones en cita no cancelada con horario disponible | Actualiza la información y persiste cambios | Funcional |
| CP-08 | Cancelar una cita | Cita activa existente | Cambia el estado a Cancelada y conserva historial | Funcional/Negocio |
| CP-09 | Consultar citas por fecha | Fecha con citas registradas | Muestra las citas correspondientes al filtro | Funcional |
| CP-10 | Acceder sin autorización | Usuario Recepcionista intenta acceder a módulo restringido (Usuarios) | Impide el acceso (redirección a Acceso denegado/403) | Seguridad |

## 5. Pruebas adicionales planificadas
- **RN-04:** No permitir fechas/horas pasadas al programar citas.
- **Instalación segura:** Creación del primer administrador solo cuando no existen usuarios.
- **Autorización por rol:** Recepcionista accede a Pacientes y Citas; no administra Usuarios/Médicos/Especialidades.
- **CRUD Especialidades/Médicos/Usuarios:** Registro, edición, activación/desactivación lógica, validaciones de duplicados.
- **Reportes:** Generación por tipo (día, rango, médico, especialidad, paciente, estados) con totales y exportación CSV/vista imprimible.
- **Dashboard:** Indicadores obtenidos desde SQL Server.

## 6. Criterios de aceptación
- El caso se marca **APROBADO** solo si se ejecuta y el resultado obtenido coincide con el esperado.
- El caso se marca **RECHAZADO** si falla o presenta comportamiento incorrecto.
- Si no puede ejecutarse por limitación del entorno, se documenta como **BLOQUEADO** o **PENDIENTE DE VERIFICACIÓN** con la causa.

## 7. Evidencia
- Resultados registrados en `06_Resultados_de_Pruebas.md`.
- Compilación: `dotnet build` de la solución.
- Base de datos: verificación de migraciones y datos en `CitasMedicasDB`.

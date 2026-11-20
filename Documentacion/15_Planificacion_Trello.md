# Planificación del Proyecto para Trello — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Nota:** Este documento organiza las tareas para trasladarlas a un tablero Trello con las listas **POR HACER**, **EN PROCESO** y **TERMINADO**. No se afirma la existencia de un tablero Trello externo creado automáticamente.

## Listas y tarjetas

### POR HACER (actividades pendientes al cierre de esta planificación)
- (Móvil) Pruebas de integración adicionales y ajustes finales de UX en dispositivos reales.
- (Móvil) Validación exhaustiva de todos los flujos móviles contra API en entorno de despliegue.
- (Entrega) Verificación final del ZIP en un equipo limpio (si aplica).
- (Mejora futura) Implementar proyecto de pruebas automatizadas (unitarias/integración) y ejecutar `dotnet test`.
- (Mejora futura) Endurecer políticas de contraseña, bloqueo por intentos y encabezados de seguridad para producción.

### EN PROCESO (actividades en desarrollo en el momento de corte)
- Ninguna (tareas del alcance web planificado completadas; actividades móviles/documentales según avance de la iteración final). *Actualizar esta lista si se continúa trabajo activo.*

### TERMINADO (actividades completadas y verificadas en el alcance entregado)
#### Análisis
- Revisión del SRS (`RF-01..RF-17`, RNF, CP, reglas) y material INS-600.
- Diagnóstico de la solución existente, estructura, BD, dependencias y estado funcional.
- Identificación de brechas, riesgos y estrategia de implementación sin pérdida de datos.

#### Diseño
- Definición/confirmación de arquitectura de 3 capas (Presentación, Lógica, Datos).
- Diseño de modelo de datos ampliado (usuarios, roles, estados, activación lógica) y reglas de integridad.
- Diseño de interfaz (layout, menú por roles, dashboard, formularios, tema claro/oscuro, responsive).

#### Base de datos
- Revisión de `CitasDbContext`, entidades y relaciones.
- Alineación de paquetes EF Core compatibles con .NET 10.
- Creación y aplicación de migración controlada `ActualizacionModeloSeguridad` conservando datos y migración inicial.
- Verificación de esquema y datos en `CitasMedicasDB`.

#### Interfaz
- Implementación de layout profesional, navegación y componentes UI.
- Vistas para autenticación, dashboard, pacientes, especialidades, médicos, citas, reportes, usuarios y configuración.
- Mensajes de éxito/error, confirmaciones y estados visuales.

#### Autenticación
- Inicio/cierre de sesión con cookies, validación de credenciales y protección de rutas.
- Hash de contraseñas con bcrypt y procedimiento seguro de primer administrador.
- Autorización por roles y página de acceso denegado.

#### Pacientes
- CRUD completo, búsqueda por documento/nombre/apellido, validaciones, activación/desactivación lógica y preservación de historial.

#### Médicos
- CRUD, vínculo con especialidad, validación de matrícula, filtros, activación/desactivación lógica.

#### Especialidades
- CRUD, validación de duplicados, activación/desactivación lógica.

#### Citas
- Programación, modificación, cancelación lógica y consulta con filtros.
- Verificación de disponibilidad real, prevención de duplicados activos, rechazo de fechas pasadas y control de estados (Programada/Atendida/Cancelada).

#### Reportes
- Reportes por día, rango, médico, especialidad, paciente y estado con filtros y totales.
- Exportación CSV y vista imprimible/PDF desde navegador.
- Dashboard con indicadores reales desde SQL Server.

#### Pruebas
- Compilación de la solución (0 errores, 0 advertencias).
- Ejecución de pruebas funcionales/sistema CP-01 a CP-10 y reglas adicionales (documentadas con resultados).
- Preparación de datos ficticios de demostración.

#### Seguridad
- Protección antiforgery, autorización en servidor, validaciones, manejo controlado de errores y documentación de seguridad.

#### Documentación
- Elaboración de arquitectura, diagrama/diccionario de BD, plan y resultados de pruebas, manuales, seguridad, COCOMO, registro de cambios, guías y versión v1.0.
- Preparación de planificación para Trello.

#### Entrega
- Organización de entregables del sistema web y preparación de respaldo/estructura de entrega.
- Control de versiones local (Git) y etiquetado de versión según procedimiento de entrega.

## Sugerencia de etiquetas Trello
- **Módulo:** Auth, Pacientes, Médicos, Especialidades, Citas, Reportes, Dashboard, BD, UI, Seguridad, Docs, Pruebas, Móvil, Entrega.
- **Prioridad:** Alta / Media / Baja.
- **Tipo:** Análisis / Diseño / Desarrollo / Prueba / Documentación.

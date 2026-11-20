# Registro de Cambios — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión documentada:** v1.0  
**Fecha:** 2026-11-20

> Nota: Este registro resume los cambios de implementación realizados sobre el proyecto existente, respetando la arquitectura de 3 capas, la base de datos `CitasMedicasDB` y los avances previos (migración inicial, CRUD parcial de pacientes, diseño base).

## [1.0.0] - 2026-11-20 (Entrega)

### Agregado
- **Autenticación y seguridad:** Login con cookies, cierre de sesión seguro (POST + antiforgery), control de acceso por roles (Administrador/Recepcionista), página de acceso denegado, procedimiento seguro de **instalación de primer administrador** (solo si no existen usuarios).
- **Gestión de usuarios y roles (RF-17):** CRUD administrativo de usuarios, asignación de roles, activación/desactivación lógica, cambio de contraseña opcional, protección de reglas de administración. Entidades `Usuario`, `Rol` y constantes `RolesSistema`.
- **Especialidades (RF-07):** CRUD completo con validación de duplicados, activación/desactivación lógica y vistas.
- **Médicos (RF-05, RF-06):** CRUD completo con vínculo a especialidad, validación de matrícula duplicada, filtros (búsqueda/especialidad), activación/desactivación lógica y vistas.
- **Citas médicas (RF-08 a RF-15):** Programación, modificación, consulta con filtros, detalle, cancelación lógica, cambio de estado (Programada/Atendida/Cancelada), verificación de disponibilidad real contra BD, prevención de duplicados activos por médico/fecha-hora, rechazo de fechas/horas pasadas (RN-04), endpoints de apoyo (médicos por especialidad, horarios ocupados, validación).
- **Reportes (RF-16):** Reportes por día, rango, médico, especialidad, paciente y estado con filtros y totales; exportación CSV (compatible con Excel) y vista imprimible para PDF desde navegador.
- **Dashboard:** Indicadores reales desde SQL Server (pacientes/médicos/especialidades activos, citas programadas hoy, atendidas, canceladas, próximas y últimas citas).
- **Configuración:** Vista informativa del sistema, entorno, base de datos y políticas operativas.
- **ViewModels:** Modelos para cuenta/instalación, usuarios y formularios/filtros de citas y reportes.
- **Documentación:** Plan y resultados de pruebas, manuales de instalación/usuario/administrador, seguridad, estimación COCOMO, arquitectura y base de datos/diccionario, guía de ejecución y demostración, planificación Trello y registro de versión.

### Modificado
- **Modelo de datos y contexto EF Core:** Ampliación de entidades (`Paciente`, `Medico`, `Especialidad` con estado activo, `Cita` con estados controlados), configuración de relaciones, índices/restricciones (unicidad, horario activo por médico), eliminación restringida para preservar historial y seed de roles.
- **Servicios de lógica de negocio:** Implementación/ampliación de `PacienteServicio`, `EspecialidadServicio`, `MedicoServicio`, `CitaServicio`, `UsuarioServicio`, `ReporteServicio` con validaciones y reglas del SRS.
- **Controladores y vistas:** Actualización de `PacientesController` y vistas (búsqueda, edición, detalle, activación lógica); nuevos controladores/vistas para autenticación, usuarios, especialidades, médicos, citas, reportes y configuración; `HomeController`/`Home/Index` convertidos en dashboard real.
- **Interfaz y navegación:** Nuevo layout con menú por roles, información de sesión, tema claro/oscuro funcional, diseño responsive y mensajes de retroalimentación.
- **Configuración de arranque:** `Program.cs` con autenticación por cookies, autorización, localización y registro de servicios.
- **Dependencias:** Alineación de paquetes EF Core a 10.0.15 y adición de `BCrypt.Net-Next` (4.1.0) en proyectos correspondientes.

### Eliminado
- Archivos de plantilla innecesarios `Class1.cs` en proyectos `SistemaCitasMedicas.Datos` y `SistemaCitasMedicas.Logica`.
- Enlaces/elementos de menú "Próximamente" sin funcionalidad real (reemplazados por módulos implementados).

### Base de datos (migraciones)
- **Conservada:** `20261117193543_Inicial`.
- **Agregada y aplicada:** `20261119162509_ActualizacionModeloSeguridad` (usuarios/roles, estados/activos, restricciones e índices de integridad). Sin eliminación de datos existentes ni recreación de la base `CitasMedicasDB`.

### Verificación
- Compilación: `dotnet build` — 0 errores, 0 advertencias.
- Pruebas funcionales: CP-01 a CP-10 y reglas adicionales ejecutadas sobre app real y SQL Server (ver `06_Resultados_de_Pruebas.md`).
- Pruebas automatizadas: no disponibles en la solución (pendiente).

### Notas
- No se modificó la finalidad del proyecto ni se eliminaron requisitos del SRS.
- No se incluyeron credenciales/secretos en el código fuente.
- La aplicación móvil independiente se desarrolla como entregable separado (ver documentación y carpeta correspondiente). 

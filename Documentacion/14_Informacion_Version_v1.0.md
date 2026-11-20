# Información de Versión v1.0 — SALUD GESTIÓN

**Producto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas para un Centro de Salud  
**Versión:** **v1.0**  
**Fecha:** 2026-11-20  
**Plataforma:** ASP.NET Core MVC (.NET 10, C#)  
**Base de datos:** Microsoft SQL Server (`CitasMedicasDB`) con Entity Framework Core 10

## 1. Alcance de la versión
Versión inicial completa del sistema web según SRS, que incluye:
- Autenticación segura (bcrypt), autorización por roles (Administrador, Recepcionista) e instalación de primer administrador.
- Gestión de Pacientes, Especialidades, Médicos y Usuarios (según permisos).
- Gestión de Citas médicas con disponibilidad real, prevención de duplicados activos, control de estados y reglas de negocio (incl. RN-04).
- Reportes con filtros/totales y exportación (CSV/impresión), dashboard con indicadores reales.
- Interfaz responsive con tema claro/oscuro e identidad "SALUD GESTIÓN".

## 2. Componentes
- **Solución:** `SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx`
- **Proyectos:** `SistemaCitasMedicas` (Presentación), `SistemaCitasMedicas.Logica` (Negocio), `SistemaCitasMedicas.Datos` (Datos/EF Core)
- **Migraciones EF Core:** `20261117193543_Inicial`, `20261119162509_ActualizacionModeloSeguridad`

## 3. Requisitos y dependencias principales
- .NET SDK 10.0.x
- SQL Server con instancia configurada (referencia `PAOLA\SQLEXPRESS`) y base `CitasMedicasDB`
- Paquetes NuGet relevantes: Entity Framework Core 10.0.15 (alineado), BCrypt.Net-Next 4.1.0

## 4. Estado de verificación (resumen)
- Compilación web: **0 errores, 0 advertencias** (`dotnet build SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx`).
- Pruebas funcionales del sistema (CP-01 a CP-10 y adicionales): **ejecutadas y aprobadas** en entorno de desarrollo contra SQL Server (ver `06_Resultados_de_Pruebas.md`).
- Pruebas automatizadas (`dotnet test`): **no disponibles** (sin proyecto de tests en la solución).
- Control de versiones (Git local, rama `master`):
  - `f6743ee` — feat(datos,logica): modelo EF Core, migraciones y servicios de negocio
  - `c41c34b` — feat(web): autenticacion, controladores, vistas y modulos del sistema
  - `c86e96c` — **(tag: v1.0)** docs: documentacion del proyecto, guias, pruebas y materiales de referencia
  - `f05c55a` — feat(api): agrega API REST con autenticacion JWT para aplicacion movil (posterior a v1.0)
  - `7cd21af` — feat(movil): implementa aplicacion Flutter y documentacion movil (posterior a v1.0)
- App móvil: `flutter pub get`, `flutter analyze` (sin incidencias), `flutter build web` exitoso y verificación de endpoints API con backend activo (**COMPLETADO Y VERIFICADO** en entorno de desarrollo; ejecución en emulador/dispositivo Android físico: **PENDIENTE DE VERIFICACIÓN** por depender del entorno del evaluador).

## 5. Entregables asociados
- Código fuente completo del sistema web (3 capas), API REST/JWT para integración móvil, migraciones y configuración.
- Aplicación móvil independiente Flutter (`SistemaCitasMedicas.Movil`), separada del proyecto web y consumiendo la API (sin acceso directo a SQL Server).
- Documentación: SRS (original), arquitectura, base de datos/diccionario, plan y resultados de pruebas, manuales, seguridad, COCOMO, registro de cambios, guías de ejecución y demostración, planificación y documentación de app móvil.
- Respaldo inicial y estructura de entrega organizada con ZIP final verificable.

## 6. Limitaciones conocidas
- Exportación PDF mediante vista imprimible del navegador (sin librería PDF embebida); exportación Excel como CSV.
- Sin suite de pruebas automatizadas integrada en la solución.
- Requiere entorno con SQL Server y configuración de conexión acorde.

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
- Compilación: **0 errores, 0 advertencias**.
- Pruebas funcionales del sistema (CP-01 a CP-10 y adicionales): **ejecutadas y aprobadas** en entorno de desarrollo contra SQL Server (ver `06_Resultados_de_Pruebas.md`).
- Pruebas automatizadas (`dotnet test`): **no disponibles** (sin proyecto de tests en la solución).
- Etiqueta Git: **`v1.0`** (a crear/registrar en el repositorio local de entrega según procedimiento documentado).

## 5. Entregables asociados
- Código fuente completo (3 capas), migraciones y configuración.
- Documentación: SRS (original), arquitectura, base de datos/diccionario, plan y resultados de pruebas, manuales, seguridad, COCOMO, registro de cambios, guías de ejecución y demostración, planificación.
- Aplicación móvil independiente: entregable separado (ver carpeta/documentación correspondiente).

## 6. Limitaciones conocidas
- Exportación PDF mediante vista imprimible del navegador (sin librería PDF embebida); exportación Excel como CSV.
- Sin suite de pruebas automatizadas integrada en la solución.
- Requiere entorno con SQL Server y configuración de conexión acorde.

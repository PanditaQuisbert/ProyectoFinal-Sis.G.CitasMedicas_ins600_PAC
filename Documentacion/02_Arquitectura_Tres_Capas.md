# Diagrama de Arquitectura de Tres Capas — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Tecnología:** ASP.NET Core MVC (.NET 10), C#, Entity Framework Core, SQL Server

## 1. Visión general

La solución se organiza en tres proyectos/capas con responsabilidades separadas:

```
┌─────────────────────────────────────────────────────────────────────┐
│                     CAPA 1: PRESENTACIÓN                            │
│                 Proyecto: SistemaCitasMedicas                        │
│  ASP.NET Core MVC (Controladores, Vistas Razor, ViewModels, UI,    │
│  autenticación/autorización, validaciones de entrada, navegación,    │
│  dashboard, reportes, mensajes, tema responsive claro/oscuro)       │
└───────────────────────────────┬─────────────────────────────────────┘
                                │ usa (inyección de dependencias)
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                   CAPA 2: LÓGICA DE NEGOCIO                         │
│               Proyecto: SistemaCitasMedicas.Logica                   │
│  Servicios: PacienteServicio, MedicoServicio, EspecialidadServicio, │
│  CitaServicio, UsuarioServicio, ReporteServicio.                    │
│  Reglas: validaciones, disponibilidad, prevención duplicados,       │
│  control de estados, unicidad, desactivación lógica, bcrypt, etc.   │
└───────────────────────────────┬─────────────────────────────────────┘
                                │ usa (DbContext/entidades)
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    CAPA 3: DATOS / ACCESO A DATOS                   │
│                Proyecto: SistemaCitasMedicas.Datos                  │
│  CitasDbContext (EF Core), Entidades, Relaciones, Índices,          │
│  Restricciones, Seed de roles, Migraciones, acceso a SQL Server     │
└───────────────────────────────┬─────────────────────────────────────┘
                                │ ADO.NET / EF Core provider
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                 BASE DE DATOS: SQL Server (PAOLA\SQLEXPRESS)        │
│                          CitasMedicasDB                             │
│   Tablas: Pacientes, Medicos, Especialidades, Citas, Usuarios, Roles│
└─────────────────────────────────────────────────────────────────────┘
```

## 2. Responsabilidades por capa

### Capa 1 — Presentación (`SistemaCitasMedicas`)
- **Controladores MVC:** `CuentaController`, `HomeController`, `PacientesController`, `MedicosController`, `EspecialidadesController`, `CitasController`, `ReportesController`, `UsuariosController`, `ConfiguracionController`.
- **Vistas Razor:** formularios, listados, filtros, detalle, login/instalación, dashboard, reportes, layout con menú por roles y tema.
- **ViewModels:** modelos de entrada/salida para formularios y filtros (`CuentaViewModels`, `UsuarioViewModels`, `CitaViewModels`).
- **Seguridad en UI/flujo:** autenticación por cookies, `[Authorize]`/roles, tokens antiforgery, mensajes de validación.
- **Dependencias:** referencia a `SistemaCitasMedicas.Logica` y `SistemaCitasMedicas.Datos` (para DI de DbContext y tipos compartidos).

### Capa 2 — Lógica de negocio (`SistemaCitasMedicas.Logica`)
- **Servicios:** encapsulan operaciones y reglas del dominio.
  - `PacienteServicio`: registro, actualización, búsqueda, listado, activación/desactivación lógica, validación de duplicados.
  - `EspecialidadServicio`: CRUD, unicidad de nombre, activación lógica.
  - `MedicoServicio`: CRUD, vínculo con especialidad, matrícula única, filtros, activación lógica.
  - `CitaServicio`: programación/modificación/cancelación/cambio de estado, verificación de disponibilidad real, prevención de duplicados activos (RF-11), rechazo de fechas pasadas (RN-04), consultas filtradas.
  - `UsuarioServicio`: validación de credenciales (bcrypt), creación/actualización, gestión de roles y estados, creación de administrador inicial.
  - `ReporteServicio`: dashboard y consultas de reportes desde SQL Server.
- **Dependencias:** referencia a `SistemaCitasMedicas.Datos`.

### Capa 3 — Datos (`SistemaCitasMedicas.Datos`)
- **Contexto:** `CitasDbContext` (DbContext de EF Core).
- **Entidades:** `Paciente`, `Medico`, `Especialidad`, `Cita`, `Usuario`, `Rol`, `EstadosCita`, `RolesSistema`.
- **Configuración de modelo:** claves primarias/foráneas, longitudes, índices (únicos y de consulta), restricciones, precisión de fechas, comportamiento de eliminación restrictivo para preservar historial, seed de roles.
- **Migraciones:** `Inicial` y `ActualizacionModeloSeguridad`.
- **Dependencias:** EF Core / proveedor SQL Server.

## 3. Flujo típico de una solicitud

1. El usuario interactúa con una **Vista** (Capa 1) y envía una petición HTTP.
2. El **Controlador** recibe la petición, valida el modelo de entrada y aplica autorización.
3. El controlador invoca el **Servicio** correspondiente (Capa 2).
4. El servicio aplica reglas de negocio y utiliza **CitasDbContext/Entidades** (Capa 3) para leer/escribir.
5. EF Core traduce las operaciones a consultas/comandos SQL contra **CitasMedicasDB**.
6. El resultado retorna por las capas hasta la vista/respuesta al usuario.

## 4. Principios aplicados
- Separación de responsabilidades entre presentación, negocio y datos.
- Controladores delgados delegando lógica a servicios.
- Persistencia y reglas críticas respaldadas en BD (restricciones/índices) y en lógica de aplicación.
- Seguridad transversal: autenticación, autorización por roles, validaciones y protección antiforgery.

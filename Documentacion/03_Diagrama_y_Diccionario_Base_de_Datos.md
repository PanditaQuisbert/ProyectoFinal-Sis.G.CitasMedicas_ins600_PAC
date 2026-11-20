# Diagrama y Diccionario de Datos — SALUD GESTIÓN

**Base de datos:** `CitasMedicasDB` (SQL Server, instancia `PAOLA\SQLEXPRESS`)  
**ORM:** Entity Framework Core 10  
**Versión:** v1.0

## 1. Diagrama entidad-relación (lógico)

```
┌──────────────┐        1     N        ┌──────────────┐
│   Pacientes  │──────────────────────▶│    Citas     │◀──────────────────────┐
│ PK Id        │                       │ PK Id        │                       │
│ Documento    │                       │ FechaHora    │                       │ N
│ Nombres      │                       │ Estado       │                       │
│ Apellidos    │                       │ Observaciones│                       │
│ Telefono     │                       │ FK PacienteId│                 ┌──────────────┐
│ FechaNac.    │                       │ FK MedicoId  │  N           1  │   Medicos    │
│ Activo       │                       └──────────────┘◀────────────────│ PK Id        │
└──────────────┘                                                 │        │ Nombres      │
                                                                  │        │ Apellidos    │
┌──────────────┐        1     N        ┌──────────────┐          │        │ Matricula    │
│    Roles     │──────────────────────▶│   Usuarios   │          │        │ FK EspecId   │
│ PK Id        │                       │ PK Id        │          │        │ Activo       │
│ Nombre       │                       │ NombreUsuario│          │        └──────┬───────┘
└──────────────┘                       │ NombreCompl. │          │               │ N
                                       │ ContrasenaH. │          │               │ 1
                                       │ Activo       │          │        ┌──────▼───────┐
                                       │ FechaCreacion│          │        │Especialidades│
                                       │ FK RolId     │          │        │ PK Id        │
                                       └──────────────┘          │        │ Nombre       │
                                                                  └───────▶│ Descripcion  │
                                                                           │ Activo       │
                                                                           └──────────────┘
```

**Relaciones principales:**
- `Roles (1) — (N) Usuarios` por `Usuarios.RolId` (Restrict).
- `Especialidades (1) — (N) Medicos` por `Medicos.EspecialidadId` (Restrict).
- `Pacientes (1) — (N) Citas` por `Citas.PacienteId` (Restrict, preserva historial).
- `Medicos (1) — (N) Citas` por `Citas.MedicoId` (Restrict, preserva historial).

**Restricciones/índices relevantes:**
- Únicos: `Pacientes.Documento`, `Medicos.Matricula`, `Especialidades.Nombre`, `Usuarios.NombreUsuario`, `Roles.Nombre`.
- Único filtrado: `Citas (MedicoId, FechaHora)` donde `Estado <> 'Cancelada'` (evita duplicados activos del mismo médico en el mismo horario).
- Índices de consulta: `Citas.FechaHora`, `Citas.Estado`, y claves foráneas según convención EF.

## 2. Diccionario de datos

### 2.1 Roles
| Columna | Tipo | Nulo | Descripción |
|---|---|---:|---|
| Id | int (PK, identidad) | No | Identificador del rol |
| Nombre | nvarchar(50) | No | Nombre del rol (Administrador, Recepcionista). Único |
| Descripcion | nvarchar(200) | Sí | Descripción del rol |

### 2.2 Usuarios
| Columna | Tipo | Nulo | Descripción |
|---|---|---:|---|
| Id | int (PK, identidad) | No | Identificador del usuario |
| NombreUsuario | nvarchar(50) | No | Nombre de acceso. Único |
| NombreCompleto | nvarchar(150) | No | Nombre completo del usuario |
| ContrasenaHash | nvarchar(200) | No | Hash bcrypt de la contraseña (no texto plano) |
| Activo | bit | No | Indica si la cuenta está activa |
| FechaCreacion | datetime2(7) | No | Fecha de creación del registro |
| RolId | int (FK → Roles.Id) | No | Rol asignado |

### 2.3 Pacientes
| Columna | Tipo | Nulo | Descripción |
|---|---|---:|---|
| Id | int (PK, identidad) | No | Identificador del paciente |
| Documento | nvarchar(20) | No | Documento de identidad. Único |
| Nombres | nvarchar(100) | No | Nombres del paciente |
| Apellidos | nvarchar(100) | No | Apellidos del paciente |
| Telefono | nvarchar(20) | No | Teléfono de contacto |
| FechaNacimiento | datetime2(7) | Sí | Fecha de nacimiento |
| Activo | bit | No | Estado lógico del paciente (default 1) |

### 2.4 Especialidades
| Columna | Tipo | Nulo | Descripción |
|---|---|---:|---|
| Id | int (PK, identidad) | No | Identificador de la especialidad |
| Nombre | nvarchar(100) | No | Nombre de la especialidad. Único |
| Descripcion | nvarchar(250) | Sí | Descripción |
| Activo | bit | No | Estado lógico (default 1) |

### 2.5 Medicos
| Columna | Tipo | Nulo | Descripción |
|---|---|---:|---|
| Id | int (PK, identidad) | No | Identificador del médico |
| Nombres | nvarchar(100) | No | Nombres |
| Apellidos | nvarchar(100) | No | Apellidos |
| Matricula | nvarchar(50) | No | Matrícula profesional. Única |
| EspecialidadId | int (FK → Especialidades.Id) | No | Especialidad del médico |
| Activo | bit | No | Estado lógico (default 1) |

### 2.6 Citas
| Columna | Tipo | Nulo | Descripción |
|---|---|---:|---|
| Id | int (PK, identidad) | No | Identificador de la cita |
| FechaHora | datetime2(7) | No | Fecha y hora de la cita |
| Estado | nvarchar(20) | No | Estado: Programada, Atendida o Cancelada |
| Observaciones | nvarchar(500) | Sí | Observaciones |
| PacienteId | int (FK → Pacientes.Id) | No | Paciente asociado |
| MedicoId | int (FK → Medicos.Id) | No | Médico asociado |

## 3. Migraciones aplicadas
1. `20261117193543_Inicial` — esquema inicial.
2. `20261119162509_ActualizacionModeloSeguridad` — seguridad/usuarios/roles, campos de estado, restricciones e índices de integridad.

> La estructura anterior se obtuvo del esquema de `CitasMedicasDB` y de la configuración EF Core del proyecto. Cualquier diferencia menor de tipos/precisiones debe contrastarse con el esquema vigente en el entorno de despliegue.

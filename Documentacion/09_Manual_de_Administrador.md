# Manual de Administrador — SALUD GESTIÓN

**Sistema:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Perfil:** Administrador

## 1. Responsabilidades del Administrador
El rol **Administrador** puede: iniciar sesión; registrar, modificar, activar/desactivar usuarios y asignar roles; registrar y administrar médicos; administrar especialidades; consultar información; generar reportes; y administrar el sistema. La **Recepcionista** no puede administrar usuarios ni cambiar roles.

## 2. Configuración inicial (primer administrador)
- Cuando no existen usuarios, el sistema habilita `/Cuenta/Instalacion` para crear el **primer administrador** de forma segura.
- Defina un usuario, nombre completo y una contraseña robusta (mínimo 6 caracteres; se recomienda mayor complejidad).
- Las contraseñas se almacenan con **hash bcrypt**; nunca en texto plano ni en registros.
- Este procedimiento no requiere ni expone credenciales predeterminadas en el código.

## 3. Gestión de Usuarios y Roles (RF-01, RF-17)
Menú: **Usuarios** (visible y accesible solo para Administrador).

### 3.1 Crear usuario
- Campos: usuario, nombre completo, rol (Administrador o Recepcionista), estado (activo/inactivo), contraseña y confirmación.
- Validaciones: campos obligatorios, unicidad de nombre de usuario, contraseña segura (hash bcrypt).

### 3.2 Editar usuario
- Puede actualizar datos y rol. La contraseña es opcional en edición (dejar en blanco para no cambiarla).
- No se puede asignar un rol inexistente; los roles válidos son los definidos por el sistema.

### 3.3 Activar/Desactivar usuario
- Desactivación lógica: el usuario no podrá iniciar sesión, pero el registro se conserva.
- Protección: el sistema impide que un administrador se desactive a sí mismo desde esta operación y evita dejar el sistema sin administradores activos (según reglas implementadas).

### 3.4 Reglas de seguridad
- No es posible que un usuario normal se convierta en administrador mediante formularios públicos; la gestión de roles está restringida a Administrador y protegida por autorización en servidor.
- Cierre la sesión al terminar, especialmente en equipos compartidos.

## 4. Administración de Especialidades (RF-07)
- Registrar, listar, editar, consultar, activar y desactivar especialidades (nombre, descripción, estado).
- Se evitan nombres duplicados.
- No se eliminan especialidades con dependencias de forma que destruya historial; se usa desactivación lógica cuando corresponde.

## 5. Administración de Médicos (RF-05, RF-06)
- Registrar médicos con nombres, apellidos, matrícula profesional, especialidad y estado.
- Cada médico debe estar vinculado a una especialidad existente.
- Validación de matrícula duplicada.
- Editar, consultar, filtrar por especialidad, activar/desactivar.
- No se eliminan físicamente médicos con citas históricas; se utiliza desactivación lógica para preservar el historial.

## 6. Supervisión de Pacientes y Citas
- El Administrador puede realizar las mismas operaciones de gestión disponibles en los módulos de Pacientes y Citas (registro, modificación, consulta, cancelación lógica, cambio de estado permitido) respetando las reglas de negocio.
- Estados de cita permitidos: **Programada, Atendida, Cancelada**. Las citas canceladas no se reactivan ni se borran.

## 7. Reportes (RF-16)
- Acceso a todos los reportes: citas del día, por rango de fechas, por médico, por especialidad, por paciente y por estado (programadas/atendidas/canceladas), con filtros y totales.
- Exportación: CSV compatible con Excel y vista imprimible para guardar como PDF desde el navegador.

## 8. Base de datos y respaldos
- Motor: Microsoft SQL Server; BD: `CitasMedicasDB`; autenticación integrada de Windows (según configuración).
- **Respaldo:** realizar copias de seguridad periódicas de `CitasMedicasDB` (ej. `BACKUP DATABASE`) mediante herramientas de SQL Server. No ejecutar operaciones destructivas (`DROP DATABASE`, borrado de tablas/datos/migraciones) en producción.
- Migraciones controladas con Entity Framework Core; no eliminar migraciones existentes.

## 9. Configuración y buenas prácticas
- Revisar `appsettings.json` solo para parámetros de despliegue necesarios; no incluir secretos/contraseñas.
- Mantener HTTPS en entornos de despliegue real.
- Verificar periódicamente usuarios activos y roles asignados.
- Ante errores, consultar logs del servidor/aplicación y la documentación de seguridad; no exponer detalles técnicos sensibles a usuarios finales.

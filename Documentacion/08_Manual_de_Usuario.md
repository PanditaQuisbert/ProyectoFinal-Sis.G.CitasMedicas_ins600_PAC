# Manual de Usuario — SALUD GESTIÓN

**Sistema:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Usuarios objetivo:** Personal autorizado del centro de salud (Administrador y Recepcionista).

## 1. Acceso al sistema
1. Ingrese a la URL del sistema en su navegador.
2. Escriba su **usuario** y **contraseña** y presione **Iniciar sesión**.
3. Si las credenciales son incorrectas o la cuenta está inactiva, el sistema mostrará un mensaje de error.
4. Para salir, use la opción **Cerrar sesión** del menú (la sesión se cierra de forma segura).

> Si es la primera vez y no hay usuarios, un administrador debe completar la **configuración inicial** (ver Manual de Instalación/Administrador).

## 2. Interfaz general
- **Menú lateral:** navegación a Dashboard, Pacientes, Médicos, Especialidades, Citas médicas, Reportes, Usuarios (solo Administrador) y Configuración.
- **Tema claro/oscuro:** botón en la barra superior para alternar; la preferencia se guarda en el navegador.
- **Mensajes:** notificaciones de éxito o error tras las operaciones.
- **Estados de cita:** Programada, Atendida, Cancelada (identificados visualmente).

## 3. Dashboard (Panel principal)
Muestra indicadores reales desde la base de datos: pacientes activos, médicos activos, especialidades, citas programadas para hoy, citas atendidas, citas canceladas, próximas citas y últimas citas registradas, además de accesos rápidos.

## 4. Gestión de Pacientes (RF-02, RF-03, RF-04)
- **Listar/Buscar:** busque por documento de identidad, nombre, apellido o teléfono; puede incluir inactivos.
- **Registrar:** complete documento, nombres, apellidos, teléfono y fecha de nacimiento. El sistema valida datos obligatorios y evita documentos duplicados activos.
- **Editar:** modifique los datos del paciente.
- **Ver detalle:** consulte la información registrada.
- **Activar/Desactivar:** desactivación lógica (el historial de citas se conserva; no se elimina físicamente si tiene citas relacionadas).

## 5. Citas médicas (RF-08 a RF-15) — módulo principal
### 5.1 Programar una cita (RF-08, RF-09, RF-10, RF-11, RN-04)
1. Seleccione **Paciente** (activo).
2. Seleccione **Especialidad** y luego el **Médico** correspondiente.
3. Elija **Fecha** y **Hora** disponible. Puede usar **Verificar disponibilidad** (consulta real en la base de datos).
4. Opcionalmente ingrese **Observaciones** y confirme **Programar cita**.
- No se permiten citas en fechas/horas pasadas.
- No se permiten dos citas activas para el mismo médico en la misma fecha y hora. Las citas canceladas no bloquean horarios.

### 5.2 Consultar citas (RF-14)
Filtre por fecha, documento de paciente, médico, especialidad y estado. Los resultados se muestran en tabla con acciones disponibles.

### 5.3 Modificar una cita (RF-12)
En citas no canceladas, puede cambiar paciente/médico/especialidad/fecha/hora/observaciones. El sistema vuelve a verificar disponibilidad antes de guardar.

### 5.4 Cancelar una cita (RF-13)
La cancelación es lógica: la cita pasa a estado **Cancelada**, se conserva en el historial y libera el horario. No se borra físicamente.

### 5.5 Cambiar estado (RF-15)
Estados permitidos: **Programada, Atendida, Cancelada**. Una cita cancelada no puede reactivarse (debe programarse una nueva cita).

## 6. Reportes (RF-16)
Genere reportes reales desde la base de datos: citas del día, por rango de fechas, por médico, por especialidad, por paciente, y por estado (programadas, atendidas, canceladas). Incluyen totales.
- **Exportar Excel (CSV):** descarga un archivo CSV compatible con Excel.
- **Imprimir / PDF:** abre una vista imprimible; puede guardar como PDF desde el navegador.

## 7. Configuración
Consulta información del sistema, base de datos, seguridad y parámetros operativos.

## 8. Consideraciones
- Las operaciones disponibles dependen de su rol. Si intenta acceder a una función no autorizada, verá un mensaje de **Acceso denegado**.
- Verifique siempre los mensajes de validación del formulario antes de guardar.

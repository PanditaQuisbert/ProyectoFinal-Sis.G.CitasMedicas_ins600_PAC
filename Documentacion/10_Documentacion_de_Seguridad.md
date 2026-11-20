# Documentación de Seguridad — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0

## 1. Objetivos de seguridad
Proteger el acceso al sistema, la confidencialidad de credenciales, la integridad de la información clínica/administrativa y la disponibilidad de los datos almacenados en SQL Server, conforme a los requisitos del SRS y buenas prácticas de ASP.NET Core.

## 2. Autenticación (RF-01)
- **Mecanismo:** Autenticación basada en cookies (`CookieAuthenticationDefaults`) con esquema configurado en la aplicación.
- **Inicio de sesión:** Validación de usuario y contraseña contra registros en base de datos; solo cuentas **activas** pueden autenticarse.
- **Sesión:** Cookie de autenticación con expiración configurada (sesión de trabajo) y cierre de sesión explícito (`Logout`) mediante POST con protección antiforgery.
- **Primer administrador:** Procedimiento de instalación inicial (`/Cuenta/Instalacion`) disponible únicamente cuando no existen usuarios; evita credenciales predeterminadas en código.

## 3. Autorización y control de acceso por roles (RF-17, CP-10)
- **Roles:** `Administrador` y `Recepcionista` (sembrados en BD).
- **Administrador:** gestión de usuarios/roles, médicos, especialidades, consultas, reportes y administración del sistema.
- **Recepcionista:** gestión de pacientes y citas, consultas y reportes; **sin** permisos para administrar usuarios, cambiar roles, ni administrar médicos/especialidades.
- **Aplicación:** Atributos `[Authorize]` y `[Authorize(Roles = ...)]` en controladores/acciones; verificación en servidor (no depende solo de la UI).
- **Acceso no autorizado:** redirección a página de **Acceso denegado**; no se exponen operaciones protegidas.

## 4. Protección de contraseñas
- **Hash seguro:** BCrypt (`BCrypt.Net-Next`) para almacenar contraseñas; verificación mediante comparación segura del hash.
- **Sin texto plano:** Las contraseñas nunca se guardan en claro, no se muestran en tablas/vistas y no deben registrarse en logs.
- **Política en formularios:** Longitud mínima aplicada en creación/cambio (ej. mínimo 6 caracteres en validaciones de entrada); se recomienda fortalecer la política en despliegue productivo.

## 5. Protección contra ataques y validación de entradas
- **Antiforgery:** Tokens anti-CSRF (`[ValidateAntiForgeryToken]` y `@Html.AntiForgeryToken()`) en formularios POST (login, instalación, CRUD, cambios de estado, cancelaciones, logout).
- **Validación de datos:** Anotaciones de datos (`[Required]`, `[StringLength]`, `[DataType]`, `[Compare]`, etc.) y validaciones de negocio en servicios/controladores (duplicados, estados permitidos, fechas válidas, correspondencia médico-especialidad).
- **Consultas parametrizadas/ORM:** Entity Framework Core para acceso a datos, reduciendo riesgo de inyección SQL cuando se usa correctamente.
- **Mensajes de error:** Se evitan detalles técnicos sensibles al usuario final; errores se manejan con mensajes controlados.

## 6. Reglas de negocio con impacto en integridad
- Citas: no fechas/horas pasadas (RN-04), no duplicados activos para mismo médico/fecha-hora (RF-11, validación en lógica y restricción/índice en BD), cancelación lógica que preserva historial y libera horario.
- Entidades: unicidad de documento/matrícula/usuario/especialidad según configuración; desactivación lógica para conservar historial; relaciones con eliminación restringida donde aplica para evitar pérdida de historial.

## 7. Transporte y configuración
- **HTTPS:** La aplicación soporta redirección/uso de HTTPS según configuración (`UseHttpsRedirection`, perfiles de lanzamiento); se recomienda HTTPS en todo entorno no local.
- **Cadena de conexión:** Uso de autenticación integrada de Windows en el entorno configurado; no incluir credenciales en repositorio ni secretos embebidos en código.
- **Cookies:** Configuradas para flujo de autenticación de la aplicación; en despliegue se recomienda `HttpOnly`, `Secure` y políticas de SameSite acordes al entorno.

## 8. Copias de seguridad y recuperación
- Realizar respaldos periódicos de `CitasMedicasDB` (SQL Server backup) y del código/configuración versionada.
- No ejecutar comandos destructivos sobre la base de datos; conservar migraciones y datos.

## 9. Limitaciones conocidas / recomendaciones
- No se implementó MFA, bloqueo por intentos fallidos ni políticas avanzadas de contraseñas más allá de las validaciones definidas (mejoras recomendadas para producción).
- Los logs de aplicación son los predeterminados del framework; se recomienda logging estructurado y monitoreo en producción.
- Revisar y endurecer encabezados de seguridad (CSP, HSTS, etc.) según el entorno de despliegue.

# Aplicación Móvil — SALUD GESTIÓN

**Proyecto:** `SistemaCitasMedicas.Movil` (Flutter)  
**Tipo:** Aplicación móvil independiente (proyecto separado y ejecutable)  
**Comunicación:** API REST segura del backend ASP.NET Core (JWT Bearer) — sin conexión directa a SQL Server.

## 1. Tecnología
- **Flutter** (SDK estable instalado en el entorno) con Dart.
- Plataformas configuradas: **Android** y **Web** (proyecto Flutter nativo; la interfaz es de aplicación móvil, no una página web del sistema MVC).
- Dependencias principales: `http` (consumo de API), `shared_preferences` (almacenamiento de sesión/token).

## 2. Backend / API
Se agregó una API REST al backend web (`/api/...`) protegida con **JWT (Bearer)**:
- `POST /api/auth/login` — autenticación (reutiliza `UsuarioServicio` y validación bcrypt).
- `GET /api/dashboard` — indicadores.
- `GET/POST/PUT /api/pacientes` — gestión de pacientes (reutiliza `PacienteServicio`).
- `GET/POST/PUT /api/citas`, `/api/citas/{id}/cancelar`, `/api/citas/{id}/estado`, `/api/citas/disponibilidad`, `/api/citas/validar` — gestión de citas y reglas de negocio (reutiliza `CitaServicio`).
- `GET /api/catalogos/especialidades`, `GET /api/catalogos/medicos` — catálogos.

La lógica de negocio y el acceso a datos permanecen en los servicios/capas del backend; la app móvil consume la API.

## 3. Funcionalidades implementadas en móvil
- Inicio de sesión con usuario/contraseña (JWT) y cierre de sesión.
- Visualización de dashboard (indicadores reales desde API/BD).
- Gestión básica de pacientes: listado, búsqueda, registro y edición.
- Gestión de citas médicas: listado/consulta con filtros (fecha, estado), detalle, programación y modificación con selección de paciente/especialidad/médico, consulta de horarios ocupados/disponibilidad y validación vía API, cancelación y cambio de estado.
- Manejo de mensajes de error/validación devueltos por el backend.

## 4. Configuración y ejecución
1. **Backend en ejecución:** iniciar el sistema web (`dotnet run --project SistemaCitasMedicas/SistemaCitasMedicas.csproj`) y anotar la URL base (ej. `http://localhost:5199`).
2. **Configurar URL de API:** en `lib/config.dart` (o archivo equivalente) ajustar `apiBaseUrl` a la URL del backend. Para emulador Android, usar `http://10.0.2.2:<puerto>` si el backend corre en la máquina host.
3. **Dependencias:** `flutter pub get` dentro de `SistemaCitasMedicas.Movil`.
4. **Ejecutar:**
   - Android (dispositivo/emulador): `flutter run`
   - Web (para verificación funcional rápida): `flutter run -d chrome`
   - Compilar APK: `flutter build apk` (requiere Android SDK configurado).

## 5. Estado y verificación realizada
- **COMPLETADO Y VERIFICADO:** estructura del proyecto Flutter, dependencias (`flutter pub get`), implementación de pantallas/servicios (`lib/config.dart`, `lib/services/*`, login, dashboard, pacientes, citas), análisis estático (`flutter analyze` sin incidencias) y compilación web (`flutter build web` exitoso) en el entorno de desarrollo.
- **COMPLETADO Y VERIFICADO (API):** comunicación real backend-API probada con backend en ejecución (`POST /api/auth/login`, `/api/dashboard`, `/api/pacientes`, `/api/citas`, `/api/catalogos/*`) contra `CitasMedicasDB`.
- **IMPLEMENTADO, PENDIENTE DE VERIFICACIÓN:** ejecución interactiva en emulador/dispositivo Android físico específico del evaluador y recorrido manual completo de UI móvil en dispositivo (depende de configuración local). Alternativa de verificación funcional: ejecutar backend y `flutter run -d chrome` con `apiBaseUrl` apuntando al backend.

## 6. Notas de seguridad
- Autenticación mediante JWT emitido por el backend; el token se envía en cabecera `Authorization: Bearer`.
- No se almacenan contraseñas en la app; no hay conexión directa a SQL Server desde el cliente móvil.
- Para producción se recomienda HTTPS, protección de claves y almacenamiento seguro de tokens.

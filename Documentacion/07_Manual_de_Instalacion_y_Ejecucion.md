# Manual de Instalación y Guía de Ejecución — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0  
**Plataforma:** ASP.NET Core MVC (.NET 10), SQL Server, Entity Framework Core

## 1. Requisitos previos
- **Sistema operativo:** Windows (probado en entorno Windows con autenticación integrada).
- **.NET SDK 10.0.x** instalado (proyecto objetivo `net10.0`). Verificar: `dotnet --version`.
- **Microsoft SQL Server** (Express o superior) con instancia accesible. Configuración de referencia: `PAOLA\SQLEXPRESS`.
- **Herramienta EF Core (opcional, para gestión de migraciones):** `dotnet-ef` compatible con EF Core 10 (ej. 10.0.15). Verificar: `dotnet ef --version`.
- Navegador web moderno (Edge/Chrome/Firefox).

## 2. Base de datos
- Base de datos: **`CitasMedicasDB`** (no eliminar ni recrear; no ejecutar `Drop-Database`).
- Autenticación: **Windows Integrated Security** (según configuración actual).
- Cadena de conexión de referencia (en `SistemaCitasMedicas/appsettings.json`):
  `Server=PAOLA\SQLEXPRESS;Database=CitasMedicasDB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True`
- **Importante:** Conserve esta cadena si es funcional en su equipo. Solo cámbiela si verifica que la nueva configuración funciona y documente el cambio. No use SQLite ni bases de datos ficticias.

### Aplicar migraciones (si es necesario)
Desde la carpeta raíz de la solución:
```powershell
dotnet ef database update --project "SistemaCitasMedicas.Datos" --startup-project "SistemaCitasMedicas" --context CitasDbContext
```
Migraciones incluidas: `20261117193543_Inicial`, `20261119162509_ActualizacionModeloSeguridad`.

## 3. Restaurar y compilar
```powershell
cd "<ruta>\SistemaCitasMedicas_Proyecto_INS-600_PAC"
dotnet restore
dotnet build "SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx"
```
Resultado esperado: compilación correcta sin errores.

## 4. Ejecutar el sistema web
```powershell
dotnet run --project "SistemaCitasMedicas\SistemaCitasMedicas.csproj"
```
O desde Visual Studio 2026: establecer `SistemaCitasMedicas` como proyecto de inicio y ejecutar.

La URL/puerto se muestra en consola (según `launchSettings.json`). Acceda desde el navegador a la dirección indicada (HTTP/HTTPS según perfil).

## 5. Primer acceso (configuración inicial segura)
1. Si no existen usuarios en la base de datos, la pantalla de inicio de sesión mostrará la opción **"Crear administrador inicial"**.
2. Acceda a `/Cuenta/Instalacion` y registre el primer usuario con rol **Administrador** (usuario, nombre completo y contraseña).
3. Este mecanismo solo está disponible mientras no existan usuarios registrados.
4. Inicie sesión con las credenciales creadas. **No se incluyen contraseñas predeterminadas en el código fuente.**

## 6. Configuración manual (si aplica)
- **SQL Server:** Si la instancia no es `PAOLA\SQLEXPRESS`, edite `SistemaCitasMedicas/appsettings.json` → `ConnectionStrings:ConexionSQL` con el `Server` correcto (manteniendo autenticación integrada si corresponde). Verifique conectividad con `sqlcmd -S <instancia> -E -Q "SELECT 1"`.
- **HTTPS:** Puede usar el perfil HTTPS de `launchSettings.json` o `dotnet run --launch-profile https` si el certificado de desarrollo está configurado (`dotnet dev-certs https --trust`).
- **Puertos en uso:** Si el puerto configurado está ocupado, ejecute con `--urls "http://localhost:5199"` (o el puerto deseado).

## 7. Solución de problemas comunes
- **No conecta a BD:** Verifique que el servicio SQL Server esté iniciado, la instancia sea correcta y que el usuario de Windows tenga permisos sobre `CitasMedicasDB`.
- **Error al aplicar migraciones:** No use `Drop-Database`. Revise `__EFMigrationsHistory` y la conectividad; aplique solo migraciones pendientes.
- **Acceso denegado a módulos:** Confirme el rol del usuario (Administrador/Recepcionista) y cierre/reinicie sesión si se cambiaron permisos.

## 8. Ejecución de pruebas
- Compilación: `dotnet build "SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx"`
- Pruebas automatizadas: `dotnet test` (actualmente no hay proyectos de test en la solución).
- Pruebas funcionales: seguir `05_Plan_de_Pruebas.md` y consultar resultados en `06_Resultados_de_Pruebas.md`.

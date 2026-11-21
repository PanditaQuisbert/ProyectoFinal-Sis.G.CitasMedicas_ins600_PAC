# SALUD GESTIÓN — Restablecimiento local de contraseña del administrador existente
#
# Uso (desde la raíz del repositorio):
#   powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Scripts\RestablecerContrasenaAdministrador.ps1
#
# La contraseña se solicita en la terminal del usuario con Console.ReadKey(intercept: true).
# No se escribe en archivos, logs, código fuente, repositorio ni en la salida de este script.
# Solo se modifica la columna ContrasenaHash del usuario Administrador ya existente.
# No se crean, eliminan ni recrean usuarios; no se cambian roles, estado ni fechas.

#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$proyecto = Join-Path $repoRoot 'Scripts\ResetAdminPassword\ResetAdminPassword.csproj'

if (-not (Test-Path -LiteralPath $proyecto)) {
    throw 'No se encontró el proyecto local ResetAdminPassword.csproj.'
}

& dotnet run --project $proyecto -- $repoRoot
exit $LASTEXITCODE

# Guía de Ejecución y Demostración — SALUD GESTIÓN

**Proyecto:** SALUD GESTIÓN — Sistema de Gestión de Citas Médicas  
**Versión:** v1.0

## 1. Guía rápida de ejecución del sistema web

### 1.1 Requisitos
- Windows, .NET SDK 10.0.x, SQL Server (instancia `PAOLA\SQLEXPRESS` o la configurada) con base `CitasMedicasDB`.

### 1.2 Pasos
1. Abrir PowerShell en la carpeta de la solución: `SistemaCitasMedicas_Proyecto_INS-600_PAC`.
2. (Opcional) Restaurar/compilar:
   - `dotnet restore`
   - `dotnet build "SistemaCitasMedicas_Proyecto_INS-600_PAC.slnx"`
3. Aplicar migraciones si corresponde:
   - `dotnet ef database update --project "SistemaCitasMedicas.Datos" --startup-project "SistemaCitasMedicas" --context CitasDbContext`
4. Ejecutar:
   - `dotnet run --project "SistemaCitasMedicas\SistemaCitasMedicas.csproj"`
5. Abrir el navegador en la URL mostrada (ej. `http://localhost:5xxx` o la indicada en consola).
6. Si no hay usuarios, usar **Crear administrador inicial** (`/Cuenta/Instalacion`), luego iniciar sesión.

### 1.3 Credenciales de demostración
- No se incluyen contraseñas en el repositorio/código. Use las cuentas creadas en el entorno de demostración (ej. Administrador y Recepcionista). Si parte de una BD sin usuarios, cree el Administrador inicial mediante el flujo seguro de instalación.

## 2. Guía de demostración ante el docente (paso a paso)

> Preparar previamente: SQL Server iniciado, BD `CitasMedicasDB` disponible, aplicación en ejecución, y datos ficticios cargados (pacientes, especialidades, médicos, citas en distintos estados).

1. **Presentación:** Mostrar nombre "SALUD GESTIÓN" y subtítulo "Sistema de Gestión de Citas Médicas". Explicar brevemente el problema (gestión manual/ineficiente de citas) y el objetivo general/específicos según SRS.
2. **Requisitos:** Resumir RF-01 a RF-17 y RNF-01 a RNF-10 (seguridad, usabilidad, disponibilidad de datos, etc.) según el SRS.
3. **Arquitectura:** Mostrar diagrama/carpeta de 3 capas (Presentación `SistemaCitasMedicas`, Lógica `SistemaCitasMedicas.Logica`, Datos `SistemaCitasMedicas.Datos`).
4. **Base de datos:** Mostrar `CitasMedicasDB` en SQL Server (tablas Pacientes, Medicos, Especialidades, Citas, Usuarios, Roles) y mencionar migraciones aplicadas.
5. **Planificación (Trello):** Presentar la organización de tareas en listas POR HACER / EN PROCESO / TERMINADO (documento de planificación incluido).
6. **Pruebas:** Mostrar plan y resultados (CP-01 a CP-10, RN-04, compilación) del documento de resultados.
7. **Inicio de sesión Administrador:** Ingresar con usuario Administrador y mostrar acceso completo (Dashboard, Usuarios, Médicos, Especialidades, etc.).
8. **Inicio de sesión Recepcionista:** Cerrar sesión e ingresar como Recepcionista; evidenciar que puede gestionar Pacientes/Citas/Reportes/Configuración pero no accede a administración de Usuarios (y módulos administrativos restringidos).
9. **Registrar paciente ficticio:** Ir a Pacientes → Registrar, completar datos y guardar; verificar en listado.
10. **Consultar paciente:** Buscar por documento/nombre y ver detalle.
11. **Registrar cita:** Ir a Citas → Programar, seleccionar paciente, especialidad, médico, fecha y hora; usar "Verificar disponibilidad" y guardar.
12. **Verificar disponibilidad / duplicado:** Intentar registrar otra cita para el mismo médico en la misma fecha/hora activa y mostrar rechazo (RF-11).
13. **Modificar cita:** Editar una cita programada (cambiar hora/observaciones) y guardar.
14. **Cancelar cita:** Cancelar una cita y mostrar cambio a estado Cancelada (historial conservado, horario liberado).
15. **Consultar citas registradas:** Usar filtros (fecha, paciente/documento, médico, especialidad, estado) y mostrar resultados.
16. **Generar reporte:** Abrir Reportes, elegir tipo (día/rango/médico/especialidad/paciente/estado), generar y mostrar totales; demostrar exportación CSV y vista imprimir/PDF.
17. **Git y versión:** Mostrar repositorio local, historial de commits y etiqueta `v1.0` (según entrega).
18. **COCOMO:** Presentar estimación (KLOC medido, modo orgánico, esfuerzo/tiempo/personas/costo y supuestos).
19. **Cierre:** Resaltar cumplimiento de requisitos, seguridad (bcrypt, roles, antiforgery), persistencia en SQL Server y documentación entregada.

## 3. Notas para la demostración
- Utilizar únicamente **datos ficticios** de pacientes.
- Si una funcionalidad depende de datos previos (ej. médicos por especialidad), verificar que existan especialidades y médicos activos.
- Las exportaciones dependen del navegador/Excel del equipo (CSV imprimible/PDF vía navegador).

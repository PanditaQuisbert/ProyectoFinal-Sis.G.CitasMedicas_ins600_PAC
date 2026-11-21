using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;

static string LeerPasswordOculto(string mensaje)
{
    Console.Write(mensaje + ": ");
    var caracteres = new List<char>();
    while (true)
    {
        var tecla = Console.ReadKey(intercept: true);
        if (tecla.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }
        if (tecla.Key == ConsoleKey.Escape)
        {
            Console.WriteLine();
            return string.Empty;
        }
        if (tecla.Key == ConsoleKey.Backspace)
        {
            if (caracteres.Count > 0)
            {
                caracteres.RemoveAt(caracteres.Count - 1);
                Console.Write("\b \b");
            }
            continue;
        }
        if (!char.IsControl(tecla.KeyChar))
        {
            caracteres.Add(tecla.KeyChar);
            Console.Write("*");
        }
    }
    return new string(caracteres.ToArray());
}

var repoRoot = args.Length > 0
    ? args[0]
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

var settingsPath = Path.Combine(repoRoot, "SistemaCitasMedicas", "appsettings.json");
if (!File.Exists(settingsPath))
{
    Console.WriteLine("[ERROR] No se encontró appsettings.json.");
    return 1;
}

using var json = JsonDocument.Parse(File.ReadAllText(settingsPath));
var connection = json.RootElement
    .GetProperty("ConnectionStrings")
    .GetProperty("ConexionSQL")
    .GetString();

if (string.IsNullOrWhiteSpace(connection))
{
    Console.WriteLine("[ERROR] No se encontró la cadena de conexión ConexionSQL.");
    return 1;
}

var options = new DbContextOptionsBuilder<CitasDbContext>()
    .UseSqlServer(connection)
    .Options;

await using var context = new CitasDbContext(options);
var servicio = new UsuarioServicio(context);

var admin = await context.Usuarios
    .Include(u => u.Rol)
    .SingleOrDefaultAsync(u => u.Rol != null
        && u.Rol.Nombre == RolesSistema.Administrador
        && u.Activo);

if (admin is null)
{
    Console.WriteLine("[ERROR] No existe exactamente un administrador activo. No se modificó nada.");
    return 1;
}

var rolAntes = admin.Rol?.Nombre;
var activoAntes = admin.Activo;
var fechaAntes = admin.FechaCreacion;
var totalAntes = await context.Usuarios.CountAsync();
var hashRecepcionAntes = await context.Usuarios
    .Include(u => u.Rol)
    .Where(u => u.Rol != null
        && u.Rol.Nombre == RolesSistema.Recepcionista
        && u.Activo)
    .Select(u => u.ContrasenaHash)
    .FirstOrDefaultAsync();

var primera = LeerPasswordOculto("Nueva contraseña para el administrador existente");
var segunda = LeerPasswordOculto("Confirme la nueva contraseña");

if (primera.Length == 0 || segunda.Length == 0)
{
    Console.WriteLine("[ERROR] Entrada cancelada. No se modificó nada.");
    return 1;
}
if (!string.Equals(primera, segunda, StringComparison.Ordinal))
{
    Console.WriteLine("[ERROR] Las contraseñas no coinciden. No se modificó nada.");
    return 1;
}
if (primera.Length < 6)
{
    Console.WriteLine("[ERROR] La contraseña debe tener al menos 6 caracteres. No se modificó nada.");
    return 1;
}

try
{
    // Única escritura: el hash del administrador ya existente.
    admin.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(primera);
    await context.SaveChangesAsync();

    // Verificación real usando la misma ruta de inicio de sesión de la aplicación.
    var verificado = await servicio.ValidarCredenciales(admin.NombreUsuario, primera);

    var totalDespues = await context.Usuarios.CountAsync();
    var hashRecepcionDespues = await context.Usuarios
        .Include(u => u.Rol)
        .Where(u => u.Rol != null
            && u.Rol.Nombre == RolesSistema.Recepcionista
            && u.Activo)
        .Select(u => u.ContrasenaHash)
        .FirstOrDefaultAsync();

    Console.WriteLine("[RESULTADO] Contraseña del administrador existente actualizada.");
    Console.WriteLine("[TEST] Login/BCrypt con la contraseña escrita: " + (verificado is not null ? "OK" : "FALLO"));
    Console.WriteLine("[TEST] Rol Administrador conservado: "
        + (rolAntes == RolesSistema.Administrador && verificado?.Rol?.Nombre == RolesSistema.Administrador ? "OK" : "FALLO"));
    Console.WriteLine("[TEST] Cuenta activa conservada: "
        + (activoAntes && admin.Activo ? "OK" : "FALLO"));
    Console.WriteLine("[TEST] Fecha de creación conservada: "
        + (admin.FechaCreacion == fechaAntes ? "OK" : "FALLO"));
    Console.WriteLine("[TEST] Número de usuarios sin cambios: "
        + (totalDespues == totalAntes ? "OK" : "FALLO"));
    Console.WriteLine("[TEST] Recepcionista activa y hash intacto: "
        + (hashRecepcionDespues is not null && hashRecepcionDespues == hashRecepcionAntes ? "OK" : "FALLO"));

    return 0;
}
finally
{
    primera = string.Empty;
    segunda = string.Empty;
}

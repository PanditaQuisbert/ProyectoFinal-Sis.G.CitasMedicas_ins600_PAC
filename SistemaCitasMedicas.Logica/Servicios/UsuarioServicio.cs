using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Logica.Servicios
{
    public class UsuarioServicio
    {
        private readonly CitasDbContext _context;

        public UsuarioServicio(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ValidarCredenciales(string nombreUsuario, string contrasena)
        {
            nombreUsuario = (nombreUsuario ?? string.Empty).Trim();
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

            if (usuario == null) return null;

            bool ok = false;
            try { ok = BCrypt.Net.BCrypt.Verify(contrasena ?? string.Empty, usuario.ContrasenaHash); }
            catch { ok = false; }

            return ok ? usuario : null;
        }

        public async Task<List<Usuario>> ObtenerTodos(string? buscar = null)
        {
            var consulta = _context.Usuarios.Include(u => u.Rol).AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                consulta = consulta.Where(u => u.NombreUsuario.Contains(buscar) || u.NombreCompleto.Contains(buscar));
            }
            return await consulta.OrderBy(u => u.NombreUsuario).ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            return await _context.Usuarios.Include(u => u.Rol).AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Rol>> ObtenerRoles()
        {
            return await _context.Roles.AsNoTracking().OrderBy(r => r.Nombre).ToListAsync();
        }

        public async Task<bool> ExisteAlgunUsuario() => await _context.Usuarios.AnyAsync();

        public async Task<(bool Ok, string? Error)> Crear(Usuario usuario, string contrasena)
        {
            usuario.NombreUsuario = (usuario.NombreUsuario ?? string.Empty).Trim();
            usuario.NombreCompleto = (usuario.NombreCompleto ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario)) return (false, "El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto)) return (false, "El nombre completo es obligatorio.");
            if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 6)
                return (false, "La contraseña es obligatoria y debe tener al menos 6 caracteres.");

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
                return (false, "Ya existe un usuario con ese nombre de usuario.");

            if (!await _context.Roles.AnyAsync(r => r.Id == usuario.RolId))
                return (false, "El rol seleccionado no es válido.");

            usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasena);
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.Now;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> Actualizar(int id, Usuario datos, string? nuevaContrasena)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return (false, "El usuario no existe.");

            datos.NombreUsuario = (datos.NombreUsuario ?? string.Empty).Trim();
            datos.NombreCompleto = (datos.NombreCompleto ?? string.Empty).Trim();

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == datos.NombreUsuario && u.Id != id))
                return (false, "Ya existe otro usuario con ese nombre de usuario.");

            if (!await _context.Roles.AnyAsync(r => r.Id == datos.RolId))
                return (false, "El rol seleccionado no es válido.");

            // Evitar dejar el sistema sin administradores activos
            if (usuario.RolId != datos.RolId || (!datos.Activo && usuario.Activo))
            {
                var rolAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolesSistema.Administrador);
                if (rolAdmin != null && usuario.RolId == rolAdmin.Id)
                {
                    int adminsActivos = await _context.Usuarios.CountAsync(u => u.RolId == rolAdmin.Id && u.Activo && u.Id != id);
                    bool seguiraSiendoAdminActivo = datos.RolId == rolAdmin.Id && datos.Activo;
                    if (adminsActivos == 0 && !seguiraSiendoAdminActivo)
                        return (false, "No se puede quitar o desactivar al último administrador activo del sistema.");
                }
            }

            usuario.NombreUsuario = datos.NombreUsuario;
            usuario.NombreCompleto = datos.NombreCompleto;
            usuario.RolId = datos.RolId;
            usuario.Activo = datos.Activo;

            if (!string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                if (nuevaContrasena.Length < 6)
                    return (false, "La nueva contraseña debe tener al menos 6 caracteres.");
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> CambiarEstado(int id, bool activo, int? usuarioActualId = null)
        {
            var usuario = await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return (false, "El usuario no existe.");

            if (!activo && usuarioActualId.HasValue && usuario.Id == usuarioActualId.Value)
                return (false, "No puede desactivar su propia cuenta mientras tiene la sesión iniciada.");

            if (!activo && usuario.Rol?.Nombre == RolesSistema.Administrador)
            {
                int adminsActivos = await _context.Usuarios.CountAsync(u => u.RolId == usuario.RolId && u.Activo && u.Id != id);
                if (adminsActivos == 0)
                    return (false, "No se puede desactivar al último administrador activo del sistema.");
            }

            usuario.Activo = activo;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> CrearAdministradorInicial(string nombreUsuario, string nombreCompleto, string contrasena)
        {
            if (await ExisteAlgunUsuario())
                return (false, "Ya existen usuarios registrados. El administrador inicial solo puede crearse cuando el sistema no tiene usuarios.");

            var rolAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolesSistema.Administrador);
            if (rolAdmin == null) return (false, "No se encontró el rol Administrador.");

            var usuario = new Usuario
            {
                NombreUsuario = nombreUsuario,
                NombreCompleto = nombreCompleto,
                RolId = rolAdmin.Id
            };
            return await Crear(usuario, contrasena);
        }
    }
}
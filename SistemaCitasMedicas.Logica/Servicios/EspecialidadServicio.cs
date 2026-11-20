using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Logica.Servicios
{
    public class EspecialidadServicio
    {
        private readonly CitasDbContext _context;

        public EspecialidadServicio(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Especialidad>> ObtenerTodas(string? buscar = null, bool soloActivas = false)
        {
            var consulta = _context.Especialidades.AsNoTracking().AsQueryable();
            if (soloActivas)
                consulta = consulta.Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                consulta = consulta.Where(e => e.Nombre.Contains(buscar) || e.Descripcion.Contains(buscar));
            }

            return await consulta.OrderBy(e => e.Nombre).ToListAsync();
        }

        public async Task<Especialidad?> ObtenerPorId(int id)
        {
            return await _context.Especialidades.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> NombreExiste(string nombre, int? excluirId = null)
        {
            nombre = (nombre ?? string.Empty).Trim();
            var consulta = _context.Especialidades.Where(e => e.Nombre == nombre);
            if (excluirId.HasValue)
                consulta = consulta.Where(e => e.Id != excluirId.Value);
            return await consulta.AnyAsync();
        }

        public async Task<(bool Ok, string? Error)> Registrar(Especialidad especialidad)
        {
            especialidad.Nombre = (especialidad.Nombre ?? string.Empty).Trim();
            especialidad.Descripcion = (especialidad.Descripcion ?? string.Empty).Trim();
            if (await NombreExiste(especialidad.Nombre))
                return (false, "Ya existe una especialidad con ese nombre.");

            especialidad.Activo = true;
            _context.Especialidades.Add(especialidad);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> Actualizar(int id, Especialidad datos)
        {
            var especialidad = await _context.Especialidades.FirstOrDefaultAsync(e => e.Id == id);
            if (especialidad == null)
                return (false, "La especialidad no existe.");

            datos.Nombre = (datos.Nombre ?? string.Empty).Trim();
            datos.Descripcion = (datos.Descripcion ?? string.Empty).Trim();
            if (await NombreExiste(datos.Nombre, id))
                return (false, "Ya existe otra especialidad con ese nombre.");

            especialidad.Nombre = datos.Nombre;
            especialidad.Descripcion = datos.Descripcion;
            especialidad.Activo = datos.Activo;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> CambiarEstado(int id, bool activo)
        {
            var especialidad = await _context.Especialidades
                .Include(e => e.Medicos)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (especialidad == null)
                return (false, "La especialidad no existe.");

            if (!activo && especialidad.Medicos.Any(m => m.Activo))
                return (false, "No se puede desactivar la especialidad porque tiene médicos activos asociados.");

            especialidad.Activo = activo;
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Logica.Servicios
{
    public class MedicoServicio
    {
        private readonly CitasDbContext _context;

        public MedicoServicio(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Medico>> ObtenerTodos(string? buscar = null, int? especialidadId = null, bool soloActivos = false)
        {
            var consulta = _context.Medicos
                .Include(m => m.Especialidad)
                .AsNoTracking()
                .AsQueryable();

            if (soloActivos)
                consulta = consulta.Where(m => m.Activo);

            if (especialidadId.HasValue)
                consulta = consulta.Where(m => m.EspecialidadId == especialidadId.Value);

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                consulta = consulta.Where(m =>
                    m.Nombres.Contains(buscar) ||
                    m.Apellidos.Contains(buscar) ||
                    m.Matricula.Contains(buscar));
            }

            return await consulta
                .OrderBy(m => m.Apellidos)
                .ThenBy(m => m.Nombres)
                .ToListAsync();
        }

        public async Task<Medico?> ObtenerPorId(int id)
        {
            return await _context.Medicos
                .Include(m => m.Especialidad)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> MatriculaExiste(string matricula, int? excluirId = null)
        {
            matricula = (matricula ?? string.Empty).Trim();
            var consulta = _context.Medicos.Where(m => m.Matricula == matricula);
            if (excluirId.HasValue)
                consulta = consulta.Where(m => m.Id != excluirId.Value);
            return await consulta.AnyAsync();
        }

        public async Task<(bool Ok, string? Error)> Registrar(Medico medico)
        {
            Normalizar(medico);
            var validacion = await ValidarEntidad(medico);
            if (validacion != null) return (false, validacion);

            if (await MatriculaExiste(medico.Matricula))
                return (false, "Ya existe un médico registrado con esa matrícula profesional.");

            medico.Activo = true;
            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> Actualizar(int id, Medico datos)
        {
            var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null)
                return (false, "El médico no existe.");

            Normalizar(datos);
            var validacion = await ValidarEntidad(datos);
            if (validacion != null) return (false, validacion);

            if (await MatriculaExiste(datos.Matricula, id))
                return (false, "Ya existe otro médico con esa matrícula profesional.");

            medico.Nombres = datos.Nombres;
            medico.Apellidos = datos.Apellidos;
            medico.Matricula = datos.Matricula;
            medico.EspecialidadId = datos.EspecialidadId;
            medico.Activo = datos.Activo;

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> CambiarEstado(int id, bool activo)
        {
            var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null)
                return (false, "El médico no existe.");

            medico.Activo = activo;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        private async Task<string?> ValidarEntidad(Medico medico)
        {
            var especialidad = await _context.Especialidades.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == medico.EspecialidadId);
            if (especialidad == null)
                return "La especialidad seleccionada no existe.";
            if (!especialidad.Activo)
                return "La especialidad seleccionada no está activa.";
            return null;
        }

        private static void Normalizar(Medico m)
        {
            m.Nombres = (m.Nombres ?? string.Empty).Trim();
            m.Apellidos = (m.Apellidos ?? string.Empty).Trim();
            m.Matricula = (m.Matricula ?? string.Empty).Trim();
        }
    }
}
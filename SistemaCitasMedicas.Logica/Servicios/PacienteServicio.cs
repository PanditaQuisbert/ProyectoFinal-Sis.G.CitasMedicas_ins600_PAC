using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Logica.Servicios
{
    public class PacienteServicio
    {
        private readonly CitasDbContext _context;

        public PacienteServicio(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Paciente>> ObtenerTodos(string? buscar = null, bool incluirInactivos = false)
        {
            var consulta = _context.Pacientes.AsNoTracking().AsQueryable();
            if (!incluirInactivos)
                consulta = consulta.Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                consulta = consulta.Where(p =>
                    p.Documento.Contains(buscar) ||
                    p.Nombres.Contains(buscar) ||
                    p.Apellidos.Contains(buscar) ||
                    p.Telefono.Contains(buscar));
            }

            return await consulta
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .ToListAsync();
        }

        public async Task<Paciente?> ObtenerPorId(int id)
        {
            return await _context.Pacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Paciente?> ObtenerPorDocumento(string documento)
        {
            documento = (documento ?? string.Empty).Trim();
            return await _context.Pacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Documento == documento);
        }

        public async Task<bool> DocumentoExiste(string documento, int? excluirId = null)
        {
            documento = (documento ?? string.Empty).Trim();
            var consulta = _context.Pacientes.Where(p => p.Documento == documento);
            if (excluirId.HasValue)
                consulta = consulta.Where(p => p.Id != excluirId.Value);
            return await consulta.AnyAsync();
        }

        public async Task<(bool Ok, string? Error)> Registrar(Paciente paciente)
        {
            Normalizar(paciente);
            if (await DocumentoExiste(paciente.Documento))
                return (false, "Ya existe un paciente registrado con ese documento de identidad.");

            paciente.Activo = true;
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> Actualizar(int id, Paciente datos)
        {
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Id == id);
            if (paciente == null)
                return (false, "El paciente no existe.");

            Normalizar(datos);
            if (await DocumentoExiste(datos.Documento, id))
                return (false, "Ya existe otro paciente con ese documento de identidad.");

            paciente.Documento = datos.Documento;
            paciente.Nombres = datos.Nombres;
            paciente.Apellidos = datos.Apellidos;
            paciente.Telefono = datos.Telefono;
            paciente.FechaNacimiento = datos.FechaNacimiento;
            paciente.Activo = datos.Activo;

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> CambiarEstado(int id, bool activo)
        {
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Id == id);
            if (paciente == null)
                return (false, "El paciente no existe.");

            paciente.Activo = activo;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        private static void Normalizar(Paciente p)
        {
            p.Documento = (p.Documento ?? string.Empty).Trim();
            p.Nombres = (p.Nombres ?? string.Empty).Trim();
            p.Apellidos = (p.Apellidos ?? string.Empty).Trim();
            p.Telefono = (p.Telefono ?? string.Empty).Trim();
        }
    }
}
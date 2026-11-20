using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Logica.Servicios
{
    public class DashboardResumen
    {
        public int PacientesActivos { get; set; }
        public int MedicosActivos { get; set; }
        public int EspecialidadesActivas { get; set; }
        public int CitasProgramadasHoy { get; set; }
        public int CitasAtendidas { get; set; }
        public int CitasCanceladas { get; set; }
        public List<Cita> ProximasCitas { get; set; } = new();
        public List<Cita> UltimasCitas { get; set; } = new();
    }

    public class ReporteServicio
    {
        private readonly CitasDbContext _context;

        public ReporteServicio(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResumen> ObtenerDashboard()
        {
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);

            var resumen = new DashboardResumen
            {
                PacientesActivos = await _context.Pacientes.CountAsync(p => p.Activo),
                MedicosActivos = await _context.Medicos.CountAsync(m => m.Activo),
                EspecialidadesActivas = await _context.Especialidades.CountAsync(e => e.Activo),
                CitasProgramadasHoy = await _context.Citas.CountAsync(c => c.FechaHora >= hoy && c.FechaHora < manana && c.Estado == EstadosCita.Programada),
                CitasAtendidas = await _context.Citas.CountAsync(c => c.Estado == EstadosCita.Atendida),
                CitasCanceladas = await _context.Citas.CountAsync(c => c.Estado == EstadosCita.Cancelada),
                ProximasCitas = await _context.Citas
                    .Include(c => c.Paciente)
                    .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                    .AsNoTracking()
                    .Where(c => c.FechaHora >= DateTime.Now && c.Estado == EstadosCita.Programada)
                    .OrderBy(c => c.FechaHora)
                    .Take(5)
                    .ToListAsync(),
                UltimasCitas = await _context.Citas
                    .Include(c => c.Paciente)
                    .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                    .AsNoTracking()
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .ToListAsync()
            };

            return resumen;
        }

        public async Task<List<Cita>> CitasDelDia(DateTime fecha)
        {
            var d = fecha.Date;
            return await BaseCitas().Where(c => c.FechaHora.Date == d)
                .OrderBy(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> CitasPorRango(DateTime desde, DateTime hasta)
        {
            return await BaseCitas()
                .Where(c => c.FechaHora.Date >= desde.Date && c.FechaHora.Date <= hasta.Date)
                .OrderBy(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> CitasPorMedico(int medicoId)
        {
            return await BaseCitas().Where(c => c.MedicoId == medicoId)
                .OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> CitasPorEspecialidad(int especialidadId)
        {
            return await BaseCitas().Where(c => c.Medico != null && c.Medico.EspecialidadId == especialidadId)
                .OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> CitasPorPaciente(int pacienteId)
        {
            return await BaseCitas().Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> CitasPorEstado(string estado)
        {
            return await BaseCitas().Where(c => c.Estado == estado)
                .OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        private IQueryable<Cita> BaseCitas()
        {
            return _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .AsNoTracking();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Logica.Servicios
{
    public class FiltroCitas
    {
        public DateTime? Fecha { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? PacienteId { get; set; }
        public string? DocumentoPaciente { get; set; }
        public int? MedicoId { get; set; }
        public int? EspecialidadId { get; set; }
        public string? Estado { get; set; }
    }

    public class CitaServicio
    {
        private readonly CitasDbContext _context;

        public CitaServicio(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cita>> Obtener(FiltroCitas filtro)
        {
            var consulta = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.Fecha.HasValue)
            {
                var d = filtro.Fecha.Value.Date;
                consulta = consulta.Where(c => c.FechaHora.Date == d);
            }
            if (filtro.Desde.HasValue)
                consulta = consulta.Where(c => c.FechaHora >= filtro.Desde.Value);
            if (filtro.Hasta.HasValue)
                consulta = consulta.Where(c => c.FechaHora <= filtro.Hasta.Value);
            if (filtro.PacienteId.HasValue)
                consulta = consulta.Where(c => c.PacienteId == filtro.PacienteId.Value);
            if (!string.IsNullOrWhiteSpace(filtro.DocumentoPaciente))
            {
                var doc = filtro.DocumentoPaciente.Trim();
                consulta = consulta.Where(c => c.Paciente != null && c.Paciente.Documento.Contains(doc));
            }
            if (filtro.MedicoId.HasValue)
                consulta = consulta.Where(c => c.MedicoId == filtro.MedicoId.Value);
            if (filtro.EspecialidadId.HasValue)
                consulta = consulta.Where(c => c.Medico != null && c.Medico.EspecialidadId == filtro.EspecialidadId.Value);
            if (!string.IsNullOrWhiteSpace(filtro.Estado))
                consulta = consulta.Where(c => c.Estado == filtro.Estado);

            return await consulta.OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<Cita?> ObtenerPorId(int id)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico).ThenInclude(m => m!.Especialidad)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(bool Ok, string? Error)> Programar(Cita cita)
        {
            cita.Observaciones = (cita.Observaciones ?? string.Empty).Trim();
            cita.Estado = EstadosCita.Programada;

            var error = await ValidarCita(cita, null);
            if (error != null) return (false, error);

            _context.Citas.Add(cita);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo registrar la cita. Verifique la disponibilidad e intente nuevamente.");
            }
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> Modificar(int id, Cita datos)
        {
            var cita = await _context.Citas.FirstOrDefaultAsync(c => c.Id == id);
            if (cita == null) return (false, "La cita no existe.");
            if (cita.Estado == EstadosCita.Cancelada)
                return (false, "No se puede modificar una cita cancelada.");

            datos.Observaciones = (datos.Observaciones ?? string.Empty).Trim();
            datos.Estado = cita.Estado; // estado se cambia por acción específica

            var error = await ValidarCita(datos, id);
            if (error != null) return (false, error);

            cita.PacienteId = datos.PacienteId;
            cita.MedicoId = datos.MedicoId;
            cita.FechaHora = datos.FechaHora;
            cita.Observaciones = datos.Observaciones;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo actualizar la cita. Verifique la disponibilidad e intente nuevamente.");
            }
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> Cancelar(int id)
        {
            var cita = await _context.Citas.FirstOrDefaultAsync(c => c.Id == id);
            if (cita == null) return (false, "La cita no existe.");
            if (cita.Estado == EstadosCita.Cancelada)
                return (false, "La cita ya se encuentra cancelada.");

            cita.Estado = EstadosCita.Cancelada;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> CambiarEstado(int id, string nuevoEstado)
        {
            if (!EstadosCita.Todos.Contains(nuevoEstado))
                return (false, "Estado de cita no válido.");

            var cita = await _context.Citas.FirstOrDefaultAsync(c => c.Id == id);
            if (cita == null) return (false, "La cita no existe.");

            if (cita.Estado == EstadosCita.Cancelada && nuevoEstado != EstadosCita.Cancelada)
                return (false, "No se puede reactivar una cita cancelada. Debe programar una nueva cita.");

            cita.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> HorarioDisponible(int medicoId, DateTime fechaHora, int? excluirCitaId = null)
        {
            var consulta = _context.Citas
                .Where(c => c.MedicoId == medicoId
                    && c.FechaHora == fechaHora
                    && c.Estado != EstadosCita.Cancelada);
            if (excluirCitaId.HasValue)
                consulta = consulta.Where(c => c.Id != excluirCitaId.Value);
            return !await consulta.AnyAsync();
        }

        public async Task<List<DateTime>> HorariosOcupados(int medicoId, DateTime fecha, int? excluirCitaId = null)
        {
            var dia = fecha.Date;
            var consulta = _context.Citas.AsNoTracking()
                .Where(c => c.MedicoId == medicoId
                    && c.FechaHora.Date == dia
                    && c.Estado != EstadosCita.Cancelada);
            if (excluirCitaId.HasValue)
                consulta = consulta.Where(c => c.Id != excluirCitaId.Value);
            return await consulta.Select(c => c.FechaHora).ToListAsync();
        }

        public async Task<(bool Ok, string? Error)> ValidarDisponibilidad(int medicoId, int? especialidadId, DateTime fechaHora, int? excluirCitaId = null)
        {
            var error = await ValidarReglasBasicas(medicoId, especialidadId, fechaHora);
            if (error != null) return (false, error);

            if (!await HorarioDisponible(medicoId, fechaHora, excluirCitaId))
                return (false, "El médico ya tiene una cita activa en ese horario.");

            return (true, null);
        }

        private async Task<string?> ValidarCita(Cita cita, int? excluirCitaId)
        {
            if (cita.FechaHora < DateTime.Now)
                return "No se permite registrar citas con fechas u horas pasadas (RN-04).";

            var paciente = await _context.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == cita.PacienteId);
            if (paciente == null) return "El paciente seleccionado no existe.";
            if (!paciente.Activo) return "El paciente seleccionado no está activo.";

            var medico = await _context.Medicos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == cita.MedicoId);
            if (medico == null) return "El médico seleccionado no existe.";
            if (!medico.Activo) return "El médico seleccionado no está activo.";

            if (!await HorarioDisponible(cita.MedicoId, cita.FechaHora, excluirCitaId))
                return "No se puede registrar: el médico ya tiene una cita activa en la misma fecha y hora (RF-11).";

            return null;
        }

        private async Task<string?> ValidarReglasBasicas(int medicoId, int? especialidadId, DateTime fechaHora)
        {
            if (fechaHora < DateTime.Now)
                return "La fecha y hora seleccionadas ya pasaron.";

            var medico = await _context.Medicos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == medicoId);
            if (medico == null) return "El médico seleccionado no existe.";
            if (!medico.Activo) return "El médico seleccionado no está activo.";
            if (especialidadId.HasValue && medico.EspecialidadId != especialidadId.Value)
                return "La especialidad seleccionada no corresponde al médico elegido.";

            return null;
        }
    }
}
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers.Api
{
    [ApiController]
    [Route("api/citas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CitasApiController : ControllerBase
    {
        private readonly CitaServicio _servicio;

        public CitasApiController(CitaServicio servicio) { _servicio = servicio; }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime? fecha, [FromQuery] string? documento, [FromQuery] int? medicoId, [FromQuery] int? especialidadId, [FromQuery] string? estado)
        {
            var lista = await _servicio.Obtener(new FiltroCitas
            {
                Fecha = fecha,
                DocumentoPaciente = documento,
                MedicoId = medicoId,
                EspecialidadId = especialidadId,
                Estado = estado
            });
            return Ok(lista.Select(Map));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _servicio.ObtenerPorId(id);
            if (c == null) return NotFound();
            return Ok(Map(c));
        }

        public record CitaRequest(int PacienteId, int MedicoId, DateTime FechaHora, string? Observaciones);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CitaRequest req)
        {
            var cita = new Cita
            {
                PacienteId = req.PacienteId,
                MedicoId = req.MedicoId,
                FechaHora = req.FechaHora,
                Observaciones = req.Observaciones ?? string.Empty
            };
            var (ok, error) = await _servicio.Programar(cita);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Cita programada correctamente.", id = cita.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CitaRequest req)
        {
            var datos = new Cita
            {
                PacienteId = req.PacienteId,
                MedicoId = req.MedicoId,
                FechaHora = req.FechaHora,
                Observaciones = req.Observaciones ?? string.Empty
            };
            var (ok, error) = await _servicio.Modificar(id, datos);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Cita actualizada correctamente." });
        }

        [HttpPost("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var (ok, error) = await _servicio.Cancelar(id);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Cita cancelada correctamente." });
        }

        [HttpPost("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] string estado)
        {
            var (ok, error) = await _servicio.CambiarEstado(id, estado);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Estado actualizado." });
        }

        [HttpGet("disponibilidad")]
        public async Task<IActionResult> Disponibilidad([FromQuery] int medicoId, [FromQuery] DateTime fecha, [FromQuery] int? excluirCitaId)
        {
            var ocupados = await _servicio.HorariosOcupados(medicoId, fecha, excluirCitaId);
            return Ok(new { ocupados = ocupados.Select(h => h.ToString("HH:mm")) });
        }

        [HttpGet("validar")]
        public async Task<IActionResult> Validar([FromQuery] int medicoId, [FromQuery] int? especialidadId, [FromQuery] DateTime fechaHora, [FromQuery] int? excluirCitaId)
        {
            var (ok, error) = await _servicio.ValidarDisponibilidad(medicoId, especialidadId, fechaHora, excluirCitaId);
            return Ok(new { ok, message = ok ? "Horario disponible." : error });
        }

        private static object Map(Cita c) => new
        {
            c.Id,
            FechaHora = c.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"),
            c.Estado,
            c.Observaciones,
            Paciente = c.Paciente == null ? null : new { c.Paciente.Id, NombreCompleto = c.Paciente.NombreCompleto, c.Paciente.Documento },
            Medico = c.Medico == null ? null : new { c.Medico.Id, NombreCompleto = c.Medico.NombreCompleto, c.Medico.Matricula, Especialidad = c.Medico.Especialidad?.Nombre, c.Medico.EspecialidadId }
        };
    }
}

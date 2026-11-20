using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers.Api
{
    [ApiController]
    [Route("api/pacientes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PacientesApiController : ControllerBase
    {
        private readonly PacienteServicio _servicio;
        public PacientesApiController(PacienteServicio servicio) { _servicio = servicio; }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? buscar, [FromQuery] bool incluirInactivos = false)
        {
            var lista = await _servicio.ObtenerTodos(buscar, incluirInactivos);
            return Ok(lista.Select(p => new {
                p.Id, p.Documento, p.Nombres, p.Apellidos, p.Telefono,
                FechaNacimiento = p.FechaNacimiento?.ToString("yyyy-MM-dd"),
                p.Activo
            }));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _servicio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return Ok(new { p.Id, p.Documento, p.Nombres, p.Apellidos, p.Telefono, FechaNacimiento = p.FechaNacimiento?.ToString("yyyy-MM-dd"), p.Activo });
        }

        public record PacienteRequest(string Documento, string Nombres, string Apellidos, string Telefono, DateTime? FechaNacimiento, bool Activo = true);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PacienteRequest req)
        {
            var paciente = new Paciente
            {
                Documento = req.Documento?.Trim() ?? "",
                Nombres = req.Nombres?.Trim() ?? "",
                Apellidos = req.Apellidos?.Trim() ?? "",
                Telefono = req.Telefono?.Trim() ?? "",
                FechaNacimiento = req.FechaNacimiento,
                Activo = req.Activo
            };
            var (ok, error) = await _servicio.Registrar(paciente);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Paciente registrado correctamente.", id = paciente.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] PacienteRequest req)
        {
            var datos = new Paciente
            {
                Documento = req.Documento?.Trim() ?? "",
                Nombres = req.Nombres?.Trim() ?? "",
                Apellidos = req.Apellidos?.Trim() ?? "",
                Telefono = req.Telefono?.Trim() ?? "",
                FechaNacimiento = req.FechaNacimiento,
                Activo = req.Activo
            };
            var (ok, error) = await _servicio.Actualizar(id, datos);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Paciente actualizado correctamente." });
        }

        [HttpPost("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool activo)
        {
            var (ok, error) = await _servicio.CambiarEstado(id, activo);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = activo ? "Paciente activado." : "Paciente desactivado." });
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers.Api
{
    [ApiController]
    [Route("api/catalogos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CatalogosApiController : ControllerBase
    {
        private readonly EspecialidadServicio _especialidadServicio;
        private readonly MedicoServicio _medicoServicio;

        public CatalogosApiController(EspecialidadServicio especialidadServicio, MedicoServicio medicoServicio)
        {
            _especialidadServicio = especialidadServicio;
            _medicoServicio = medicoServicio;
        }

        [HttpGet("especialidades")]
        public async Task<IActionResult> Especialidades([FromQuery] bool soloActivas = true)
        {
            var lista = await _especialidadServicio.ObtenerTodas(soloActivas: soloActivas);
            return Ok(lista.Select(e => new { e.Id, e.Nombre, e.Descripcion, e.Activo }));
        }

        [HttpGet("medicos")]
        public async Task<IActionResult> Medicos([FromQuery] int? especialidadId, [FromQuery] bool soloActivos = true)
        {
            var lista = await _medicoServicio.ObtenerTodos(null, especialidadId, soloActivos);
            return Ok(lista.Select(m => new {
                m.Id,
                NombreCompleto = m.NombreCompleto,
                m.Matricula,
                m.EspecialidadId,
                Especialidad = m.Especialidad?.Nombre,
                m.Activo
            }));
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers.Api
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardApiController : ControllerBase
    {
        private readonly ReporteServicio _reporteServicio;
        public DashboardApiController(ReporteServicio reporteServicio) { _reporteServicio = reporteServicio; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var r = await _reporteServicio.ObtenerDashboard();
            return Ok(new
            {
                r.PacientesActivos,
                r.MedicosActivos,
                r.EspecialidadesActivas,
                r.CitasProgramadasHoy,
                r.CitasAtendidas,
                r.CitasCanceladas,
                proximasCitas = r.ProximasCitas.Select(c => new { c.Id, FechaHora = c.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"), Paciente = c.Paciente?.NombreCompleto, Medico = c.Medico?.NombreCompleto, c.Estado }),
                ultimasCitas = r.UltimasCitas.Select(c => new { c.Id, FechaHora = c.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"), Paciente = c.Paciente?.NombreCompleto, Medico = c.Medico?.NombreCompleto, c.Estado })
            });
        }
    }
}

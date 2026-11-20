using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;
using SistemaCitasMedicas.ViewModels;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly ReporteServicio _reporteServicio;
        private readonly MedicoServicio _medicoServicio;
        private readonly EspecialidadServicio _especialidadServicio;
        private readonly PacienteServicio _pacienteServicio;

        public ReportesController(ReporteServicio reporteServicio, MedicoServicio medicoServicio, EspecialidadServicio especialidadServicio, PacienteServicio pacienteServicio)
        {
            _reporteServicio = reporteServicio;
            _medicoServicio = medicoServicio;
            _especialidadServicio = especialidadServicio;
            _pacienteServicio = pacienteServicio;
        }

        public async Task<IActionResult> Index(ReporteFiltroViewModel filtro)
        {
            await CargarListas(filtro);
            filtro.Resultados = await ObtenerResultados(filtro);
            ViewBag.Total = filtro.Resultados.Count;
            ViewBag.Programadas = filtro.Resultados.Count(c => c.Estado == EstadosCita.Programada);
            ViewBag.Atendidas = filtro.Resultados.Count(c => c.Estado == EstadosCita.Atendida);
            ViewBag.Canceladas = filtro.Resultados.Count(c => c.Estado == EstadosCita.Cancelada);
            return View(filtro);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarCsv([FromQuery] ReporteFiltroViewModel filtro)
        {
            var resultados = await ObtenerResultados(filtro);
            var sb = new StringBuilder();
            sb.AppendLine("FechaHora;Paciente;Documento;Medico;Especialidad;Estado;Observaciones");
            foreach (var c in resultados)
            {
                string Esc(string? s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
                sb.AppendLine(string.Join(';', new[]
                {
                    Esc(c.FechaHora.ToString("dd/MM/yyyy HH:mm")),
                    Esc(c.Paciente?.NombreCompleto),
                    Esc(c.Paciente?.Documento),
                    Esc(c.Medico?.NombreCompleto),
                    Esc(c.Medico?.Especialidad?.Nombre),
                    Esc(c.Estado),
                    Esc(c.Observaciones)
                }));
            }
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"reporte_citas_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> Imprimir([FromQuery] ReporteFiltroViewModel filtro)
        {
            await CargarListas(filtro);
            filtro.Resultados = await ObtenerResultados(filtro);
            ViewBag.Total = filtro.Resultados.Count;
            ViewBag.Programadas = filtro.Resultados.Count(c => c.Estado == EstadosCita.Programada);
            ViewBag.Atendidas = filtro.Resultados.Count(c => c.Estado == EstadosCita.Atendida);
            ViewBag.Canceladas = filtro.Resultados.Count(c => c.Estado == EstadosCita.Cancelada);
            return View(filtro);
        }

        private async Task<List<Cita>> ObtenerResultados(ReporteFiltroViewModel filtro)
        {
            var hoy = DateTime.Today;
            return filtro.Tipo switch
            {
                "dia" => await _reporteServicio.CitasDelDia(filtro.Fecha ?? hoy),
                "rango" => await _reporteServicio.CitasPorRango(filtro.Desde ?? hoy, filtro.Hasta ?? hoy),
                "medico" when filtro.MedicoId.HasValue => await _reporteServicio.CitasPorMedico(filtro.MedicoId.Value),
                "especialidad" when filtro.EspecialidadId.HasValue => await _reporteServicio.CitasPorEspecialidad(filtro.EspecialidadId.Value),
                "paciente" when filtro.PacienteId.HasValue => await _reporteServicio.CitasPorPaciente(filtro.PacienteId.Value),
                "programadas" => await _reporteServicio.CitasPorEstado(EstadosCita.Programada),
                "atendidas" => await _reporteServicio.CitasPorEstado(EstadosCita.Atendida),
                "canceladas" => await _reporteServicio.CitasPorEstado(EstadosCita.Cancelada),
                _ => await _reporteServicio.CitasDelDia(hoy)
            };
        }

        private async Task CargarListas(ReporteFiltroViewModel filtro)
        {
            filtro.Medicos = await _medicoServicio.ObtenerTodos();
            filtro.Especialidades = await _especialidadServicio.ObtenerTodas();
            filtro.Pacientes = await _pacienteServicio.ObtenerTodos(incluirInactivos: true);
        }
    }
}
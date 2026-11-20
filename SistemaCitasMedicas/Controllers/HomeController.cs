using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Logica.Servicios;
using SistemaCitasMedicas.Models;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ReporteServicio _reporteServicio;

        public HomeController(ReporteServicio reporteServicio)
        {
            _reporteServicio = reporteServicio;
        }

        public async Task<IActionResult> Index()
        {
            var resumen = await _reporteServicio.ObtenerDashboard();
            return View(resumen);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

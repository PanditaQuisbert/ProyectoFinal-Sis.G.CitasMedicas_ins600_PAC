using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize]
    public class ConfiguracionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ConfiguracionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public IActionResult Index()
        {
            ViewBag.Entorno = _env.EnvironmentName;
            ViewBag.Version = "v1.0";
            ViewBag.BaseDatos = "CitasMedicasDB (PAOLA\\SQLEXPRESS)";
            return View();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
            var cadena = _configuration.GetConnectionString("ConexionSQL") ?? string.Empty;
            var proveedor = new SqlConnectionStringBuilder(cadena);
            ViewBag.BaseDatos = string.IsNullOrWhiteSpace(proveedor.InitialCatalog)
                ? "Base de datos no configurada"
                : $"{proveedor.InitialCatalog} ({proveedor.DataSource})";
            return View();
        }
    }
}
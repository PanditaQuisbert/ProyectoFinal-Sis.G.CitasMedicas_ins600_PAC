using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize]
    public class PacientesController : Controller
    {
        private readonly PacienteServicio _servicio;

        public PacientesController(PacienteServicio servicio)
        {
            _servicio = servicio;
        }

        public async Task<IActionResult> Index(string? buscar, bool incluirInactivos = false)
        {
            ViewBag.Buscar = buscar;
            ViewBag.IncluirInactivos = incluirInactivos;
            var pacientes = await _servicio.ObtenerTodos(buscar, incluirInactivos);
            return View(pacientes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var paciente = await _servicio.ObtenerPorId(id);
            if (paciente == null) return NotFound();
            return View(paciente);
        }

        [HttpGet]
        public IActionResult Create() => View(new Paciente { Activo = true });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Paciente paciente)
        {
            if (!ModelState.IsValid) return View(paciente);
            var (ok, error) = await _servicio.Registrar(paciente);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo registrar el paciente.");
                return View(paciente);
            }
            TempData["Mensaje"] = "Paciente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var paciente = await _servicio.ObtenerPorId(id);
            if (paciente == null) return NotFound();
            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Paciente paciente)
        {
            if (id != paciente.Id) return BadRequest();
            if (!ModelState.IsValid) return View(paciente);

            var (ok, error) = await _servicio.Actualizar(id, paciente);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo actualizar el paciente.");
                return View(paciente);
            }
            TempData["Mensaje"] = "Paciente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            var (ok, error) = await _servicio.CambiarEstado(id, activo);
            TempData[ok ? "Mensaje" : "Error"] = ok ? (activo ? "Paciente activado." : "Paciente desactivado. Su historial de citas se conserva.") : error;
            return RedirectToAction(nameof(Index), new { incluirInactivos = true });
        }
    }
}

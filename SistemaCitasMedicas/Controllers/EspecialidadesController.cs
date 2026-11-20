using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class EspecialidadesController : Controller
    {
        private readonly EspecialidadServicio _servicio;

        public EspecialidadesController(EspecialidadServicio servicio)
        {
            _servicio = servicio;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            ViewBag.Buscar = buscar;
            var lista = await _servicio.ObtenerTodas(buscar);
            return View(lista);
        }

        public IActionResult Create() => View(new Especialidad { Activo = true });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especialidad modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            var (ok, error) = await _servicio.Registrar(modelo);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "Error al registrar."); return View(modelo); }
            TempData["Mensaje"] = "Especialidad registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _servicio.ObtenerPorId(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especialidad modelo)
        {
            if (id != modelo.Id) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            var (ok, error) = await _servicio.Actualizar(id, modelo);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "Error al actualizar."); return View(modelo); }
            TempData["Mensaje"] = "Especialidad actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _servicio.ObtenerPorId(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            var (ok, error) = await _servicio.CambiarEstado(id, activo);
            TempData[ok ? "Mensaje" : "Error"] = ok ? (activo ? "Especialidad activada." : "Especialidad desactivada.") : error;
            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class MedicosController : Controller
    {
        private readonly MedicoServicio _servicio;
        private readonly EspecialidadServicio _especialidadServicio;

        public MedicosController(MedicoServicio servicio, EspecialidadServicio especialidadServicio)
        {
            _servicio = servicio;
            _especialidadServicio = especialidadServicio;
        }

        public async Task<IActionResult> Index(string? buscar, int? especialidadId)
        {
            ViewBag.Buscar = buscar;
            ViewBag.EspecialidadId = especialidadId;
            ViewBag.Especialidades = new SelectList(await _especialidadServicio.ObtenerTodas(soloActivas: false), "Id", "Nombre", especialidadId);
            var lista = await _servicio.ObtenerTodos(buscar, especialidadId);
            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            await CargarEspecialidades();
            return View(new Medico { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medico modelo)
        {
            if (!ModelState.IsValid) { await CargarEspecialidades(modelo.EspecialidadId); return View(modelo); }
            var (ok, error) = await _servicio.Registrar(modelo);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "Error al registrar."); await CargarEspecialidades(modelo.EspecialidadId); return View(modelo); }
            TempData["Mensaje"] = "Médico registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _servicio.ObtenerPorId(id);
            if (item == null) return NotFound();
            await CargarEspecialidades(item.EspecialidadId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medico modelo)
        {
            if (id != modelo.Id) return BadRequest();
            if (!ModelState.IsValid) { await CargarEspecialidades(modelo.EspecialidadId); return View(modelo); }
            var (ok, error) = await _servicio.Actualizar(id, modelo);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "Error al actualizar."); await CargarEspecialidades(modelo.EspecialidadId); return View(modelo); }
            TempData["Mensaje"] = "Médico actualizado correctamente.";
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
            TempData[ok ? "Mensaje" : "Error"] = ok ? (activo ? "Médico activado." : "Médico desactivado. Su historial de citas se conserva.") : error;
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarEspecialidades(int? seleccionada = null)
        {
            var especialidades = await _especialidadServicio.ObtenerTodas(soloActivas: true);
            ViewBag.Especialidades = new SelectList(especialidades, "Id", "Nombre", seleccionada);
        }
    }
}
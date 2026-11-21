using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;
using SistemaCitasMedicas.ViewModels;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize]
    public class CitasController : Controller
    {
        private readonly CitaServicio _servicio;
        private readonly PacienteServicio _pacienteServicio;
        private readonly MedicoServicio _medicoServicio;
        private readonly EspecialidadServicio _especialidadServicio;

        public CitasController(CitaServicio servicio, PacienteServicio pacienteServicio, MedicoServicio medicoServicio, EspecialidadServicio especialidadServicio)
        {
            _servicio = servicio;
            _pacienteServicio = pacienteServicio;
            _medicoServicio = medicoServicio;
            _especialidadServicio = especialidadServicio;
        }

        public async Task<IActionResult> Index(CitaFiltroViewModel filtro)
        {
            filtro.Medicos = await _medicoServicio.ObtenerTodos(soloActivos: false);
            filtro.Especialidades = await _especialidadServicio.ObtenerTodas();
            filtro.Resultados = await _servicio.Obtener(new FiltroCitas
            {
                Fecha = filtro.Fecha,
                DocumentoPaciente = filtro.DocumentoPaciente,
                MedicoId = filtro.MedicoId,
                EspecialidadId = filtro.EspecialidadId,
                Estado = filtro.Estado
            });
            ViewBag.Estados = new SelectList(EstadosCita.Todos, filtro.Estado);
            return View(filtro);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cita = await _servicio.ObtenerPorId(id);
            if (cita == null) return NotFound();
            return View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CitaFormViewModel { Fecha = DateTime.Today };
            await CargarListas(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaFormViewModel vm)
        {
            await NormalizarYValidarVm(vm);
            if (!ModelState.IsValid) { await CargarListas(vm); return View(vm); }

            var cita = new Cita
            {
                PacienteId = vm.PacienteId,
                MedicoId = vm.MedicoId,
                FechaHora = vm.FechaHora,
                Observaciones = vm.Observaciones ?? string.Empty
            };
            var (ok, error) = await _servicio.Programar(cita);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo programar la cita.");
                await CargarListas(vm);
                return View(vm);
            }
            TempData["Mensaje"] = "Cita programada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cita = await _servicio.ObtenerPorId(id);
            if (cita == null) return NotFound();
            if (cita.Estado == EstadosCita.Cancelada)
            {
                TempData["Error"] = "No se puede modificar una cita cancelada.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CitaFormViewModel
            {
                Id = cita.Id,
                PacienteId = cita.PacienteId,
                MedicoId = cita.MedicoId,
                EspecialidadId = cita.Medico?.EspecialidadId ?? 0,
                Fecha = cita.FechaHora.Date,
                Hora = cita.FechaHora.ToString("HH:mm"),
                Observaciones = cita.Observaciones,
                Estado = cita.Estado
            };
            await CargarListas(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CitaFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            await NormalizarYValidarVm(vm, id);
            if (!ModelState.IsValid) { await CargarListas(vm); return View(vm); }

            var datos = new Cita
            {
                PacienteId = vm.PacienteId,
                MedicoId = vm.MedicoId,
                FechaHora = vm.FechaHora,
                Observaciones = vm.Observaciones ?? string.Empty
            };
            var (ok, error) = await _servicio.Modificar(id, datos);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo modificar la cita.");
                await CargarListas(vm);
                return View(vm);
            }
            TempData["Mensaje"] = "Cita actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var (ok, error) = await _servicio.Cancelar(id);
            TempData[ok ? "Mensaje" : "Error"] = ok ? "Cita cancelada correctamente. El horario ha quedado disponible." : error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            var (ok, error) = await _servicio.CambiarEstado(id, estado);
            TempData[ok ? "Mensaje" : "Error"] = ok ? "Estado de la cita actualizado." : error;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> MedicosPorEspecialidad(int especialidadId)
        {
            var medicos = await _medicoServicio.ObtenerTodos(especialidadId: especialidadId, soloActivos: true);
            return Json(medicos.Select(m => new { m.Id, nombre = m.NombreCompleto, m.Matricula, m.EspecialidadId }));
        }

        [HttpGet]
        public async Task<IActionResult> Disponibilidad(int medicoId, DateTime fecha, int? excluirCitaId = null)
        {
            var ocupados = await _servicio.HorariosOcupados(medicoId, fecha, excluirCitaId);
            return Json(new { ocupados = ocupados.Select(h => h.ToString("HH:mm")) });
        }

        [HttpGet]
        public async Task<IActionResult> Validar(int medicoId, int especialidadId, DateTime fecha, string hora, int? excluirCitaId = null)
        {
            if (!TimeSpan.TryParse(hora, out var ts))
                return Json(new { ok = false, mensaje = "Hora inválida." });
            var fechaHora = fecha.Date.Add(ts);
            var (ok, error) = await _servicio.ValidarDisponibilidad(medicoId, especialidadId, fechaHora, excluirCitaId);
            return Json(new { ok, mensaje = ok ? "Horario disponible." : error });
        }

        private async Task CargarListas(CitaFormViewModel vm)
        {
            vm.Pacientes = await _pacienteServicio.ObtenerTodos(incluirInactivos: false);
            vm.Especialidades = await _especialidadServicio.ObtenerTodas(soloActivas: true);
            vm.Medicos = vm.EspecialidadId > 0
                ? await _medicoServicio.ObtenerTodos(especialidadId: vm.EspecialidadId, soloActivos: true)
                : new List<Medico>();
        }

        private async Task NormalizarYValidarVm(CitaFormViewModel vm, int? excluirId = null)
        {
            if (vm.Fecha == default)
                ModelState.AddModelError(nameof(vm.Fecha), "Debe seleccionar una fecha válida.");
            if (vm.FechaHora == DateTime.MinValue)
                ModelState.AddModelError(nameof(vm.Hora), "Debe seleccionar una hora válida.");

            // asegurar que médico corresponda a especialidad
            var medico = await _medicoServicio.ObtenerPorId(vm.MedicoId);
            if (medico != null && medico.EspecialidadId != vm.EspecialidadId)
                ModelState.AddModelError(nameof(vm.MedicoId), "El médico seleccionado no corresponde a la especialidad elegida.");
        }
    }
}
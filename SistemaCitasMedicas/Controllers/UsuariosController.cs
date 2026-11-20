using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;
using SistemaCitasMedicas.ViewModels;

namespace SistemaCitasMedicas.Controllers
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class UsuariosController : Controller
    {
        private readonly UsuarioServicio _servicio;

        public UsuariosController(UsuarioServicio servicio)
        {
            _servicio = servicio;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            ViewBag.Buscar = buscar;
            var usuarios = await _servicio.ObtenerTodos(buscar);
            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            await CargarRoles();
            return View(new UsuarioFormViewModel { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioFormViewModel modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.Contrasena))
                ModelState.AddModelError(nameof(modelo.Contrasena), "La contraseña es obligatoria para un nuevo usuario.");

            if (!ModelState.IsValid) { await CargarRoles(modelo.RolId); return View(modelo); }

            var usuario = new Usuario
            {
                NombreUsuario = modelo.NombreUsuario,
                NombreCompleto = modelo.NombreCompleto,
                RolId = modelo.RolId,
                Activo = modelo.Activo
            };
            var (ok, error) = await _servicio.Crear(usuario, modelo.Contrasena ?? string.Empty);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "No se pudo crear el usuario."); await CargarRoles(modelo.RolId); return View(modelo); }

            TempData["Mensaje"] = "Usuario registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var u = await _servicio.ObtenerPorId(id);
            if (u == null) return NotFound();
            await CargarRoles(u.RolId);
            return View(new UsuarioFormViewModel
            {
                Id = u.Id,
                NombreUsuario = u.NombreUsuario,
                NombreCompleto = u.NombreCompleto,
                RolId = u.RolId,
                Activo = u.Activo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioFormViewModel modelo)
        {
            if (id != modelo.Id) return BadRequest();
            // contraseña opcional en edición
            if (!ModelState.IsValid) { await CargarRoles(modelo.RolId); return View(modelo); }

            var datos = new Usuario
            {
                NombreUsuario = modelo.NombreUsuario,
                NombreCompleto = modelo.NombreCompleto,
                RolId = modelo.RolId,
                Activo = modelo.Activo
            };
            var (ok, error) = await _servicio.Actualizar(id, datos, string.IsNullOrWhiteSpace(modelo.Contrasena) ? null : modelo.Contrasena);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "No se pudo actualizar el usuario."); await CargarRoles(modelo.RolId); return View(modelo); }

            TempData["Mensaje"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            int? actualId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : null;
            var (ok, error) = await _servicio.CambiarEstado(id, activo, actualId);
            TempData[ok ? "Mensaje" : "Error"] = ok ? (activo ? "Usuario activado." : "Usuario desactivado.") : error;
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarRoles(int? seleccionada = null)
        {
            var roles = await _servicio.ObtenerRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Nombre", seleccionada);
        }
    }
}
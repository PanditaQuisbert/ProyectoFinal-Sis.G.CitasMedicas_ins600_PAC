using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitasMedicas.Datos.Entidades;
using SistemaCitasMedicas.Logica.Servicios;
using SistemaCitasMedicas.ViewModels;

namespace SistemaCitasMedicas.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioServicio _usuarioServicio;

        public CuentaController(UsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.RequiereInstalacion = !await _usuarioServicio.ExisteAlgunUsuario();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            ViewBag.RequiereInstalacion = !await _usuarioServicio.ExisteAlgunUsuario();
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = await _usuarioServicio.ValidarCredenciales(modelo.NombreUsuario, modelo.Contrasena);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos, o la cuenta está inactiva.");
                return View(modelo);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim("NombreCompleto", usuario.NombreCompleto),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty)
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identidad);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

            if (!string.IsNullOrWhiteSpace(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
                return Redirect(modelo.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Instalacion()
        {
            if (await _usuarioServicio.ExisteAlgunUsuario())
                return RedirectToAction(nameof(Login));

            return View(new InstalacionAdminViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Instalacion(InstalacionAdminViewModel modelo)
        {
            if (await _usuarioServicio.ExisteAlgunUsuario())
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(modelo);

            var (ok, error) = await _usuarioServicio.CrearAdministradorInicial(modelo.NombreUsuario, modelo.NombreCompleto, modelo.Contrasena);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo crear el administrador inicial.");
                return View(modelo);
            }

            TempData["Mensaje"] = "Administrador inicial creado correctamente. Ahora puede iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
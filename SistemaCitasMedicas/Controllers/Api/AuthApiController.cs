using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaCitasMedicas.Logica.Servicios;

namespace SistemaCitasMedicas.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;
        private readonly IConfiguration _configuration;

        public AuthApiController(UsuarioServicio usuarioServicio, IConfiguration configuration)
        {
            _usuarioServicio = usuarioServicio;
            _configuration = configuration;
        }

        public record LoginRequest(string NombreUsuario, string Contrasena);

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NombreUsuario) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest(new { message = "Usuario y contraseña son obligatorios." });

            var usuario = await _usuarioServicio.ValidarCredenciales(request.NombreUsuario, request.Contrasena);
            if (usuario == null)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos, o la cuenta está inactiva." });

            var jwt = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddMinutes(int.TryParse(jwt["ExpiraMinutos"], out var m) ? m : 480);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim("NombreCompleto", usuario.NombreCompleto),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expira = expira,
                usuario = new
                {
                    usuario.Id,
                    usuario.NombreUsuario,
                    usuario.NombreCompleto,
                    rol = usuario.Rol?.Nombre
                }
            });
        }
    }
}

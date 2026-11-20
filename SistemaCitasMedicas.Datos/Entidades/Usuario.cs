using System.ComponentModel.DataAnnotations;

namespace SistemaCitasMedicas.Datos.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no debe exceder 50 caracteres.")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre completo no debe exceder 150 caracteres.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ContrasenaHash { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        public Rol? Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
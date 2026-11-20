using System.ComponentModel.DataAnnotations;

namespace SistemaCitasMedicas.Datos.Entidades
{
    public static class RolesSistema
    {
        public const string Administrador = "Administrador";
        public const string Recepcionista = "Recepcionista";
    }

    public class Rol
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
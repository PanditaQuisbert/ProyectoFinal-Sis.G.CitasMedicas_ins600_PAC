using System.ComponentModel.DataAnnotations;

namespace SistemaCitasMedicas.Datos.Entidades
{
    public class Especialidad
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La descripción no debe exceder 250 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public ICollection<Medico> Medicos { get; set; } = new List<Medico>();
    }
}

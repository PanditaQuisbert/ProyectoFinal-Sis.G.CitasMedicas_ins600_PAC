using System.ComponentModel.DataAnnotations;

namespace SistemaCitasMedicas.Datos.Entidades
{
    public class Medico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los nombres no deben exceder 100 caracteres.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no deben exceder 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula profesional es obligatoria.")]
        [StringLength(50, ErrorMessage = "La matrícula no debe exceder 50 caracteres.")]
        [Display(Name = "Matrícula profesional")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una especialidad.")]
        [Display(Name = "Especialidad")]
        public int EspecialidadId { get; set; }

        public Especialidad? Especialidad { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();

        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    }
}

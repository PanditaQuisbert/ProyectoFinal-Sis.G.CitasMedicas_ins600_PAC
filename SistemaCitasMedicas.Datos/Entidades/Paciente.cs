using System.ComponentModel.DataAnnotations;

namespace SistemaCitasMedicas.Datos.Entidades
{
    public class Paciente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
        [StringLength(20, ErrorMessage = "El documento no debe exceder 20 caracteres.")]
        [Display(Name = "Documento de identidad")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los nombres no deben exceder 100 caracteres.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no deben exceder 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El teléfono no debe exceder 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();

        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    }
}

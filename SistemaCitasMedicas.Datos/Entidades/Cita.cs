using System.ComponentModel.DataAnnotations;

namespace SistemaCitasMedicas.Datos.Entidades
{
    public static class EstadosCita
    {
        public const string Programada = "Programada";
        public const string Atendida = "Atendida";
        public const string Cancelada = "Cancelada";

        public static readonly string[] Todos = { Programada, Atendida, Cancelada };
    }

    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un paciente.")]
        [Display(Name = "Paciente")]
        public int PacienteId { get; set; }

        public Paciente? Paciente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un médico.")]
        [Display(Name = "Médico")]
        public int MedicoId { get; set; }

        public Medico? Medico { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
        [Display(Name = "Fecha y hora")]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = EstadosCita.Programada;

        [StringLength(500, ErrorMessage = "Las observaciones no deben exceder 500 caracteres.")]
        public string Observaciones { get; set; } = string.Empty;
    }
}

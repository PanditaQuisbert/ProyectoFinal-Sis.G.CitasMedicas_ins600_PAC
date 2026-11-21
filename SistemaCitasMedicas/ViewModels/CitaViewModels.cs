using System.ComponentModel.DataAnnotations;
using System.Globalization;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.ViewModels
{
    public class CitaFormViewModel
    {
        public static readonly string[] Horarios = Enumerable.Range(0, 19)
            .Select(i => $"{(8 + i / 2):00}:{(i % 2) * 30:00}")
            .ToArray();

        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un paciente.")]
        [Display(Name = "Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una especialidad.")]
        [Display(Name = "Especialidad")]
        public int EspecialidadId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un médico.")]
        [Display(Name = "Médico")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la fecha.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Debe seleccionar la hora.")]
        [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "La hora debe tener el formato HH:mm.")]
        [Display(Name = "Hora")]
        public string Hora { get; set; } = "08:00";

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = EstadosCita.Programada;

        public DateTime FechaHora => DateTime.TryParseExact(
            $"{Fecha:yyyy-MM-dd} {Hora}",
            "yyyy-MM-dd HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var fechaHora)
            ? fechaHora
            : DateTime.MinValue;

        public List<Paciente> Pacientes { get; set; } = new();
        public List<Especialidad> Especialidades { get; set; } = new();
        public List<Medico> Medicos { get; set; } = new();
    }

    public class CitaFiltroViewModel
    {
        [DataType(DataType.Date)] public DateTime? Fecha { get; set; }
        public string? DocumentoPaciente { get; set; }
        public int? MedicoId { get; set; }
        public int? EspecialidadId { get; set; }
        public string? Estado { get; set; }
        public List<Cita> Resultados { get; set; } = new();
        public List<Medico> Medicos { get; set; } = new();
        public List<Especialidad> Especialidades { get; set; } = new();
    }

    public class ReporteFiltroViewModel
    {
        public string Tipo { get; set; } = "dia";
        [DataType(DataType.Date)] public DateTime? Fecha { get; set; } = DateTime.Today;
        [DataType(DataType.Date)] public DateTime? Desde { get; set; } = DateTime.Today;
        [DataType(DataType.Date)] public DateTime? Hasta { get; set; } = DateTime.Today;
        public int? MedicoId { get; set; }
        public int? EspecialidadId { get; set; }
        public int? PacienteId { get; set; }
        public string? Estado { get; set; }
        public List<Cita> Resultados { get; set; } = new();
        public List<Medico> Medicos { get; set; } = new();
        public List<Especialidad> Especialidades { get; set; } = new();
        public List<Paciente> Pacientes { get; set; } = new();
    }
}
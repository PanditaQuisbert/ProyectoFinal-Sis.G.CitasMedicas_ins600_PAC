using System.ComponentModel.DataAnnotations;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.ViewModels
{
    public class CitaFormViewModel
    {
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
        [Display(Name = "Hora")]
        public string Hora { get; set; } = "08:00";

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = EstadosCita.Programada;

        public DateTime FechaHora => DateTime.Parse($"{Fecha:yyyy-MM-dd} {Hora}");

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
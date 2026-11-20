using Microsoft.EntityFrameworkCore;
using SistemaCitasMedicas.Datos.Entidades;

namespace SistemaCitasMedicas.Datos
{
    public class CitasDbContext : DbContext
    {
        public CitasDbContext(DbContextOptions<CitasDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Medico> Medicos => Set<Medico>();
        public DbSet<Especialidad> Especialidades => Set<Especialidad>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.HasData(
                    new Rol { Id = 1, Nombre = RolesSistema.Administrador, Descripcion = "Acceso total al sistema" },
                    new Rol { Id = 2, Nombre = RolesSistema.Recepcionista, Descripcion = "Gestión de pacientes y citas" }
                );
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.NombreUsuario).HasMaxLength(50).IsRequired();
                entity.Property(e => e.NombreCompleto).HasMaxLength(150).IsRequired();
                entity.Property(e => e.ContrasenaHash).HasMaxLength(200).IsRequired();
                entity.HasIndex(e => e.NombreUsuario).IsUnique();
                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.RolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Especialidad>(entity =>
            {
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(250);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.Property(e => e.Nombres).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Apellidos).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Matricula).HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Matricula).IsUnique();
                entity.HasOne(e => e.Especialidad)
                    .WithMany(es => es.Medicos)
                    .HasForeignKey(e => e.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.Property(e => e.Documento).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Nombres).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Apellidos).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Telefono).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.Documento).IsUnique();
            });

            modelBuilder.Entity<Cita>(entity =>
            {
                entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.HasOne(e => e.Paciente)
                    .WithMany(p => p.Citas)
                    .HasForeignKey(e => e.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Medico)
                    .WithMany(m => m.Citas)
                    .HasForeignKey(e => e.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.MedicoId, e.FechaHora });
                entity.HasIndex(e => e.PacienteId);
                entity.HasIndex(e => e.FechaHora);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaCitasMedicas.Datos.Migrations
{
    /// <inheritdoc />
    public partial class IndiceUnicoCitasActivas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas",
                columns: new[] { "MedicoId", "FechaHora" },
                unique: true,
                filter: "Estado <> 'Cancelada'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas",
                columns: new[] { "MedicoId", "FechaHora" });
        }
    }
}

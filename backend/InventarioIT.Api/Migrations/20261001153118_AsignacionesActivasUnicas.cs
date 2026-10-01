using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioIT.Api.Migrations
{
    /// <inheritdoc />
    public partial class AsignacionesActivasUnicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asignaciones_EquipoId",
                table: "Asignaciones");

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesDevolucion",
                table: "Asignaciones",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_EquipoActivo",
                table: "Asignaciones",
                column: "EquipoId",
                unique: true,
                filter: "\"FechaDevolucion\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asignaciones_EquipoActivo",
                table: "Asignaciones");

            migrationBuilder.DropColumn(
                name: "ObservacionesDevolucion",
                table: "Asignaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_EquipoId",
                table: "Asignaciones",
                column: "EquipoId");
        }
    }
}

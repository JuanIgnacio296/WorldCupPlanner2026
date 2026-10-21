using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FechaOcupadaPorFixture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FechasOcupadas",
                table: "FechasOcupadas");

            migrationBuilder.AddColumn<int>(
                name: "FixtureToken",
                table: "FechasOcupadas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FechasOcupadas",
                table: "FechasOcupadas",
                columns: new[] { "EstadioNombre", "Fecha", "FixtureToken" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FechasOcupadas",
                table: "FechasOcupadas");

            migrationBuilder.DropColumn(
                name: "FixtureToken",
                table: "FechasOcupadas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FechasOcupadas",
                table: "FechasOcupadas",
                columns: new[] { "EstadioNombre", "Fecha" });
        }
    }
}

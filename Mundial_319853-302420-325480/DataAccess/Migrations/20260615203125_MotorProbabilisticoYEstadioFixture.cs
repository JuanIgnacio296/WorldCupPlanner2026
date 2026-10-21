using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MotorProbabilisticoYEstadioFixture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estadios_Fixtures_FixtureToken",
                table: "Estadios");

            migrationBuilder.DropIndex(
                name: "IX_Estadios_FixtureToken",
                table: "Estadios");

            migrationBuilder.DropColumn(
                name: "FixtureToken",
                table: "Estadios");

            migrationBuilder.AddColumn<bool>(
                name: "UsaMotorProbabilistico",
                table: "Fixtures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EstadioFixture",
                columns: table => new
                {
                    EstadiosNombre = table.Column<string>(type: "nvarchar(80)", nullable: false),
                    FixtureToken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadioFixture", x => new { x.EstadiosNombre, x.FixtureToken });
                    table.ForeignKey(
                        name: "FK_EstadioFixture_Estadios_EstadiosNombre",
                        column: x => x.EstadiosNombre,
                        principalTable: "Estadios",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstadioFixture_Fixtures_FixtureToken",
                        column: x => x.FixtureToken,
                        principalTable: "Fixtures",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstadioFixture_FixtureToken",
                table: "EstadioFixture",
                column: "FixtureToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadioFixture");

            migrationBuilder.DropColumn(
                name: "UsaMotorProbabilistico",
                table: "Fixtures");

            migrationBuilder.AddColumn<int>(
                name: "FixtureToken",
                table: "Estadios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estadios_FixtureToken",
                table: "Estadios",
                column: "FixtureToken");

            migrationBuilder.AddForeignKey(
                name: "FK_Estadios_Fixtures_FixtureToken",
                table: "Estadios",
                column: "FixtureToken",
                principalTable: "Fixtures",
                principalColumn: "Token");
        }
    }
}

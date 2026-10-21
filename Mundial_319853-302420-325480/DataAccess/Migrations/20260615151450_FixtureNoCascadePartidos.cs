using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixtureNoCascadePartidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Fixtures_Token",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_Partidos_Token",
                table: "Partidos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Token",
                table: "Partidos",
                column: "Token");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Fixtures_Token",
                table: "Partidos",
                column: "Token",
                principalTable: "Fixtures",
                principalColumn: "Token",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Confederacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankingFifa = table.Column<int>(type: "int", nullable: false),
                    Puntos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GolesAFavor = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GolesEnContra = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BanderaBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Nombre);
                    table.CheckConstraint("CK_Equipo_Confederacion", "[Confederacion] IN ('AFC', 'CAF', 'CONCACAF', 'CONMEBOL', 'OFC', 'UEFA')");
                    table.CheckConstraint("CK_Equipo_RankingFifa", "[RankingFifa] BETWEEN 300 AND 2500");
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioCorreo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Semillas",
                columns: table => new
                {
                    Token = table.Column<int>(type: "int", nullable: false),
                    SemillaFixture = table.Column<int>(type: "int", nullable: false),
                    SemillaCompletar = table.Column<int>(type: "int", nullable: false),
                    SemillaCrucesFase = table.Column<int>(type: "int", nullable: false),
                    SemillaSimulacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semillas", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Correo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Correo);
                });

            migrationBuilder.CreateTable(
                name: "Fixtures",
                columns: table => new
                {
                    Token = table.Column<int>(type: "int", nullable: false),
                    SemillaToken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixtures", x => x.Token);
                    table.ForeignKey(
                        name: "FK_Fixtures_Semillas_SemillaToken",
                        column: x => x.SemillaToken,
                        principalTable: "Semillas",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sorteos",
                columns: table => new
                {
                    Token = table.Column<int>(type: "int", nullable: false),
                    SemillaToken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sorteos", x => x.Token);
                    table.ForeignKey(
                        name: "FK_Sorteos_Semillas_SemillaToken",
                        column: x => x.SemillaToken,
                        principalTable: "Semillas",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioCorreo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Usuarios_UsuarioCorreo",
                        column: x => x.UsuarioCorreo,
                        principalTable: "Usuarios",
                        principalColumn: "Correo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipoSorteo",
                columns: table => new
                {
                    EquiposNombre = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    SorteoToken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipoSorteo", x => new { x.EquiposNombre, x.SorteoToken });
                    table.ForeignKey(
                        name: "FK_EquipoSorteo_Equipos_EquiposNombre",
                        column: x => x.EquiposNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipoSorteo_Sorteos_SorteoToken",
                        column: x => x.SorteoToken,
                        principalTable: "Sorteos",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Token = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => new { x.Id, x.Token });
                    table.CheckConstraint("CK_Grupo_Id", "[Id] IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L')");
                    table.ForeignKey(
                        name: "FK_Grupos_Sorteos_Token",
                        column: x => x.Token,
                        principalTable: "Sorteos",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipoGrupo",
                columns: table => new
                {
                    EquiposNombre = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    GrupoId = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    GrupoToken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipoGrupo", x => new { x.EquiposNombre, x.GrupoId, x.GrupoToken });
                    table.ForeignKey(
                        name: "FK_EquipoGrupo_Equipos_EquiposNombre",
                        column: x => x.EquiposNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipoGrupo_Grupos_GrupoId_GrupoToken",
                        columns: x => new { x.GrupoId, x.GrupoToken },
                        principalTable: "Grupos",
                        principalColumns: new[] { "Id", "Token" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estadios",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    FixtureToken = table.Column<int>(type: "int", nullable: true),
                    GrupoId = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    GrupoToken = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadios", x => x.Nombre);
                    table.CheckConstraint("CK_Estadio_Capacidad", "[Capacidad] >= 20000");
                    table.ForeignKey(
                        name: "FK_Estadios_Fixtures_FixtureToken",
                        column: x => x.FixtureToken,
                        principalTable: "Fixtures",
                        principalColumn: "Token");
                    table.ForeignKey(
                        name: "FK_Estadios_Grupos_GrupoId_GrupoToken",
                        columns: x => new { x.GrupoId, x.GrupoToken },
                        principalTable: "Grupos",
                        principalColumns: new[] { "Id", "Token" });
                });

            migrationBuilder.CreateTable(
                name: "EstadioSorteo",
                columns: table => new
                {
                    EstadiosNombre = table.Column<string>(type: "nvarchar(80)", nullable: false),
                    SorteoToken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadioSorteo", x => new { x.EstadiosNombre, x.SorteoToken });
                    table.ForeignKey(
                        name: "FK_EstadioSorteo_Estadios_EstadiosNombre",
                        column: x => x.EstadiosNombre,
                        principalTable: "Estadios",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstadioSorteo_Sorteos_SorteoToken",
                        column: x => x.SorteoToken,
                        principalTable: "Sorteos",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FechasOcupadas",
                columns: table => new
                {
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadioNombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FechasOcupadas", x => new { x.EstadioNombre, x.Fecha });
                    table.ForeignKey(
                        name: "FK_FechasOcupadas_Estadios_EstadioNombre",
                        column: x => x.EstadioNombre,
                        principalTable: "Estadios",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<int>(type: "int", nullable: false),
                    GrupoID = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadioNombre = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    EquipoLocalNombre = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    EquipoVisitanteNombre = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    GolesLocal = table.Column<int>(type: "int", nullable: true),
                    GolesVisitante = table.Column<int>(type: "int", nullable: true),
                    Fase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Horario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jornada = table.Column<int>(type: "int", nullable: true),
                    PenalesLocal = table.Column<int>(type: "int", nullable: true),
                    PenalesVisitante = table.Column<int>(type: "int", nullable: true),
                    TarjetasAmarillasLocal = table.Column<int>(type: "int", nullable: false),
                    TarjetasRojasLocal = table.Column<int>(type: "int", nullable: false),
                    TarjetasAmarillasVisitante = table.Column<int>(type: "int", nullable: false),
                    TarjetasRojasVisitante = table.Column<int>(type: "int", nullable: false),
                    GrupoToken = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => new { x.Id, x.Token });
                    table.CheckConstraint("CK_Partido_Fase", "[Fase] IN ('Fase de Grupos', 'Dieciseisavos de Final', 'Octavos de Final', 'Cuartos de Final', 'Semifinal', 'Tercer y Cuarto Puesto', 'Final')");
                    table.CheckConstraint("CK_Partido_GolesLocal", "[GolesLocal] IS NULL OR [GolesLocal] >= 0");
                    table.CheckConstraint("CK_Partido_GolesVisitante", "[GolesVisitante] IS NULL OR [GolesVisitante] >= 0");
                    table.CheckConstraint("CK_Partido_Jornada", "[Jornada] IS NULL OR [Jornada] BETWEEN 1 AND 3");
                    table.CheckConstraint("CK_Partido_PenalesLocal", "[PenalesLocal] IS NULL OR [PenalesLocal] >= 0");
                    table.CheckConstraint("CK_Partido_PenalesVisitante", "[PenalesVisitante] IS NULL OR [PenalesVisitante] >= 0");
                    table.CheckConstraint("CK_Partido_TarjetasAmarillasLocal", "[TarjetasAmarillasLocal] IS NULL OR [TarjetasAmarillasLocal] >= 0");
                    table.CheckConstraint("CK_Partido_TarjetasAmarillasVisitante", "[TarjetasAmarillasVisitante] IS NULL OR [TarjetasAmarillasVisitante] >= 0");
                    table.CheckConstraint("CK_Partido_TarjetasRojasLocal", "[TarjetasRojasLocal] IS NULL OR [TarjetasRojasLocal] >= 0");
                    table.CheckConstraint("CK_Partido_TarjetasRojasVisitante", "[TarjetasRojasVisitante] IS NULL OR [TarjetasRojasVisitante] >= 0");
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoLocalNombre",
                        column: x => x.EquipoLocalNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoVisitanteNombre",
                        column: x => x.EquipoVisitanteNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Estadios_EstadioNombre",
                        column: x => x.EstadioNombre,
                        principalTable: "Estadios",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Fixtures_Token",
                        column: x => x.Token,
                        principalTable: "Fixtures",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Partidos_Grupos_GrupoID_GrupoToken",
                        columns: x => new { x.GrupoID, x.GrupoToken },
                        principalTable: "Grupos",
                        principalColumns: new[] { "Id", "Token" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipoGrupo_GrupoId_GrupoToken",
                table: "EquipoGrupo",
                columns: new[] { "GrupoId", "GrupoToken" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Nombre",
                table: "Equipos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipoSorteo_SorteoToken",
                table: "EquipoSorteo",
                column: "SorteoToken");

            migrationBuilder.CreateIndex(
                name: "IX_Estadios_FixtureToken",
                table: "Estadios",
                column: "FixtureToken");

            migrationBuilder.CreateIndex(
                name: "IX_Estadios_GrupoId_GrupoToken",
                table: "Estadios",
                columns: new[] { "GrupoId", "GrupoToken" });

            migrationBuilder.CreateIndex(
                name: "IX_Estadios_Nombre",
                table: "Estadios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadioSorteo_SorteoToken",
                table: "EstadioSorteo",
                column: "SorteoToken");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_SemillaToken",
                table: "Fixtures",
                column: "SemillaToken");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_Token",
                table: "Grupos",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UsuarioCorreo",
                table: "Logs",
                column: "UsuarioCorreo");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EquipoLocalNombre",
                table: "Partidos",
                column: "EquipoLocalNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EquipoVisitanteNombre",
                table: "Partidos",
                column: "EquipoVisitanteNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EstadioNombre",
                table: "Partidos",
                column: "EstadioNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_GrupoID_GrupoToken",
                table: "Partidos",
                columns: new[] { "GrupoID", "GrupoToken" });

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Token",
                table: "Partidos",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_Sorteos_SemillaToken",
                table: "Sorteos",
                column: "SemillaToken");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipoGrupo");

            migrationBuilder.DropTable(
                name: "EquipoSorteo");

            migrationBuilder.DropTable(
                name: "EstadioSorteo");

            migrationBuilder.DropTable(
                name: "FechasOcupadas");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Partidos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Estadios");

            migrationBuilder.DropTable(
                name: "Fixtures");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Sorteos");

            migrationBuilder.DropTable(
                name: "Semillas");
        }
    }
}

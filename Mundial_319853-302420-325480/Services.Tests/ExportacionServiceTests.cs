using System.Text;
using System.IO.Compression;
using App.Dominio;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using DTOs;

namespace Services.Tests;

[TestClass]
public class ExportacionServiceTests
{
    private AppDbContext _context;

    [TestInitialize]
    public void TestInitialize()
    {
        _context = new AppDbContext(new DbContextOptions<AppDbContext>());
    }
    [TestMethod]
    public void ExportarAuditoriaCsv_CuandoEsAdministrador_DebeExportarLogsDelRango()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);

        logs.AgregarLog(new Log(admin, "Accion dentro", new DateTime(2026, 6, 1, 10, 0, 0)));
        logs.AgregarLog(new Log(admin, "Accion fuera", new DateTime(2026, 6, 10, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaCsv(admin, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));
        string contenido = Encoding.UTF8.GetString(archivo);

        StringAssert.Contains(contenido, "Fecha,Usuario,Correo,Accion");
        StringAssert.Contains(contenido, "Accion dentro");
        Assert.IsFalse(contenido.Contains("Accion fuera"));
    }

    [TestMethod]
    public void ExportarAuditoriaCsv_CuandoEsPeriodista_DebeExportarLogsDelRango()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        Usuario periodista = new Usuario("Pedro", "Periodista", "periodista@ort.edu.uy", DateTime.Now.AddYears(-30), "Periodista123#", [Rol.Periodista]);

        logs.AgregarLog(new Log(admin, "Accion periodista", new DateTime(2026, 6, 1, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaCsv(periodista, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));
        string contenido = Encoding.UTF8.GetString(archivo);

        StringAssert.Contains(contenido, "Accion periodista");
    }

    [TestMethod]
    public void ExportarAuditoriaCsv_CuandoEsEditor_DebeLanzarExcepcion()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        Usuario editor = new Usuario("Editor", "Sistema", "editor@ort.edu.uy", DateTime.Now.AddYears(-30), "Editor123#", [Rol.Editor]);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.ExportarAuditoriaCsv(editor, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2)));
    }

    [TestMethod]
    public void ExportarFixtureFinalCsv_CuandoEsAdministrador_DebeExportarPartidosFinalDelRango()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        Fixture fixture = CrearFixtureConPartidosFinales();
        fixtureService.GuardarFixture(1, fixture);

        byte[] archivo = service.ExportarFixtureFinalCsv(admin, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));
        string contenido = Encoding.UTF8.GetString(archivo);

        StringAssert.Contains(contenido, "Fecha,Fase,Jornada,Local,GolesLocal,GolesVisitante,Visitante,Estadio");
        StringAssert.Contains(contenido, "Uruguay");
        Assert.IsFalse(contenido.Contains("Brasil"));
    }

    [TestMethod]
    public void ExportarAuditoriaXlsx_CuandoEsAdministrador_DebeExportarArchivoXlsx()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        logs.AgregarLog(new Log(admin, "Accion xlsx", new DateTime(2026, 6, 1, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaXlsx(admin, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));

        Assert.IsTrue(archivo.Length > 0);
        using MemoryStream stream = new MemoryStream(archivo);
        using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
        Assert.IsNotNull(zip.GetEntry("xl/workbook.xml"));
    }

    [TestMethod]
    public void ExportarFixtureFinalXlsx_CuandoEsAdministrador_DebeExportarArchivoXlsx()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        fixtureService.GuardarFixture(1, CrearFixtureConPartidosFinales());

        byte[] archivo = service.ExportarFixtureFinalXlsx(admin, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));

        Assert.IsTrue(archivo.Length > 0);
        using MemoryStream stream = new MemoryStream(archivo);
        using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
        Assert.IsNotNull(zip.GetEntry("xl/workbook.xml"));
    }

    [TestMethod]
    public void ExportarAuditoriaCsv_ConDto_DebeExportarLogs()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        UsuarioReadDto adminDto = new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Sistema",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            Roles = new List<string> { "Administrador" }
        };
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        logs.AgregarLog(new Log(admin, "Accion dentro", new DateTime(2026, 6, 1, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaCsv(adminDto, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));
        string contenido = Encoding.UTF8.GetString(archivo);

        StringAssert.Contains(contenido, "Accion dentro");
    }

    [TestMethod]
    public void ExportarFixtureFinalCsv_ConDto_DebeExportarPartidos()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        UsuarioReadDto adminDto = new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Sistema",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            Roles = new List<string> { "Administrador" }
        };
        fixtureService.GuardarFixture(1, CrearFixtureConPartidosFinales());

        byte[] archivo = service.ExportarFixtureFinalCsv(adminDto, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));
        string contenido = Encoding.UTF8.GetString(archivo);

        StringAssert.Contains(contenido, "Uruguay");
    }

    [TestMethod]
    public void ExportarAuditoriaXlsx_ConDto_DebeExportarArchivo()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        UsuarioReadDto adminDto = new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Sistema",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            Roles = new List<string> { "Administrador" }
        };
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        logs.AgregarLog(new Log(admin, "Accion xlsx", new DateTime(2026, 6, 1, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaXlsx(adminDto, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));

        Assert.IsTrue(archivo.Length > 0);
    }

    [TestMethod]
    public void ExportarFixtureFinalXlsx_ConDto_DebeExportarArchivo()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido(), new NotificacionService(_context, auditoriaService));
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        IExportacionService service = new ExportacionService(logs, fixtureService, auditoriaService);
        UsuarioReadDto adminDto = new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Sistema",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            Roles = new List<string> { "Administrador" }
        };
        fixtureService.GuardarFixture(1, CrearFixtureConPartidosFinales());

        byte[] archivo = service.ExportarFixtureFinalXlsx(adminDto, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));

        Assert.IsTrue(archivo.Length > 0);
    }

    private static Fixture CrearFixtureConPartidosFinales()
    {
        Estadio estadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");
        Semilla semilla = new Semilla(1, 1, 1, 1, 1);
        Fixture fixture = new Fixture(1, [], [estadio], semilla, true);

        fixture.Partidos.Add(new Partido(104, 1, null,
            new DateTime(2026, 6, 1, 14, 0, 0),
            estadio,
            new Equipo("Uruguay", "CONMEBOL", 400),
            new Equipo("Argentina", "CONMEBOL", 900),
            2,
            1,
            "Final",
            "14:00",
            null
        ));

        fixture.Partidos.Add(new Partido(103, 1, null,
            new DateTime(2026, 6, 10, 14, 0, 0),
            estadio,
            new Equipo("Brasil", "CONMEBOL", 800),
            new Equipo("Francia", "UEFA", 850),
            1,
            0,
            "Final",
            "14:00",
            null
        ));

        fixture.Partidos.Add(new Partido(102, 1, null,
            new DateTime(2026, 6, 1, 18, 0, 0),
            estadio,
            new Equipo("Espana", "UEFA", 700),
            new Equipo("Italia", "UEFA", 650),
            1,
            1,
            "Semifinal",
            "18:00",
            null
        ));

        return fixture;
    }
}

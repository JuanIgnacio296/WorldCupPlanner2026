using App.Dominio;
using DTOs;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class FixtureDtoServiceTests
{
    private IFixtureService _servicio = null!;
    private AppDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        _context = new AppDbContext(new DbContextOptions<AppDbContext>());
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido());
        _servicio = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
    }

    [TestMethod]
    public void ObtenerFixture_ConFixtureGuardado_DebeDevolverFixtureReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(10, fixture);

        FixtureReadDto obtenido = _servicio.ObtenerFixtureDto();

        Assert.AreEqual(10, obtenido.TokenActual);
        Assert.AreEqual(0, obtenido.PartidosJugados);
        Assert.AreEqual(1, obtenido.Partidos.Count);
        Assert.AreEqual("Uruguay", obtenido.Partidos[0].EquipoLocal.Nombre);
        Assert.AreEqual("Argentina", obtenido.Partidos[0].EquipoVisitante.Nombre);
        Assert.AreNotEqual(typeof(Fixture), obtenido.GetType());
        Assert.AreNotEqual(typeof(Partido), obtenido.Partidos[0].GetType());
    }

    [TestMethod]
    public void ObtenerFixturePorToken_ConTokenExistente_DebeDevolverFixtureReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(20, fixture);

        FixtureReadDto obtenido = _servicio.ObtenerFixturePorTokenDto(20);

        Assert.AreEqual(20, obtenido.TokenActual);
        Assert.AreEqual(1, obtenido.Partidos.Count);
        Assert.AreEqual("Uruguay", obtenido.Partidos[0].EquipoLocal.Nombre);
        Assert.AreNotEqual(typeof(Fixture), obtenido.GetType());
    }

    [TestMethod]
    public void ObtenerFixturePorTokenSemilla_ConTokenSemillaExistente_DebeDevolverFixtureReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(30, fixture);

        FixtureReadDto obtenido = _servicio.ObtenerFixturePorTokenSemillaDto(1);

        Assert.AreEqual(30, obtenido.TokenActual);
        Assert.AreEqual(1, obtenido.Partidos.Count);
        Assert.AreEqual("Argentina", obtenido.Partidos[0].EquipoVisitante.Nombre);
        Assert.AreNotEqual(typeof(Fixture), obtenido.GetType());
    }

    [TestMethod]
    public void ObtenerPartidosListado_ConFixtureGuardado_DebeDevolverListaDePartidoReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(40, fixture);

        List<PartidoReadDto> partidos = _servicio.ObtenerPartidosListado();

        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual("Uruguay", partidos[0].EquipoLocal.Nombre);
        Assert.AreEqual("Fase de Grupos", partidos[0].Fase);
        Assert.AreNotEqual(typeof(Partido), partidos[0].GetType());
    }

    [TestMethod]
    public void ObtenerPartidosPorFaseListado_ConFaseExistente_DebeDevolverListaDePartidoReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(50, fixture);

        List<PartidoReadDto> partidos = _servicio.ObtenerPartidosPorFaseListado("Fase de Grupos");

        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual("Fase de Grupos", partidos[0].Fase);
        Assert.AreEqual("Uruguay", partidos[0].EquipoLocal.Nombre);
        Assert.AreNotEqual(typeof(Partido), partidos[0].GetType());
    }

    [TestMethod]
    public void SimularFecha_ConUsuarioReadDto_DebeSimularYDevolverFixtureReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(60, fixture);
        UsuarioReadDto editor = CrearEditor();
        DateTime fecha = new DateTime(2026, 6, 1, 14, 0, 0);

        FixtureReadDto actualizado = _servicio.SimularFecha(fecha, editor, new Random(1));

        Assert.AreEqual(60, actualizado.TokenActual);
        Assert.AreEqual(1, actualizado.PartidosJugados);
        Assert.IsNotNull(actualizado.Partidos[0].GolesLocal);
        Assert.IsNotNull(actualizado.Partidos[0].GolesVisitante);
        Assert.AreNotEqual(typeof(Fixture), actualizado.GetType());
    }

    [TestMethod]
    public void SimularJornada_ConUsuarioReadDto_DebeSimularYDevolverFixtureReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(70, fixture);
        UsuarioReadDto editor = CrearEditor();

        FixtureReadDto actualizado = _servicio.SimularJornada(1, editor, new Random(1));

        Assert.AreEqual(70, actualizado.TokenActual);
        Assert.AreEqual(1, actualizado.PartidosJugados);
        Assert.IsNotNull(actualizado.Partidos[0].GolesLocal);
        Assert.IsNotNull(actualizado.Partidos[0].GolesVisitante);
        Assert.AreNotEqual(typeof(Fixture), actualizado.GetType());
    }

    [TestMethod]
    public void SimularFase_ConUsuarioReadDto_DebeSimularYDevolverFixtureReadDto()
    {
        Fixture fixture = CrearFixtureConPartido();
        _servicio.GuardarFixture(80, fixture);
        UsuarioReadDto editor = CrearEditor();

        FixtureReadDto actualizado = _servicio.SimularFase("Fase de Grupos", editor, new Random(1));

        Assert.AreEqual(80, actualizado.TokenActual);
        Assert.AreEqual(1, actualizado.PartidosJugados);
        Assert.IsNotNull(actualizado.Partidos[0].GolesLocal);
        Assert.IsNotNull(actualizado.Partidos[0].GolesVisitante);
        Assert.AreNotEqual(typeof(Fixture), actualizado.GetType());
    }

    private static Fixture CrearFixtureConPartido()
    {
        Estadio estadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 400);
        Equipo visitante = new Equipo("Argentina", "CONMEBOL", 900);
        Semilla semilla = new Semilla(1, 1, 1, 1, 1);
        Fixture fixture = new Fixture(1, [], [estadio], semilla, true);

        fixture.Partidos.Add(new Partido(
            1,
            1,
            'A',
            new DateTime(2026, 6, 1, 14, 0, 0),
            estadio,
            local,
            visitante,
            "Fase de Grupos",
            1
        ));

        return fixture;
    }

    private static UsuarioReadDto CrearEditor()
    {
        return new UsuarioReadDto
        {
            Nombre = "Editor",
            Apellido = "Fixture",
            Correo = "editor@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            Roles = ["Editor"]
        };
    }
}

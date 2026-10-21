using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

[TestClass]
public class FixtureEFRepositoryTests
{
    private AppDbContext _context;
    private FixtureEFRepository _repository;
    private Fixture _fixture;
    
    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new FixtureEFRepository(_context);

        _fixture = InicializarFixture();
    }

    private Fixture InicializarFixture()
    {
        List<Equipo> equipos = new List<Equipo>()
        {
            new Equipo("Equipo1", "CONMEBOL", 1200),
            new Equipo("Equipo2", "UEFA", 700),
            new Equipo("Equipo3", "AFC", 600),
            new Equipo("Equipo4", "CAF", 500),
            new Equipo("Equipo5", "UEFA", 1400),
            new Equipo("Equipo6", "CONMEBOL", 800),
            new Equipo("Equipo7", "CONCACAF", 650),
            new Equipo("Equipo8", "AFC", 550),
            new Equipo("Equipo9", "CONMEBOL", 1200),
            new Equipo("Equipo10", "UEFA", 700),
            new Equipo("Equipo11", "AFC", 600),
            new Equipo("Equipo12", "CAF", 500),
            new Equipo("Equipo13", "CONMEBOL", 1200),
            new Equipo("Equipo14", "UEFA", 700),
            new Equipo("Equipo15", "AFC", 600),
            new Equipo("Equipo16", "CAF", 500),
            new Equipo("Equipo17", "CONMEBOL", 1200),
            new Equipo("Equipo18", "UEFA", 700),
            new Equipo("Equipo19", "AFC", 600),
            new Equipo("Equipo20", "CAF", 500),
            new Equipo("Equipo21", "UEFA", 1400),
            new Equipo("Equipo22", "CONMEBOL", 800),
            new Equipo("Equipo23", "CONCACAF", 650),
            new Equipo("Equipo24", "AFC", 550),
            new Equipo("Equipo25", "CONMEBOL", 1200),
            new Equipo("Equipo26", "UEFA", 700),
            new Equipo("Equipo27", "AFC", 600),
            new Equipo("Equipo28", "CAF", 500),
            new Equipo("Equipo29", "CONMEBOL", 1200),
            new Equipo("Equipo30", "UEFA", 700),
            new Equipo("Equipo31", "AFC", 600),
            new Equipo("Equipo32", "CAF", 500),
            new Equipo("Equipo33", "CONMEBOL", 1200),
            new Equipo("Equipo34", "UEFA", 700),
            new Equipo("Equipo35", "AFC", 600),
            new Equipo("Equipo36", "CAF", 500),
            new Equipo("Equipo37", "UEFA", 1400),
            new Equipo("Equipo38", "CONMEBOL", 800),
            new Equipo("Equipo39", "CONCACAF", 650),
            new Equipo("Equipo40", "AFC", 550),
            new Equipo("Equipo41", "CONMEBOL", 1200),
            new Equipo("Equipo42", "UEFA", 700),
            new Equipo("Equipo43", "AFC", 600),
            new Equipo("Equipo44", "CAF", 500),
            new Equipo("Equipo45", "CONMEBOL", 1200),
            new Equipo("Equipo46", "UEFA", 700),
            new Equipo("Equipo47", "AFC", 600),
            new Equipo("Equipo48", "CAF", 500)
        };

        List<Grupo> grupos = new List<Grupo>();
        for (int j = 0; j < 12; j++) grupos.Add(new Grupo((char)('A' + j), 1));
        
        for (int i = 0; i < 48; i++) grupos[i / 4].AddEquipo(equipos[i]);

        List<Estadio> estadios = new List<Estadio>()
        {
            new Estadio("Estadio1", "Ciudad", 40000, "Descripcion"),
            new Estadio("Estadio2", "Ciudad", 42000, "Descripcion"),
            new Estadio("Estadio3", "Ciudad", 45000, "Descripcion"),
            new Estadio("Estadio4", "Ciudad", 50000, "Descripcion"),
            new Estadio("Estadio5", "Ciudad", 55000, "Descripcion")
        };
        Semilla semilla = new Semilla(1, 123, 456, 789, 0);
        return new Fixture(1, grupos, estadios, semilla, true);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarFixture_AumentaLaCantidadDeFixtures()
    {
        int cantIni = _context.Fixtures.Count();
        _repository.AgregarFixture(_fixture.Token, _fixture);
        int cantFin = _context.Fixtures.Count();
        Assert.AreEqual(cantFin, cantIni + 1);
    }

    [TestMethod]
    public void ObtenerFixturePorToken_SiNoEstaDeberiaDarUnaExcepcion()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        Exception ex = Assert.ThrowsException<NullReferenceException>(() =>
        {
            _repository.ObtenerFixturePorToken(_fixture.Token + 1);
        });
        Assert.AreEqual("No existe un fixture para ese token.", ex.Message);
    }

    [TestMethod]
    public void ObtenerFixturePorToken_SiLoEncuentraDeberiaDevolverElCorrecto()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        Fixture? f = _repository.ObtenerFixturePorToken(_fixture.Token);
        Assert.AreEqual(f.Token, _fixture.Token);
    }

    [TestMethod]
    public void ObtenerFixturePorSemilla_SiNoEstaDeberiaDarUnaExcepcion()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        Exception ex = Assert.ThrowsException<NullReferenceException>(() =>
        {
            _repository.ObtenerTokenPorSemilla(_fixture.Semilla.Token + 1);
        });
        Assert.AreEqual("No existe un token para esa semilla.", ex.Message);
    }
    
    [TestMethod]
    public void ObtenerFixturePorSemilla_SiEstaDeberiaDevolverElCorrecto()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        int token = _repository.ObtenerTokenPorSemilla(_fixture.Semilla.Token);
        Assert.AreEqual(token, _fixture.Token);
    }

    [TestMethod]
    public void ExisteFixture_SiNoEstaDeberiaDarFalse()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        bool esta = _repository.ExisteFixture(_fixture.Token + 1);
        Assert.IsFalse(esta);
    }

    [TestMethod]
    public void ExisteFixture_SiEstaDeberiaDarTrue()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        bool esta = _repository.ExisteFixture(_fixture.Token);
        Assert.IsTrue(esta);
    }

    [TestMethod]
    public void ObtenerFixtures_SiNoHayFixtureDeberiaDar0()
    {
        List<Fixture> fixtures = _repository.ObtenerFixtures();
        Assert.AreEqual(0, fixtures.Count);
    }

    [TestMethod]
    public void ObtenerFixtures_SiHayFixtureDeberiaDarLaCantidadCorrecta()
    {
        _repository.AgregarFixture(_fixture.Token, _fixture);
        
        List<Fixture> fixtures = _repository.ObtenerFixtures();
        Assert.AreEqual(1, fixtures.Count);
    }
}
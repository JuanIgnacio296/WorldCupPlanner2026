using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Tests;

[TestClass]
public class SorteoEFRepositoryTests
{
    private AppDbContext _context;
    private SorteoEFRepository _repository;
    private Sorteo _sorteo;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new SorteoEFRepository(_context);

        _sorteo = CrearSorteo(1, new Semilla(1, 100, 200, 300, 400));
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void GenerarToken_SinSorteos_DeberiaRetornarUno()
    {
        Assert.AreEqual(1, _repository.GenerarToken());
    }

    [TestMethod]
    public void GenerarToken_ConSorteoExistente_DeberiaRetornarElSiguiente()
    {
        _repository.AgregarSorteo(1, _sorteo);

        Assert.AreEqual(2, _repository.GenerarToken());
    }

    [TestMethod]
    public void AgregarSorteo_DeberiaGuardarSorteoEnBaseDeDatos()
    {
        _repository.AgregarSorteo(1, _sorteo);

        Assert.AreEqual(1, _context.Sorteos.Count());
        Assert.AreEqual(1, _context.Sorteos.First().Token);
    }

    [TestMethod]
    public void ObtenerSorteoPorToken_DeberiaRetornarSorteoCorrecto()
    {
        _repository.AgregarSorteo(1, _sorteo);

        Sorteo result = _repository.ObtenerSorteoPorToken(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Token);
    }

    [TestMethod]
    public void ObtenerSorteoPorToken_CuandoNoExiste_DeberiaLanzarExcepcion()
    {
        Exception ex = Assert.ThrowsException<Exception>(() => _repository.ObtenerSorteoPorToken(999));

        Assert.AreEqual("No existe un sorteo para ese token.", ex.Message);
    }

    [TestMethod]
    public void ExisteSorteoConSemilla_ConSemillaDeIgualesValores_DeberiaRetornarTrue()
    {
        _repository.AgregarSorteo(1, _sorteo);

        Semilla mismosValoresOtroToken = new Semilla(99, 100, 200, 300, 400);

        Assert.IsTrue(_repository.ExisteSorteoConSemilla(mismosValoresOtroToken));
    }

    [TestMethod]
    public void AgregarSorteo_CuandoEquiposYaRastreadosComoOtraInstancia_NoLanzaError()
    {
        _context.Equipos.Add(new Equipo("Uruguay", "CONMEBOL", 500));
        _context.Equipos.Add(new Equipo("Argentina", "CONMEBOL", 600));
        _context.Estadios.Add(new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico"));
        _context.SaveChanges();

        Sorteo sorteo = CrearSorteo(5, new Semilla(5, 1, 2, 3, 4));

        _repository.AgregarSorteo(5, sorteo);

        Assert.AreEqual(1, _context.Sorteos.Count(s => s.Token == 5));
        Assert.AreEqual(2, _context.Equipos.Count());
    }

    private static Sorteo CrearSorteo(int token, Semilla semilla)
    {
        List<Equipo> equipos = new List<Equipo>
        {
            new Equipo("Uruguay", "CONMEBOL", 500),
            new Equipo("Argentina", "CONMEBOL", 600)
        };
        List<Estadio> estadios = new List<Estadio>
        {
            new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico")
        };
        return new Sorteo(equipos, token, semilla, estadios);
    }
}
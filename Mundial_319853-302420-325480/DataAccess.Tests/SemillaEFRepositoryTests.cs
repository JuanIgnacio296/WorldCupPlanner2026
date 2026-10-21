using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Tests;

[TestClass]
public class SemillaEFRepositoryTests
{
    private AppDbContext _context;
    private SemillaEFRepository _repository;
    private Semilla _semilla;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new SemillaEFRepository(_context);

        _semilla = new Semilla(1, 100, 200, 300, 400);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarSemilla_DeberiaGuardarSemillaEnBaseDeDatos()
    {
        _repository.AgregarSemilla(_semilla);

        Assert.AreEqual(1, _context.Semillas.Count());
        Assert.AreEqual(1, _context.Semillas.First().Token);
    }

    [TestMethod]
    public void ObtenerSemillas_DeberiaRetornarTodasLasSemillas()
    {
        _context.Semillas.Add(_semilla);
        _context.Semillas.Add(new Semilla(2, 101, 201, 301, 401));
        _context.SaveChanges();

        List<Semilla> result = _repository.ObtenerSemillas();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ObtenerSemillaPorToken_DeberiaRetornarSemillaCorrecta()
    {
        _context.Semillas.Add(_semilla);
        _context.SaveChanges();

        Semilla result = _repository.ObtenerSemilla(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Token);
    }

    [TestMethod]
    public void ObtenerSemillaPorToken_CuandoNoExiste_DeberiaLanzarExcepcion()
    {
        Exception ex = Assert.ThrowsException<Exception>(() => _repository.ObtenerSemilla(999));

        Assert.AreEqual("No existe una semilla con ese token", ex.Message);
    }

    [TestMethod]
    public void ExisteSemillaPorToken_DeberiaRetornarTrueSiExiste()
    {
        _context.Semillas.Add(_semilla);
        _context.SaveChanges();

        Assert.IsTrue(_repository.ExisteSemilla(1));
        Assert.IsFalse(_repository.ExisteSemilla(999));
    }

    [TestMethod]
    public void ExisteSemilla_ConSemillaDeIgualesValores_DeberiaRetornarTrue()
    {
        _context.Semillas.Add(_semilla);
        _context.SaveChanges();

        Semilla mismoToken = new Semilla(1, 100, 200, 300, 400);
        Semilla mismosValoresOtroToken = new Semilla(99, 100, 200, 300, 400);

        Assert.IsTrue(_repository.ExisteSemilla(mismoToken), "mismo token");
        Assert.IsTrue(_repository.ExisteSemilla(mismosValoresOtroToken), "mismos valores, otro token");
    }

    [TestMethod]
    public void ObtenerSemillaPorSemilla_DeberiaRetornarLaSemillaConIgualesValores()
    {
        _context.Semillas.Add(_semilla);
        _context.SaveChanges();

        Semilla mismosValoresOtroToken = new Semilla(99, 100, 200, 300, 400);

        Semilla result = _repository.ObtenerSemilla(mismosValoresOtroToken);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Token);
    }

    [TestMethod]
    public void ObtenerSemillaNullable_DeberiaRetornarLaSemillaSiExisteYNullSiNo()
    {
        _context.Semillas.Add(_semilla);
        _context.SaveChanges();

        Assert.IsNotNull(_repository.ObtenerSemillaNullable(1));
        Assert.IsNull(_repository.ObtenerSemillaNullable(999));
    }
}

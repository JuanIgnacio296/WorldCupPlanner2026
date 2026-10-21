using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

[TestClass]
public class EstadioEFRepositoryTests
{
    private AppDbContext _context;
    private EstadioEFRepository _repository;
    private Estadio _estadio;
    
    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new EstadioEFRepository(_context);
        
        _estadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarEstadio_DeberiaAumentarCantidadEstadios()
    {
        int cantIni = _context.Estadios.Count();
        _repository.AgregarEstadio(_estadio);
        
        int cantFin =  _context.Estadios.Count();
        Assert.AreEqual(cantIni + 1, cantFin);
    }

    [TestMethod]
    public void ObtenerEstadios_DeberiaObtenerTodosLosEstadios()
    {
        Estadio _estadio2 = new Estadio("Estadio A", "Ciudad A", 40000, "Desc");
        _repository.AgregarEstadio(_estadio);
        _repository.AgregarEstadio(_estadio2);

        List<Estadio> _estadios = _repository.ObtenerEstadios();
        Assert.IsTrue(_estadios.Contains(_estadio));
        Assert.IsTrue(_estadios.Contains(_estadio2));
    }

    [TestMethod]
    public void ObtenerEstadio_DeberiaObtenerElEstadioCorrecto()
    {
        Estadio _estadio2 = new Estadio("Estadio A", "Ciudad A", 40000, "Desc");
        _repository.AgregarEstadio(_estadio);
        _repository.AgregarEstadio(_estadio2);

        Estadio estadioObtenido = _repository.ObtenerEstadio("Estadio A");
        Assert.AreEqual(estadioObtenido, _estadio2);
    }

    [TestMethod]
    public void ObtenerEstadio_SiNoHayEstadiosDeberiaDevolverNull()
    {
        Estadio? _estadioNull = _repository.ObtenerEstadio("EstadioQueNoEsta");
        Assert.IsNull(_estadioNull);
    }

    [TestMethod]
    public void ObtenerEstadio_NoEstaEstadioDeberiaDevolverNull()
    {
        _repository.AgregarEstadio(_estadio);
        Estadio? _estadioNull = _repository.ObtenerEstadio("EstadioQueNoEsta");
        Assert.IsNull(_estadioNull);
    }

    [TestMethod]
    public void ExisteEstadio_SiEstaDaTrue()
    {
        _repository.AgregarEstadio(_estadio);
        bool esta = _repository.ExisteEstadio("Centenario");
        Assert.IsTrue(esta);
    }

    [TestMethod]
    public void ExisteEstadio_SiNoEstaDaFalse()
    {
        _repository.AgregarEstadio(_estadio);
        bool esta = _repository.ExisteEstadio("EstadioNoEsta");
        Assert.IsFalse(esta);
    }

    [TestMethod]
    public void EliminarEstadio_SiEstaLaCantidadDisminuye()
    {
        _repository.AgregarEstadio(_estadio);
        int cantIni = _context.Estadios.Count();
        
        _repository.EliminarEstadio(_estadio);
        int cantFin = _context.Estadios.Count();
        
        Assert.AreEqual(cantIni, cantFin + 1);
    }

    [TestMethod]
    public void EliminarEstadio_SiNoEstaMantieneLaCantidad()
    {
        _repository.AgregarEstadio(_estadio);
        int cantIni = _context.Estadios.Count();
        Estadio noExiste = new Estadio("Inexistente", "Ciudad", 30000, "Desc");
        
        _repository.EliminarEstadio(noExiste);
        
        int cantFin = _context.Estadios.Count();
        Assert.AreEqual(cantIni, cantFin);
    }
}

using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

[TestClass]
public class FechasOcupadasEFRepositoryTests
{
    private AppDbContext _context = null!;
    private FechaOcupadaEFRepository _repository = null!;
    private Estadio _estadio = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new FechaOcupadaEFRepository(_context);

        _estadio = new Estadio("Centenario", "Montevideo", 60000, "Historico");
        _context.Estadios.Add(_estadio);
        _context.SaveChanges();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (_context != null)
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    [TestMethod]
    public void AgregarFecha_DebePersistirLaFecha()
    {
        var fecha = new FechaOcupada(DateTime.Today, _estadio);
        
        _repository.AgregarFecha(fecha);
        
        Assert.IsTrue(_repository.EsFechaOcupada(fecha));
    }

    [TestMethod]
    public void EsFechaOcupada_DebeRetornarTrueSiExiste()
    {
        var fecha = new FechaOcupada(DateTime.Today, _estadio);
        _repository.AgregarFecha(fecha);
        
        bool resultado = _repository.EsFechaOcupada(new FechaOcupada(DateTime.Today, _estadio));
        
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsFechaOcupada_DebeRetornarFalseSiNoExiste()
    {
        var fecha = new FechaOcupada(DateTime.Today, _estadio);
        
        bool resultado = _repository.EsFechaOcupada(fecha);
        
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ObtenerFechasOcupadasPorEstadio_DebeRetornarSoloLasDelEstadio()
    {
        var estadio2 = new Estadio("Gran Parque Central", "Montevideo", 34000, "Nacional");
        _context.Estadios.Add(estadio2);
        _context.SaveChanges();

        var fecha1 = new FechaOcupada(DateTime.Today, _estadio);
        var fecha2 = new FechaOcupada(DateTime.Today, estadio2);
        
        _repository.AgregarFecha(fecha1);
        _repository.AgregarFecha(fecha2);
        
        var resultado = _repository.ObtenerFechasOcupadasPorEstadio(_estadio);
        
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(_estadio.Nombre, resultado[0].Estadio.Nombre);
    }

    [TestMethod]
    public void EliminarFecha_DebeRemoverLaFechaSiExiste()
    {
        var fecha = new FechaOcupada(DateTime.Today, _estadio);
        _repository.AgregarFecha(fecha);
        
        _repository.EliminarFecha(fecha);
        
        Assert.IsFalse(_repository.EsFechaOcupada(fecha));
    }

    [TestMethod]
    public void EliminarFecha_NoDebeFallarSiNoExiste()
    {
        var fecha = new FechaOcupada(DateTime.Today, _estadio);
        
        _repository.EliminarFecha(fecha);
        
        Assert.IsFalse(_repository.EsFechaOcupada(fecha));
    }

    [TestMethod]
    public void EliminarFechasPorEstadio_DebeLimpiarTodasLasFechasDelEstadio()
    {
        var fecha1 = new FechaOcupada(DateTime.Today, _estadio);
        var fecha2 = new FechaOcupada(DateTime.Today.AddDays(1), _estadio);
        
        _repository.AgregarFecha(fecha1);
        _repository.AgregarFecha(fecha2);
        
        _repository.EliminarFechasPorEstadio(_estadio);
        
        Assert.AreEqual(0, _repository.ObtenerFechasOcupadasPorEstadio(_estadio).Count);
    }
}

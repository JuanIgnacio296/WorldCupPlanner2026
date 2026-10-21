using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

[TestClass]
public class LogEFRepositoryTests
{
    private AppDbContext _context;
    private LogEFRepository _repository;
    private Log _log;
    private Usuario admin;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new LogEFRepository(_context);

        admin = new Usuario("Juan", "Perez", "admin@ort.edu.uy", DateTime.Now.AddYears(-25), "Admin123#", new List<Rol> { Rol.Administrador });
        _log = new Log(admin,"accion",new DateTime(DateTime.Now.Year,DateTime.Now.Month,1));
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarLog_DeneriaAumentarLaCantidadDeLogs()
    {
        int cantIni = _context.Logs.Count();
        
        _repository.AgregarLog(_log);
        int cantFin =  _context.Logs.Count();
        
        Assert.AreEqual(cantFin, cantIni+1);
    }

    [TestMethod]
    public void ObtenerLogs_SiNoHayLogsDeberiaRetornar0()
    {
        List<Log> logs =  _repository.ObtenerLogs();
        Assert.AreEqual(0, logs.Count());
    }

    [TestMethod]
    public void ObtenerLogs_SiHayLogsDeberiaRetornarLaCantidadAdecuada()
    {
        _repository.AgregarLog(_log);
        List<Log> logs =  _repository.ObtenerLogs();
        Assert.AreEqual(1, logs.Count());
    }
    
    [TestMethod]
    public void ObtenerLogsConRango_SiNoHayLogsEnRangoDeberiaRetornar0()
    {
        _repository.AgregarLog(_log);
        List<Log> logs = _repository.ObtenerLogs(DateTime.Now, DateTime.Now.AddDays(-1));
        Assert.AreEqual(0, logs.Count());
    }
    
    [TestMethod]
    public void ObtenerLogsConRango_SiHayLogsEnRangoDeberiaRetornarLaCantidadAdecuada()
    {
        _repository.AgregarLog(_log);
        Log log2 = new Log(admin, "accion2", DateTime.Now.AddDays(-1));
        _repository.AgregarLog(log2);
        
        List<Log> logs = _repository.ObtenerLogs(DateTime.Now.AddDays(-2), DateTime.Now);
        Assert.AreEqual(1, logs.Count());
    }
}
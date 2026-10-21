using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Tests;

[TestClass]
public class EquipoEFRepositoryTests
{
    private AppDbContext _context;
    private EquipoEFRepository _repository;
    private Equipo uru;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new EquipoEFRepository(_context);
        
        uru = new Equipo("Uruguay", "CONMEBOL", 500);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarEquipo_DeberiaGuardarEquipoEnBaseDeDatos()
    {
        _repository.AgregarEquipo(uru);
        Assert.AreEqual(1, _context.Equipos.Count());
        Assert.AreEqual("Uruguay", _context.Equipos.First().Nombre);
    }

    [TestMethod]
    public void ObtenerEquipos_DeberiaRetornarTodosLosEquipos()
    {
        _context.Equipos.Add(uru);
        _context.Equipos.Add(new Equipo("Argentina", "CONMEBOL", 600));
        _context.SaveChanges();

        List<Equipo> result = _repository.ObtenerEquipos();

        Assert.AreEqual(2, result.Count);
    }
    
    [TestMethod]
    public void ExisteEquipo_DeberiaRetornarTrueSiElEquipoExiste()
    {
        
        _context.Equipos.Add(uru);
        _context.SaveChanges();

        
        bool existe = _repository.ExisteEquipo("Uruguay");

        
        Assert.IsTrue(existe);
    }
    
    [TestMethod]
    public void ObtenerEquipo_DeberiaRetornarEquipoCorrectoPorNombre()
    {

        Equipo equipo = uru;
        _context.Equipos.Add(equipo);
        _context.SaveChanges();

        
        Equipo result = _repository.ObtenerEquipo("Uruguay");

        
        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay", result.Nombre);
    }

    [TestMethod]
    public void EliminarEquipo_DeberiaTenerUnoMenosEnCantidadEquipos()
    {
        _repository.AgregarEquipo(uru);
        int cantIni = _context.Equipos.Count();
        
        _repository.EliminarEquipo(uru);
        int cantFin = _context.Equipos.Count();
        
        Assert.AreEqual(cantFin+1, cantIni);
    }
}

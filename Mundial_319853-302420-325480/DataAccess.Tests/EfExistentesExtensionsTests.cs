using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

[TestClass]
public class EfExistentesExtensionsTests
{
    private AppDbContext _context;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void MarcarExistente_ConEntidadNula_NoDebeHacerNada()
    {
        _context.MarcarExistente(null);
    }

    [TestMethod]
    public void MarcarExistente_ConEntidadNoAgregada_NoDebeCambiarEstado()
    {
        var equipo = new Equipo("Uruguay", "CONMEBOL", 500);
        _context.Equipos.Add(equipo);
        _context.SaveChanges();
        
        _context.Entry(equipo).State = EntityState.Modified;
        
        _context.MarcarExistente(equipo);
        
        Assert.AreEqual(EntityState.Modified, _context.Entry(equipo).State);
    }

    [TestMethod]
    public void MarcarExistente_ConEntidadAgregada_DebeMarcarComoUnchanged()
    {
        var equipo = new Equipo("Uruguay", "CONMEBOL", 500);
        _context.Equipos.Add(equipo);
        
        Assert.AreEqual(EntityState.Added, _context.Entry(equipo).State);
        
        _context.MarcarExistente(equipo);
        
        Assert.AreEqual(EntityState.Unchanged, _context.Entry(equipo).State);
    }

    [TestMethod]
    public void MarcarExistentes_ConColeccionNula_NoDebeHacerNada()
    {
        _context.MarcarExistentes(null);
    }

    [TestMethod]
    public void MarcarExistentes_ConColeccionVacia_NoDebeHacerNada()
    {
        _context.MarcarExistentes(new List<Equipo>());
    }

    [TestMethod]
    public void MarcarExistentes_ConColeccionValida_DebeMarcarComoUnchanged()
    {
        var equipo1 = new Equipo("Uruguay", "CONMEBOL", 500);
        var equipo2 = new Equipo("Argentina", "CONMEBOL", 400);
        
        _context.Equipos.Add(equipo1);
        _context.Equipos.Add(equipo2);
        
        _context.MarcarExistentes(new List<Equipo> { equipo1, equipo2 });
        
        Assert.AreEqual(EntityState.Unchanged, _context.Entry(equipo1).State);
        Assert.AreEqual(EntityState.Unchanged, _context.Entry(equipo2).State);
    }

    [TestMethod]
    public void MarcarTipoExistente_DebeMarcarTodasLasEntidadesDeEseTipoComoUnchanged()
    {
        var equipo1 = new Equipo("Uruguay", "CONMEBOL", 500);
        var equipo2 = new Equipo("Argentina", "CONMEBOL", 400);
        var estadio = new Estadio("Centenario", "Montevideo", 60000, "Historico");
        
        _context.Equipos.Add(equipo1);
        _context.Equipos.Add(equipo2);
        _context.Estadios.Add(estadio);
        
        _context.MarcarTipoExistente<Equipo>();
        
        Assert.AreEqual(EntityState.Unchanged, _context.Entry(equipo1).State);
        Assert.AreEqual(EntityState.Unchanged, _context.Entry(equipo2).State);
        Assert.AreEqual(EntityState.Added, _context.Entry(estadio).State);
    }
}

using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

[TestClass]
public class GrupoEFRepositoryTests
{
    private AppDbContext _context = null!;
    private GrupoEFRepository _repository = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new GrupoEFRepository(_context);
        
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
    public void AgregarGrupo_DebePersistirLaGrupo()
    {
        var grupo = new Grupo('A', 1);
        
        _repository.AgregarGrupo(grupo);

        Assert.IsTrue(_repository.ExisteGrupo('A',1));
    }
    

    [TestMethod]
    public void EsGrupo_DebeRetornarFalseSiNoExiste()
    {
        var grupo = new Grupo('A',1);
        
        bool resultado = _repository.ExisteGrupo('A',1);
        
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ObtenerGrupos_DebeDevolverTodosLosGrupos()
    {
        var grupo = new Grupo('A', 1);
        var grupo2 = new Grupo('B', 1);
        
        _repository.AgregarGrupo(grupo);
        _repository.AgregarGrupo(grupo2);
        
        List<Grupo> grupos = _repository.ObtenerGrupos();
        Assert.IsTrue(grupos.Count == 2);
    }

    [TestMethod]
    public void ObtenerGruposPorToken_DebeDevolverLosGruposAdecuados()
    {
        var grupo = new Grupo('A', 1);
        var grupo2 = new Grupo('B', 2);
        _repository.AgregarGrupo(grupo);
        _repository.AgregarGrupo(grupo2);
        
        List<Grupo> grupos = _repository.ObtenerGruposPorToken(1);
        
        Assert.IsTrue(grupos.Contains(grupo));
    }

    [TestMethod]
    public void ObtenerGrupo_DebeDevolverElAdecuado()
    {
        var grupo = new Grupo('A', 1);
        var grupo2 = new Grupo('B', 2);
        _repository.AgregarGrupo(grupo);
        _repository.AgregarGrupo(grupo2);
        var grupoObtenido = _repository.ObtenerGrupo('A',1);
        Assert.IsTrue(grupoObtenido == grupo);
    }
}
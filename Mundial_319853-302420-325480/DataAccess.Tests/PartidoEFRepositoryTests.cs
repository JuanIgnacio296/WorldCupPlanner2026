using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Tests;

[TestClass]
public class PartidoEFRepositoryTests
{
    private AppDbContext _context;
    private PartidoEFRepository _repository;
    private Partido _partido;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new PartidoEFRepository(_context);

        Estadio estadio = new Estadio("Estadio Central", "Montevideo", 60000, "Descripcion");
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Argentina", "CONMEBOL", 1700);
        _partido = new Partido(1, 1, 'A', new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, null, null, "Fase de Grupos", "14:00", 1);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarPartido_DeberiaGuardarPartidoEnBaseDeDatos()
    {
        _repository.AgregarPartido(_partido);

        Assert.AreEqual(1, _context.Partidos.Count());
        Assert.AreEqual(1, _context.Partidos.First().Id);
    }

    [TestMethod]
    public void ObtenerPartidos_DeberiaRetornarTodosLosPartidos()
    {
        _context.Partidos.Add(_partido);
        _context.Partidos.Add(CrearOtroPartido());
        _context.SaveChanges();

        List<Partido> result = _repository.ObtenerPartidos();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ObtenerPartido_DeberiaRetornarPartidoCorrectoPorId()
    {
        _context.Partidos.Add(_partido);
        _context.SaveChanges();

        Partido result = _repository.ObtenerPartido(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public void LimpiarPartidos_DeberiaEliminarTodosLosPartidos()
    {
        _context.Partidos.Add(_partido);
        _context.Partidos.Add(CrearOtroPartido());
        _context.SaveChanges();

        _repository.LimpiarPartidos();

        Assert.AreEqual(0, _context.Partidos.Count());
    }

    [TestMethod]
    public void AgregarPartido_CuandoEquipoYaRastreadoComoOtraInstancia_NoLanzaError()
    {
        _context.Estadios.Add(new Estadio("Estadio Central", "Montevideo", 60000, "Descripcion"));
        _context.Equipos.Add(new Equipo("Uruguay", "CONMEBOL", 1800));
        _context.Equipos.Add(new Equipo("Argentina", "CONMEBOL", 1700));
        _context.SaveChanges();

        Estadio estadio = new Estadio("Estadio Central", "Montevideo", 60000, "Descripcion");
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Argentina", "CONMEBOL", 1700);
        Partido partido = new Partido(99, 1, 'A', new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, null, null, "Fase de Grupos", "14:00", 1);

        _repository.AgregarPartido(partido);

        Assert.AreEqual(1, _context.Partidos.Count(p => p.Id == 99));
        Assert.AreEqual(2, _context.Equipos.Count());
    }

    private static Partido CrearOtroPartido()
    {
        Estadio estadio = new Estadio("Estadio Norte", "Salto", 40000, "Descripcion");
        Equipo local = new Equipo("Brasil", "CONMEBOL", 1900);
        Equipo visitante = new Equipo("Chile", "CONMEBOL", 1500);
        return new Partido(2, 1, 'B', new DateTime(2026, 6, 2, 18, 0, 0), estadio, local, visitante, null, null, "Fase de Grupos", "18:00", 1);
    }
}
using App.Dominio;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Tests;

[TestClass]
public class UsuarioEFRepositoryTests
{
    private AppDbContext _context;
    private UsuarioEFRepository _repository;
    private Usuario _usuario;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new UsuarioEFRepository(_context);

        _usuario = new Usuario("Juan", "Perez", "juan@ort.edu.uy", DateTime.Now.AddYears(-30), "Password123#", new List<Rol> { Rol.Administrador });
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AgregarUsuario_DeberiaGuardarUsuarioEnBaseDeDatos()
    {
        _repository.AgregarUsuario(_usuario);

        Assert.AreEqual(1, _context.Usuarios.Count());
        Assert.AreEqual("juan@ort.edu.uy", _context.Usuarios.First().Correo);
    }

    [TestMethod]
    public void ObtenerUsuarios_DeberiaRetornarTodosLosUsuarios()
    {
        _context.Usuarios.Add(_usuario);
        _context.Usuarios.Add(new Usuario("Ana", "Gomez", "ana@ort.edu.uy", DateTime.Now.AddYears(-25), "Password456#", new List<Rol> { Rol.Editor }));
        _context.SaveChanges();

        List<Usuario> result = _repository.ObtenerUsuarios();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ObtenerUsuario_DeberiaRetornarUsuarioCorrectoPorCorreo()
    {
        _context.Usuarios.Add(_usuario);
        _context.SaveChanges();

        Usuario result = _repository.ObtenerUsuario("juan@ort.edu.uy");

        Assert.IsNotNull(result);
        Assert.AreEqual("juan@ort.edu.uy", result.Correo);
    }

    [TestMethod]
    public void ExisteUsuario_DeberiaRetornarTrueSiElUsuarioExiste()
    {
        _context.Usuarios.Add(_usuario);
        _context.SaveChanges();

        Usuario otro = new Usuario("Ana", "Gomez", "ana@ort.edu.uy", DateTime.Now.AddYears(-25), "Password456#", new List<Rol> { Rol.Editor });

        Assert.IsTrue(_repository.ExisteUsuario(_usuario));
        Assert.IsFalse(_repository.ExisteUsuario(otro));
    }

    [TestMethod]
    public void BorrarUsuario_DeberiaEliminarElUsuarioDeLaBaseDeDatos()
    {
        _context.Usuarios.Add(_usuario);
        _context.SaveChanges();

        _repository.BorrarUsuario(_usuario);

        Assert.AreEqual(0, _context.Usuarios.Count());
    }
}

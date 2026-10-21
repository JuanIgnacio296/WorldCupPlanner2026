using App.Dominio;
using DTOs;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class UsuarioActualTests
{
    [TestMethod]
    public void ImplicitOperatorUsuario_ConUsuarioActualValido_DebeRetornarUsuario()
    {
        Usuario usuario = new Usuario("Juan", "Perez", "juan@perez.com", DateTime.Now.AddYears(-20), "Juan123#");
        UsuarioActual usuarioActual = new UsuarioActual(usuario);

        Usuario? resultado = usuarioActual;

        Assert.IsNotNull(resultado);
        Assert.AreEqual(usuario, resultado);
    }

    [TestMethod]
    public void ImplicitOperatorUsuario_ConUsuarioActualNulo_DebeRetornarNulo()
    {
        UsuarioActual? usuarioActual = null;

        Usuario? resultado = usuarioActual;

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ImplicitOperatorUsuarioReadDto_ConUsuarioActualValido_DebeRetornarUsuarioReadDto()
    {
        Usuario usuario = new Usuario("Juan", "Perez", "juan@perez.com", DateTime.Now.AddYears(-20), "Juan123#", [Rol.Administrador]);
        UsuarioActual usuarioActual = new UsuarioActual(usuario);

        UsuarioReadDto? resultado = usuarioActual;

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Juan", resultado.Nombre);
        Assert.AreEqual("Perez", resultado.Apellido);
        Assert.AreEqual("juan@perez.com", resultado.Correo);
        Assert.IsTrue(resultado.Roles.Contains("Administrador"));
    }

    [TestMethod]
    public void ImplicitOperatorUsuarioReadDto_ConUsuarioActualNulo_DebeRetornarNulo()
    {
        UsuarioActual? usuarioActual = null;

        UsuarioReadDto? resultado = usuarioActual;

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ImplicitOperatorUsuarioReadDto_ConRolesNulos_DebeRetornarListaVacia()
    {
        Usuario usuario = new Usuario("Juan", "Perez", "juan@perez.com", DateTime.Now.AddYears(-20), "Juan123#", null);
        UsuarioActual usuarioActual = new UsuarioActual(usuario);

        UsuarioReadDto? resultado = usuarioActual;

        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Roles.Count);
    }
}

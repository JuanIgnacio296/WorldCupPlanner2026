using App.Dominio;
using DTOs;
using Services;

namespace Services.Tests;

[TestClass]
public class UsuarioMapeadorTests
{
    [TestMethod]
    public void ADto_CuandoRecibeUsuario_DebeMapearCamposYRoles()
    {
        UsuarioMapeador mapeador = new UsuarioMapeador();
        Usuario usuario = new Usuario("Ana", "Lopez", "ana@ort.edu.uy", new DateTime(1990, 1, 1), "Clave123#", new List<Rol> { Rol.Administrador });

        UsuarioReadDto dto = mapeador.ADto(usuario);

        Assert.AreEqual("Ana", dto.Nombre);
        Assert.AreEqual("Lopez", dto.Apellido);
        Assert.AreEqual("ana@ort.edu.uy", dto.Correo);
        Assert.AreEqual(new DateTime(1990, 1, 1), dto.FechaNacimiento);
        CollectionAssert.AreEqual(new List<string> { "Administrador" }, dto.Roles);
    }

    [TestMethod]
    public void ADominio_CuandoRecibeDto_DebeReconstruirUsuarioConRoles()
    {
        UsuarioMapeador mapeador = new UsuarioMapeador();
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Nombre = "Ana",
            Apellido = "Lopez",
            Correo = "ana@ort.edu.uy",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Roles = new List<string> { "Administrador" }
        };

        Usuario usuario = mapeador.ADominio(dto);

        Assert.AreEqual("Ana", usuario.Nombre);
        Assert.AreEqual("Lopez", usuario.Apellido);
        Assert.AreEqual("ana@ort.edu.uy", usuario.Correo);
        Assert.IsTrue(usuario.TieneRol(Rol.Administrador));
    }

    [TestMethod]
    public void MapearRoles_CuandoListaVacia_DebeDevolverNinguno()
    {
        UsuarioMapeador mapeador = new UsuarioMapeador();

        List<Rol> roles = mapeador.MapearRoles(new List<string>());

        CollectionAssert.AreEqual(new List<Rol> { Rol.Ninguno }, roles);
    }
}

using App.Dominio;
using DTOs;
using DataAccess;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class SesionDtoServiceTests
{
    private SesionService _servicioSesion = null!;

    [TestInitialize]
    public void Setup()
    {
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        _servicioSesion = new SesionService(new InMemoryUsuario(), auditoriaService);
    }

    [TestMethod]
    public void Login_ConLoginDtoValido_DebeDevolverUsuarioReadDtoSinDatosSensibles()
    {
        LoginDto login = new LoginDto
        {
            Correo = "admin@ort.edu.uy",
            Contrasena = "Admin123#"
        };

        UsuarioReadDto usuario = _servicioSesion.Login(login);
        UsuarioReadDto? actual = _servicioSesion.ObtenerUsuarioActual();

        Assert.IsNotNull(actual);
        Assert.AreEqual("admin@ort.edu.uy", usuario.Correo);
        Assert.AreEqual(usuario.Correo, actual.Correo);
        Assert.AreNotEqual(typeof(Usuario), usuario.GetType());
        AssertNoExponeDatosSensibles(usuario.GetType());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Login_ConLoginDtoSinCorreo_DebeLanzarExcepcion()
    {
        LoginDto login = new LoginDto
        {
            Correo = "",
            Contrasena = "Admin123#"
        };

        _servicioSesion.Login(login);
    }

    private static void AssertNoExponeDatosSensibles(Type tipo)
    {
        string[] nombresSensibles = ["Password", "PasswordHash", "Contrasena", "Contrasenia", "Hash"];

        foreach (string nombre in nombresSensibles)
        {
            bool existe = tipo.GetProperties()
                .Any(propiedad => propiedad.Name.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            Assert.IsFalse(existe, $"{tipo.Name} expone {nombre}.");
        }
    }
}

using App.Dominio;
using Services.Interfaces;
using Services;
using DataAccess;

namespace Services.Tests;

[TestClass]
public class SesionServiceTest
{
    private InMemoryLog _inMemoryLog = null!;
    private InMemoryUsuario _inMemoryUsuario = null!;
    private IAuditoriaService _auditoriaService = null!;
    private SesionService _servicioSesion = null!;

    [TestInitialize]
    public void Setup()
    {
        _inMemoryLog = new InMemoryLog();
        _inMemoryUsuario = new InMemoryUsuario();

        _auditoriaService = new AuditoriaService(_inMemoryLog);

        _servicioSesion = new SesionService(_inMemoryUsuario, _auditoriaService);
    }

    [TestMethod]
    public void Login_ConCredencialesValidas_DebeSetearUsuarioActual()
    {
        _servicioSesion.Login("admin@ort.edu.uy", "Admin123#");

        Usuario? actual = _servicioSesion.ObtenerUsuarioActual();
        Assert.IsNotNull(actual);
        Assert.AreEqual("admin@ort.edu.uy", actual.Correo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Login_ConContraseniaIncorrecta_DebeLanzarExcepcion()
    {
        _servicioSesion.Login("admin@ort.edu.uy", "PasswordEquivocada123!");
    }
    
    [TestMethod]
    public void Logout_DebeLimpiarUsuarioActual()
    {
        _servicioSesion.Login("admin@ort.edu.uy", "Admin123#");

        _servicioSesion.Logout();

        Usuario? actual = _servicioSesion.ObtenerUsuarioActual();
        Assert.IsNull(actual, "El usuario actual debería ser nulo después del Logout");
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Login_ConCorreoVacio_DebeLanzarExcepcion()
    {
        _servicioSesion.Login("", "Password123!");
    }

    [TestMethod]
    public void Login_ConCorreoInexistente_DebeLanzarExcepcion()
    {
        Assert.ThrowsException<ArgumentException>(() => 
            _servicioSesion.Login("correoInexistente@ort.edu.uy", "Password123#")    
        );
    }

    [TestMethod]
    public void ObtenerUsuarioActual_SinLogin_DebeRetornarNull()
    {
        Usuario? actual = _servicioSesion.ObtenerUsuarioActual();
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void ObtenerUsuarioActual_DespuesDeLogin_DebeRetornarUsuarioActual()
    {
        _servicioSesion.Login("admin@ort.edu.uy", "Admin123#");

        Usuario? actual = _servicioSesion.ObtenerUsuarioActual();
        
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void RestaurarSesion_ConCorreoInexistente_DebeRetornarFalse()
    {
        Assert.IsFalse(_servicioSesion.RestaurarSesion("correoInexistente@ort.edu.uy"));
    }
    
    [TestMethod]
    public void RestaurarSesion_ConCorreoExistente_DebeRetornarTrue()
    {
        Assert.IsTrue(_servicioSesion.RestaurarSesion("admin@ort.edu.uy"));
    }

    [TestMethod]
    public void RestaurarSesion_ConCorreoInesxistente_DebeLimpiarUsuarioActual()
    {
        _servicioSesion.RestaurarSesion("correoInexistente@ort.edu.uy");
        Usuario? actual = _servicioSesion.ObtenerUsuarioActual();
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void RestaurarSesion_ConCorreoExistente_DebeSetearUsuarioActual()
    {
        _servicioSesion.RestaurarSesion("admin@ort.edu.uy");
        Usuario? actual = _servicioSesion.ObtenerUsuarioActual();
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void RestaurarSesion_ConCorreoVacio_DebeRetornarFalse()
    {
        Assert.IsFalse(_servicioSesion.RestaurarSesion(""));
    }

    [TestMethod]
    public void RestaurarSesion_ConCorreoNulo_DebeRetornarFalse()
    {
        Assert.IsFalse(_servicioSesion.RestaurarSesion(null));
    }
}



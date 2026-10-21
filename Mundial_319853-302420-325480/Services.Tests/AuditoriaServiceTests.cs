using App.Dominio;
using Services;
using DataAccess;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class AuditoriaServiceTest
{
    [TestMethod]
    public void RegistrarLog_DatosValidos_DebeGuardarseEnLaLista()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService service = new AuditoriaService(inMemoryLog);
        Usuario admin = new Usuario("Juan", "Perez", "admin@ort.edu.uy", DateTime.Now.AddYears(-25), "Admin123#", new List<Rol> { Rol.Administrador });
        string accion = "Creación de Equipo";

        service.RegistrarLog(admin, accion);
        var resultado = service.ObtenerLogs(admin);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(accion, resultado[0].Accion);
        Assert.AreEqual(admin.Correo, resultado[0].Usuario.Correo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void ObtenerLogs_CuandoUsuarioNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService service = new AuditoriaService(inMemoryLog);
        Usuario noAdmin = new Usuario("Juan", "Editor", "editor@test.com", DateTime.Now.AddYears(-20), "Editor123#", new List<Rol> { Rol.Editor });

        service.ObtenerLogs(noAdmin);
    }
    
    [TestMethod]
    public void ObtenerLogsPorFecha_RangoValido_DebeRetornarLogsEnEsePeriodo()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService service = new AuditoriaService(inMemoryLog);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
    
        service.RegistrarLog(admin, "Acción dentro de rango");

        DateTime inicio = DateTime.Now.AddMinutes(-1);
        DateTime fin = DateTime.Now.AddMinutes(1);

        List<Log> resultado = service.ObtenerLogsPorFecha(admin, inicio, fin);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Acción dentro de rango", resultado[0].Accion);
    }

    [TestMethod]
    public void ObtenerLogsPorFecha_RangoSinResultados_DebeRetornarListaVacia()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService service = new AuditoriaService(inMemoryLog);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
    
        service.RegistrarLog(admin, "Acción de hoy");

        DateTime inicio = DateTime.Now.AddDays(-10);
        DateTime fin = DateTime.Now.AddDays(-9);

        List<Log> resultado = service.ObtenerLogsPorFecha(admin, inicio, fin);

        Assert.AreEqual(0, resultado.Count);
    }
    
    [TestMethod]
    public void ObtenerLogsPorFecha_CuandoNoEsAdministrador_LanzaExcepcion()
    {
        AuditoriaService service = new AuditoriaService(new InMemoryLog());
        Usuario editor = new Usuario("Editor", "Sist", "editor@test.com", DateTime.Now.AddYears(-20), "Editor123#", [Rol.Editor]);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.ObtenerLogsPorFecha(editor, DateTime.Now.AddDays(-1), DateTime.Now));
    }
}



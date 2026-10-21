using App.Dominio;
using DataAccess;
using DTOs;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class AuditoriaDtoServiceTests
{
    private IAuditoriaService _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new AuditoriaService(new InMemoryLog());
    }

    [TestMethod]
    public void ObtenerLogs_ConUsuarioReadDtoAdministrador_DebeDevolverLogReadDto()
    {
        Usuario adminDominio = CrearAdministradorDominio();
        _servicio.RegistrarLog(adminDominio, "Creacion de Equipo");

        List<LogReadDto> logs = _servicio.ObtenerLogs(CrearAdministradorDto());

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("Creacion de Equipo", logs[0].Accion);
        Assert.AreEqual("admin@ort.edu.uy", logs[0].Usuario.Correo);
        Assert.AreNotEqual(typeof(Log), logs[0].GetType());
    }

    [TestMethod]
    public void ObtenerLogsPorFecha_ConUsuarioReadDtoAdministrador_DebeDevolverLogReadDto()
    {
        Usuario adminDominio = CrearAdministradorDominio();
        _servicio.RegistrarLog(adminDominio, "Accion dentro de rango");

        DateTime inicio = DateTime.Now.AddMinutes(-1);
        DateTime fin = DateTime.Now.AddMinutes(1);

        List<LogReadDto> logs = _servicio.ObtenerLogsPorFecha(CrearAdministradorDto(), inicio, fin);

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("Accion dentro de rango", logs[0].Accion);
        Assert.AreNotEqual(typeof(Log), logs[0].GetType());
    }

    private static Usuario CrearAdministradorDominio()
    {
        return new Usuario(
            "Admin",
            "Auditoria",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-30),
            "Admin123#",
            [Rol.Administrador]
        );
    }

    private static UsuarioReadDto CrearAdministradorDto()
    {
        return new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Auditoria",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            Roles = ["Administrador"]
        };
    }
}

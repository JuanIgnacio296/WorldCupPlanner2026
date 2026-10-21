using App.Dominio;
using DTOs;
using DataAccess;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class EstadioDtoServiceTests
{
    private IEstadioService _servicio = null!;
    private UsuarioReadDto _admin = null!;

    [TestInitialize]
    public void Setup()
    {
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        _servicio = new EstadioService(auditoriaService, new InMemoryEstadio());
        _admin = new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Sist",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Administrador"]
        };
    }

    [TestMethod]
    public void CrearEstadio_ConEstadioCreateDto_DebeAgregarYDevolverEstadioReadDto()
    {
        EstadioCreateDto nuevoEstadio = new EstadioCreateDto
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000,
            Descripcion = "Estadio Historico"
        };

        EstadioReadDto creado = _servicio.CrearEstadio(_admin, nuevoEstadio);

        Assert.AreEqual("Centenario", creado.Nombre);
        Assert.AreEqual("Montevideo", creado.Ciudad);
        Assert.AreEqual(60000, creado.Capacidad);
        Assert.AreEqual("Estadio Historico", creado.Descripcion);
        Assert.AreNotEqual(typeof(Estadio), creado.GetType());
    }

    [TestMethod]
    public void CrearEstadio_ConEstadioCreateDtoDuplicado_DebeLanzarArgumentException()
    {
        EstadioCreateDto estadioOriginal = new EstadioCreateDto
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000,
            Descripcion = "Estadio Historico"
        };
        EstadioCreateDto estadioDuplicado = new EstadioCreateDto
        {
            Nombre = "Centenario",
            Ciudad = "Canelones",
            Capacidad = 25000,
            Descripcion = "Estadio duplicado"
        };

        _servicio.CrearEstadio(_admin, estadioOriginal);

        Assert.ThrowsException<ArgumentException>(() => _servicio.CrearEstadio(_admin, estadioDuplicado));
    }

    [TestMethod]
    public void ObtenerEstadiosListado_DebeDevolverEstadioListItemDtoSinDescripcion()
    {
        _servicio.CrearEstadio(_admin, new EstadioCreateDto
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000,
            Descripcion = "Estadio Historico"
        });

        List<EstadioListItemDto> estadios = _servicio.ObtenerEstadiosListado();

        Assert.AreEqual(1, estadios.Count);
        Assert.AreEqual("Centenario", estadios[0].Nombre);
        Assert.AreEqual("Montevideo", estadios[0].Ciudad);
        Assert.AreEqual(60000, estadios[0].Capacidad);
        Assert.AreNotEqual(typeof(Estadio), estadios[0].GetType());
        Assert.IsFalse(typeof(EstadioListItemDto).GetProperties().Any(propiedad => propiedad.Name == "Descripcion"));
    }

    [TestMethod]
    public void ModificarEstadio_ConEstadioUpdateDto_DebeActualizarYDevolverEstadioReadDto()
    {
        _servicio.CrearEstadio(_admin, new EstadioCreateDto
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000,
            Descripcion = "Estadio Historico"
        });
        EstadioUpdateDto estadioModificado = new EstadioUpdateDto
        {
            Nombre = "Centenario Renovado",
            Ciudad = "Montevideo",
            Capacidad = 65000,
            Descripcion = "Estadio actualizado"
        };

        EstadioReadDto actualizado = _servicio.ModificarEstadio(_admin, "Centenario", estadioModificado);

        Assert.AreEqual("Centenario Renovado", actualizado.Nombre);
        Assert.AreEqual("Montevideo", actualizado.Ciudad);
        Assert.AreEqual(65000, actualizado.Capacidad);
        Assert.AreEqual("Estadio actualizado", actualizado.Descripcion);
        Assert.AreNotEqual(typeof(Estadio), actualizado.GetType());
    }

    [TestMethod]
    public void EliminarEstadio_ConUsuarioReadDto_DebeEliminarEstadio()
    {
        _servicio.CrearEstadio(_admin, new EstadioCreateDto
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000,
            Descripcion = "Estadio Historico"
        });

        _servicio.EliminarEstadio(_admin, "Centenario");

        Assert.AreEqual(0, _servicio.ObtenerEstadiosListado().Count);
    }

    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_ConUsuarioReadDto_DebeGenerarEstadiosYListarlosComoDto()
    {
        _servicio.CompletarEstadiosMinimosParaFixture(_admin);

        List<EstadioListItemDto> estadios = _servicio.ObtenerEstadiosListado();

        Assert.AreEqual(4, estadios.Count);
        Assert.AreNotEqual(typeof(Estadio), estadios[0].GetType());
    }
}

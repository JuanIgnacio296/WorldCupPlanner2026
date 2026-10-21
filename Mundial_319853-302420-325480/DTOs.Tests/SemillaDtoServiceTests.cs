using App.Dominio;
using DTOs;
using DataAccess;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class SemillaDtoServiceTests
{
    private ISemillaService _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new SemillaService(new InMemorySemilla());
    }

    [TestMethod]
    public void AgregarSemilla_ConSemillaCreateDto_DebeAgregarYDevolverSemillaReadDto()
    {
        SemillaCreateDto nuevaSemilla = new SemillaCreateDto
        {
            Token = 1,
            SemillaFixture = 123,
            SemillaCompletar = 456,
            SemillaCrucesFase = 789,
            SemillaSimulacion = 0
        };

        SemillaReadDto creada = _servicio.AgregarSemilla(nuevaSemilla);

        Assert.AreEqual(1, creada.Token);
        Assert.AreEqual(123, creada.SemillaFixture);
        Assert.AreEqual(456, creada.SemillaCompletar);
        Assert.AreEqual(789, creada.SemillaCrucesFase);
        Assert.AreEqual(0, creada.SemillaSimulacion);
        Assert.AreNotEqual(typeof(Semilla), creada.GetType());
    }

    [TestMethod]
    public void AgregarSemilla_ConSemillaCreateDtoDuplicada_DebeLanzarArgumentException()
    {
        SemillaCreateDto semillaOriginal = new SemillaCreateDto
        {
            Token = 1,
            SemillaFixture = 123,
            SemillaCompletar = 456,
            SemillaCrucesFase = 789,
            SemillaSimulacion = 0
        };
        SemillaCreateDto semillaDuplicada = new SemillaCreateDto
        {
            Token = 1,
            SemillaFixture = 321,
            SemillaCompletar = 654,
            SemillaCrucesFase = 987,
            SemillaSimulacion = 10
        };

        _servicio.AgregarSemilla(semillaOriginal);

        Assert.ThrowsException<ArgumentException>(() => _servicio.AgregarSemilla(semillaDuplicada));
    }

    [TestMethod]
    public void ObtenerSemilla_ConTokenExistente_DebeDevolverSemillaReadDto()
    {
        _servicio.AgregarSemilla(new SemillaCreateDto
        {
            Token = 1,
            SemillaFixture = 123,
            SemillaCompletar = 456,
            SemillaCrucesFase = 789,
            SemillaSimulacion = 0
        });

        SemillaReadDto semilla = _servicio.ObtenerSemillaDto(1);

        Assert.AreEqual(1, semilla.Token);
        Assert.AreEqual(123, semilla.SemillaFixture);
        Assert.AreEqual(456, semilla.SemillaCompletar);
        Assert.AreEqual(789, semilla.SemillaCrucesFase);
        Assert.AreEqual(0, semilla.SemillaSimulacion);
        Assert.AreNotEqual(typeof(Semilla), semilla.GetType());
    }

    [TestMethod]
    public void ObtenerSemillasListado_DebeDevolverListaDeSemillaReadDto()
    {
        _servicio.AgregarSemilla(new SemillaCreateDto
        {
            Token = 1,
            SemillaFixture = 123,
            SemillaCompletar = 456,
            SemillaCrucesFase = 789,
            SemillaSimulacion = 0
        });
        _servicio.AgregarSemilla(new SemillaCreateDto
        {
            Token = 2,
            SemillaFixture = 321,
            SemillaCompletar = 654,
            SemillaCrucesFase = 987,
            SemillaSimulacion = 10
        });

        List<SemillaReadDto> semillas = _servicio.ObtenerSemillasListado();

        Assert.AreEqual(2, semillas.Count);
        Assert.AreEqual(1, semillas[0].Token);
        Assert.AreEqual(2, semillas[1].Token);
        Assert.AreNotEqual(typeof(Semilla), semillas[0].GetType());
    }
}

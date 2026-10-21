using App.Dominio;
using Services;
using DataAccess;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class SemillaServiceTests
{
    [TestMethod]
    public void ObtenerSemilla_WhenExisteSemilla_ThenRetornaSemillaGuardada()
    {
        Semilla semilla = new Semilla(1, 123, 456, 789, 0);
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        inMemorySemilla.AgregarSemilla(semilla);

        SemillaService service = new SemillaService(inMemorySemilla);

        Semilla resultado = service.ObtenerSemilla(1);
        
        Assert.AreEqual(123, resultado.SemillaFixture);
        Assert.AreEqual(456, resultado.SemillaCompletar);
        Assert.AreEqual(789, resultado.SemillaCrucesFase);
        Assert.AreEqual(0, resultado.SemillaSimulacion);
    }
    
    [TestMethod]
    public void ObtenerSemilla_WhenDoesNotExisteSemilla_ThenThrowException()
    {
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        SemillaService service = new SemillaService(inMemorySemilla);

        Exception e = Assert.ThrowsException<ArgumentException>(() => service.ObtenerSemilla(1));
        Assert.AreEqual("No existe semilla con el token", e.Message);
    }

    [TestMethod]
    public void AgregarSemilla_WhenTokenExists_ThenThrowsException()
    {
        Semilla semilla = new Semilla(1, 123, 456, 789, 0);
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        inMemorySemilla.AgregarSemilla(semilla);

        SemillaService service = new SemillaService(inMemorySemilla);
        
        Exception e = Assert.ThrowsException<ArgumentException>(() => { service.AgregarSemilla(semilla); });
        Assert.AreEqual("Ya existe una semilla con ese mismo token", e.Message);
        
    }
    
    [TestMethod]
    public void AgregarSemilla_WhenTokenDoesNotExists_ThenAgregaSemilla()
    {
        Semilla semilla = new Semilla(1, 123, 456, 789, 0);
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        SemillaService service = new SemillaService(inMemorySemilla);

        service.AgregarSemilla(semilla);
        
        Assert.AreEqual(1, service.ObtenerSemillas().Count);
        
    }
    
    [TestMethod]
    public void ObtenerSiguienteTokenDisponible_WhenHaySemillas_ReturnsNextToken()
    {
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        SemillaService service = new SemillaService(inMemorySemilla);
        Semilla semilla1 = new Semilla(1, 10, 20, 30, 40);
        Semilla semilla2 = new Semilla(2, 50, 60, 70, 80);

        service.AgregarSemilla(semilla1); 
        service.AgregarSemilla(semilla2);

        int resultado = service.ObtenerSiguienteTokenDisponible();

        Assert.AreEqual(3, resultado);
    }

    [TestMethod]
    public void ObtenerSiguienteTokenDisponible_WhenNoHaySemillas_ThenReturnsOne()
    {
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        SemillaService service = new SemillaService(inMemorySemilla);
        int resultado = service.ObtenerSiguienteTokenDisponible();
        Assert.AreEqual(1, resultado);
    }
    
    [TestMethod]
    public void AgregarSemilla_WhenAlreadyExistsAnEqualSemilla_ThenThrowsException()
    {
        Semilla semilla = new Semilla(1, 123, 456, 789, 0);
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        inMemorySemilla.AgregarSemilla(semilla);
        SemillaService service = new SemillaService(inMemorySemilla);
        Semilla semilla2 = new Semilla(2, 123, 456, 789, 0);
        
        Exception e = Assert.ThrowsException<ArgumentException>(() => { service.AgregarSemilla(semilla2); });
        Assert.AreEqual("Ya existe una semilla igual con el token 1", e.Message);
    }
    
    [TestMethod]
    public void ObtenerSemilla_CuandoNoExiste_LanzaExcepcion()
    {
        SemillaService service = new SemillaService(new InMemorySemilla());

        Assert.ThrowsException<ArgumentException>(() => service.ObtenerSemilla(999));
    }

    [TestMethod]
    public void AgregarSemilla_CuandoTokenYaExiste_LanzaExcepcion()
    {
        SemillaService service = new SemillaService(new InMemorySemilla());

        service.AgregarSemilla(new Semilla(1, 1, 1, 1, 1));

        Assert.ThrowsException<ArgumentException>(() =>
            service.AgregarSemilla(new Semilla(1, 2, 2, 2, 2)));
    }

    [TestMethod]
    public void AsegurarSemillaExiste_CuandoNoExiste_LaAgrega()
    {
        SemillaService service = new SemillaService(new InMemorySemilla());
        Semilla semilla = new Semilla(10, 1, 2, 3, 4);

        service.AsegurarSemillaExiste(semilla);

        Assert.AreEqual(semilla, service.ObtenerSemilla(10));
    }
}



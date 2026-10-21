using App.Dominio;
using DTOs;
using Services;

namespace Services.Tests;

[TestClass]
public class EstadioMapeadorTests
{
    [TestMethod]
    public void ADto_CuandoRecibeEstadio_DebeMapearTodosLosCampos()
    {
        EstadioMapeador mapeador = new EstadioMapeador();
        Estadio estadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");

        EstadioReadDto dto = mapeador.ADto(estadio);

        Assert.AreEqual("Centenario", dto.Nombre);
        Assert.AreEqual("Montevideo", dto.Ciudad);
        Assert.AreEqual(60000, dto.Capacidad);
        Assert.AreEqual("Estadio Historico", dto.Descripcion);
    }

    [TestMethod]
    public void ADominio_CuandoRecibeDto_DebeReconstruirElEstadio()
    {
        EstadioMapeador mapeador = new EstadioMapeador();
        EstadioReadDto dto = new EstadioReadDto
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000,
            Descripcion = "Estadio Historico"
        };

        Estadio estadio = mapeador.ADominio(dto);

        Assert.AreEqual("Centenario", estadio.Nombre);
        Assert.AreEqual("Montevideo", estadio.Ciudad);
        Assert.AreEqual(60000, estadio.Capacidad);
        Assert.AreEqual("Estadio Historico", estadio.Descripcion);
    }
}

using App.Dominio;
using DTOs;
using Services;

namespace Services.Tests;

[TestClass]
public class EquipoMapeadorTests
{
    [TestMethod]
    public void ADto_CuandoRecibeEquipo_DebeMapearTodosLosCamposIncluyendoBandera()
    {
        EquipoMapeador mapeador = new EquipoMapeador();
        Equipo equipo = new Equipo("Uruguay", "CONMEBOL", 400, "banderaBase64");

        EquipoReadDto dto = mapeador.ADto(equipo);

        Assert.AreEqual("Uruguay", dto.Nombre);
        Assert.AreEqual("CONMEBOL", dto.Confederacion);
        Assert.AreEqual(400, dto.RankingFifa);
        Assert.AreEqual("banderaBase64", dto.BanderaBase64);
    }

    [TestMethod]
    public void ADominio_CuandoRecibeDto_DebeReconstruirEquipoConservandoBandera()
    {
        EquipoMapeador mapeador = new EquipoMapeador();
        EquipoReadDto dto = new EquipoReadDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "banderaBase64"
        };

        Equipo equipo = mapeador.ADominio(dto);

        Assert.AreEqual("Uruguay", equipo.Nombre);
        Assert.AreEqual("CONMEBOL", equipo.Confederacion);
        Assert.AreEqual(400, equipo.RankingFifa);
        Assert.AreEqual("banderaBase64", equipo.BanderaBase64);
    }
}

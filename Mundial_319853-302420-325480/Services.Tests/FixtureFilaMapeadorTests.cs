using App.Dominio;
using Services;

namespace Services.Tests;

[TestClass]
public class FixtureFilaMapeadorTests
{
    [TestMethod]
    public void Mapear_CuandoRecibePartidos_DebeConstruirFilasConDatosDelPartido()
    {
        FixtureFilaMapeador mapeador = new FixtureFilaMapeador();
        Estadio estadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");
        List<Partido> partidos = new List<Partido>
        {
            new Partido(
                1,
                1,
                null,
                new DateTime(2026, 6, 1, 14, 0, 0),
                estadio,
                new Equipo("Uruguay", "CONMEBOL", 400),
                new Equipo("Argentina", "CONMEBOL", 900),
                2,
                1,
                "Final",
                "14:00",
                null)
        };

        List<string[]> filas = mapeador.Mapear(partidos);

        Assert.AreEqual(1, filas.Count);
        CollectionAssert.AreEqual(
            new string[] { "2026-06-01 14:00:00", "Final", "", "Uruguay", "2", "1", "Argentina", "Centenario" },
            filas[0]);
    }
}

using App.Dominio;
using Services;

namespace Services.Tests;

[TestClass]
public class AuditoriaFilaMapeadorTests
{
    [TestMethod]
    public void Mapear_CuandoRecibeLogs_DebeConstruirFilasConDatosDelLog()
    {
        AuditoriaFilaMapeador mapeador = new AuditoriaFilaMapeador();
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", new List<Rol> { Rol.Administrador });
        List<Log> logs = new List<Log>
        {
            new Log(admin, "Accion CSV", new DateTime(2026, 6, 1, 10, 0, 0))
        };

        List<string[]> filas = mapeador.Mapear(logs);

        Assert.AreEqual(1, filas.Count);
        CollectionAssert.AreEqual(
            new string[] { "2026-06-01 10:00:00", "Admin Sistema", "admin@ort.edu.uy", "Accion CSV" },
            filas[0]);
    }
}
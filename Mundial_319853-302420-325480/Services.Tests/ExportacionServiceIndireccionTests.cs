using App.Dominio;
using DataAccess;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class ExportacionServiceIndireccionTests
{
    [TestMethod]
    public void ExportarAuditoriaXlsx_CuandoTieneExportadorInyectado_DebeDelegarCreacionDelArchivo()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido());
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        ExportadorXlsxFake exportador = new ExportadorXlsxFake();
        IExportacionService service = new ExportacionService(logs, fixtureService, exportador, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        logs.AgregarLog(new Log(admin, "Accion", new DateTime(2026, 6, 1, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaXlsx(admin, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));

        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, archivo);
        Assert.IsTrue(exportador.FueInvocado);
    }

    private class ExportadorXlsxFake : IXlsxExportador
    {
        public bool FueInvocado { get; private set; }

        public byte[] CrearArchivo(string[] encabezados, List<string[]> filas)
        {
            FueInvocado = true;
            return [1, 2, 3];
        }
    }
}

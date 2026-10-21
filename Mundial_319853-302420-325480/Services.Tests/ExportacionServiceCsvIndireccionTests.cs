using App.Dominio;
using DataAccess;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class ExportacionServiceCsvIndireccionTests
{
    [TestMethod]
    public void ExportarAuditoriaCsv_CuandoExporta_DebeDelegarEnCsvExportador()
    {
        InMemoryLog logs = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(logs);
        IPartidoService partidoService = new PartidoService(auditoriaService, new InMemoryPartido());
        IFixtureService fixtureService = new FixtureService(new InMemoryFixture(), partidoService, auditoriaService);
        FakeCsvExportador csvExportador = new FakeCsvExportador();
        IExportacionService service = new ExportacionService(logs, fixtureService, new XlsxExportador(), csvExportador, auditoriaService);
        Usuario admin = new Usuario("Admin", "Sistema", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", new List<Rol> { Rol.Administrador });

        logs.AgregarLog(new Log(admin, "Accion CSV", new DateTime(2026, 6, 1, 10, 0, 0)));

        byte[] archivo = service.ExportarAuditoriaCsv(admin, new DateTime(2026, 6, 1), new DateTime(2026, 6, 2));

        CollectionAssert.AreEqual(FakeCsvExportador.ArchivoGenerado, archivo);
        Assert.IsTrue(csvExportador.FueInvocado);
        CollectionAssert.AreEqual(new[] { "Fecha", "Usuario", "Correo", "Accion" }, csvExportador.Encabezados);
    }

    private class FakeCsvExportador : ICsvExportador
    {
        public bool FueInvocado { get; private set; }
        public static readonly byte[] ArchivoGenerado = new byte[] { 1, 2, 3 };

        public string[] Encabezados { get; private set; } = Array.Empty<string>();

        public byte[] CrearArchivo(string[] encabezados, List<string[]> filas)
        {
            FueInvocado = true;
            Encabezados = encabezados;
            return ArchivoGenerado;
        }
    }
}

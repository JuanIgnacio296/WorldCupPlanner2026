using System.IO.Compression;
using Services;

namespace Services.Tests;

[TestClass]
public class XlsxExportadorTests
{
    [TestMethod]
    public void CrearArchivo_CuandoRecibeEncabezadosYFilas_DebeCrearXlsxValido()
    {
        XlsxExportador exportador = new XlsxExportador();

        byte[] archivo = exportador.CrearArchivo(
            ["Columna"],
            [new[] { "Valor" }]
        );

        Assert.IsTrue(archivo.Length > 0);
        using MemoryStream stream = new MemoryStream(archivo);
        using ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
        Assert.IsNotNull(zip.GetEntry("xl/workbook.xml"));
    }
}

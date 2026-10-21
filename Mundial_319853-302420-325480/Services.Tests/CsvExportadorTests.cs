using System.Text;
using Services;

namespace Services.Tests;

[TestClass]
public class CsvExportadorTests
{
    [TestMethod]
    public void CrearArchivo_CuandoRecibeEncabezadosYFilas_DebeCrearCsvEscapado()
    {
        CsvExportador exportador = new CsvExportador();

        string[] encabezados = new string[] { "Nombre", "Accion" };
        List<string[]> filas = new List<string[]>
        {
            new string[] { "Admin", "Alta, equipo \"Uruguay\"" }
        };

        byte[] archivo = exportador.CrearArchivo(encabezados, filas);

        string contenido = Encoding.UTF8.GetString(archivo);

        StringAssert.Contains(contenido, "Nombre,Accion");
        StringAssert.Contains(contenido, "Admin,\"Alta, equipo \"\"Uruguay\"\"\"");
    }
}

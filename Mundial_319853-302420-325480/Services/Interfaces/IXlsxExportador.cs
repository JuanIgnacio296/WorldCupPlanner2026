namespace Services.Interfaces;

public interface IXlsxExportador
{
    byte[] CrearArchivo(string[] encabezados, List<string[]> filas);
}

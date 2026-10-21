namespace Services.Interfaces;

public interface ICsvExportador
{
    byte[] CrearArchivo(string[] encabezados, List<string[]> filas);
}

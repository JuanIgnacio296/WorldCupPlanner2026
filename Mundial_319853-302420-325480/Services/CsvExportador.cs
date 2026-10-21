using System.Text;
using Services.Interfaces;

namespace Services;

public class CsvExportador : ICsvExportador
{
    public byte[] CrearArchivo(string[] encabezados, List<string[]> filas)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine(CrearLinea(encabezados));

        foreach (string[] fila in filas)
        {
            csv.AppendLine(CrearLinea(fila));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private static string CrearLinea(string[] valores)
    {
        List<string> valoresEscapados = new List<string>();

        foreach (string valor in valores)
        {
            valoresEscapados.Add(Escapar(valor));
        }

        return string.Join(",", valoresEscapados);
    }

    private static string Escapar(string valor)
    {
        if (!valor.Contains(',') && !valor.Contains('"') && !valor.Contains('\n'))
        {
            return valor;
        }

        return $"\"{valor.Replace("\"", "\"\"")}\"";
    }
}

using App.Dominio;

namespace Services;

public class AuditoriaFilaMapeador
{
    public List<string[]> Mapear(IEnumerable<Log> logs)
    {
        List<string[]> filas = new List<string[]>();

        foreach (Log log in logs)
        {
            string[] fila = new string[]
            {
                FormatearFecha(log.Fecha),
                log.Usuario.Nombre + " " + log.Usuario.Apellido,
                log.Usuario.Correo,
                log.Accion
            };
            filas.Add(fila);
        }

        return filas;
    }

    private static string FormatearFecha(DateTime fecha)
    {
        return fecha.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
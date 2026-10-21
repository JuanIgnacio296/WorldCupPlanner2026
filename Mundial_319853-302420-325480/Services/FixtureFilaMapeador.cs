using App.Dominio;

namespace Services;

public class FixtureFilaMapeador
{
    public List<string[]> Mapear(IEnumerable<Partido> partidos)
    {
        List<string[]> filas = new List<string[]>();

        foreach (Partido partido in partidos)
        {
            string[] fila = new string[]
            {
                FormatearFecha(partido.Fecha!.Value),
                partido.Fase,
                partido.Jornada?.ToString() ?? "",
                partido.EquipoLocal!.Nombre,
                partido.GolesLocal?.ToString() ?? "",
                partido.GolesVisitante?.ToString() ?? "",
                partido.EquipoVisitante!.Nombre,
                partido.Estadio!.Nombre
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
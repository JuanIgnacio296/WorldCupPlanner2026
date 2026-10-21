using App.Dominio;

namespace Services;

public static class EstadiosPorDefecto
{
    private const string DescripcionGenerada = "Estadio generado por defecto";

    public static List<Estadio> Crear()
    {
        return new List<Estadio>
        {
            new Estadio("Estadio generado 1", "Montevideo", 60000, DescripcionGenerada),
            new Estadio("Estadio generado 2", "Montevideo", 40000, DescripcionGenerada),
            new Estadio("Estadio generado 3", "Montevideo", 34000, DescripcionGenerada),
            new Estadio("Estadio generado 4", "Maldonado", 22000, DescripcionGenerada)
        };
    }
}
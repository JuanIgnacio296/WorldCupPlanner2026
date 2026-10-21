namespace Services;

public static class CuposConfederacion
{
    private static readonly Dictionary<string, int> _cupos = new Dictionary<string, int>()
    {
        { "UEFA", 16 },
        { "CONMEBOL", 7 },
        { "CONCACAF", 7 },
        { "CAF", 9 },
        { "AFC", 8 },
        { "OFC", 1 }
    };

    public static int Maximo(string confederacion) => _cupos[confederacion];

    public static IReadOnlyDictionary<string, int> Todos => _cupos;
}
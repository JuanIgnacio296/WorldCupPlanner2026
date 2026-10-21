namespace App.Dominio;

public class Sorteo
{
    private const int CantidadGrupos = 12;
    private const int CantidadBombos = 4;
    private const int EquiposPorBombo = 12;
    private const int CantidadEquiposMundial = 48;
    private const int RankingMinimoFifa = 300;
    private const int RankingMaximoFifaExclusivo = 2501;

    private readonly Semilla _semilla;
    private readonly int _token;
    private List<Equipo> _listaEquipos;
    private List<Grupo> _listaGrupos = [];
    private List<Estadio> _listaEstadios;
    private static readonly Barajador _barajador = new();

    public Sorteo(List<Equipo> equipos, Semilla semilla)
        : this(equipos, 0, semilla, [])
    {
    }

    public Sorteo(List<Equipo> equipos, Semilla semilla, List<Estadio> estadios)
        : this(equipos, 0, semilla, estadios)
    {
    }

    public Sorteo(List<Equipo> equipos, int token, Semilla semilla, List<Estadio> estadios)
    {
        _listaEquipos = equipos ?? [];
        _listaEstadios = estadios ?? [];
        _semilla = semilla;
        _token = token;
    }

    private Sorteo() { }

    public int Token => _token;

    public Semilla Semilla => _semilla;

    public List<Grupo> Grupos => _listaGrupos;

    public List<Estadio> Estadios => _listaEstadios;

    public List<Equipo> Equipos => _listaEquipos;

    public List<Grupo> CrearGrupos()
    {
        _listaGrupos = Enumerable
            .Range(0, CantidadGrupos)
            .Select(indice => new Grupo((char)('A' + indice), _token))
            .ToList();

        return _listaGrupos;
    }

    public List<Grupo> SortearEquipos()
    {
        CompletarEquiposSiFaltan();
        CrearGrupos();

        Random random = new(_semilla.SemillaFixture);
        List<Equipo> equiposOrdenados = OrdenarEquiposPorRanking(random);
        List<List<Equipo>> bombos = CrearBombos(equiposOrdenados);

        if (!SortearBombos(bombos, 0, 0, random))
        {
            throw new InvalidOperationException("No se pudo generar el sorteo.");
        }

        return _listaGrupos;
    }

    public void CompletarEquiposSiFaltan()
    {
        Dictionary<string, int> cuposPorConfederacion = new()
        {
            { "UEFA", 16 },
            { "CONMEBOL", 7 },
            { "CONCACAF", 7 },
            { "CAF", 9 },
            { "AFC", 8 },
            { "OFC", 1 }
        };

        Random random = new(_semilla.SemillaCompletar);

        foreach (KeyValuePair<string, int> cupo in cuposPorConfederacion)
        {
            CompletarConfederacion(cupo.Key, cupo.Value, random);
        }
    }

    public List<Equipo> OrdenarEquiposPorRanking(Random random)
    {
        return _listaEquipos
            .GroupBy(equipo => equipo.RankingFifa)
            .OrderByDescending(grupo => grupo.Key)
            .SelectMany(grupo => BarajarEquipos(grupo.ToList(), random))
            .ToList();
    }

    private bool SortearBombos(List<List<Equipo>> bombos, int numeroBombo, int indiceEquipo, Random random)
    {
        if (numeroBombo >= CantidadBombos)
        {
            return true;
        }

        if (indiceEquipo >= EquiposPorBombo)
        {
            return SortearBombos(bombos, numeroBombo + 1, 0, random);
        }

        Equipo equipo = bombos[numeroBombo][indiceEquipo];
        List<Grupo> gruposValidos = ObtenerGruposValidos(equipo, numeroBombo, random);

        foreach (Grupo grupo in gruposValidos)
        {
            grupo.AddEquipo(equipo);

            if (SortearBombos(bombos, numeroBombo, indiceEquipo + 1, random))
            {
                return true;
            }

            grupo.RemoveEquipo(equipo);
        }

        return false;
    }

    private List<Grupo>ObtenerGruposValidos(Equipo equipo, int numeroBombo, Random random)
    {
        List<Grupo> gruposValidos = [];
        int desplazamientoInicial = random.Next(0, CantidadGrupos);

        for (int desplazamiento = 0; desplazamiento < CantidadGrupos; desplazamiento++)
        {
            int indiceGrupo = (desplazamientoInicial + desplazamiento) % CantidadGrupos;
            Grupo grupo = _listaGrupos[indiceGrupo];

            if (grupo.Equipos.Count == numeroBombo && PuedeAgregarEquipoAlGrupo(grupo, equipo))
            {
                gruposValidos.Add(grupo);
            }
        }

        return gruposValidos;
    }

    private static List<List<Equipo>> CrearBombos(List<Equipo> equiposOrdenados)
    {
        return Enumerable
            .Range(0, CantidadBombos)
            .Select(numeroBombo => equiposOrdenados
                .Skip(numeroBombo * EquiposPorBombo)
                .Take(EquiposPorBombo)
                .ToList())
            .ToList();
    }

    private static bool PuedeAgregarEquipoAlGrupo(Grupo grupo, Equipo equipo)
    {
        List<Equipo> es = grupo.Equipos;
        int cantidadMismaConfederacion = 
            es.Count(equipoDelGrupo => equipoDelGrupo.Confederacion == equipo.Confederacion);

        return equipo.Confederacion == "UEFA"
            ? cantidadMismaConfederacion < 2
            : cantidadMismaConfederacion == 0;
    }

    private void CompletarConfederacion(string confederacion, int cantidadMaxima, Random random)
    {
        int cantidadActual = _listaEquipos.Count(equipo => equipo.Confederacion == confederacion);

        while (cantidadActual < cantidadMaxima && _listaEquipos.Count < CantidadEquiposMundial)
        {
            cantidadActual++;

            Equipo equipoNuevo = new(
                $"{confederacion}_{cantidadActual:00}",
                confederacion,
                random.Next(RankingMinimoFifa, RankingMaximoFifaExclusivo));

            if (!_listaEquipos.Any(equipo => equipo.Nombre == equipoNuevo.Nombre))
            {
                _listaEquipos.Add(equipoNuevo);
            }
        }
    }

    private static List<Equipo> BarajarEquipos(List<Equipo> equipos, Random random)
    {
        return _barajador.FisherYates(equipos, random);
    }
}




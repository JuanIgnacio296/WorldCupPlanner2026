using App.Dominio.Exceptions;

namespace App.Dominio;

public class Fixture
{
    private const int CantidadMinimaEstadios = 4;

    private int _token;
    private List<Grupo> _listaGrupos = [];
    private readonly List<Partido> _listaPartidos = [];
    private List<Estadio> _listaEstadios = [];
    private readonly List<Equipo> _listaPrimeros = [];
    private readonly List<Equipo> _listaSegundos = [];
    private readonly List<Equipo> _listaTerceros = [];
    private Semilla _semilla = null!;
    private readonly Barajador _barajador = new();
    private Calendarizador? _calendarizador;
    private bool _usaMotorProbabilistico;

    private Calendarizador Calendarizador => _calendarizador ??= new Calendarizador(_listaPartidos, _listaEstadios, _token);

    public Fixture(int token, List<Grupo> grupos, List<Estadio> estadios, Semilla semilla, bool usaMotorProbabilistico)
    {
        Token = token;
        Estadios = estadios;
        Grupos = grupos;
        Semilla = semilla;
        UsaMotorProbabilistico = usaMotorProbabilistico;
    }

    private Fixture()
    {
    }

    public int Token
    {
        get => _token;
        set => _token = value;
    }

    public List<Estadio> Estadios
    {
        get => _listaEstadios;
        set
        {
            _listaEstadios = value;
            _listaEstadios.Sort((estadio, otroEstadio) => estadio.Nombre.CompareTo(otroEstadio.Nombre));
        }
    }

    public List<Partido> Partidos => _listaPartidos;

    public void CargarPartidos(IEnumerable<Partido> partidos)
    {
        _listaPartidos.Clear();
        _listaPartidos.AddRange(partidos);
    }

    public List<Grupo> Grupos
    {
        get => _listaGrupos;
        set => _listaGrupos = value ?? [];
    }

    public int PartidosJugados => _listaPartidos.Count(PartidoEstaTerminado);

    public Semilla Semilla
    {
        get => _semilla;
        set => _semilla = value;
    }
    
    public bool UsaMotorProbabilistico
    {
        get => _usaMotorProbabilistico;
        set => _usaMotorProbabilistico = value;
    }

    public List<Equipo> ListaPrimeros => _listaPrimeros;

    public List<Equipo> ListaSegundos => _listaSegundos;

    public List<Equipo> ListaTerceros => _listaTerceros;

    public void AddPrimero(Equipo equipo)
    {
        AgregarEquipoOrdenado(_listaPrimeros, equipo);
    }

    public void AddSegundo(Equipo equipo)
    {
        AgregarEquipoOrdenado(_listaSegundos, equipo);
    }

    public void AddTercero(Equipo equipo)
    {
        AgregarEquipoOrdenado(_listaTerceros, equipo);
    }

    public void ArmarFase(string fase)
    {
        ValidarCantidadEstadios();

        if (fase == "Fase de Grupos")
        {
            ArmarFaseDeGrupos();
        }
        else
        {
            new ConstructorFaseEliminatoria(this, Calendarizador).Armar(fase);
        }
    }

    public void OrganizarListas()
    {
        foreach (Grupo grupo in _listaGrupos)
        {
            grupo.SortEquipos();
            AddPrimero(grupo.Equipos[0]);
            AddSegundo(grupo.Equipos[1]);
            AddTercero(grupo.Equipos[2]);
        }
    }

    public Equipo? ObtenerGanador(Partido partido)
    {
        return partido.EquipoGanador;
    }

    public Equipo? ObtenerPerdedor(Partido partido)
    {
        return partido.EquipoPerdedor;
    }

    public List<Partido> ObtenerPartidosFinalPorFecha(DateTime inicio, DateTime fin)
    {
        return _listaPartidos
            .Where(partido => partido.Fase == "Final" && partido.Fecha >= inicio && partido.Fecha <= fin)
            .ToList();
    }

    private void ValidarCantidadEstadios()
    {
        if (_listaEstadios.Count < CantidadMinimaEstadios)
        {
            throw new ArgumentosDeFixtureNoValidos("Debe haber al menos 4 estadios");
        }
    }

    private void ArmarFaseDeGrupos()
    {
        foreach (Grupo grupo in _listaGrupos)
        {
            Equipo equipo1 = grupo.Equipos[0];
            Equipo equipo2 = grupo.Equipos[1];
            Equipo equipo3 = grupo.Equipos[2];
            Equipo equipo4 = grupo.Equipos[3];

            Partido partido1 = Calendarizador.CrearPartido(equipo1, equipo4, "Fase de Grupos", 1, grupo.Id);
            Partido partido2 = Calendarizador.CrearPartido(equipo2, equipo3, "Fase de Grupos", 1, grupo.Id);
            Partido partido3 = Calendarizador.CrearPartido(equipo1, equipo3, "Fase de Grupos", 2, grupo.Id);
            Partido partido4 = Calendarizador.CrearPartido(equipo2, equipo4, "Fase de Grupos", 2, grupo.Id);
            List<Partido> partidosJornada3 = Calendarizador.CrearJornada3(grupo);

            grupo.Partidos =
            [
                partido1,
                partido2,
                partido3,
                partido4,
                partidosJornada3[0],
                partidosJornada3[1]
            ];
        }
    }

    private void AgregarEquipoOrdenado(List<Equipo> lista, Equipo equipo)
    {
        lista.Add(equipo);
        _barajador.OrdenarPorTablaYDesempatar(lista, new Random(_semilla.SemillaCrucesFase));
    }

    private static bool PartidoEstaTerminado(Partido partido)
    {
        if (partido.GolesLocal == null || partido.GolesVisitante == null)
        {
            return false;
        }

        if (partido.Fase == "Fase de Grupos")
        {
            return true;
        }

        if (partido.GolesLocal != partido.GolesVisitante)
        {
            return true;
        }

        return partido.PenalesLocal != null && partido.PenalesVisitante != null;
    }
}
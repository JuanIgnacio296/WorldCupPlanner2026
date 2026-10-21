using App.Dominio;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;
using System.Text;

namespace Services;

public class FixtureService : IFixtureService
{
    private readonly IinMemoryFixture _inMemoryFixture;
    private readonly IPartidoService _partidoService;
    private IAuditoriaService _auditoriaService;
    private int _tokenActual;

    private static readonly List<string> FasesMundial = Partido.Fases;

    public FixtureService(IinMemoryFixture inMemoryFixture, IPartidoService partidoService, IAuditoriaService auditoriaService)
    {
        _inMemoryFixture = inMemoryFixture;
        _partidoService = partidoService;
        _auditoriaService = auditoriaService;
        _tokenActual = 0;
    }

    #region Consultas y Persistencia de Fixture

    public int ObtenerTokenActual() => _tokenActual;

    public void GuardarFixture(int token, Fixture fixture)
    {
        if (!_inMemoryFixture.ExisteFixture(token))
        {
            _inMemoryFixture.AgregarFixture(token, fixture);
            SincronizarPartidosEnService(fixture);
        }
        _tokenActual = token;
    }

    public Fixture ObtenerFixturePorToken(int token)
    {
        _tokenActual = token;
        return _inMemoryFixture.ObtenerFixturePorToken(token);
    }

    public Fixture ObtenerFixturePorTokenSemilla(int tokenSemilla)
    {
        _tokenActual = _inMemoryFixture.ObtenerTokenPorSemilla(tokenSemilla);
        return ObtenerFixturePorToken(_tokenActual);
    }

    public Fixture ObtenerFixture()
    {
        var fixture = _tokenActual == 0
            ? _inMemoryFixture.ObtenerFixtures().LastOrDefault()
            : _inMemoryFixture.ObtenerFixturePorToken(_tokenActual) ?? _inMemoryFixture.ObtenerFixtures().LastOrDefault();

        return fixture!;
    }

    private void SincronizarPartidosEnService(Fixture fixture)
    {
        _partidoService.LimpiarPartidos();
        foreach (var partido in fixture.Partidos)
        {
            _partidoService.AgregarPartido(partido);
        }
    }

    #endregion

    #region Generación de Fases
    
    
    
    private void GenerarFaseEspecifica(string nombreFase)
    {
        var fixture = ObtenerFixture();
        
        if (fixture.Partidos.Any(p => p.Fase == nombreFase))
            throw new InvalidOperationException($"La fase {nombreFase} ya fue generada.");

        int cantidadAntes = fixture.Partidos.Count;
        fixture.ArmarFase(nombreFase);

        foreach (var nuevoPartido in fixture.Partidos.Skip(cantidadAntes))
        {
            _partidoService.AgregarPartido(nuevoPartido);
        }
    }

    public void GenerarFaseDeGrupos() => GenerarFaseEspecifica("Fase de Grupos");
    public void GenerarDieciseisavos() => GenerarFaseEspecifica("Dieciseisavos de Final");
    public void GenerarOctavos() => GenerarFaseEspecifica("Octavos de Final");
    public void GenerarCuartos() => GenerarFaseEspecifica("Cuartos de Final");
    public void GenerarSemifinal() => GenerarFaseEspecifica("Semifinal");
    public void GenerarTercerYCuartoPuesto() => GenerarFaseEspecifica("Tercer y Cuarto Puesto");
    public void GenerarFinal() => GenerarFaseEspecifica("Final");

    #endregion

    #region Obtención de Partidos

    public List<Partido> ObtenerPartidos() => ObtenerFixture().Partidos;

    public List<Partido> ObtenerPartidosPorFase(string fase)
    {
        return ObtenerPartidos().Where(p => p.Fase == fase).ToList();
    }

    public string MostrarCruces()
    {
        var dieciseisavos = ObtenerPartidosPorFase("Dieciseisavos de Final");
        if (!dieciseisavos.Any()) return string.Empty;

        var resultado = new StringBuilder().AppendLine("DIECISEISAVOS DE FINAL");
        foreach (var partido in dieciseisavos)
        {
            resultado.AppendLine($"{partido.EquipoLocal!.Nombre} vs {partido.EquipoVisitante!.Nombre}");
        }

        return resultado.ToString();
    }

    #endregion

    #region Simulaciones (Lógica unificada con LINQ)

    public void SimularFecha(DateTime fecha, Usuario user, Random rnd)
    {
        var fixture = ObtenerFixture();

        if (fixture.Partidos.Any(p => fecha > p.Fecha && (p.GolesLocal == null || p.GolesVisitante == null)))
            throw new InvalidOperationException($"No se pueden simular los partidos de {fecha} sin jugar los anteriores.");

        var partidosDeLaFecha = fixture.Partidos.Where(p => p.Fecha == fecha).ToList();
        if (!partidosDeLaFecha.Any())
            throw new InvalidOperationException($"No se juegan partidos en la fecha: {fecha}");

        EjecutarSimulacionDeLista(partidosDeLaFecha, fixture, user, rnd);
    }

    public void SimularJornada(int jornada, Usuario user, Random rnd)
    {
        var fixture = ObtenerFixture();

        if (fixture.Partidos.Any(p => p.Fase == "Fase de Grupos" && jornada > p.Jornada && (p.GolesLocal == null || p.GolesVisitante == null)))
            throw new InvalidOperationException($"No se pueden simular los partidos de la jornada {jornada} sin jugar las anteriores.");

        var partidosDeLaJornada = fixture.Partidos.Where(p => p.Fase == "Fase de Grupos" && p.Jornada == jornada).ToList();
        if (!partidosDeLaJornada.Any())
            throw new InvalidOperationException($"No existe la jornada: {jornada}");

        EjecutarSimulacionDeLista(partidosDeLaJornada, fixture, user, rnd);
    }

    public void SimularFase(string fase, Usuario user, Random rnd)
    {
        if (!FasesMundial.Contains(fase))
            throw new InvalidOperationException($"No existe la fase: {fase}");

        var fixture = ObtenerFixture();
        int indiceFaseObjetivo = FasesMundial.IndexOf(fase);

        if (fixture.Partidos.Any(p => indiceFaseObjetivo > FasesMundial.IndexOf(p.Fase) && (p.GolesLocal == null || p.GolesVisitante == null)))
            throw new InvalidOperationException($"No se pueden simular los partidos de la fase {fase} sin completar las anteriores.");

        var partidosDeLaFase = fixture.Partidos.Where(p => p.Fase == fase).ToList();
        EjecutarSimulacionDeLista(partidosDeLaFase, fixture, user, rnd);
    }

    private void EjecutarSimulacionDeLista(List<Partido> partidos, Fixture fixture, Usuario user, Random rnd)
    {
        foreach (var partido in partidos.Where(partido => partido.GolesLocal == null && partido.GolesVisitante == null))
        {
            _partidoService.SimularPartido(user, partido.Id, rnd, fixture.UsaMotorProbabilistico);
        }
        _auditoriaService.RegistrarLog(user, $"Se realizó una simulación con la semilla de token: {fixture.Semilla.Token}");
    }

    #endregion

    #region Implementación de DTOs y Mapeos (Acoplados limpios)

    public FixtureReadDto ObtenerFixturePorTokenDto(int token) => MapearFixture(ObtenerFixturePorToken(token));
    public FixtureReadDto ObtenerFixturePorTokenSemillaDto(int tokenSemilla) => MapearFixture(ObtenerFixturePorTokenSemilla(tokenSemilla));
    public FixtureReadDto ObtenerFixtureDto() => MapearFixture(ObtenerFixture());
    public List<PartidoReadDto> ObtenerPartidosListado() => ObtenerPartidos().Select(MapearPartido).ToList();
    public List<PartidoReadDto> ObtenerPartidosPorFaseListado(string fase) => ObtenerPartidosPorFase(fase).Select(MapearPartido).ToList();

    public FixtureReadDto SimularFecha(DateTime fecha, UsuarioReadDto user, Random rnd)
    {
        SimularFecha(fecha, MapearUsuario(user), rnd);
        return ObtenerFixtureDto();
    }

    public FixtureReadDto SimularJornada(int jornada, UsuarioReadDto user, Random rnd)
    {
        SimularJornada(jornada, MapearUsuario(user), rnd);
        return ObtenerFixtureDto();
    }

    public FixtureReadDto SimularFase(string fase, UsuarioReadDto user, Random rnd)
    {
        SimularFase(fase, MapearUsuario(user), rnd);
        return ObtenerFixtureDto();
    }

    public FixtureReadDto SimularFecha(DateTime fecha, UsuarioReadDto user)
        => SimularFecha(fecha, user, ObtenerFixture().Semilla.RandomSimulacion);

    public FixtureReadDto SimularJornada(int jornada, UsuarioReadDto user)
        => SimularJornada(jornada, user, ObtenerFixture().Semilla.RandomSimulacion);

    public FixtureReadDto SimularFase(string fase, UsuarioReadDto user)
        => SimularFase(fase, user, ObtenerFixture().Semilla.RandomSimulacion);

    private FixtureReadDto MapearFixture(Fixture fixture) => new()
    {
        TokenActual = _tokenActual,
        PartidosJugados = fixture.PartidosJugados,
        UsaMotorProbabilistico = fixture.UsaMotorProbabilistico,
        Semilla = MapearSemilla(fixture.Semilla),
        Grupos = fixture.Grupos.Select(MapearGrupo).ToList(),
        Partidos = fixture.Partidos.Select(MapearPartido).ToList()
    };

    private static GrupoReadDto MapearGrupo(Grupo grupo) => new()
    {
        Id = grupo.Id,
        Equipos = grupo.Equipos.Select(MapearEquipo).ToList(),
        Partidos = grupo.Partidos.Select(MapearPartido).ToList()
    };

    private static SemillaReadDto MapearSemilla(Semilla semilla) => new()
    {
        Token = semilla.Token,
        SemillaFixture = semilla.SemillaFixture,
        SemillaCompletar = semilla.SemillaCompletar,
        SemillaCrucesFase = semilla.SemillaCrucesFase,
        SemillaSimulacion = semilla.SemillaSimulacion
    };

    private static PartidoReadDto MapearPartido(Partido partido) => new()
    {
        Id = partido.Id,
        Fecha = partido.Fecha,
        Horario = partido.Horario,
        Estadio = partido.Estadio != null ? MapearEstadio(partido.Estadio) : null,
        EquipoLocal = partido.EquipoLocal != null ? MapearEquipo(partido.EquipoLocal) : null,
        EquipoVisitante = partido.EquipoVisitante != null ? MapearEquipo(partido.EquipoVisitante) : null,
        GolesLocal = partido.GolesLocal,
        GolesVisitante = partido.GolesVisitante,
        Fase = partido.Fase,
        Jornada = partido.Jornada,
        PenalesLocal = partido.PenalesLocal,
        PenalesVisitante = partido.PenalesVisitante,
        Vencedor = partido.Vencedor,
        TarjetasAmarillasLocal = partido.TarjetasAmarillasLocal,
        TarjetasRojasLocal = partido.TarjetasRojasLocal,
        TarjetasAmarillasVisitante = partido.TarjetasAmarillasVisitante,
        TarjetasRojasVisitante = partido.TarjetasRojasVisitante
    };

    private static EstadioReadDto MapearEstadio(Estadio estadio) => new()
    {
        Nombre = estadio.Nombre,
        Ciudad = estadio.Ciudad,
        Capacidad = estadio.Capacidad,
        Descripcion = estadio.Descripcion
    };

    private static EquipoReadDto MapearEquipo(Equipo equipo) => new()
    {
        Nombre = equipo.Nombre,
        Confederacion = equipo.Confederacion,
        RankingFifa = equipo.RankingFifa,
        BanderaBase64 = equipo.BanderaBase64,
        Puntos = equipo.Puntos,
        GolesAFavor = equipo.GolesAFavor,
        GolesEnContra = equipo.GolesEnContra
    };

    private static Usuario MapearUsuario(UsuarioReadDto usuario) => new(
        usuario.Nombre,
        usuario.Apellido,
        usuario.Correo,
        usuario.FechaNacimiento,
        "Temporal123#",
        MapearRoles(usuario.Roles)
    );

    private static List<Rol> MapearRoles(List<string> roles)
    {
        if (roles == null || !roles.Any()) return new List<Rol> { Rol.Ninguno };
        return roles.Select(rol => Enum.TryParse(rol, true, out Rol rolDominio) ? rolDominio : Rol.Ninguno).ToList();
    }

    #endregion
}
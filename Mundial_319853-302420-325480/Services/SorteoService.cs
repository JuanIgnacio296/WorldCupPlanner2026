using App.Dominio;
using Services.Interfaces;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;

namespace Services;

public class SorteoService : ISorteoService
{
    private IinMemorySorteo _IinMemorySorteo;
    private IinMemoryFixture _IinMemoryFixture;
    private IinMemoryEquipo _IinMemoryEquipo;
    private IinMemoryPartido _IinMemoryPartido;
    private IinMemoryEstadio _IinMemoryEstadio;
    private IinMemorySemilla _IinMemorySemilla;
    private IAuditoriaService _auditoriaService;
    private static readonly EquipoMapeador _equipoMapeador = new();
    private static readonly EstadioMapeador _estadioMapeador = new();
    private static readonly UsuarioMapeador _usuarioMapeador = new();

    public SorteoService(
        IAuditoriaService auditoriaService,
        IinMemorySorteo IinMemorySorteo,
        IinMemoryFixture IinMemoryFixture,
        IinMemoryEquipo IinMemoryEquipo,
        IinMemoryPartido IinMemoryPartido,
        IinMemoryEstadio IinMemoryEstadio,
        IinMemorySemilla IinMemorySemilla)
    {
        _IinMemorySorteo = IinMemorySorteo;
        _IinMemoryFixture = IinMemoryFixture;
        _IinMemoryEquipo = IinMemoryEquipo;
        _IinMemoryPartido = IinMemoryPartido;
        _IinMemoryEstadio =  IinMemoryEstadio;
        _auditoriaService = auditoriaService;
        _IinMemorySemilla = IinMemorySemilla;
    }

    public Sorteo CrearSorteo(Usuario editor, List<Equipo> equipos, List<Estadio> estadios, int tokenSemilla)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new InvalidOperationException("Solo pueden crear sorteos los editores.");
        }

        Semilla? semilla = _IinMemorySemilla.ObtenerSemillaNullable(tokenSemilla);

        if (semilla == null)
        {
            semilla = new Semilla(tokenSemilla, tokenSemilla, tokenSemilla, tokenSemilla, tokenSemilla);
            _IinMemorySemilla.AgregarSemilla(semilla);
        }

        int token = _IinMemorySorteo.GenerarToken();
        Sorteo sorteo = new Sorteo(equipos, token, semilla, estadios);
        _IinMemorySorteo.AgregarSorteo(token, sorteo);
        _auditoriaService.RegistrarLog(editor, $"Creación de sorteo con token: {token}. Semilla: {tokenSemilla}");

        return sorteo;
    }

    public SorteoReadDto CrearSorteo(
        UsuarioReadDto editor,
        List<EquipoReadDto> equipos,
        List<EstadioReadDto> estadios,
        int tokenSemilla)
    {
        Sorteo sorteo = CrearSorteo(
            mapearUsuario(editor),
            equipos.Select(mapearEquipo).ToList(),
            estadios.Select(mapearEstadio).ToList(),
            tokenSemilla
        );

        return mapearSorteo(sorteo);
    }
    
    public Sorteo CrearSorteo(Usuario editor, List<Equipo> equipos, List<Estadio> estadios, Semilla semilla)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new InvalidOperationException("Solo pueden crear sorteos los editores.");
        }

        if (_IinMemorySorteo.ExisteSorteoConSemilla(semilla))
        {
            throw new InvalidOperationException("Ya existe un sorteo con esa semilla.");
        }

        int token = _IinMemorySorteo.GenerarToken();

        Sorteo sorteo = new Sorteo(equipos, token, semilla, estadios);

        _IinMemorySorteo.AgregarSorteo(token, sorteo);

        _auditoriaService.RegistrarLog(
            editor,
            $"Creación de sorteo con token: {token}. " +
            $"SemillaFixture: {semilla.SemillaFixture}, " +
            $"SemillaCompletar: {semilla.SemillaCompletar}, " +
            $"SemillaCrucesFase: {semilla.SemillaCrucesFase}, " +
            $"SemillaSimulacion: {semilla.SemillaSimulacion}"
        );

        return sorteo;
    }

    public Sorteo ObtenerSorteoPorToken(int token)
    {
        return _IinMemorySorteo.ObtenerSorteoPorToken(token);
    }

    public SorteoReadDto ObtenerSorteoPorTokenDto(int token)
    {
        return mapearSorteo(ObtenerSorteoPorToken(token));
    }

    public List<Grupo> SortearEquipos(Usuario editor, int token)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new InvalidOperationException("Solo pueden sortear equipos los editores.");
        }

        Sorteo sorteo = ObtenerSorteoPorToken(token);

        validarEquiposNoSorteados(sorteo);

        List<Grupo> grupos = sorteo.SortearEquipos();

        GuardarEquiposDelSorteo(sorteo);

        _auditoriaService.RegistrarLog(
            editor,
            $"Sortear equipos con token : {token}"
        );

        return grupos;
    }

    public List<GrupoReadDto> SortearEquipos(UsuarioReadDto editor, int token)
    {
        return SortearEquipos(mapearUsuario(editor), token)
            .Select(mapearGrupo)
            .ToList();
    }

    private void GuardarEquiposDelSorteo(Sorteo sorteo)
    {
        foreach (Equipo equipo in sorteo.Equipos)
        {
            if (!_IinMemoryEquipo.ExisteEquipo(equipo.Nombre))
            {
                _IinMemoryEquipo.AgregarEquipo(equipo);
            }
        }
    }

    private void validarEquiposNoSorteados(Sorteo sorteo)
    {
        int cantidadEquiposSorteados = sorteo.Grupos.Sum(grupo => grupo.Equipos.Count);

        if (cantidadEquiposSorteados > 0)
        {
            throw new InvalidOperationException("Los equipos ya fueron sorteados.");
        }
    }

    public Fixture CrearFixture(Usuario editor, int token, bool usaMotorProbabilistico)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new InvalidOperationException("Solo pueden crear fixture los editores.");
        }

        Sorteo sorteo = ObtenerSorteoPorToken(token);

        completarEstadiosSiFaltan(sorteo);
        validarCantidadDeEstadios(sorteo);
        validarEquiposSorteados(sorteo);
        validarFixtureNoGenerado(token);

        Fixture fixture = new Fixture(sorteo.Token, ClonarGruposParaFixture(sorteo.Grupos), sorteo.Estadios, sorteo.Semilla, usaMotorProbabilistico);

        fixture.ArmarFase("Fase de Grupos");

        _IinMemoryFixture.AgregarFixture(token, fixture);

        GuardarPartidosDelFixture(fixture);

        _auditoriaService.RegistrarLog(
            editor,
            $"Creacion de fixture con token : {token}"
        );

        return fixture;
    }

    public FixtureReadDto CrearFixture(UsuarioReadDto editor, int token, bool usaMotorProbabilistico)
    {
        return mapearFixture(CrearFixture(mapearUsuario(editor), token, usaMotorProbabilistico), token);
    }

    private List<Grupo> ClonarGruposParaFixture(List<Grupo> grupos)
    {
        List<Grupo> gruposClonados = new List<Grupo>();

        foreach (Grupo grupo in grupos)
        {
            Grupo grupoClonado = new Grupo(grupo.Id, grupo.Token);

            foreach (Equipo equipo in grupo.Equipos)
            {
                grupoClonado.AddEquipo(equipo);
            }

            gruposClonados.Add(grupoClonado);
        }

        return gruposClonados;
    }

    private void GuardarPartidosDelFixture(Fixture fixture)
    {
        _IinMemoryPartido.LimpiarPartidos();

        foreach (Partido partido in fixture.Partidos)
        {
            _IinMemoryPartido.AgregarPartido(partido);
        }
    }

    private void validarCantidadDeEstadios(Sorteo sorteo)
    {
        if (sorteo.Estadios.Count < 4)
        {
            throw new InvalidOperationException("No se puede crear el fixture con menos de 4 estadios.");
        }
    }

    private void validarEquiposSorteados(Sorteo sorteo)
    {
        int cantidadEquipos = sorteo.Grupos.Sum(grupo => grupo.Equipos.Count);

        if (cantidadEquipos != 48)
        {
            throw new InvalidOperationException("No se puede crear el fixture si no hay 48 equipos sorteados.");
        }
    }

    private void validarFixtureNoGenerado(int token)
    {
        if (_IinMemoryFixture.ExisteFixture(token))
        {
            throw new InvalidOperationException("Ya existe un fixture para ese sorteo.");
        }
    }

    public Fixture ObtenerFixturePorToken(Usuario editor, int token)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new InvalidOperationException("Solo pueden consultar fixture editores.");
        }

        validarFixtureExistente(token);

        _auditoriaService.RegistrarLog(
            editor,
            $"Consulta fixture con token : {token}"
        );

        return _IinMemoryFixture.ObtenerFixturePorToken(token);
    }

    public FixtureReadDto ObtenerFixturePorToken(UsuarioReadDto editor, int token)
    {
        return mapearFixture(ObtenerFixturePorToken(mapearUsuario(editor), token), token);
    }

    private void validarFixtureExistente(int token)
    {
        if (!_IinMemoryFixture.ExisteFixture(token))
        {
            throw new InvalidOperationException("No existe un fixture para ese token.");
        }
    }
    
    private void completarEstadiosSiFaltan(Sorteo sorteo)
    {
        agregarEstadiosExistentesAlSorteo(sorteo);

        List<Estadio> estadiosPorDefecto = CrearEstadiosPorDefecto();

        foreach (Estadio estadio in estadiosPorDefecto)
        {
            if (sorteo.Estadios.Count >= 4)
            {
                return;
            }

            if (!_IinMemoryEstadio.ExisteEstadio(estadio.Nombre))
            {
                _IinMemoryEstadio.AgregarEstadio(estadio);
            }

            if (!sorteo.Estadios.Any(e => e.Nombre == estadio.Nombre))
            {
                sorteo.Estadios.Add(estadio);
            }
        }
    }
    
    private void agregarEstadiosExistentesAlSorteo(Sorteo sorteo)
    {
        List<Estadio> estadiosExistentes = _IinMemoryEstadio.ObtenerEstadios();

        foreach (Estadio estadio in estadiosExistentes)
        {
            if (sorteo.Estadios.Count >= 4)
            {
                return;
            }

            bool yaEstaEnSorteo = sorteo.Estadios.Any(e => e.Nombre == estadio.Nombre);

            if (!yaEstaEnSorteo)
            {
                sorteo.Estadios.Add(estadio);
            }
        }
    }

    private List<Estadio> CrearEstadiosPorDefecto()
    {
        return EstadiosPorDefecto.Crear();
    }

    private static SorteoReadDto mapearSorteo(Sorteo sorteo)
    {
        return new SorteoReadDto
        {
            Token = sorteo.Token,
            Semilla = mapearSemilla(sorteo.Semilla),
            Equipos = sorteo.Equipos.Select(mapearEquipo).ToList(),
            Estadios = sorteo.Estadios.Select(mapearEstadio).ToList(),
            Grupos = sorteo.Grupos.Select(mapearGrupo).ToList()
        };
    }

    private static GrupoReadDto mapearGrupo(Grupo grupo)
    {
        return new GrupoReadDto
        {
            Id = grupo.Id,
            Equipos = grupo.Equipos.Select(mapearEquipo).ToList(),
            Partidos = grupo.Partidos.Select(mapearPartido).ToList()
        };
    }

    private static SemillaReadDto mapearSemilla(Semilla semilla)
    {
        return new SemillaReadDto
        {
            Token = semilla.Token,
            SemillaFixture = semilla.SemillaFixture,
            SemillaCompletar = semilla.SemillaCompletar,
            SemillaCrucesFase = semilla.SemillaCrucesFase,
            SemillaSimulacion = semilla.SemillaSimulacion
        };
    }

    private static FixtureReadDto mapearFixture(Fixture fixture, int token)
    {
        return new FixtureReadDto
        {
            TokenActual = token,
            PartidosJugados = fixture.PartidosJugados,
            UsaMotorProbabilistico = fixture.UsaMotorProbabilistico,
            Semilla = mapearSemilla(fixture.Semilla),
            Grupos = fixture.Grupos.Select(mapearGrupo).ToList(),
            Partidos = fixture.Partidos.Select(mapearPartido).ToList()
        };
    }

    private static PartidoReadDto mapearPartido(Partido partido)
    {
        return new PartidoReadDto
        {
            Id = partido.Id,
            Fecha = partido.Fecha,
            Horario = partido.Horario,
            Estadio = mapearEstadio(partido.Estadio!),
            EquipoLocal = mapearEquipo(partido.EquipoLocal!),
            EquipoVisitante = mapearEquipo(partido.EquipoVisitante!),
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
    }

    private static EquipoReadDto mapearEquipo(Equipo equipo)
    {
        return _equipoMapeador.ADto(equipo);
    }

    private static Equipo mapearEquipo(EquipoReadDto equipo)
    {
        return _equipoMapeador.ADominio(equipo);
    }

    private static EstadioReadDto mapearEstadio(Estadio estadio)
    {
        return _estadioMapeador.ADto(estadio);
    }

    private static Estadio mapearEstadio(EstadioReadDto estadio)
    {
        return _estadioMapeador.ADominio(estadio);
    }

    private static Usuario mapearUsuario(UsuarioReadDto usuario)
    {
        return _usuarioMapeador.ADominio(usuario);
    }
}


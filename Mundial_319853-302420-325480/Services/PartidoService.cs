using App.Dominio;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;

namespace Services;

public class PartidoService : IPartidoService
{
    private IinMemoryPartido _IinMemoryPartido;
    private IAuditoriaService _auditoriaService;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();
    private static readonly EstadioMapeador _estadioMapeador = new EstadioMapeador();
    private static readonly EquipoMapeador _equipoMapeador = new EquipoMapeador();
    private INotificacionService _notificacionService;
    private IinMemoryFixture _IinMemoryFixture;

    public PartidoService(IAuditoriaService auditoriaService, IinMemoryPartido IinMemoryPartido, INotificacionService notificacionService)
    {
        _auditoriaService = auditoriaService;
        _IinMemoryPartido = IinMemoryPartido;
        _notificacionService = notificacionService;
    }
    
    public PartidoService(IAuditoriaService auditoriaService, IinMemoryPartido IinMemoryPartido)
    {
        _auditoriaService = auditoriaService;
        _IinMemoryPartido = IinMemoryPartido;
        _notificacionService = new DummyNotificacionService();
    }

    public PartidoService(IAuditoriaService auditoriaService, IinMemoryPartido IinMemoryPartido, INotificacionService notificacionService, IinMemoryFixture IinMemoryFixture)
    {
        _auditoriaService = auditoriaService;
        _IinMemoryPartido = IinMemoryPartido;
        _notificacionService = notificacionService;
        _IinMemoryFixture = IinMemoryFixture;
    }

    public PartidoService(List<Partido> partidos, IAuditoriaService auditoriaService, IinMemoryPartido IinMemoryPartido)
    { 
        _IinMemoryPartido = IinMemoryPartido;
        _auditoriaService = auditoriaService;
        _notificacionService = new DummyNotificacionService();
        foreach (var partido in partidos)
        {
            _IinMemoryPartido.AgregarPartido(partido);
        }
    }
    public Partido ObtenerPartidoPorId(int id)
    {

        return _IinMemoryPartido.ObtenerPartido(id);
    }

    public PartidoReadDto ObtenerPartidoPorIdDto(int id)
    {
        Partido partido = ObtenerPartidoPorId(id);
        return mapearPartido(partido);
    }
    
    public List<Partido> ObtenerPartidos()
    {
        return _IinMemoryPartido.ObtenerPartidos();
    }

    public List<PartidoReadDto> ObtenerPartidosListado()
    {
        return ObtenerPartidos()
            .Select(mapearPartido)
            .ToList();
    }
    
    public void EditarFecha(Usuario editor, int id, DateTime nuevaFecha)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede editar fechas de partidos.");
        }

        Partido partido = ObtenerPartidoPorId(id);

        DateTime fechaInicioTorneo = new DateTime(2026, 6, 1);

        if (nuevaFecha.Date < fechaInicioTorneo.Date)
        {
            throw new ArgumentException("La fecha del partido no puede ser anterior al inicio del torneo.");
        }

        List<int> horasPermitidas = new List<int> { 14, 18, 22 };

        if (!horasPermitidas.Contains(nuevaFecha.Hour) || nuevaFecha.Minute != 0 || nuevaFecha.Second != 0)
        {
            throw new ArgumentException("El horario del partido debe ser 14:00, 18:00 o 22:00.");
        }

        if (partido.Estadio == null)
        {
            throw new ArgumentException("El partido debe tener estadio asignado.");
        }

        if (partido.Fecha != nuevaFecha && !partido.Estadio.ReservaEstadioValidacion.EsFechaDisponible(nuevaFecha, partido.Estadio.FechasOcupadas))
        {
            throw new ArgumentException("El estadio no está disponible en la fecha seleccionada.");
        }
        
        validarFechaNoAnteriorAEtapaPrevia(partido, nuevaFecha);

        partido.Fecha = nuevaFecha;
        _IinMemoryPartido.GuardarCambios();

        _auditoriaService.RegistrarLog(editor, $"Se editó la fecha del partido ID {id} a {nuevaFecha}");
    }

    public PartidoReadDto EditarFecha(UsuarioReadDto editor, int id, DateTime nuevaFecha)
    {
        EditarFecha(_usuarioMapeador.ADominio(editor), id, nuevaFecha);

        return ObtenerPartidoPorIdDto(id);
    }
    
    public void EditarEstadio(Usuario editor, int id, Estadio nuevoEstadio)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede editar el estadio de un partido.");
        }

        Partido partido = ObtenerPartidoPorId(id);
        partido.Estadio = nuevoEstadio;
        _IinMemoryPartido.GuardarCambios();

        _auditoriaService.RegistrarLog(editor, $"Se cambió el estadio del partido ID {id} a {nuevoEstadio.Nombre}");
    }

    public PartidoReadDto EditarEstadio(UsuarioReadDto editor, int id, EstadioReadDto nuevoEstadio)
    {
        EditarEstadio(_usuarioMapeador.ADominio(editor), id, _estadioMapeador.ADominio(nuevoEstadio));

        return ObtenerPartidoPorIdDto(id);
    }
    
    public void CargarResultado(Usuario editor, int id, int golesLocal, int golesVisitante)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede cargar resultados.");
        }

        Partido partido = ObtenerPartidoPorId(id);

        if (FaseYaTermino(partido))
        {
            throw new InvalidOperationException("No se puede modificar el resultado: la fase ya terminó.");
        }

        partido.AsignarResultado(golesLocal, golesVisitante);
        _IinMemoryPartido.GuardarCambios();

        _auditoriaService.RegistrarLog(editor, $"Resultado cargado para partido ID {id}: {golesLocal} - {golesVisitante}");
        
        Equipo equipoLocal = partido.EquipoLocal;
        Equipo equipoVisitante = partido.EquipoVisitante;
        _notificacionService.CrearNotificacionParaPeriodistas(equipoLocal.Nombre, golesLocal, golesVisitante, equipoVisitante.Nombre);
    }

    public PartidoReadDto CargarResultado(UsuarioReadDto editor, int id, int golesLocal, int golesVisitante)
    {
        CargarResultado(_usuarioMapeador.ADominio(editor), id, golesLocal, golesVisitante);

        return ObtenerPartidoPorIdDto(id);
    }

    private bool FaseYaTermino(Partido partido)
    {
        if (_IinMemoryFixture == null || !_IinMemoryFixture.ExisteFixture(partido.Token))
        {
            return false;
        }

        Fixture fixture = _IinMemoryFixture.ObtenerFixturePorToken(partido.Token);
        int indiceFasePartido = Partido.Fases.IndexOf(partido.Fase);

        return fixture.Partidos.Any(p => Partido.Fases.IndexOf(p.Fase) > indiceFasePartido);
    }

    public PartidoReadDto CargarTarjetas(
        UsuarioReadDto editor,
        int id,
        int amarillasLocal,
        int rojasLocal,
        int amarillasVisitante,
        int rojasVisitante)
    {
        Usuario editorDominio = _usuarioMapeador.ADominio(editor);
        if (!editorDominio.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede cargar tarjetas.");
        }

        Partido partido = ObtenerPartidoPorId(id);
        partido.RegistrarTarjetas(amarillasLocal, rojasLocal, amarillasVisitante, rojasVisitante);
        _IinMemoryPartido.GuardarCambios();

        _auditoriaService.RegistrarLog(editorDominio, $"Tarjetas cargadas para partido ID {id}");

        return ObtenerPartidoPorIdDto(id);
    }
    
    public void AgregarPartido(Partido nuevoPartido)
    {
        _IinMemoryPartido.AgregarPartido(nuevoPartido);
    }

    public void LimpiarPartidos()
    {
        _IinMemoryPartido.LimpiarPartidos();
    }

    public void SimularPartido(Usuario editor, int id, Random rnd, bool usaMotorProbabilistico)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede simular partidos");
        }

        Partido partido = ObtenerPartidoPorId(id);
        if (partido.GolesLocal != null || partido.GolesVisitante != null)
        {
            throw new InvalidOperationException("El partido ya tiene resultado");
        }

        partido.Simular(rnd, usaMotorProbabilistico);
        _IinMemoryPartido.GuardarCambios();
    
        int golesLocal = partido.GolesLocal.Value;
        int golesVisitante = partido.GolesVisitante.Value;

        _auditoriaService.RegistrarLog(editor, $"Partido ID {id} simulado automáticamente. Resultado: {golesLocal} - {golesVisitante}");
    
        _notificacionService.CrearNotificacionParaPeriodistas(
            partido.EquipoLocal.Nombre, 
            golesLocal, 
            golesVisitante, 
            partido.EquipoVisitante.Nombre
        );
    }

    public PartidoReadDto SimularPartido(UsuarioReadDto editor, int id, Random rnd, bool usaMotorProbabilistico)
    {
        SimularPartido(_usuarioMapeador.ADominio(editor), id, rnd, usaMotorProbabilistico);

        return ObtenerPartidoPorIdDto(id);
    }

    public void SimularPenales(Usuario editor, int id, Random rnd)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede simular penales.");
        }

        Partido partido = ObtenerPartidoPorId(id);
        Fixture fixture = _IinMemoryFixture.ObtenerFixturePorToken(partido.Token);
        
        validarPartidoPermitePenales(partido);

        if (partido.PenalesLocal != null || partido.PenalesVisitante != null)
        {
            throw new InvalidOperationException("El partido ya tiene un resultado por penales.");
        }

        partido.SimularPenales(rnd, fixture.UsaMotorProbabilistico);
        _IinMemoryPartido.GuardarCambios();

        _auditoriaService.RegistrarLog(
            editor,
            $"Penales simulados para partido ID {id}: {partido.PenalesLocal} - {partido.PenalesVisitante}"
        );
    }

    public PartidoReadDto SimularPenales(UsuarioReadDto editor, int id, Random rnd)
    {
        SimularPenales(_usuarioMapeador.ADominio(editor), id, rnd);

        return ObtenerPartidoPorIdDto(id);
    }

    public PartidoReadDto SimularPenales(UsuarioReadDto editor, int id)
    {
        Partido partido = ObtenerPartidoPorId(id);
        Random rnd = _IinMemoryFixture.ObtenerFixturePorToken(partido.Token).Semilla.RandomSimulacion;
        return SimularPenales(editor, id, rnd);
    }
    public void CargarPenales(Usuario editor, int id, int penalesLocal, int penalesVisitante)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new UnauthorizedAccessException("Solo el editor puede cargar penales.");
        }

        Partido partido = ObtenerPartidoPorId(id);

        validarPartidoPermitePenales(partido);

        partido.AsignarPenales(penalesLocal, penalesVisitante);
        _IinMemoryPartido.GuardarCambios();

        _auditoriaService.RegistrarLog(
            editor,
            $"Penales cargados para partido ID {id}: {penalesLocal} - {penalesVisitante}"
        );
    }

    public PartidoReadDto CargarPenales(UsuarioReadDto editor, int id, int penalesLocal, int penalesVisitante)
    {
        CargarPenales(_usuarioMapeador.ADominio(editor), id, penalesLocal, penalesVisitante);

        return ObtenerPartidoPorIdDto(id);
    }
    private void validarPartidoPermitePenales(Partido partido)
    {
        if (partido.Fase == "Fase de Grupos")
        {
            throw new InvalidOperationException("Un partido de fase de grupos no puede definirse por penales.");
        }

        if (partido.GolesLocal == null || partido.GolesVisitante == null)
        {
            throw new InvalidOperationException("No se pueden cargar penales si el partido no tiene resultado.");
        }

        if (partido.GolesLocal != partido.GolesVisitante)
        {
            throw new InvalidOperationException("Solo se pueden cargar penales si el partido terminó empatado.");
        }
    }
    
    private void validarFechaNoAnteriorAEtapaPrevia(Partido partido, DateTime nuevaFecha)
    {
        List<Partido> partidos = _IinMemoryPartido.ObtenerPartidos();

        if (partido.Fase == "Fase de Grupos")
        {
            int jornadaActual = partido.Jornada.Value;

            if (jornadaActual <= 1)
            {
                return;
            }

            DateTime fechaMaximaJornadaAnterior = partidos
                .Where(p => p.Fase == "Fase de Grupos")
                .Where(p => p.Jornada < jornadaActual)
                .Where(p => p.Fecha != null)
                .Max(p => p.Fecha.Value);

            if (nuevaFecha < fechaMaximaJornadaAnterior)
            {
                throw new ArgumentException("La fecha no puede ser anterior a la fecha de una jornada anterior.");
            }

            return;
        }

        List<string> ordenFases = Partido.Fases;

        int indiceFaseActual = ordenFases.IndexOf(partido.Fase);

        if (indiceFaseActual <= 0)
        {
            return;
        }

        List<string> fasesAnteriores = ordenFases.Take(indiceFaseActual).ToList();

        DateTime fechaMaximaFaseAnterior = partidos
            .Where(p => fasesAnteriores.Contains(p.Fase))
            .Where(p => p.Fecha != null)
            .Max(p => p.Fecha.Value);

        if (nuevaFecha < fechaMaximaFaseAnterior)
        {
            throw new ArgumentException("La fecha no puede ser anterior a la fecha de una fase anterior.");
        }
    }

    private static PartidoReadDto mapearPartido(Partido partido)
    {
        return new PartidoReadDto
        {
            Id = partido.Id,
            Fecha = partido.Fecha,
            Estadio = _estadioMapeador.ADto(partido.Estadio!),
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


private class DummyNotificacionService : INotificacionService
{
    public event Action? NotificacionesActualizadas
    {
        add { }
        remove { }
    }

    public void CrearNotificacionParaPeriodistas(string nombreLocal, int golesLocal, int golesVisitante, string nombreVisitante) { }
    public void CrearNotificacionParaPeriodistas(string mensaje) { }
    public List<NotificacionDto> ObtenerNoLeidasPorUsuario(string usuarioCorreo) => new List<NotificacionDto>();
    public void MarcarLeida(int notificacionId) { }
    public void MarcarTodasLeidas(string usuarioCorreo) { }
}

}

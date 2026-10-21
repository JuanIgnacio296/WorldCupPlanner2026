using App.Dominio;
using DTOs;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class PartidoDtoServiceTests
{
    private IPartidoService _servicio = null!;
    private InMemoryPartido _inMemoryPartido = null!;
    private AppDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        _context = new AppDbContext(new DbContextOptions<AppDbContext>());
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        _inMemoryPartido = new InMemoryPartido();
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        inMemoryFixture.AgregarFixture(1, new Fixture(1, new List<Grupo>(), new List<Estadio>(), new Semilla(1, 1, 1, 1, 1), false));
        _servicio = new PartidoService(auditoriaService, _inMemoryPartido, new NotificacionServiceNoOp(), inMemoryFixture);
    }

    private sealed class NotificacionServiceNoOp : INotificacionService
    {
        public event Action? NotificacionesActualizadas;
        public void CrearNotificacionParaPeriodistas(string nombreLocal, int golesLocal, int golesVisitante, string nombreVisitante) { }
        public void CrearNotificacionParaPeriodistas(string mensaje) { }
        public List<NotificacionDto> ObtenerNoLeidasPorUsuario(string usuarioCorreo) => new List<NotificacionDto>();
        public void MarcarLeida(int notificacionId) { }
        public void MarcarTodasLeidas(string usuarioCorreo) { }
    }

    [TestMethod]
    public void ObtenerPartidoPorId_ConIdExistente_DebeDevolverPartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);

        PartidoReadDto obtenido = _servicio.ObtenerPartidoPorIdDto(partido.Id);

        Assert.AreEqual(partido.Id, obtenido.Id);
        Assert.AreEqual(new DateTime(2026, 6, 1, 14, 0, 0), obtenido.Fecha);
        Assert.AreEqual("Centenario", obtenido.Estadio.Nombre);
        Assert.AreEqual("Uruguay", obtenido.EquipoLocal.Nombre);
        Assert.AreEqual("Argentina", obtenido.EquipoVisitante.Nombre);
        Assert.AreEqual("Fase de Grupos", obtenido.Fase);
        Assert.AreEqual(1, obtenido.Jornada);
        Assert.AreNotEqual(typeof(Partido), obtenido.GetType());
    }

    [TestMethod]
    public void ObtenerPartidoPorId_ConTarjetasRegistradas_DebeDevolverPartidoReadDtoConTarjetas()
    {
        Partido partido = CrearPartido();
        partido.RegistrarTarjetas(2, 1, 3, 0);
        _inMemoryPartido.AgregarPartido(partido);

        PartidoReadDto obtenido = _servicio.ObtenerPartidoPorIdDto(partido.Id);

        Assert.AreEqual(2, obtenido.TarjetasAmarillasLocal);
        Assert.AreEqual(1, obtenido.TarjetasRojasLocal);
        Assert.AreEqual(3, obtenido.TarjetasAmarillasVisitante);
        Assert.AreEqual(0, obtenido.TarjetasRojasVisitante);
    }

    [TestMethod]
    public void ObtenerPartidosListado_DebeDevolverListaDePartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);

        List<PartidoReadDto> partidos = _servicio.ObtenerPartidosListado();

        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual(partido.Id, partidos[0].Id);
        Assert.AreEqual("Uruguay", partidos[0].EquipoLocal.Nombre);
        Assert.AreEqual("Argentina", partidos[0].EquipoVisitante.Nombre);
        Assert.AreNotEqual(typeof(Partido), partidos[0].GetType());
    }

    [TestMethod]
    public void EditarFecha_ConUsuarioReadDto_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();
        DateTime nuevaFecha = new DateTime(2026, 6, 2, 18, 0, 0);

        PartidoReadDto actualizado = _servicio.EditarFecha(editor, partido.Id, nuevaFecha);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.AreEqual(nuevaFecha, actualizado.Fecha);
        Assert.AreEqual(nuevaFecha, _inMemoryPartido.ObtenerPartido(partido.Id).Fecha);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    [TestMethod]
    public void EditarEstadio_ConDtos_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();
        EstadioReadDto nuevoEstadio = new EstadioReadDto
        {
            Nombre = "MetLife",
            Ciudad = "New Jersey",
            Capacidad = 82500,
            Descripcion = "Estadio mundialista"
        };

        PartidoReadDto actualizado = _servicio.EditarEstadio(editor, partido.Id, nuevoEstadio);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.AreEqual("MetLife", actualizado.Estadio.Nombre);
        Assert.AreEqual("MetLife", _inMemoryPartido.ObtenerPartido(partido.Id).Estadio!.Nombre);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    [TestMethod]
    public void CargarResultado_ConUsuarioReadDto_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();

        PartidoReadDto actualizado = _servicio.CargarResultado(editor, partido.Id, 2, 1);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.AreEqual(2, actualizado.GolesLocal);
        Assert.AreEqual(1, actualizado.GolesVisitante);
        Assert.AreEqual(2, _inMemoryPartido.ObtenerPartido(partido.Id).GolesLocal);
        Assert.AreEqual(1, _inMemoryPartido.ObtenerPartido(partido.Id).GolesVisitante);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    [TestMethod]
    public void CargarTarjetas_ConUsuarioReadDto_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();

        PartidoReadDto actualizado = _servicio.CargarTarjetas(editor, partido.Id, 2, 1, 3, 0);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.AreEqual(2, actualizado.TarjetasAmarillasLocal);
        Assert.AreEqual(1, actualizado.TarjetasRojasLocal);
        Assert.AreEqual(3, _inMemoryPartido.ObtenerPartido(partido.Id).TarjetasAmarillasVisitante);
        Assert.AreEqual(0, _inMemoryPartido.ObtenerPartido(partido.Id).TarjetasRojasVisitante);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    [TestMethod]
    public void CargarPenales_ConUsuarioReadDto_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartidoFinalEmpatado();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();

        PartidoReadDto actualizado = _servicio.CargarPenales(editor, partido.Id, 4, 2);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.AreEqual(4, actualizado.PenalesLocal);
        Assert.AreEqual(2, actualizado.PenalesVisitante);
        Assert.AreEqual(4, _inMemoryPartido.ObtenerPartido(partido.Id).PenalesLocal);
        Assert.AreEqual(2, _inMemoryPartido.ObtenerPartido(partido.Id).PenalesVisitante);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    [TestMethod]
    public void SimularPenales_ConUsuarioReadDto_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartidoFinalEmpatado();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();
        Random random = new Random(1);

        PartidoReadDto actualizado = _servicio.SimularPenales(editor, partido.Id, random);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.IsNotNull(actualizado.PenalesLocal);
        Assert.IsNotNull(actualizado.PenalesVisitante);
        Assert.IsNotNull(_inMemoryPartido.ObtenerPartido(partido.Id).PenalesLocal);
        Assert.IsNotNull(_inMemoryPartido.ObtenerPartido(partido.Id).PenalesVisitante);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    [TestMethod]
    public void SimularPartido_ConUsuarioReadDto_DebeActualizarYDevolverPartidoReadDto()
    {
        Partido partido = CrearPartido();
        _inMemoryPartido.AgregarPartido(partido);
        UsuarioReadDto editor = CrearEditor();
        Random random = new Random(1);

        PartidoReadDto actualizado = _servicio.SimularPartido(editor, partido.Id, random, true);

        Assert.AreEqual(partido.Id, actualizado.Id);
        Assert.IsNotNull(actualizado.GolesLocal);
        Assert.IsNotNull(actualizado.GolesVisitante);
        Assert.IsNotNull(_inMemoryPartido.ObtenerPartido(partido.Id).GolesLocal);
        Assert.IsNotNull(_inMemoryPartido.ObtenerPartido(partido.Id).GolesVisitante);
        Assert.AreNotEqual(typeof(Partido), actualizado.GetType());
    }

    private static Partido CrearPartido()
    {
        Estadio estadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 400);
        Equipo visitante = new Equipo("Argentina", "CONMEBOL", 900);

        return new Partido(
            1,
            1,
            'A',
            new DateTime(2026, 6, 1, 14, 0, 0),
            estadio,
            local,
            visitante,
            "Fase de Grupos",
            1
        );
    }

    private static Partido CrearPartidoFinalEmpatado()
    {
        Estadio estadio = new Estadio("MetLife", "New Jersey", 82500, "Estadio mundialista");
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 400);
        Equipo visitante = new Equipo("Francia", "UEFA", 900);

        return new Partido(
            1,
            1,
            null,
            new DateTime(2026, 7, 19, 18, 0, 0),
            estadio,
            local,
            visitante,
            1,
            1,
            "Final",
            "18:00",
            null
        );
    }

    private static UsuarioReadDto CrearEditor()
    {
        return new UsuarioReadDto
        {
            Nombre = "Editor",
            Apellido = "Fixture",
            Correo = "editor@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            Roles = ["Editor"]
        };
    }
}

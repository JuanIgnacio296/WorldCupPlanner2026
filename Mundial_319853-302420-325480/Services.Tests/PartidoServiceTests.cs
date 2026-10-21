using App.Dominio;
using Services;
using DataAccess;
using DataAccess.Interfaces;
using Services.Interfaces;
using DTOs;
using Microsoft.EntityFrameworkCore;

namespace Services.Tests;

[TestClass]
public class PartidoServiceTests
{
    private IAuditoriaService _auditoriaService;
    private Usuario editor;
    private int _siguienteIdPartidoTest = 1;
    private AppDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        _auditoriaService = new AuditoriaService(inMemoryLog);
        editor = new Usuario("Editor", "Sist", "editor@ort.edu.uy", DateTime.Now.AddYears(-20), "Editor123#", new List<Rol> { Rol.Editor });
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    private Partido CrearPartido(int id)
    {
        Estadio estadio = new Estadio("Estadio A", "Ciudad A", 30000, "Descripcion A");
        Equipo local = new Equipo("Equipo Local", "UEFA", 2000);
        Equipo visitante = new Equipo("Equipo Visitante", "CONMEBOL", 1900);

        Partido p = new Partido(id, 1, 'A', new DateTime(2026, 06, 01, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);
        p.Horario = "14:00";
        return p;
    }

    private Partido CrearPartidoParaFecha(
        string nombreLocal,
        string nombreVisitante,
        Estadio estadio,
        string fase,
        int? jornada,
        DateTime fecha)
    {
        Equipo local = new Equipo(nombreLocal, "UEFA", 1800);
        Equipo visitante = new Equipo(nombreVisitante, "CONMEBOL", 1700);

        int id = _siguienteIdPartidoTest++;

        Partido partido;
        if (fase == "Fase de Grupos")
        {
            partido = new Partido(id, 1, 'A', fecha, estadio, local, visitante, fase, jornada);
            partido.Horario = fecha.ToString("HH:mm");
        }
        else
        {
            partido = new Partido(id, 1, null, fecha, estadio, local, visitante, null, null, fase, fecha.ToString("HH:mm"), jornada);
        }

        return partido;
    }

    [TestMethod]
    public void ObtenerPartidoPorId_WhenPartidoExists_ThenReturnsPartido()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        Partido partidoObtenido = service.ObtenerPartidoPorId(partido.Id);

        Assert.AreSame(partido, partidoObtenido);
    }

    [TestMethod]
    public void ObtenerPartidoPorIdDto_WhenEquiposTienenBandera_ThenPreservaBanderaBase64()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Estadio estadio = new Estadio("Estadio A", "Ciudad A", 30000, "Descripcion A");
        Equipo local = new Equipo("Equipo Local", "UEFA", 2000, "bandera-local-base64");
        Equipo visitante = new Equipo("Equipo Visitante", "CONMEBOL", 1900, "bandera-visitante-base64");
        Partido partido = new Partido(1, 1, 'A', new DateTime(2026, 06, 01, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);
        partido.Horario = "14:00";
        inMemoryPartido.AgregarPartido(partido);

        PartidoReadDto dto = service.ObtenerPartidoPorIdDto(1);

        Assert.AreEqual("bandera-local-base64", dto.EquipoLocal.BanderaBase64);
        Assert.AreEqual("bandera-visitante-base64", dto.EquipoVisitante.BanderaBase64);
    }

    [TestMethod]
    public void ObtenerPartidoPorId_WhenPartidoDoesNotExist_ThenThrowsException()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Assert.ThrowsException<Exception>(() =>
        {
            service.ObtenerPartidoPorId(999);
        });
    }

    [TestMethod]
    public void ObtenerPartidos_WhenCalled_ThenReturnsAllPartidos()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido1 = CrearPartido(1);
        Partido partido2 = CrearPartido(2);
        inMemoryPartido.AgregarPartido(partido1);
        inMemoryPartido.AgregarPartido(partido2);

        List<Partido> partidosObtenidos = service.ObtenerPartidos();

        Assert.AreEqual(2, partidosObtenidos.Count);
        Assert.IsTrue(partidosObtenidos.Contains(partido1));
        Assert.IsTrue(partidosObtenidos.Contains(partido2));
    }

    [TestMethod]
    public void EditarFecha_WhenPartidoExists_ThenUpdatesFecha()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        DateTime nuevaFecha = new DateTime(2026, 06, 04, 18, 0, 0);
        service.EditarFecha(editor, partido.Id, nuevaFecha);

        Assert.AreEqual(nuevaFecha, partido.Fecha);
    }

    [TestMethod]
    public void EditarEstadio_WhenPartidoExists_ThenUpdatesEstadio()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        Estadio nuevoEstadio = new Estadio("Estadio B", "Ciudad B", 40000, "Descripcion B");
        service.EditarEstadio(editor, partido.Id, nuevoEstadio);

        Assert.AreSame(nuevoEstadio, partido.Estadio);
    }

    [TestMethod]
    public void CargarResultado_WhenPartidoExists_ThenUpdatesResultado()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        service.CargarResultado(editor, partido.Id, 2, 1);

        Assert.AreEqual(2, partido.GolesLocal);
        Assert.AreEqual(1, partido.GolesVisitante);
    }

    [TestMethod]
    public void CargarResultado_WhenPartidoDoesNotExist_ThenThrowsException()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Assert.ThrowsException<Exception>(() =>
        {
            service.CargarResultado(editor, 999, 2, 1);
        });
    }

    [TestMethod]
    public void CargarResultado_WhenLocalWins_ThenVencedorIsLocal()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        service.CargarResultado(editor, partido.Id, 3, 1);

        Assert.AreEqual(partido.EquipoLocal.Nombre, partido.Vencedor);
    }

    [TestMethod]
    public void EditarFecha_WhenFechaAnteriorAlInicioDelTorneo_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        DateTime fechaInvalida = new DateTime(2026, 5, 31, 14, 0, 0);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            service.EditarFecha(editor, partido.Id, fechaInvalida);
        });
    }

    [TestMethod]
    public void EditarFecha_WhenHoraNoPermitida_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        DateTime fechaInvalida = new DateTime(2026, 6, 2, 15, 0, 0);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            service.EditarFecha(editor, partido.Id, fechaInvalida);
        });
    }

    [TestMethod]
    public void EditarFecha_WhenMinutosNoSonCero_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        DateTime fechaInvalida = new DateTime(2026, 6, 2, 18, 30, 0);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            service.EditarFecha(editor, partido.Id, fechaInvalida);
        });
    }

    [TestMethod]
    public void EditarFecha_WhenEstadioNoDisponible_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        DateTime fechaOcupada = new DateTime(2026, 6, 2, 18, 0, 0);
        partido.Estadio.AddFecha(fechaOcupada);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            service.EditarFecha(editor, partido.Id, fechaOcupada);
        });
    }

    [TestMethod]
    public void EditarFecha_WhenUsuarioNoEsEditor_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Usuario noEditor = new Usuario("Usuario", "Comun", "u@ort.edu.uy", DateTime.Now.AddYears(-20), "Pass123#", new List<Rol> { Rol.Ninguno });
        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            service.EditarFecha(noEditor, partido.Id, DateTime.Now.AddDays(10));
        });
    }

    [TestMethod]
    public void EditarEstadio_WhenUsuarioNoEsEditor_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Usuario noEditor = new Usuario("NoEditor", "Sist", "u@ort.edu.uy", DateTime.Now.AddYears(-20), "NoEditor123#", new List<Rol> { Rol.Ninguno });
        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);
        Estadio nuevoEstadio = new Estadio("Estadio B", "Ciudad B", 40000, "Desc");

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            service.EditarEstadio(noEditor, partido.Id, nuevoEstadio);
        });
    }

    [TestMethod]
    public void CargarResultado_WhenUsuarioNoEsEditor_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Usuario noEditor = new Usuario("NoEditor", "Sist", "u@ort.edu.uy", DateTime.Now.AddYears(-20), "NoEditor123#", new List<Rol> { Rol.Ninguno });
        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            service.CargarResultado(noEditor, partido.Id, 1, 1);
        });
    }

    [TestMethod]
    public void ConstructorLista_WhenRecibeLista_ThenCargaTodosLosPartidosEnMemoria()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        List<Partido> listaPartidos = new List<Partido>
        {
            CrearPartido(1),
            CrearPartido(2),
            CrearPartido(3)
        };

        PartidoService service = new PartidoService(listaPartidos, _auditoriaService, inMemoryPartido);

        List<Partido> partidosEnMemoria = service.ObtenerPartidos();
        Assert.AreEqual(3, partidosEnMemoria.Count);
    }

    [TestMethod]
    public void AgregarPartido_WhenCalled_ThenPartidoIsStoredSuccessfully()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Partido nuevoPartido = CrearPartido(1);
        service.AgregarPartido(nuevoPartido);

        Partido partidoRecuperado = service.ObtenerPartidoPorId(nuevoPartido.Id);

        Assert.IsNotNull(partidoRecuperado);
        Assert.AreEqual(nuevoPartido.Id, partidoRecuperado.Id);
    }

    [TestMethod]
    public void SimularPartido_WhenFaseDeGrupos_SetsResultsWithoutPenalties()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        Random rnd = new Random(10);
        service.SimularPartido(editor, partido.Id, rnd, true);

        Assert.IsNotNull(partido.GolesLocal);
        Assert.IsNotNull(partido.GolesVisitante);
        Assert.IsNull(partido.PenalesLocal);
    }

    [TestMethod]
    public void SimularPenales_WhenPartidoEliminatorioEmpatado_ThenAsignaResultadosDistintos()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        inMemoryFixture.AgregarFixture(1, new Fixture(1, new List<Grupo>(), new List<Estadio>(), new Semilla(1, 1, 1, 1, 1), false));
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService), inMemoryFixture);

        Partido partido = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Octavos de Final", null, new DateTime(2026, 7, 1, 14, 0, 0));
        partido.AsignarResultado(1, 1);
        inMemoryPartido.AgregarPartido(partido);

        Random rnd = new Random(1);
        service.SimularPenales(editor, partido.Id, rnd);

        Assert.IsNotNull(partido.PenalesLocal);
        Assert.IsNotNull(partido.PenalesVisitante);
        Assert.AreNotEqual(partido.PenalesLocal, partido.PenalesVisitante);
    }

    [TestMethod]
    public void SimularPartido_WhenUserIsNotEditor_ThrowsUnauthorizedAccessException()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));
        Usuario noEditor = new Usuario("Test", "User", "test@ort.edu.uy", DateTime.Now.AddYears(-20), "Password123#", new List<Rol> { Rol.Ninguno });
        Partido partido = CrearPartido(1);
        inMemoryPartido.AgregarPartido(partido);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.SimularPartido(noEditor, partido.Id, new Random(), true));
    }

    [TestMethod]
    public void SimularPartido_WhenPartidoAlreadyHasResult_ThrowsInvalidOperationException()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartido(1);
        partido.AsignarResultado(1, 1);
        inMemoryPartido.AgregarPartido(partido);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.SimularPartido(editor, partido.Id, new Random(), true));
    }

    [TestMethod]
    public void EditarFecha_CuandoJornada2QuedaAntesDeJornada1_ThenLanzaExcepcion()
    {
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, inMemoryPartido, new NotificacionService(_context, _auditoriaService));

        Estadio estadio1 = new Estadio("Estadio 1", "Ciudad", 30000, "Desc");
        Estadio estadio2 = new Estadio("Estadio 2", "Ciudad", 30000, "Desc");

        Partido jornada1 = CrearPartidoParaFecha("A", "B", estadio1, "Fase de Grupos", 1, new DateTime(2026, 6, 5, 14, 0, 0));
        Partido jornada2 = CrearPartidoParaFecha("C", "D", estadio2, "Fase de Grupos", 2, new DateTime(2026, 6, 8, 14, 0, 0));

        inMemoryPartido.AgregarPartido(jornada1);
        inMemoryPartido.AgregarPartido(jornada2);

        DateTime fechaInvalida = new DateTime(2026, 6, 4, 18, 0, 0);

        Assert.ThrowsException<ArgumentException>(() =>
        {
            service.EditarFecha(editor, jornada2.Id, fechaInvalida);
        });
    }

    [TestMethod]
    public void CargarPenales_CuandoPartidoValido_AsignaPenales()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Final", null, new DateTime(2026, 7, 1, 14, 0, 0));
        partido.AsignarResultado(1, 1);
        mem.AgregarPartido(partido);

        service.CargarPenales(editor, partido.Id, 4, 3);

        Assert.AreEqual(4, partido.PenalesLocal);
        Assert.AreEqual(3, partido.PenalesVisitante);
    }

    [TestMethod]
    public void CargarPenales_CuandoNoEsEditor_LanzaExcepcion()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Final", null, new DateTime(2026, 7, 1, 14, 0, 0));
        partido.AsignarResultado(1, 1);
        mem.AgregarPartido(partido);
        Usuario noEditor = new Usuario("No", "Rol", "no@test.com", DateTime.Now.AddYears(-20), "Norol123#", [Rol.Ninguno]);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.CargarPenales(noEditor, partido.Id, 4, 3));
    }

    [TestMethod]
    public void CargarPenales_CuandoEsFaseGrupos_LanzaInvalidOperationException()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
    
        Partido faseGrupos = CrearPartido(1);
        faseGrupos.AsignarResultado(1, 1);
        mem.AgregarPartido(faseGrupos);

        Assert.ThrowsException<InvalidOperationException>(() => service.CargarPenales(editor, faseGrupos.Id, 4, 3));
    }

    [TestMethod]
    public void CargarPenales_CuandoPartidoNoTieneResultado_LanzaInvalidOperationException()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
    
        Partido sinResultado = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Final", null, new DateTime(2026, 7, 1, 14, 0, 0));
        mem.AgregarPartido(sinResultado);

        Assert.ThrowsException<InvalidOperationException>(() => service.CargarPenales(editor, sinResultado.Id, 4, 3));
    }

    [TestMethod]
    public void CargarPenales_CuandoPartidoNoTerminoEmpatado_LanzaInvalidOperationException()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
    
        Partido noEmpatado = CrearPartidoParaFecha("C", "D", new Estadio("E2", "C2", 30000, "D2"), "Final", null, new DateTime(2026, 7, 2, 14, 0, 0));
        noEmpatado.AsignarResultado(2, 1);
        mem.AgregarPartido(noEmpatado);

        Assert.ThrowsException<InvalidOperationException>(() => service.CargarPenales(editor, noEmpatado.Id, 4, 3));
    }
    
    [TestMethod]
    public void CargarTarjetas_CuandoNoEsEditor_LanzaExcepcion()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartido(1);
        mem.AgregarPartido(partido);
        UsuarioReadDto noEditorDto = new UsuarioReadDto { Nombre = "guest", Apellido = "sist", Correo = "guest@mail.com" ,Roles = new List<string> { "Ninguno" } };

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.CargarTarjetas(noEditorDto, partido.Id, 1, 0, 1, 0));
    }

    [TestMethod]
    public void CargarTarjetas_CuandoEsEditor_ActualizaTarjetas()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartido(1);
        mem.AgregarPartido(partido);
        UsuarioReadDto editorDto = new UsuarioReadDto { Nombre = "Editor", Apellido = "Sist", Correo = "editor@mail.com" ,Roles = new List<string> { "Editor" } };

        service.CargarTarjetas(editorDto, partido.Id, 2, 1, 3, 0);

        Assert.AreEqual(2, partido.TarjetasAmarillasLocal);
        Assert.AreEqual(1, partido.TarjetasRojasLocal);
        Assert.AreEqual(3, partido.TarjetasAmarillasVisitante);
        Assert.AreEqual(0, partido.TarjetasRojasVisitante);
    }

    [TestMethod]
    public void SimularPenales_CuandoNoEsEditor_LanzaExcepcion()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartido(1);
        mem.AgregarPartido(partido);
        Usuario noEditor = new Usuario("No", "Rol", "no@test.com", DateTime.Now.AddYears(-20), "Norol123#", [Rol.Ninguno]);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.SimularPenales(noEditor, partido.Id, new Random()));
    }

    [TestMethod]
    public void SimularPenales_CuandoYaTienePenales_LanzaExcepcion()
    {
        InMemoryPartido mem = new InMemoryPartido();
        InMemoryFixture inMemFix = new InMemoryFixture();
        inMemFix.AgregarFixture(1, new Fixture(1, [], [], new Semilla(1,1,1,1,1), false));
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService), inMemFix);
        
        Partido partido = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Final", null, new DateTime(2026, 7, 1, 14, 0, 0));
        partido.AsignarResultado(1, 1);
        partido.AsignarPenales(4, 3);
        mem.AgregarPartido(partido);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.SimularPenales(editor, partido.Id, new Random()));
    }

    [TestMethod]
    public void SimularPenales_CuandoNoEmpatado_LanzaExcepcion()
    {
        InMemoryPartido mem = new InMemoryPartido();
        InMemoryFixture inMemFix = new InMemoryFixture();
        inMemFix.AgregarFixture(1, new Fixture(1, [], [], new Semilla(1,1,1,1,1), false));
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService), inMemFix);
        
        Partido partido = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Final", null, new DateTime(2026, 7, 1, 14, 0, 0));
        partido.AsignarResultado(2, 1);
        mem.AgregarPartido(partido);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.SimularPenales(editor, partido.Id, new Random()));
    }

    [TestMethod]
    public void EditarFecha_FaseGruposJornada1_PermiteCualquierFechaValida()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        Partido partido = CrearPartidoParaFecha("A", "B", new Estadio("E", "C", 30000, "D"), "Fase de Grupos", 1, new DateTime(2026, 6, 5, 14, 0, 0));
        mem.AgregarPartido(partido);

        DateTime nuevaFecha = new DateTime(2026, 6, 2, 18, 0, 0);
        service.EditarFecha(editor, partido.Id, nuevaFecha);

        Assert.AreEqual(nuevaFecha, partido.Fecha);
    }

    [TestMethod]
    public void EditarFecha_OctavosAntesDeGrupos_LanzaExcepcion()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        
        Partido grupos = CrearPartidoParaFecha("A", "B", new Estadio("E1", "C", 30000, "D"), "Fase de Grupos", 3, new DateTime(2026, 6, 20, 14, 0, 0));
        Partido octavos = CrearPartidoParaFecha("C", "D", new Estadio("E2", "C", 30000, "D"), "Octavos de Final", null, new DateTime(2026, 6, 25, 14, 0, 0));
        
        mem.AgregarPartido(grupos);
        mem.AgregarPartido(octavos);

        DateTime fechaInvalida = new DateTime(2026, 6, 15, 14, 0, 0);

        Assert.ThrowsException<ArgumentException>(() =>
            service.EditarFecha(editor, octavos.Id, fechaInvalida));
    }

    [TestMethod]
    public void EditarFecha_CuartosDespuesDeOctavos_Exito()
    {
        InMemoryPartido mem = new InMemoryPartido();
        PartidoService service = new PartidoService(_auditoriaService, mem, new NotificacionService(_context, _auditoriaService));
        
        Partido octavos = CrearPartidoParaFecha("A", "B", new Estadio("E1", "C", 30000, "D"), "Octavos de Final", null, new DateTime(2026, 6, 25, 14, 0, 0));
        Partido cuartos = CrearPartidoParaFecha("C", "D", new Estadio("E2", "C", 30000, "D"), "Cuartos de Final", null, new DateTime(2026, 6, 30, 14, 0, 0));
        
        mem.AgregarPartido(octavos);
        mem.AgregarPartido(cuartos);

        DateTime nuevaFecha = new DateTime(2026, 7, 5, 14, 0, 0);
        service.EditarFecha(editor, cuartos.Id, nuevaFecha);

        Assert.AreEqual(nuevaFecha, cuartos.Fecha);
    }
}

using App.Dominio;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Services;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class NotificacionServiceTest
{
    private DbContextOptions<AppDbContext> _options;
    private AppDbContext _context;
    private IAuditoriaService _auditoriaService;

    [TestInitialize]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
            
        _context = new AppDbContext(_options);
        _auditoriaService = new AuditoriaService(new InMemoryLog());
    }

    [TestMethod]
    public void CrearNotificacionParaPeriodistas_CuandoSeInvoca_DebeGuardarNotificacionSoloParaPeriodistas()
    {
        Usuario periodista1 = new Usuario("Juan", "Perez", "juan@periodista.com", DateTime.Now.AddYears(-25), "Password123#", new List<Rol> { Rol.Periodista });
        Usuario periodista2 = new Usuario("Maria", "Gomez", "maria@periodista.com", DateTime.Now.AddYears(-30), "Password123#", new List<Rol> { Rol.Periodista });
        Usuario editor = new Usuario("Carlos", "Lopez", "carlos@editor.com", DateTime.Now.AddYears(-28), "Password123#", new List<Rol> { Rol.Editor });
        
        _context.Usuarios.AddRange(periodista1, periodista2, editor);
        _context.SaveChanges();

        INotificacionService service = new NotificacionService(_context, _auditoriaService);
        string mensaje = "Se ha actualizado un resultado.";

        service.CrearNotificacionParaPeriodistas(mensaje);

        var notificaciones = _context.Notificaciones.ToList();
        
        Assert.AreEqual(2, notificaciones.Count);
        Assert.IsTrue(notificaciones.Any(n => n.UsuarioCorreo == periodista1.Correo));
    }

    [TestMethod]
    public void AsignarResultado_AlGuardarExitosamente_DebeDispararEventoYCrearNotificaciones()
    {
        Usuario periodista = new Usuario("Pedro", "Alvarez", "p@p.com", DateTime.Now.AddYears(-25), "Pass1234#", new List<Rol> { Rol.Periodista });
        _context.Usuarios.Add(periodista);

        Estadio estadio = new Estadio("Centenario", "Montevideo", 40000);
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 300);
        Equipo visitante = new Equipo("Brasil", "CONMEBOL", 300);
        Partido partido = new Partido(1, 1,'A',DateTime.Now, estadio, local, visitante, "Fase de Grupos", 1);
        partido.Horario = "14:00";
        _context.Partidos.Add(partido);
        _context.SaveChanges();

        INotificacionService notificacionService = new NotificacionService(_context, _auditoriaService);
        
        partido.AsignarResultado(2, 1);
        notificacionService.CrearNotificacionParaPeriodistas(partido.EquipoLocal.Nombre, partido.GolesLocal.Value, partido.GolesVisitante.Value, partido.EquipoVisitante.Nombre);

        Notificacion? notifGuardada = _context.Notificaciones.FirstOrDefault();
        Assert.IsNotNull(notifGuardada);
        Assert.AreEqual(periodista.Correo, notifGuardada.UsuarioCorreo);
        Assert.IsTrue(notifGuardada.Mensaje.Contains("Uruguay 2 - 1 Brasil"));
    }

    [TestMethod]
    public void MarcarTodasLeidas_CuandoSeInvoca_DebeMarcarComoLeidasTodasLasNotificacionesDelUsuario()
    {
        string correo = "juan@periodista.com";
        _context.Notificaciones.Add(new Notificacion(correo, "Mensaje 1"));
        _context.Notificaciones.Add(new Notificacion(correo, "Mensaje 2"));
        _context.Notificaciones.Add(new Notificacion("otro@correo.com", "Mensaje 3"));
        _context.SaveChanges();
        
        INotificacionService service = new NotificacionService(_context, _auditoriaService);
        
        service.MarcarTodasLeidas(correo);

        var notificacionesJuan = _context.Notificaciones.Where(n => n.UsuarioCorreo == correo).ToList();
        var notificacionOtro = _context.Notificaciones.First(n => n.UsuarioCorreo == "otro@correo.com");

        Assert.IsTrue(notificacionesJuan.All(n => n.Leida));
        Assert.IsFalse(notificacionOtro.Leida);
    }
}

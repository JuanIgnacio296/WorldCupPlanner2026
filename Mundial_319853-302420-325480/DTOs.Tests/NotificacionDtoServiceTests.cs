using App.Dominio;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOs;
using Microsoft.EntityFrameworkCore;

namespace Services.Tests;

[TestClass]
public class NotificacionDtoServiceTests
{
    private INotificacionService _servicio;
    private AppDbContext _context;

    [TestInitialize]
    public void Initialize()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        InMemoryLog logs  = new InMemoryLog();
        IAuditoriaService service = new AuditoriaService(logs);
        _servicio = new NotificacionService(_context, service);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void CrearNotificacionParaPeriodistas_ConPeriodistasExistentes_DebeGenerarNotificacionesParaEllos()
    {
        var periodista1 = new Usuario("Juan", "Perez", "juan@periodista.com", DateTime.Now.AddYears(-25), "Pass123#", [Rol.Periodista]);
        var administrador = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-30), "Admin123#", [Rol.Administrador]);
        
        _context.Usuarios.Add(periodista1);
        _context.Usuarios.Add(administrador);
        _context.SaveChanges();

        _servicio.CrearNotificacionParaPeriodistas("Uruguay", 2, 1, "Brasil");

        var notificacionesGuardadas = _context.Notificaciones.ToList();
        
        Assert.AreEqual(1, notificacionesGuardadas.Count);
        Assert.AreEqual("juan@periodista.com", notificacionesGuardadas.First().UsuarioCorreo);
        Assert.AreEqual("Resultado partido: Uruguay 2 - 1 Brasil", notificacionesGuardadas.First().Mensaje);
        Assert.IsFalse(notificacionesGuardadas.First().Leida);
    }

    [TestMethod]
    public void CrearNotificacionParaPeriodistas_DebeLanzarEventoNotificacionesActualizadas()
    {
        var periodista = new Usuario("Pedro", "Gomez", "pedro@periodista.com", DateTime.Now.AddYears(-25), "Pass123#", [Rol.Periodista]);
        _context.Usuarios.Add(periodista);
        _context.SaveChanges();

        bool eventoFueDetonado = false;
        _servicio.NotificacionesActualizadas += () => { eventoFueDetonado = true; };

        _servicio.CrearNotificacionParaPeriodistas("Se simuló la fecha");

        Assert.IsTrue(eventoFueDetonado, "El evento NotificacionesActualizadas debería haberse invocado.");
    }

    [TestMethod]
    public void ObtenerNoLeidasPorUsuario_DebeRetornarSoloLasNoLeidasDeEseUsuario()
    {
        string correoBuscado = "periodista@ort.edu.uy";
        string otroCorreo = "otro@ort.edu.uy";

        var n1 = new Notificacion(correoBuscado, "Mensaje 1") { Leida = false };
        var n2 = new Notificacion(correoBuscado, "Mensaje 2") { Leida = true };
        var n3 = new Notificacion(otroCorreo, "Mensaje 3") { Leida = false };

        _context.Notificaciones.AddRange(n1, n2, n3);
        _context.SaveChanges();

        List<NotificacionDto> resultado = _servicio.ObtenerNoLeidasPorUsuario(correoBuscado);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Mensaje 1", resultado.First().Mensaje);
    }

    [TestMethod]
    public void MarcarLeida_ConIdValido_DebeCambiarEstadoATrueYFlashearEvento()
    {
        var notificacion = new Notificacion("periodista@ort.edu.uy", "Partido finalizado");
        _context.Notificaciones.Add(notificacion);
        _context.SaveChanges();

        bool eventoFueDetonado = false;
        _servicio.NotificacionesActualizadas += () => { eventoFueDetonado = true; };

        _servicio.MarcarLeida(notificacion.Id);

        var notifEnBd = _context.Notificaciones.Find(notificacion.Id);
        Assert.IsNotNull(notifEnBd);
        Assert.IsTrue(notifEnBd.Leida, "La notificación debería figurar como leída.");
        Assert.IsTrue(eventoFueDetonado, "El evento debió dispararse tras modificar el estado de lectura.");
    }

    [TestMethod]
    public void MarcarTodasLeidas_DebeActualizarSoloLasDelUsuarioEspecificado()
    {
        string correoTarget = "target@periodista.com";
        string correoAjeno = "ajeno@periodista.com";

        var notif1 = new Notificacion(correoTarget, "Notif 1");
        var notif2 = new Notificacion(correoTarget, "Notif 2");
        var notifAjena = new Notificacion(correoAjeno, "Notif Ajena");

        _context.Notificaciones.AddRange(notif1, notif2, notifAjena);
        _context.SaveChanges();

        _servicio.MarcarTodasLeidas(correoTarget);

        Assert.IsTrue(_context.Notificaciones.Find(notif1.Id).Leida);
        Assert.IsTrue(_context.Notificaciones.Find(notif2.Id).Leida);
        Assert.IsFalse(_context.Notificaciones.Find(notifAjena.Id).Leida, "Las notificaciones de otros periodistas no deben verse afectadas.");
    }
}
using App.Dominio;
using DataAccess;
using DTOs;
using Services.Interfaces;
using Services.Mapeadores;

namespace Services;

public class NotificacionService : INotificacionService
{
    private readonly AppDbContext _context;
    private readonly IAuditoriaService _auditoriaService;
    private readonly NotificacionMapeador _notificacionMapeador = new();

    public NotificacionService(AppDbContext context, IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public event Action? NotificacionesActualizadas;

    public void CrearNotificacionParaPeriodistas(string nombreLocal, int golesLocal, int golesVisitante, string nombreVisitante)
    {
        string mensaje = CrearMensajeNotificacion(nombreLocal, golesLocal, golesVisitante, nombreVisitante);
        CrearNotificacionParaPeriodistas(mensaje);
    }

    public void CrearNotificacionParaPeriodistas(string mensaje)
    {
        List<Usuario> periodistas = _context.Usuarios
            .ToList()
            .Where(u => u.TieneRol(Rol.Periodista))
            .ToList();

        foreach (var periodista in periodistas)
        {
            var nuevaNotificacion = new Notificacion(periodista.Correo, mensaje);
            _context.Notificaciones.Add(nuevaNotificacion);
        }
        _context.SaveChanges();
        NotificacionesActualizadas?.Invoke();
    }

    private string CrearMensajeNotificacion(string nombreLocal, int golesLocal, int golesVisitante, string nombreVisitante)
    {
        return $"Resultado partido: {nombreLocal} {golesLocal} - {golesVisitante} {nombreVisitante}";
    }

    public List<NotificacionDto> ObtenerNoLeidasPorUsuario(string usuarioCorreo)
    {
        List<Notificacion> noLeidas = _context.Notificaciones
            .Where(n => n.UsuarioCorreo == usuarioCorreo && !n.Leida)
            .OrderByDescending(n => n.Fecha)
            .ToList();

        return _notificacionMapeador.AListaDto(noLeidas);
    }

    public void MarcarLeida(int notificacionId)
    {
        var notif = _context.Notificaciones.Find(notificacionId);
        if (notif != null && !notif.Leida)
        {
            Usuario? usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == notif.UsuarioCorreo);
            notif.MarcarComoLeida();
            if (usuario != null) _auditoriaService.RegistrarLog(usuario, $"La notificación {notif.Id} fue leída.");
            _context.SaveChanges();
            NotificacionesActualizadas?.Invoke();
        }
    }

    public void MarcarTodasLeidas(string usuarioCorreo)
    {
        List<Notificacion> noLeidas = _context.Notificaciones
            .Where(n => n.UsuarioCorreo == usuarioCorreo && !n.Leida)
            .ToList();

        foreach (Notificacion notificacion in noLeidas)
        {
            notificacion.MarcarComoLeida();
        }

        _context.SaveChanges();
        NotificacionesActualizadas?.Invoke();
    }
}
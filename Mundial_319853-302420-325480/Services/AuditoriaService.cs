using App.Dominio;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;
using Services;

public class AuditoriaService : IAuditoriaService
{
    private IinMemoryLog _IinMemoryLog;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();

    public AuditoriaService(IinMemoryLog IinMemoryLog)
    {
        _IinMemoryLog = IinMemoryLog;
    }
    
    public void RegistrarLog(Usuario usuario, string accion)
    {
        Log nuevoLog = new Log(usuario, accion, DateTime.Now);
        _IinMemoryLog.AgregarLog(nuevoLog);
    }

    public List<Log> ObtenerLogs(Usuario solicitante)
    {
        if (!solicitante.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Acceso denegado. Se requiere rol de Administrador");
        }
        return _IinMemoryLog.ObtenerLogs();
    }

    public List<LogReadDto> ObtenerLogs(UsuarioReadDto solicitante)
    {
        return ObtenerLogs(_usuarioMapeador.ADominio(solicitante))
            .Select(mapearLog)
            .ToList();
    }
    
    public List<Log> ObtenerLogsPorFecha(Usuario solicitante, DateTime inicio, DateTime fin)
    {
        if (!solicitante.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Acceso denegado.");
        }

        return _IinMemoryLog.ObtenerLogs(inicio, fin);
    }

    public List<LogReadDto> ObtenerLogsPorFecha(UsuarioReadDto solicitante, DateTime inicio, DateTime fin)
    {
        return ObtenerLogsPorFecha(_usuarioMapeador.ADominio(solicitante), inicio, fin)
            .Select(mapearLog)
            .ToList();
    }

    private LogReadDto mapearLog(Log log)
    {
        return new LogReadDto
        {
            Id = log.Id,
            Usuario = _usuarioMapeador.ADto(log.Usuario),
            Accion = log.Accion,
            Fecha = log.Fecha
        };
    }
}


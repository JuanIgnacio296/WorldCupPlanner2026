using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IAuditoriaService
{
    void RegistrarLog(Usuario usuario, string accion);
    List<Log> ObtenerLogs(Usuario solicitante);
    List<LogReadDto> ObtenerLogs(UsuarioReadDto solicitante);
    List<Log> ObtenerLogsPorFecha(Usuario solicitante, DateTime inicio, DateTime fin);
    List<LogReadDto> ObtenerLogsPorFecha(UsuarioReadDto solicitante, DateTime inicio, DateTime fin);
}

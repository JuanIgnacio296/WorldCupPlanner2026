using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IUsuarioService
{
    void CrearUsuario(Usuario administrador, Usuario nuevoUsuario);
    UsuarioReadDto CrearUsuario(UsuarioReadDto administrador, UsuarioCreateDto nuevoUsuario);
    void ModificarUsuario(Usuario administrador, string correoOriginal, Usuario usuarioModificado);
    UsuarioReadDto ModificarUsuario(UsuarioReadDto administrador, string correoOriginal, UsuarioUpdateDto usuarioModificado);
    void EliminarUsuario(Usuario administrador, string email);
    void EliminarUsuario(UsuarioReadDto administrador, string email);
    List<Usuario> ObtenerUsuarios(Usuario administrador);
    List<UsuarioReadDto> ObtenerUsuarios(UsuarioReadDto administrador);
    void CambiarPassword(UsuarioReadDto solicitante, CambiarPasswordDto cambioPassword);
    List<Log> ObtenerLogs(Usuario administrador);
    List<LogReadDto> ObtenerLogs(UsuarioReadDto administrador);
}


using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface ISesionService
{
    void Login(string email, string password);
    UsuarioActual? ObtenerUsuarioActual();
    bool RestaurarSesion(string correo);
    void Logout();
}

public class UsuarioActual
{
    private readonly Usuario _usuario;

    public UsuarioActual(Usuario usuario)
    {
        _usuario = usuario;
    }

    public static implicit operator Usuario?(UsuarioActual? usuarioActual)
    {
        return usuarioActual?._usuario;
    }

    public static implicit operator UsuarioReadDto?(UsuarioActual? usuarioActual)
    {
        if (usuarioActual == null)
        {
            return null;
        }

        Usuario usuario = usuarioActual._usuario;
        return new UsuarioReadDto
        {
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Correo = usuario.Correo,
            FechaNacimiento = usuario.FechaNacimiento,
            Roles = usuario.Roles?.Select(rol => rol.ToString()).ToList() ?? []
        };
    }
}

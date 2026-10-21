using App.Dominio;
using DTOs;
using Services.Interfaces;
using DataAccess;
using DataAccess.Interfaces;

namespace Services;

public class SesionService : ISesionService
{
    private IinMemoryUsuario _IinMemoryUsuario;
    private IAuditoriaService _auditoriaService;
    private Usuario? _usuarioActual;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();


    public SesionService(IinMemoryUsuario usuarioService,  IAuditoriaService auditoriaService)
    {
        _IinMemoryUsuario = usuarioService;
        _auditoriaService = auditoriaService;
    }

    
    public void Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("El correo y la contraseña son obligatorios.");
        }

        Usuario? usuario = _IinMemoryUsuario.ObtenerUsuarios()
            .FirstOrDefault(u => u.Correo == email && u.Contrasena == password);

        if (usuario == null)
        {
            throw new ArgumentException("Credenciales incorrectas.");
        }

        _usuarioActual = usuario;

        _auditoriaService.RegistrarLog(usuario, $"Login exitoso del usuario {email}");
    }

    public UsuarioReadDto Login(LoginDto login)
    {
        Login(login.Correo, login.Contrasena);

        Usuario usuario = _usuarioActual!;
        return _usuarioMapeador.ADto(usuario);
    }

    public UsuarioActual? ObtenerUsuarioActual()
    {
        return _usuarioActual == null ? null : new UsuarioActual(_usuarioActual);
    }

    // Restaura la sesión a partir del correo persistido (p. ej. tras un refresh), sin contraseña.
    public bool RestaurarSesion(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            return false;
        }

        Usuario? usuario = _IinMemoryUsuario.ObtenerUsuarios().FirstOrDefault(u => u.Correo == correo);
        _usuarioActual = usuario;
        return usuario != null;
    }

    public void Logout()
    {
        _usuarioActual = null;
    }
    
}




using App.Dominio;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;

namespace Services;

public class UsuarioService : IUsuarioService
{
    private IinMemoryUsuario _IinMemoryUsuario;
    private IAuditoriaService _auditoriaService;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();

    public UsuarioService(IAuditoriaService auditoriaService, IinMemoryUsuario IinMemoryUsuario)
    {
        _auditoriaService = auditoriaService;
        _IinMemoryUsuario = IinMemoryUsuario;
    }
    
    public void CrearUsuario(Usuario administrador, Usuario nuevoUsuario)
    {
        validarAdministrador(administrador);

        if (_IinMemoryUsuario.ExisteUsuario(nuevoUsuario))
        {
            throw new ArgumentException("El correo ya está registrado.");
        }

        _IinMemoryUsuario.AgregarUsuario(nuevoUsuario);

        _auditoriaService.RegistrarLog(
            administrador,
            $"Creación de usuario: {nuevoUsuario.Correo}"
        );
    }

    public UsuarioReadDto CrearUsuario(UsuarioReadDto administrador, UsuarioCreateDto nuevoUsuario)
    {
        Usuario administradorDominio = obtenerUsuarioDesdeDto(administrador);
        Usuario nuevoUsuarioDominio = new Usuario(
            nuevoUsuario.Nombre,
            nuevoUsuario.Apellido,
            nuevoUsuario.Correo,
            nuevoUsuario.FechaNacimiento,
            nuevoUsuario.Contrasena,
            mapearRoles(nuevoUsuario.Roles)
        );

        CrearUsuario(administradorDominio, nuevoUsuarioDominio);

        return _usuarioMapeador.ADto(nuevoUsuarioDominio);
    }
    
    public void ModificarUsuario(Usuario solicitante, string correoOriginal, Usuario usuarioModificado)
    {
        Usuario usuario = _IinMemoryUsuario.ObtenerUsuario(correoOriginal);

        bool esAdmin = solicitante.TieneRol(Rol.Administrador);
        bool esElMismo = solicitante.Correo == correoOriginal;

        if (!esAdmin && !esElMismo)
        {
            throw new UnauthorizedAccessException("No tiene permisos para modificar este usuario.");
        }

        if (!esAdmin && !tienenMismosRoles(usuario.Roles, usuarioModificado.Roles))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede cambiar roles.");
        }

        if (correoOriginal != usuarioModificado.Correo)
        {
            throw new ArgumentException("No se puede cambiar el correo del usuario.");
        }

        usuario.Nombre = usuarioModificado.Nombre;
        usuario.Apellido = usuarioModificado.Apellido;
        usuario.Contrasena = usuarioModificado.Contrasena;
        usuario.FechaNacimiento = usuarioModificado.FechaNacimiento;

        if (esAdmin)
        {
            usuario.Roles = usuarioModificado.Roles;
        }

        _auditoriaService.RegistrarLog(
            solicitante,
            $"Se modificó el usuario: {usuario.Correo}"
        );
    }

    public UsuarioReadDto ModificarUsuario(UsuarioReadDto solicitante, string correoOriginal, UsuarioUpdateDto usuarioModificado)
    {
        Usuario solicitanteDominio = obtenerUsuarioDesdeDto(solicitante);
        Usuario usuarioExistente = _IinMemoryUsuario.ObtenerUsuario(correoOriginal);
        string contrasenaFinal = string.IsNullOrWhiteSpace(usuarioModificado.Contrasena)
            ? usuarioExistente.Contrasena
            : usuarioModificado.Contrasena;
        Usuario usuarioModificadoDominio = new Usuario(
            usuarioModificado.Nombre,
            usuarioModificado.Apellido,
            usuarioModificado.Correo,
            usuarioModificado.FechaNacimiento,
            contrasenaFinal,
            mapearRoles(usuarioModificado.Roles)
        );

        ModificarUsuario(solicitanteDominio, correoOriginal, usuarioModificadoDominio);

        return _usuarioMapeador.ADto(_IinMemoryUsuario.ObtenerUsuario(usuarioModificado.Correo));
    }

    public void EliminarUsuario(Usuario administrador, string correo)
    {
        validarAdministrador(administrador);

        Usuario usuario = _IinMemoryUsuario.ObtenerUsuario(correo);

        _IinMemoryUsuario.BorrarUsuario(usuario);

        _auditoriaService.RegistrarLog(
            administrador,
            $"El usuario {usuario.Correo} fue eliminado"
        );
    }

    public void EliminarUsuario(UsuarioReadDto administrador, string correo)
    {
        EliminarUsuario(obtenerUsuarioDesdeDto(administrador), correo);
    }

    public List<Usuario> ObtenerUsuarios(Usuario administrador)
    {
        validarAdministrador(administrador);

        return _IinMemoryUsuario.ObtenerUsuarios();
    }

    public List<UsuarioReadDto> ObtenerUsuarios(UsuarioReadDto administrador)
    {
        Usuario administradorDominio = obtenerUsuarioDesdeDto(administrador);

        return ObtenerUsuarios(administradorDominio)
            .Select(_usuarioMapeador.ADto)
            .ToList();
    }

    public void CambiarPassword(UsuarioReadDto solicitante, CambiarPasswordDto cambioPassword)
    {
        Usuario solicitanteDominio = obtenerUsuarioDesdeDto(solicitante);
        Usuario usuario = _IinMemoryUsuario.ObtenerUsuario(cambioPassword.Correo);

        bool esAdmin = solicitanteDominio.TieneRol(Rol.Administrador);
        bool esElMismo = solicitanteDominio.Correo == cambioPassword.Correo;

        if (!esAdmin && !esElMismo)
        {
            throw new UnauthorizedAccessException("No tiene permisos para cambiar esta contraseña.");
        }

        if (cambioPassword.NuevaContrasena != cambioPassword.ConfirmacionNuevaContrasena)
        {
            throw new ArgumentException("La confirmación de contraseña no coincide.");
        }

        usuario.Contrasena = cambioPassword.NuevaContrasena;

        _auditoriaService.RegistrarLog(
            solicitanteDominio,
            $"Se modificó la contraseña del usuario: {usuario.Correo}"
        );
    }

    private void validarAdministrador(Usuario usuario)
    {
        if (!usuario.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede realizar esta acción.");
        }
    }

    private bool tienenMismosRoles(List<Rol> roles, List<Rol> otrosRoles)
    {
        return roles.OrderBy(rol => rol).SequenceEqual(otrosRoles.OrderBy(rol => rol));
    }

    public List<Log> ObtenerLogs(Usuario administrador)
    {
        return _auditoriaService.ObtenerLogs(administrador);
    }

    public List<LogReadDto> ObtenerLogs(UsuarioReadDto administrador)
    {
        return _auditoriaService.ObtenerLogs(administrador);
    }

    private Usuario obtenerUsuarioDesdeDto(UsuarioReadDto usuario)
    {
        return _IinMemoryUsuario.ObtenerUsuario(usuario.Correo);
    }

    private List<Rol> mapearRoles(List<string> roles)
    {
        return roles
            .Select(rol => Enum.Parse<Rol>(rol, ignoreCase: true))
            .ToList();
    }
}



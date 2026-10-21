using App.Dominio;
using DTOs;

namespace Services;

public class UsuarioMapeador
{
    private const string PasswordTemporal = "Temporal123#";

    public Usuario ADominio(UsuarioReadDto dto)
    {
        return new Usuario(
            dto.Nombre,
            dto.Apellido,
            dto.Correo,
            dto.FechaNacimiento,
            PasswordTemporal,
            MapearRoles(dto.Roles));
    }

    public UsuarioReadDto ADto(Usuario usuario)
    {
        List<string> roles = new List<string>();

        if (usuario.Roles != null)
        {
            foreach (Rol rol in usuario.Roles)
            {
                roles.Add(rol.ToString());
            }
        }

        return new UsuarioReadDto
        {
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Correo = usuario.Correo,
            FechaNacimiento = usuario.FechaNacimiento,
            Roles = roles
        };
    }

    public List<Rol> MapearRoles(List<string> roles)
    {
        if (roles.Count == 0)
        {
            return new List<Rol> { Rol.Ninguno };
        }

        List<Rol> rolesDominio = new List<Rol>();

        foreach (string rol in roles)
        {
            if (Enum.TryParse(rol, true, out Rol rolDominio))
            {
                rolesDominio.Add(rolDominio);
            }
            else
            {
                rolesDominio.Add(Rol.Ninguno);
            }
        }

        return rolesDominio;
    }
}

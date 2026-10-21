using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemoryUsuario : IinMemoryUsuario
{
    private List<Usuario> _listaUsuarios;

    public InMemoryUsuario()
    {
        _listaUsuarios = new List<Usuario>();
        _listaUsuarios.Add(new Usuario(
            "Admin", 
            "Sist", 
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20), 
            "Admin123#",
            [Rol.Administrador]
        ));
        _listaUsuarios.Add(new Usuario(
            "Editor", 
            "Sist", 
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20), 
            "Editor123#",
            [Rol.Editor]
        ));
        _listaUsuarios.Add(new Usuario(
            "Guest", 
            "Sist", 
            "guest@ort.edu.uy",
            DateTime.Now.AddYears(-20), 
            "Guest123#",
            [Rol.Ninguno]
        ));
        _listaUsuarios.Add(new Usuario(
            "Owner", 
            "Sist", 
            "owner@ort.edu.uy",
            DateTime.Now.AddYears(-20), 
            "Owner123#",
            [Rol.Administrador, Rol.Editor, Rol.Periodista]
        ));
    }
    
    public void AgregarUsuario(Usuario usuario)
    {
        _listaUsuarios.Add(usuario);
    }
    
    public List<Usuario> ObtenerUsuarios()
    {
        return _listaUsuarios.ToList();
    }

    public Usuario ObtenerUsuario(string correo)
    {
        Usuario usuario = _listaUsuarios.FirstOrDefault(u => u.Correo == correo);
        if (usuario == null) throw new Exception("Usuario no encontrado para este correo");
        return usuario;
    }

    public bool ExisteUsuario(Usuario usuario)
    {
        return _listaUsuarios.Any(u => u.Correo == usuario.Correo);
    }

    public void BorrarUsuario(Usuario usuario)
    {
        _listaUsuarios.Remove(usuario);
    }
}

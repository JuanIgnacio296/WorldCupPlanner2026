using App.Dominio;
using DataAccess.Interfaces;
using System.Linq;

namespace DataAccess.Repositories;

public class UsuarioEFRepository : IinMemoryUsuario
{
    private readonly AppDbContext _context;
    
    public UsuarioEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarUsuario(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public List<Usuario> ObtenerUsuarios()
    {
        return _context.Usuarios.ToList();
    }

    public Usuario ObtenerUsuario(string correo)
    {
        return _context.Usuarios.FirstOrDefault(u => u.Correo == correo);
    }

    public bool ExisteUsuario(Usuario usuario)
    {
        return _context.Usuarios.Any(u => u.Correo == usuario.Correo);
    }

    public void BorrarUsuario(Usuario usuario)
    {
        _context.Usuarios.Remove(usuario);
        _context.SaveChanges();
    }
    
}
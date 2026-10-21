using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryUsuario
{
    public void AgregarUsuario(Usuario usuario);
    public List<Usuario> ObtenerUsuarios();
    public Usuario ObtenerUsuario(string correo);
    public bool ExisteUsuario(Usuario usuario);
    public void BorrarUsuario(Usuario usuario);
}

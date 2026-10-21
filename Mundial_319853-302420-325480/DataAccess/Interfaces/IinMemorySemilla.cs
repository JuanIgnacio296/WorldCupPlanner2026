using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemorySemilla
{
    public void AgregarSemilla(Semilla semilla);
    public List<Semilla> ObtenerSemillas();
    public Semilla ObtenerSemilla(int token);
    public bool ExisteSemilla(int token);
    public bool ExisteSemilla(Semilla semilla);
    public Semilla ObtenerSemilla(Semilla semilla);
    public Semilla? ObtenerSemillaNullable(int token);
}

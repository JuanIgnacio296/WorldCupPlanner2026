using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemorySemilla : IinMemorySemilla
{
    private List<Semilla> _semillas;

    public InMemorySemilla()
    {
        _semillas = new List<Semilla>();
    }

    public void AgregarSemilla(Semilla semilla)
    {
        _semillas.Add(semilla);
    }

    public List<Semilla> ObtenerSemillas()
    {
        return _semillas.ToList();
    }

    public Semilla ObtenerSemilla(int token)
    {
        Semilla? semilla = _semillas.FirstOrDefault(s => s.Token == token);
        if (semilla == null) throw new Exception("No existe una");
        return semilla;
    }

    public bool ExisteSemilla(int token)
    {
        return _semillas.Any(s => s.Token == token);
    }

    public bool ExisteSemilla(Semilla semilla)
    {
        return _semillas.Any(s => s.Equals(semilla));
    }

    public Semilla ObtenerSemilla(Semilla semilla)
    {
        return _semillas.FirstOrDefault(s => s.Equals(semilla));
    }
    
    public Semilla? ObtenerSemillaNullable(int token)
    {
        if (!ExisteSemilla(token))
        {
            return null;
        }

        return ObtenerSemilla(token);
    }
}

using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemoryEstadio : IinMemoryEstadio
{
    private List<Estadio> _listaEstadios;

    public InMemoryEstadio()
    {
        _listaEstadios = new List<Estadio>();
    }

    public void AgregarEstadio(Estadio estadio)
    {
        _listaEstadios.Add(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _listaEstadios.ToList();
    }

    public Estadio ObtenerEstadio(string nombreEstadio)
    {
        Estadio? estadio = _listaEstadios.FirstOrDefault(e=> e.Nombre == nombreEstadio);
        if (estadio == null) throw new Exception("No existe un estadio con ese nombre.");
        return estadio;
    }

    public bool ExisteEstadio(string nombreEstadio)
    {
        return _listaEstadios.Any(e => e.Nombre == nombreEstadio);
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _listaEstadios.Remove(estadio);
    }
    
}

using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemoryPartido : IinMemoryPartido
{
    private List<Partido> _listaPartidos;

    public InMemoryPartido()
    {
        _listaPartidos = new List<Partido>();
    }

    public void AgregarPartido(Partido partido)
    {
        _listaPartidos.Add(partido);
    }

    public List<Partido> ObtenerPartidos()
    {
        return _listaPartidos.ToList();
    }

    public Partido ObtenerPartido(int id)
    {
        Partido? partido = _listaPartidos.FirstOrDefault(p => p.Id == id);
        if (partido == null) throw new Exception("No existe partido para este id");
        return partido; 
    }

    public void GuardarCambios()
    {
    }

    public void LimpiarPartidos()
    {
        _listaPartidos.Clear();
    }
}

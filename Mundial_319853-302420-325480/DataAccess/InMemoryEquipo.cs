using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemoryEquipo : IinMemoryEquipo
{
    private List<Equipo> _listaEquipos;

    public InMemoryEquipo()
    {
        _listaEquipos = new List<Equipo>();
    }

    public void AgregarEquipo(Equipo equipo)
    {
        _listaEquipos.Add(equipo);
    }

    public List<Equipo> ObtenerEquipos()
    {
        return _listaEquipos;
    }

    public void EliminarEquipo(Equipo equipo)
    {
        _listaEquipos.Remove(equipo);
    }

    public bool ExisteEquipo(string nombreEquipo)
    {
       return _listaEquipos.Any(equipo => equipo.Nombre == nombreEquipo);
    }

    public Equipo ObtenerEquipo(string nombreEquipo)
    {
        Equipo? equipo = _listaEquipos.FirstOrDefault(e => e.Nombre == nombreEquipo);

        if (equipo == null)
        {
            throw new InvalidOperationException("No existe un equipo con ese nombre.");
        }

        return equipo;
    }

    public bool EstaEnUso(string nombreEquipo)
    {
        return false;
    }
}



using App.Dominio;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class EquipoEFRepository : IinMemoryEquipo
{
    private readonly AppDbContext _context;

    public EquipoEFRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public void AgregarEquipo(Equipo equipo)
    {
        _context.Equipos.Add(equipo);
        _context.SaveChanges();
    }

    public List<Equipo> ObtenerEquipos()
    {
        return _context.Equipos.ToList();
    }

    public void EliminarEquipo(Equipo equipo)
    {
        _context.Equipos.Remove(equipo);
        _context.SaveChanges();
    }

    public bool ExisteEquipo(string nombreEquipo)
    {
        return _context.Equipos.Any(e => e.Nombre == nombreEquipo);
    }

    public Equipo ObtenerEquipo(string nombreEquipo)
    {
        return _context.Equipos.FirstOrDefault(e => e.Nombre == nombreEquipo);
    }

    public bool EstaEnUso(string nombreEquipo)
    {
        bool enGrupo = _context.Grupos.Any(g => g.Equipos.Any(e => e.Nombre == nombreEquipo));
        bool enPartido = _context.Partidos.Any(p =>
            EF.Property<string>(p, "EquipoLocalNombre") == nombreEquipo ||
            EF.Property<string>(p, "EquipoVisitanteNombre") == nombreEquipo);

        return enGrupo || enPartido;
    }
}
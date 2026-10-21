using App.Dominio;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PartidoEFRepository : IinMemoryPartido
{
    private readonly AppDbContext _context;

    public PartidoEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarPartido(Partido partido)
    {
        _context.DesadjuntarDuplicados(partido.Estadio);
        _context.DesadjuntarDuplicados(partido.EquipoLocal);
        _context.DesadjuntarDuplicados(partido.EquipoVisitante);

        _context.Partidos.Add(partido);
        _context.MarcarExistente(partido.Estadio);
        _context.MarcarExistente(partido.EquipoLocal);
        _context.MarcarExistente(partido.EquipoVisitante);
        _context.SaveChanges();
    }

    public List<Partido> ObtenerPartidos()
    {
        return _context.Partidos
            .Include(p => p.Estadio)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .ToList();
    }

    public Partido ObtenerPartido(int id)
    {
        return _context.Partidos
            .Include(p => p.Estadio)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .FirstOrDefault(p => p.Id == id);
    }

    public void GuardarCambios()
    {
        _context.SaveChanges();
    }

    public void LimpiarPartidos()
    {
        _context.RemoveRange(_context.Partidos);
        _context.SaveChanges();
    }
}

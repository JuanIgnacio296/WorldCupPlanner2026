using App.Dominio;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class EstadioEFRepository : IinMemoryEstadio
{
    private readonly AppDbContext _context;
    
    public EstadioEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarEstadio(Estadio estadio)
    {
        _context.Estadios.Add(estadio);
        _context.SaveChanges();
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _context.Estadios.ToList();
    }
    

    public Estadio ObtenerEstadio(string nombreEstadio)
    {
        return _context.Estadios
            .Include(e => e.FechasOcupadas)
            .FirstOrDefault(e => e.Nombre == nombreEstadio);
    }

    public bool ExisteEstadio(string nombreEstadio)
    {
        return _context.Estadios.Any(e => e.Nombre == nombreEstadio);
    }

    public void EliminarEstadio(Estadio estadio)
    {
        if (_context.Estadios.Contains(estadio))
        {
            _context.Estadios.Remove(estadio);
            _context.SaveChanges();
        }
    }
}

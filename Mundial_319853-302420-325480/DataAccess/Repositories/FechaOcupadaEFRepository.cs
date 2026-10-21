using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class FechaOcupadaEFRepository : IinMemoryFechaOcupada
{
    private AppDbContext _context;

    public FechaOcupadaEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarFecha(FechaOcupada fecha)
    {
        _context.FechasOcupadas.Add(fecha);
        _context.MarcarExistente(fecha.Estadio);
        _context.SaveChanges();
    }

    public bool EsFechaOcupada(FechaOcupada fecha)
    {
        return _context.FechasOcupadas.Any(f => f.Estadio.Nombre == fecha.Estadio.Nombre && f.Fecha == fecha.Fecha);
    }

    public List<FechaOcupada> ObtenerFechasOcupadasPorEstadio(Estadio estadio)
    {
        return _context.FechasOcupadas.Where(f => f.Estadio.Nombre == estadio.Nombre).ToList();
    }

    public void EliminarFecha(FechaOcupada fecha)
    {
        FechaOcupada? existente =
            _context.FechasOcupadas.FirstOrDefault(f => f.Estadio.Nombre == fecha.Estadio.Nombre && f.Fecha == fecha.Fecha);

        if (existente != null)
        {
            _context.FechasOcupadas.Remove(existente);
            _context.SaveChanges();
        }
    }

    public void EliminarFechasPorEstadio(Estadio estadio)
    {
        List<FechaOcupada> fechas = _context.FechasOcupadas.Where(f => f.Estadio.Nombre == estadio.Nombre).ToList();
        _context.FechasOcupadas.RemoveRange(fechas);
        _context.SaveChanges();
    }
}
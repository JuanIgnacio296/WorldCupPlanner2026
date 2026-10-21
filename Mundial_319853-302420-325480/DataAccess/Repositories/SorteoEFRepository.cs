using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class SorteoEFRepository : IinMemorySorteo
{
    private readonly AppDbContext _context;

    public SorteoEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public int GenerarToken()
    {
        int maximo = _context.Sorteos.Any() ? _context.Sorteos.Max(s => s.Token) : 0;
        return maximo + 1;
    }

    public void AgregarSorteo(int token, Sorteo sorteo)
    {
        foreach (Equipo equipo in sorteo.Equipos)
        {
            _context.DesadjuntarDuplicados(equipo);
        }
        foreach (Estadio estadio in sorteo.Estadios)
        {
            _context.DesadjuntarDuplicados(estadio);
        }

        _context.Sorteos.Add(sorteo);
        _context.MarcarExistentes(sorteo.Equipos);
        _context.MarcarExistentes(sorteo.Estadios);
        _context.SaveChanges();
    }

    public Sorteo ObtenerSorteoPorToken(int token)
    {
        Sorteo? sorteo = _context.Sorteos.FirstOrDefault(s => s.Token == token);
        if (sorteo == null) throw new Exception("No existe un sorteo para ese token.");
        return sorteo;
    }

    public bool ExisteSorteoConSemilla(Semilla semilla){
        return _context.Sorteos.Any(s =>
            s.Semilla.SemillaFixture == semilla.SemillaFixture
            && s.Semilla.SemillaCompletar == semilla.SemillaCompletar
            && s.Semilla.SemillaCrucesFase == semilla.SemillaCrucesFase
            && s.Semilla.SemillaSimulacion == semilla.SemillaSimulacion);
    }
}
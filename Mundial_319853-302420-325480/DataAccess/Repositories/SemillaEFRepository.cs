using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class SemillaEFRepository : IinMemorySemilla
{
    private readonly AppDbContext _context;
    
    public SemillaEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarSemilla(Semilla semilla)
    {
        _context.Semillas.Add(semilla);
        _context.SaveChanges();   
    }

    public List<Semilla> ObtenerSemillas()
    {
        return _context.Semillas.ToList();
    }

    public Semilla ObtenerSemilla(int token)
    {
        Semilla? semilla = _context.Semillas.FirstOrDefault(s => s.Token == token);
        if (semilla == null) throw new Exception("No existe una semilla con ese token");
        return semilla;
    }

    public bool ExisteSemilla(int token)
    {
        return _context.Semillas.Any(s => s.Token == token);
    }

    public bool ExisteSemilla(Semilla semilla)
    {
        return _context.Semillas.Any(s =>
            s.SemillaFixture == semilla.SemillaFixture
            && s.SemillaCompletar == semilla.SemillaCompletar
            && s.SemillaCrucesFase == semilla.SemillaCrucesFase
            && s.SemillaSimulacion == semilla.SemillaSimulacion);
    }

    public Semilla ObtenerSemilla(Semilla semilla)
    {
        return _context.Semillas.FirstOrDefault(s =>
            s.SemillaFixture == semilla.SemillaFixture
            && s.SemillaCompletar == semilla.SemillaCompletar
            && s.SemillaCrucesFase == semilla.SemillaCrucesFase
            && s.SemillaSimulacion == semilla.SemillaSimulacion);
    }

    public Semilla? ObtenerSemillaNullable(int token)
    {
        return _context.Semillas.FirstOrDefault(s => s.Token == token); 
    }
    
}
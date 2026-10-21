using App.Dominio;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class FixtureEFRepository : IinMemoryFixture
{
    private readonly AppDbContext _context;
    
    public FixtureEFRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public void AgregarFixture(int token, Fixture fixture)
    {
        foreach (Estadio estadio in fixture.Estadios)
        {
            _context.DesadjuntarDuplicados(estadio);
        }

        _context.Fixtures.Add(fixture);
        _context.MarcarTipoExistente<Equipo>();
        _context.MarcarTipoExistente<Estadio>();
        _context.SaveChanges();
    }

    public Fixture ObtenerFixturePorToken(int token)
    {
        Fixture? fixture = _context.Fixtures
            .Include(f => f.Semilla)
            .Include(f => f.Estadios)
            .FirstOrDefault(f => f.Token == token);
        if (fixture == null) throw new NullReferenceException("No existe un fixture para ese token.");

        ReconstruirGruposYPartidos(fixture);
        return fixture;
    }

    private void ReconstruirGruposYPartidos(Fixture fixture)
    {
        fixture.Grupos = _context.Grupos
            .Include(g => g.Equipos)
            .Where(g => g.Token == fixture.Token)
            .ToList();

        List<Partido> partidos = _context.Partidos
            .Include(p => p.Estadio)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Where(p => p.Token == fixture.Token)
            .ToList();
        fixture.CargarPartidos(partidos);
    }

    public int ObtenerTokenPorSemilla(int tokenSemilla)
    {
        Fixture? fixture = _context.Fixtures
            .Include(f => f.Semilla)
            .FirstOrDefault(f => f.Semilla.Token == tokenSemilla);

        if (fixture == null) throw new NullReferenceException("No existe un token para esa semilla.");
        
        return fixture.Token;
    }

    public bool ExisteFixture(int token)
    {
        return _context.Fixtures.Any(f => f.Token == token);    
    }

    public List<Fixture> ObtenerFixtures()
    {
        List<Fixture> fixtures = _context.Fixtures
            .Include(f => f.Semilla)
            .Include(f => f.Estadios)
            .ToList();

        foreach (Fixture fixture in fixtures)
        {
            ReconstruirGruposYPartidos(fixture);
        }

        return fixtures;
    }
}
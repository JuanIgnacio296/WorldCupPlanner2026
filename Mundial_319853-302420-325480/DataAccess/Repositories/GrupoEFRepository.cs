using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class GrupoEFRepository : IinMemoryGrupo
{
    private readonly AppDbContext _context;
    
    public GrupoEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarGrupo(Grupo grupo)
    {
        foreach (Equipo equipo in grupo.Equipos)
        {
            _context.DesadjuntarDuplicados(equipo);
        }

        _context.Grupos.Add(grupo);
        _context.MarcarExistentes(grupo.Equipos);
        _context.SaveChanges();
    }

    public List<Grupo> ObtenerGrupos()
    {
        return _context.Grupos.ToList();
    }

    public List<Grupo> ObtenerGruposPorToken(int token)
    {
        return _context.Grupos.Where(g => g.Token == token).ToList();
    }

    public Grupo ObtenerGrupo(char id, int token)
    {
        return _context.Grupos.FirstOrDefault(g => g.Id == id && g.Token == token);
    }

    public bool ExisteGrupo(char id, int token)
    {
        return _context.Grupos.Any(g => g.Id == id && g.Token == token);
    }
}
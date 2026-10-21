using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryGrupo
{
    public void AgregarGrupo(Grupo grupo);  
    public List<Grupo> ObtenerGrupos();
    public List<Grupo> ObtenerGruposPorToken(int token);
    public Grupo ObtenerGrupo(char id, int token);
    public bool ExisteGrupo(char id, int token);
}
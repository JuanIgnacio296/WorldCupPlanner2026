using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryPartido
{
    public void AgregarPartido(Partido partido);
    public List<Partido> ObtenerPartidos();
    public Partido ObtenerPartido(int id);
    public void GuardarCambios();
    public void LimpiarPartidos();
}

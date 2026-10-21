using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryEstadio
{
    public void AgregarEstadio(Estadio estadio);
    public List<Estadio> ObtenerEstadios();
    public Estadio ObtenerEstadio(string nombreEstadio);
    public bool ExisteEstadio(string nombreEstadio);
    public void EliminarEstadio(Estadio estadio);
}

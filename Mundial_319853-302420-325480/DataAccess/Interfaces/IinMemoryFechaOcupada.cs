using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryFechaOcupada
{
    public void AgregarFecha(FechaOcupada fecha);
    public bool EsFechaOcupada(FechaOcupada fecha);
    public List<FechaOcupada> ObtenerFechasOcupadasPorEstadio(Estadio estadio);
    public void EliminarFecha(FechaOcupada fecha);
    public void EliminarFechasPorEstadio(Estadio estadio);
}
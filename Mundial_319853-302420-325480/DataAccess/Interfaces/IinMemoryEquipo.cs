using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryEquipo
{
    public void AgregarEquipo(Equipo equipo);
    public List<Equipo> ObtenerEquipos();
    public void EliminarEquipo(Equipo equipo);
    public bool ExisteEquipo(string nombreEquipo);
    public Equipo ObtenerEquipo(string nombreEquipo);
    public bool EstaEnUso(string nombreEquipo);
}
using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemorySorteo
{
    public int GenerarToken();
    public void AgregarSorteo(int token, Sorteo sorteo);
    public Sorteo ObtenerSorteoPorToken(int token);
    public bool ExisteSorteoConSemilla(Semilla semilla);
}

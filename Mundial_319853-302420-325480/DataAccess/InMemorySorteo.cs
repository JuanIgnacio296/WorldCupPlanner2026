using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemorySorteo : IinMemorySorteo
{
    private Dictionary<int, Sorteo> _sorteosPorToken;
    private int _proximoToken;

    public InMemorySorteo()
    {
        _sorteosPorToken = new Dictionary<int, Sorteo>();
        _proximoToken = 1;
    }

    public int GenerarToken()
    {
        int token = _proximoToken;
        _proximoToken++;

        return token;
    }

    public void AgregarSorteo(int token, Sorteo sorteo)
    {
        _sorteosPorToken.Add(token, sorteo);
    }

    public Sorteo ObtenerSorteoPorToken(int token)
    {
        if (!_sorteosPorToken.ContainsKey(token))
        {
            throw new InvalidOperationException("No existe un sorteo con ese token.");
        }

        return _sorteosPorToken[token];
    }

    public bool ExisteSorteoConSemilla(Semilla semilla)
    {
        return _sorteosPorToken.Values.Any(sorteo => sorteo.Semilla.Equals(semilla));
    }
}

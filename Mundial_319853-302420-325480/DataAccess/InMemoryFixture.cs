using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemoryFixture : IinMemoryFixture
{
    private Dictionary<int, Fixture> _fixturesPorToken;

    public InMemoryFixture()
    {
        _fixturesPorToken = new Dictionary<int, Fixture>();
    }

    public void AgregarFixture(int token, Fixture fixture)
    {
        _fixturesPorToken.Add(token, fixture);
    }

    public Fixture ObtenerFixturePorToken(int token)
    {
        if (!_fixturesPorToken.ContainsKey(token))
        {
            throw new InvalidOperationException("No existe un fixture para ese token.");
        }

        return _fixturesPorToken[token];
    }

    public int ObtenerTokenPorSemilla(int tokenSemilla)
    {
        foreach (KeyValuePair<int, Fixture> fixturePorToken in _fixturesPorToken.Reverse())
        {
            if (fixturePorToken.Value.Semilla.Token == tokenSemilla)
            {
                return fixturePorToken.Key;
            }
        }

        throw new InvalidOperationException("No existe un fixture para esa semilla.");
    }

    public bool ExisteFixture(int token)
    {
        return _fixturesPorToken.ContainsKey(token);
    }

    public List<Fixture> ObtenerFixtures()
    {
        return _fixturesPorToken.Values.ToList();
    }

}

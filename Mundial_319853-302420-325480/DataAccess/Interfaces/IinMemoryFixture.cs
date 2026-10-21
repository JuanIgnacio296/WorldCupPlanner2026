using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryFixture
{
    public void AgregarFixture(int token, Fixture fixture);
    public Fixture ObtenerFixturePorToken(int token);
    public int ObtenerTokenPorSemilla(int tokenSemilla);
    public bool ExisteFixture(int token);
    public List<Fixture> ObtenerFixtures();
}

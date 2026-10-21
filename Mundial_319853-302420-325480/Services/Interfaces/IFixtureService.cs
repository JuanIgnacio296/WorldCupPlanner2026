using App.Dominio;
using DTOs;
namespace Services.Interfaces;

public interface IFixtureService
{
    void GenerarFaseDeGrupos();
    void GenerarDieciseisavos();
    void GenerarOctavos();
    void GenerarCuartos();
    void GenerarSemifinal();
    void GenerarTercerYCuartoPuesto();
    void GenerarFinal();
    void GuardarFixture(int token, Fixture fixture);
    Fixture ObtenerFixturePorToken(int token);
    FixtureReadDto ObtenerFixturePorTokenDto(int token);
    Fixture ObtenerFixturePorTokenSemilla(int tokenSemilla);
    FixtureReadDto ObtenerFixturePorTokenSemillaDto(int tokenSemilla);
    List<Partido> ObtenerPartidos();
    List<PartidoReadDto> ObtenerPartidosListado();
    List<Partido> ObtenerPartidosPorFase(string fase);
    List<PartidoReadDto> ObtenerPartidosPorFaseListado(string fase);
    Fixture ObtenerFixture();
    FixtureReadDto ObtenerFixtureDto();
    int ObtenerTokenActual();
    string MostrarCruces();
    void SimularFecha(DateTime _fecha, Usuario user, Random rnd);
    FixtureReadDto SimularFecha(DateTime _fecha, UsuarioReadDto user, Random rnd);
    void SimularJornada(int _jornada, Usuario user, Random rnd);
    FixtureReadDto SimularJornada(int _jornada, UsuarioReadDto user, Random rnd);
    void SimularFase(string _fase, Usuario user, Random rnd);
    FixtureReadDto SimularFase(string _fase, UsuarioReadDto user, Random rnd);
    FixtureReadDto SimularFecha(DateTime _fecha, UsuarioReadDto user);
    FixtureReadDto SimularJornada(int _jornada, UsuarioReadDto user);
    FixtureReadDto SimularFase(string _fase, UsuarioReadDto user);
}

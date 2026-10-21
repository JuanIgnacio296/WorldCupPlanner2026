using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface ISorteoService
{
    Sorteo CrearSorteo(Usuario editor, List<Equipo> equipos, List<Estadio> estadios, int token);
    SorteoReadDto CrearSorteo(UsuarioReadDto editor, List<EquipoReadDto> equipos, List<EstadioReadDto> estadios, int token);
    Sorteo CrearSorteo(Usuario editor, List<Equipo> equipos, List<Estadio> estadios, Semilla semilla);
    Sorteo ObtenerSorteoPorToken(int token);
    SorteoReadDto ObtenerSorteoPorTokenDto(int token);
    List<Grupo> SortearEquipos(Usuario editor, int token);
    List<GrupoReadDto> SortearEquipos(UsuarioReadDto editor, int token);
    Fixture CrearFixture(Usuario editor, int token, bool usaMotorProbabilistico);
    FixtureReadDto CrearFixture(UsuarioReadDto editor, int token, bool usaMotorProbabilistico);
    Fixture ObtenerFixturePorToken(Usuario editor, int token);
    FixtureReadDto ObtenerFixturePorToken(UsuarioReadDto editor, int token);
}

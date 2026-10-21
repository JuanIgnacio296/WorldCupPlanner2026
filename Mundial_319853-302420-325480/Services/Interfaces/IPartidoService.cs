using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IPartidoService
{
    Partido ObtenerPartidoPorId(int id);
    PartidoReadDto ObtenerPartidoPorIdDto(int id);
    List<Partido> ObtenerPartidos();
    List<PartidoReadDto> ObtenerPartidosListado();
    void EditarFecha(Usuario admin, int id, DateTime nuevaFecha);
    PartidoReadDto EditarFecha(UsuarioReadDto admin, int id, DateTime nuevaFecha);
    void EditarEstadio(Usuario admin, int id, Estadio nuevoEstadio);
    PartidoReadDto EditarEstadio(UsuarioReadDto admin, int id, EstadioReadDto nuevoEstadio);
    void CargarResultado(Usuario admin, int id, int golesLocal, int golesVisitante);
    PartidoReadDto CargarResultado(UsuarioReadDto admin, int id, int golesLocal, int golesVisitante);
    PartidoReadDto CargarTarjetas(UsuarioReadDto editor, int id, int amarillasLocal, int rojasLocal, int amarillasVisitante, int rojasVisitante);
    void SimularPartido(Usuario editor, int id, Random rnd, bool usaMotorProbabilistico);
    PartidoReadDto SimularPartido(UsuarioReadDto editor, int id, Random rnd, bool usaMotorProbabilistico);
    void SimularPenales(Usuario editor, int id, Random rnd);
    PartidoReadDto SimularPenales(UsuarioReadDto editor, int id, Random rnd);
    PartidoReadDto SimularPenales(UsuarioReadDto editor, int id);
    void CargarPenales(Usuario editor, int id, int penalesLocal, int penalesVisitante);
    PartidoReadDto CargarPenales(UsuarioReadDto editor, int id, int penalesLocal, int penalesVisitante);
    void AgregarPartido(Partido partido);
    void LimpiarPartidos();
}

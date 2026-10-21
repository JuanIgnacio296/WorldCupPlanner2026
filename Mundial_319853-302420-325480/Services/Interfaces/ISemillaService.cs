using App.Dominio;
using DTOs;
namespace Services.Interfaces;

public interface ISemillaService
{
    Semilla ObtenerSemilla(int token);
    SemillaReadDto ObtenerSemillaDto(int token);
    void AgregarSemilla(Semilla semilla);
    SemillaReadDto AgregarSemilla(SemillaCreateDto semilla);
    List<Semilla> ObtenerSemillas();
    List<SemillaReadDto> ObtenerSemillasListado();
    int ObtenerSiguienteTokenDisponible();
}

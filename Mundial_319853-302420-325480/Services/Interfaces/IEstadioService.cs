using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IEstadioService
{
    void CrearEstadio(Usuario administrador, Estadio nuevoEstadio);
    EstadioReadDto CrearEstadio(UsuarioReadDto administrador, EstadioCreateDto nuevoEstadio);
    List<Estadio> ObtenerEstadios();
    List<EstadioListItemDto> ObtenerEstadiosListado();
    List<EstadioReadDto> ObtenerEstadiosDetalle();
    void EliminarEstadio(Usuario administrador, string nombre);
    void EliminarEstadio(UsuarioReadDto administrador, string nombre);
    void ModificarEstadio(Usuario administrador, string nombreOriginal, Estadio estadioModificado);
    EstadioReadDto ModificarEstadio(UsuarioReadDto administrador, string nombreOriginal, EstadioUpdateDto estadioModificado);
    void CompletarEstadiosMinimosParaFixture(Usuario administrador);
    void CompletarEstadiosMinimosParaFixture(UsuarioReadDto administrador);
    
}

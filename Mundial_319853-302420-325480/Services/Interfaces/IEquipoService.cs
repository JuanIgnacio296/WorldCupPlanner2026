using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IEquipoService
{
    void CrearEquipo(Usuario administrador, Equipo nuevoEquipo);
    EquipoReadDto CrearEquipo(UsuarioReadDto administrador, EquipoCreateDto nuevoEquipo);
    List<Equipo> ObtenerEquipos();
    EquipoReadDto ObtenerEquipo(string nombreEquipo);
    List<EquipoListItemDto> ObtenerEquiposListado();
    List<EquipoReadDto> ObtenerEquiposDetalle();
    List<RankingEquipoDto> ObtenerRankingEquipos();
    void EliminarEquipo(Usuario administrador, string nombreEquipo);
    void EliminarEquipo(UsuarioReadDto administrador, string nombreEquipo);
    void ModificarEquipo(Usuario administrador, string nombreOriginal, Equipo equipoModificado);
    EquipoReadDto ModificarEquipo(UsuarioReadDto administrador, string nombreOriginal, EquipoUpdateDto equipoModificado);
    void CompletarEquiposFaltantes(Usuario administrador);
    void CompletarEquiposFaltantes(UsuarioReadDto administrador);
}



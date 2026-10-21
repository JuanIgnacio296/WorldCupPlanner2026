using App.Dominio;
using DTOs;

namespace Services;

public class EquipoMapeador
{
    public EquipoReadDto ADto(Equipo equipo)
    {
        return new EquipoReadDto
        {
            Nombre = equipo.Nombre,
            Confederacion = equipo.Confederacion,
            RankingFifa = equipo.RankingFifa,
            BanderaBase64 = equipo.BanderaBase64,
            Puntos = equipo.Puntos,
            GolesAFavor = equipo.GolesAFavor,
            GolesEnContra = equipo.GolesEnContra
        };
    }

    public Equipo ADominio(EquipoReadDto dto)
    {
        return new Equipo(
            dto.Nombre,
            dto.Confederacion,
            dto.RankingFifa,
            dto.BanderaBase64);
    }
}

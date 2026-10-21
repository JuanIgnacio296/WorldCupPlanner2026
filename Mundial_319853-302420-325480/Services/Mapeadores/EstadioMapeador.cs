using App.Dominio;
using DTOs;

namespace Services;

public class EstadioMapeador
{
    public EstadioReadDto ADto(Estadio estadio)
    {
        return new EstadioReadDto
        {
            Nombre = estadio.Nombre,
            Ciudad = estadio.Ciudad,
            Capacidad = estadio.Capacidad,
            Descripcion = estadio.Descripcion
        };
    }

    public Estadio ADominio(EstadioReadDto dto)
    {
        return new Estadio(
            dto.Nombre,
            dto.Ciudad,
            dto.Capacidad,
            dto.Descripcion);
    }
}

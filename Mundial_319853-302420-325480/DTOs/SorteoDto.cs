namespace DTOs;

public class SorteoReadDto
{
    public int Token { get; set; }
    public SemillaReadDto Semilla { get; set; } = new();
    public List<EquipoReadDto> Equipos { get; set; } = [];
    public List<EstadioReadDto> Estadios { get; set; } = [];
    public List<GrupoReadDto> Grupos { get; set; } = [];
}

public class GrupoReadDto
{
    public char? Id { get; set; }
    public List<EquipoReadDto> Equipos { get; set; } = [];
    public List<PartidoReadDto> Partidos { get; set; } = [];
}

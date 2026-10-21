namespace DTOs;

public class FixtureReadDto
{
    public int TokenActual { get; set; }
    public int PartidosJugados { get; set; }
    public bool UsaMotorProbabilistico { get; set; }
    public SemillaReadDto Semilla { get; set; } = new();
    public List<GrupoReadDto> Grupos { get; set; } = [];
    public List<PartidoReadDto> Partidos { get; set; } = [];
}

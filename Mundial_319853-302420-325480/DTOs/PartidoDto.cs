namespace DTOs;

public class PartidoReadDto
{
    public int Id { get; set; }
    public DateTime? Fecha { get; set; }
    public string Horario { get; set; } = "";
    public EstadioReadDto Estadio { get; set; } = new();
    public EquipoReadDto EquipoLocal { get; set; } = new();
    public EquipoReadDto EquipoVisitante { get; set; } = new();
    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }
    public string Fase { get; set; } = "";
    public int? Jornada { get; set; }
    public int? PenalesLocal { get; set; }
    public int? PenalesVisitante { get; set; }
    public string Vencedor { get; set; } = "";
    public int TarjetasAmarillasLocal { get; set; }
    public int TarjetasRojasLocal { get; set; }
    public int TarjetasAmarillasVisitante { get; set; }
    public int TarjetasRojasVisitante { get; set; }
}

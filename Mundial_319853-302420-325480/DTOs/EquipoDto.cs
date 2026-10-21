namespace DTOs;

public class EquipoCreateDto
{
    public string Nombre { get; set; } = "";
    public string Confederacion { get; set; } = "";
    public int RankingFifa { get; set; }
    public string BanderaBase64 { get; set; } = "";
}

public class EquipoUpdateDto
{
    public string Nombre { get; set; } = "";
    public string Confederacion { get; set; } = "";
    public int RankingFifa { get; set; }
    public string BanderaBase64 { get; set; } = "";
}

public class EquipoReadDto
{
    public string Nombre { get; set; } = "";
    public string Confederacion { get; set; } = "";
    public int RankingFifa { get; set; }
    public string BanderaBase64 { get; set; } = "";
    public int Puntos { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }
}

public class EquipoListItemDto
{
    public string Nombre { get; set; } = "";
    public string Confederacion { get; set; } = "";
    public int RankingFifa { get; set; }
    public bool TieneBandera { get; set; }
}

public class RankingEquipoDto
{
    public string Nombre { get; set; } = "";
    public string Confederacion { get; set; } = "";
    public int RankingFifa { get; set; }
}

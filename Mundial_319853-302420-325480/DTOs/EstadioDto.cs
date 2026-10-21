namespace DTOs;

public class EstadioCreateDto
{
    public string Nombre { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Capacidad { get; set; }
    public string Descripcion { get; set; } = "";
}

public class EstadioUpdateDto
{
    public string Nombre { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Capacidad { get; set; }
    public string Descripcion { get; set; } = "";
}

public class EstadioReadDto
{
    public string Nombre { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Capacidad { get; set; }
    public string Descripcion { get; set; } = "";
}

public class EstadioListItemDto
{
    public string Nombre { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Capacidad { get; set; }
}

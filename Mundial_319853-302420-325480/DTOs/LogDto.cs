namespace DTOs;

public class LogReadDto
{
    public int Id { get; set; }
    public UsuarioReadDto Usuario { get; set; } = new();
    public string Accion { get; set; } = "";
    public DateTime Fecha { get; set; }
}

namespace DTOs;

public class NotificacionDto
{
    public int Id { get; set; }
    public string UsuarioCorreo { get; set; }
    public string Mensaje { get; set; }
    public bool Leida { get; set; }
    public DateTime Fecha { get; set; }
}
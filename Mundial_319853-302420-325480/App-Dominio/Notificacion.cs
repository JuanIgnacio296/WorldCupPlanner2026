namespace App.Dominio;

public class Notificacion
{
    private int _id;
    private string _usuarioCorreo;
    private string _mensaje;
    private bool _leida;
    private DateTime _fecha;

    private Notificacion() { }

    public Notificacion(string usuarioCorreo, string mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje)) 
            throw new ArgumentException("El mensaje no puede ser vacío.");
            
        UsuarioCorreo = usuarioCorreo;
        Mensaje = mensaje;
        Leida = false;
        Fecha = DateTime.Now;
    }
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string UsuarioCorreo
    {
        get => _usuarioCorreo;
        private set => _usuarioCorreo = value;
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => _mensaje = value;
    }

    public bool Leida
    {
        get => _leida;
        set => _leida = value;
    }

    public DateTime Fecha
    {
        get => _fecha;
        private set => _fecha = value;
    }

    public void MarcarComoLeida()
    {
        Leida = true;
    }
}
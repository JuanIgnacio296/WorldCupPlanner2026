using App.Dominio.Exceptions;

namespace App.Dominio;

public class Log
{
    private const int LargoMaximoAccion = 200;

    private int _id;
    private Usuario _usuario = null!;
    private string _accion = string.Empty;
    private DateTime _fecha;

    
    public Log(Usuario unUsuario, string unaAccion, DateTime unaFecha)
    {
        Usuario = unUsuario;
        Accion = unaAccion;
        Fecha = unaFecha;
    }

    public Log()
    {
    }


    public int Id
    {
        get => _id;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentosDeLogNoValidos("El Id debe ser mayor a cero");
            }

            _id = value;
        }
    }

    public Usuario Usuario
    {
        get => _usuario;
        set
        {
            if (value == null)
            {
                throw new ArgumentosDeLogNoValidos("El usuario no puede ser nulo");
            }

            _usuario = value;
        }
    }

    public string Accion
    {
        get => _accion;
        set
        {
            ValidarAccion(value);
            _accion = value;
        }
    }

    public DateTime Fecha
    {
        get => _fecha;
        set
        {
            if (value > DateTime.Now)
            {
                throw new ArgumentosDeLogNoValidos("La fecha no puede ser en el futuro");
            }

            _fecha = value;
        }
    }

    private static void ValidarAccion(string accion)
    {
        if (string.IsNullOrWhiteSpace(accion))
        {
            throw new ArgumentosDeLogNoValidos("La acción no puede ser nula ni vacía");
        }

        if (accion.Length > LargoMaximoAccion)
        {
            throw new ArgumentosDeLogNoValidos("La acción no puede exceder los 200 caracteres");
        }
    }
}





using App.Dominio.Exceptions;

namespace App.Dominio;

public class EstadioValidacion
{
    
    private const int CapacidadMinima = 20000;
    private const int LargoMaximoNombre = 80;
    private const int LargoMaximoCiudad = 60;
    private const int LargoMaximoDescripcion = 400;
    
    public void ValidarNombre(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            throw new ArgumentosDeEstadioNoValidos("Nombre no puede ser vacío");
        }

        if (nombre.Length > LargoMaximoNombre)
        {
            throw new ArgumentosDeEstadioNoValidos("Nombre no puede tener mas de 80 caracteres");
        }
    }

    public void ValidarCiudad(string ciudad)
    {
        if (string.IsNullOrEmpty(ciudad))
        {
            throw new ArgumentosDeEstadioNoValidos("Ciudad no puede ser vacío");
        }

        if (ciudad.Length > LargoMaximoCiudad)
        {
            throw new ArgumentosDeEstadioNoValidos("Ciudad no puede tener mas de 60 caracteres");
        }
    }

    public void ValidarDescripcion(string descripcion)
    {
        if (descripcion.Length > LargoMaximoDescripcion)
        {
            throw new ArgumentosDeEstadioNoValidos("Descripcion no puede tener mas de 400 caracteres");
        }
    }

    public void ValidarCapacidad(int capacidad)
    {
        if (capacidad < CapacidadMinima)
        {
            throw new ArgumentosDeEstadioNoValidos("Capacidad no puede ser menor a 20.000 personas");
        }
    }
}
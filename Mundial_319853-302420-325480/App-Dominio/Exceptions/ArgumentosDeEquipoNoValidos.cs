namespace App.Dominio.Exceptions;

using System;

public class ArgumentosDeEquipoNoValidos : ArgumentException
{
    public ArgumentosDeEquipoNoValidos() : base("Los argumentos del equipo no son válidos.") { }

    public ArgumentosDeEquipoNoValidos(string message) : base(message) { }

    public ArgumentosDeEquipoNoValidos(string message, Exception innerException) : base(message, innerException) { }
}



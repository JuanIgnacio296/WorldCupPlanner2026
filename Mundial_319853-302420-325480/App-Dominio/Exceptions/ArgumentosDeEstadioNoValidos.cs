using System;

namespace App.Dominio.Exceptions;

public class ArgumentosDeEstadioNoValidos : ArgumentException
{
    public ArgumentosDeEstadioNoValidos() : base("Los argumentos del estadio no son válidos.") { }

    public ArgumentosDeEstadioNoValidos(string message) : base(message) { }

    public ArgumentosDeEstadioNoValidos(string message, Exception innerException) : base(message, innerException) { }
}


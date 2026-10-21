using System;

namespace App.Dominio.Exceptions;

public class ArgumentosDeLogNoValidos : ArgumentException
{
    public ArgumentosDeLogNoValidos() : base("Los argumentos del log no son válidos.") { }

    public ArgumentosDeLogNoValidos(string message) : base(message) { }

    public ArgumentosDeLogNoValidos(string message, Exception innerException) : base(message, innerException) { }
}



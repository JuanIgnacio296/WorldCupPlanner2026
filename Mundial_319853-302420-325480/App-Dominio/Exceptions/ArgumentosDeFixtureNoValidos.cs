using System;

namespace App.Dominio.Exceptions;

public class ArgumentosDeFixtureNoValidos : ArgumentException
{
    public ArgumentosDeFixtureNoValidos() : base("Los argumentos del fixture no son válidos.") { }

    public ArgumentosDeFixtureNoValidos(string message) : base(message) { }

    public ArgumentosDeFixtureNoValidos(string message, Exception innerException) : base(message, innerException) { }
}



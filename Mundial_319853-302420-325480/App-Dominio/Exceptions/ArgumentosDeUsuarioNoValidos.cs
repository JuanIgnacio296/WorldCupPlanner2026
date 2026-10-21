namespace App.Dominio.Exceptions;

using System;

public class ArgumentosDeUsuarioNoValidos : ArgumentException
{
    public ArgumentosDeUsuarioNoValidos() : base("Los argumentos del usuario no son válidos.") { }

    public ArgumentosDeUsuarioNoValidos(string message) : base(message) { }

    public ArgumentosDeUsuarioNoValidos(string message, Exception innerException) : base(message, innerException) { }
}


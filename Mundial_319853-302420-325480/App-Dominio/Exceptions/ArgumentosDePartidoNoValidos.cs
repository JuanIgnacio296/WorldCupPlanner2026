using System;

namespace App.Dominio.Exceptions;

public class ArgumentosDePartidoNoValidos : ArgumentException
{
    public ArgumentosDePartidoNoValidos() : base("Los argumentos del partido no son válidos.") { }

    public ArgumentosDePartidoNoValidos(string message) : base(message) { }

    public ArgumentosDePartidoNoValidos(string message, Exception innerException) : base(message, innerException) { }
}



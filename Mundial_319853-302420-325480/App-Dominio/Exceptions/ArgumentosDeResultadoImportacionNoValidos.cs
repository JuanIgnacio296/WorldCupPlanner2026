using System;

namespace App.Dominio.Exceptions;

public class ArgumentosDeResultadoImportacionNoValidos : ArgumentException
{
    public ArgumentosDeResultadoImportacionNoValidos() : base("Los argumentos del resultado de importación no son válidos.") { }

    public ArgumentosDeResultadoImportacionNoValidos(string message) : base(message) { }

    public ArgumentosDeResultadoImportacionNoValidos(string message, Exception innerException) : base(message, innerException) { }
}



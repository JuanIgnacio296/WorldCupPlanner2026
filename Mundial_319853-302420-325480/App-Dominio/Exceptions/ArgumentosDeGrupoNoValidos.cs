using System;

namespace App.Dominio.Exceptions;

public class ArgumentosDeGrupoNoValidos : ArgumentException
{
    public ArgumentosDeGrupoNoValidos() : base("Los argumentos del grupo no son válidos.") { }

    public ArgumentosDeGrupoNoValidos(string message) : base(message) { }

    public ArgumentosDeGrupoNoValidos(string message, Exception innerException) : base(message, innerException) { }
}



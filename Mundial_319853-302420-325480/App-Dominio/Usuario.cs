using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using App.Dominio.Exceptions;

namespace App.Dominio;

[Flags]
public enum Rol
{
    Ninguno = 0,
    Administrador = 1,
    Editor = 2,
    Periodista = 3
}

public class Usuario
{
    private const int EdadMinima = 18;
    private const int LargoMinimoContrasena = 8;
    private const string CaracteresEspecialesPermitidos = "@#$.";

    private static readonly Regex FormatoCorreo = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);

    private string _nombre = string.Empty;
    private string _apellido = string.Empty;
    private string _correo = string.Empty;
    private string _contrasena = string.Empty;
    private DateTime _fechaNacimiento;
    private List<Rol> _roles = [];

    public Usuario(string nombre, string apellido, string correo, DateTime fechaNacimiento, string contrasena, List<Rol>? rolesIniciales = null)
    {
        Nombre = nombre;
        Apellido = apellido;
        Correo = correo;
        Contrasena = contrasena;
        FechaNacimiento = fechaNacimiento;
        Roles = rolesIniciales ?? [];
    }

    private Usuario() { }

    public string Nombre
    {
        get => _nombre;
        set
        {
            ValidarNombre(value);
            _nombre = value;
        }
    }

    public string Apellido
    {
        get => _apellido;
        set
        {
            ValidarApellido(value);
            _apellido = value;
        }
    }

    public string Correo
    {
        get => _correo;
        set
        {
            ValidarCorreo(value);
            _correo = value;
        }
    }

    public string Contrasena
    {
        get => _contrasena;
        set
        {
            ValidarContrasena(value);
            _contrasena = value;
        }
    }

    public DateTime FechaNacimiento
    {
        get => _fechaNacimiento;
        set
        {
            ValidarFechaNacimiento(value);
            _fechaNacimiento = value;
        }
    }

    public List<Rol> Roles
    {
        get => _roles;
        set => _roles = value ?? [];
    }

    public bool TieneRol(Rol rol)
    {
        return Roles.Contains(rol);
    }

    private static void ValidarNombre(string nombre)
    {
        ValidarTextoObligatorio(nombre, "Nombre no puede ser vacío");

        if (ContieneNumerosOSimbolos(nombre))
        {
            throw new ArgumentosDeUsuarioNoValidos("Nombre no puede contener números ni caracteres especiales");
        }
    }

    private static void ValidarApellido(string apellido)
    {
        ValidarTextoObligatorio(apellido, "Apellido no puede ser vacío");

        if (ContieneNumerosOSimbolos(apellido))
        {
            throw new ArgumentosDeUsuarioNoValidos("Apellido no puede contener números ni caracteres especiales");
        }
    }

    private static void ValidarCorreo(string correo)
    {
        ValidarTextoObligatorio(correo, "Correo no puede ser vacío");

        if (!FormatoCorreo.IsMatch(correo))
        {
            throw new ArgumentosDeUsuarioNoValidos("Formato de correo inválido");
        }
    }

    private static void ValidarContrasena(string contrasena)
    {
        ValidarTextoObligatorio(contrasena, "Contraseña debe tener al menos 8 caracteres");

        if (contrasena.Length < LargoMinimoContrasena)
        {
            throw new ArgumentosDeUsuarioNoValidos("Contraseña debe tener al menos 8 caracteres");
        }

        if (!CumplePoliticaContrasena(contrasena))
        {
            throw new ArgumentosDeUsuarioNoValidos(
                "Contraseña debe tener mayúscula, minúscula, número y un carácter especial (@, #, $, .)");
        }
    }

    private static void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        DateTime hoy = DateTime.Now;

        if (fechaNacimiento > hoy)
        {
            throw new ArgumentosDeUsuarioNoValidos("La fecha de nacimiento no puede ser después de la fecha de hoy");
        }

        if (fechaNacimiento > hoy.AddYears(-EdadMinima))
        {
            throw new ArgumentosDeUsuarioNoValidos("Debe ser mayor de edad");
        }
    }

    private static void ValidarTextoObligatorio(string valor, string mensaje)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentosDeUsuarioNoValidos(mensaje);
        }
    }

    private static bool ContieneNumerosOSimbolos(string texto)
    {
        return texto.Any(char.IsDigit)
               || texto.Any(char.IsPunctuation)
               || texto.Any(char.IsSymbol);
    }

    private static bool CumplePoliticaContrasena(string contrasena)
    {
        bool tieneEspecial = contrasena.Any(CaracteresEspecialesPermitidos.Contains);
        bool tieneNumero = contrasena.Any(char.IsDigit);
        bool tieneMayuscula = contrasena.Any(char.IsUpper);
        bool tieneMinuscula = contrasena.Any(char.IsLower);

        return tieneEspecial && tieneNumero && tieneMayuscula && tieneMinuscula;
    }
}




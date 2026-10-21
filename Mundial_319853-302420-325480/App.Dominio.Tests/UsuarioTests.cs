using System;
using App.Dominio.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.Dominio.Tests;

[TestClass]
public class UsuarioTests
{
    private string n = "Juan";
    private string a = "Gutierrez";
    private string e = "jg@gmail.com";
    private DateTime f = new DateTime(2000, 1, 1);
    private string c = "Password123#";

    [TestMethod]
    public void CreateNewUser_WithValidData_ShouldIntegrateCorrectly()
    {
        Usuario u = new Usuario(n, a, e, f, c);
        Assert.AreEqual(u.Nombre, n);
        Assert.AreEqual(u.Apellido, a);
        Assert.AreEqual(u.Correo, e);
        Assert.AreEqual(u.FechaNacimiento, f);
        Assert.AreEqual(u.Contrasena, c);
    }
    
    [TestMethod]
    public void CreateNewUser_WhenNombreIsEmpty_ThenThrowsException()
    {
        string nombreVacio = "";
        Exception exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(nombreVacio, a, e, f, c)
        );

        Assert.AreEqual("Nombre no puede ser vacío", exception.Message);
    }

    [TestMethod]
    public void CreateNewUser_WhenNombreContainsSpecialCharacters_ThenThrowsException()
    {
        string nombreInvalido = "Ju4n!";
        var exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(nombreInvalido, a, e, f, c)
        );
        Assert.AreEqual("Nombre no puede contener números ni caracteres especiales", exception.Message);
    }

    [TestMethod]
    public void CreateNewUser_WhenApellidoIsEmpty_ThenThrowsException()
    {
        string apellidoVacio = "";
        Exception exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() =>
            new Usuario(n, apellidoVacio, e, f, c)
        );
        Assert.AreEqual("Apellido no puede ser vacío", exception.Message);
    }

    [TestMethod]
    public void CreateNewUser_WhenApellidoContainsSpecialCharacters_ThenThrowsException()
    {
        string apellidoInvalido = "Gut1erre#";
        var exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, apellidoInvalido, e, f, c)
        );
        Assert.AreEqual("Apellido no puede contener números ni caracteres especiales", exception.Message);
    }
    
    [TestMethod]
    public void CreateUser_WhenEmailIsEmpty_ThenThrowsException()
    {
        string emailVacio = "";
        var exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, emailVacio, f, c)
        );

        Assert.AreEqual("Correo no puede ser vacío", exception.Message);
    }
    
    [TestMethod]
    public void CreateUser_WhenEmailFormatIsInvalid_ThenThrowsException()
    {
        string emailInvalido = "juan.perez.com";

        var exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, emailInvalido, f, c)
        );

        Assert.AreEqual("Formato de correo inválido", exception.Message);
    }

    [TestMethod]
    [DataRow("usuario@dominio")]
    [DataRow("@dominio.com")]
    [DataRow("usuario@.com")]
    [DataRow("usuario@dominio.")]
    public void CreateUser_WithVariousInvalidEmails_ThenThrowsException(string emailInvalido)
    {
        Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, emailInvalido, f, c)
        );
    }

    [TestMethod]
    public void CreateUser_WhenFechaNacimientoIsAfterToday_ThenThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, e, new DateTime(9999, 1, 1), c)
        );
        Assert.AreEqual("La fecha de nacimiento no puede ser después de la fecha de hoy", ex.Message);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("P3#")]
    [DataRow("Pass1@7")]
    public void CreateUser_WhenPasswordIsLessThan8Characters_ThenThrowsException(string contraseniaInvalida)
    {
        var exception = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() =>
            new Usuario(n, a, e, f, contraseniaInvalida)
        );

        Assert.AreEqual("Contraseña debe tener al menos 8 caracteres", exception.Message);
    }
    
    [TestMethod]
    public void CreateUser_WhenPasswordMissingSpecialChar_ThenThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, e, f, "Password123")
        );
        Assert.IsTrue(ex.Message.Contains("un carácter especial"));
    }

    [TestMethod]
    public void CreateUser_WhenPasswordMissingNumber_ThenThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, e, f, "Password!#")
        );
        Assert.IsTrue(ex.Message.Contains("número"));
    }

    [TestMethod]
    public void CreateUser_WhenPasswordMissingUpperCase_ThenThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, e, f, "password123#")
        );
        Assert.IsTrue(ex.Message.Contains("mayúscula"));
    }

    [TestMethod]
    public void CreateUser_WhenPasswordMissingLowerCase_ThenThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() => 
            new Usuario(n, a, e, f, "PASSWORD123#")
        );
        Assert.IsTrue(ex.Message.Contains("minúscula"));
    }
    
    [TestMethod]
    public void CrearUsuario_CuandoFechaNacimientoEsFuturaOMenorEdad_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() =>
            new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddDays(1), "Juan123#", [Rol.Editor])
        );

        Assert.ThrowsException<ArgumentosDeUsuarioNoValidos>(() =>
            new Usuario("Ana", "Perez", "ana@test.com", DateTime.Now.AddYears(-10), "Ana1234#", [Rol.Editor])
        );
    }
}


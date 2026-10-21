using App.Dominio.Exceptions;
namespace App.Dominio.Tests;

[TestClass]
public class LogTests
{
    [TestMethod]
    public void Log_Creation_ShouldSetPropertiesCorrectly()
    {
        Usuario elAdmin = new Usuario("Admin", "a", "admin@mail.com", new DateTime(2000, 1 ,1), "constraseNa#123"); 
        string laAccion = "Creación de Equipo";
        DateTime laFecha = DateTime.Now;

        Log unLog = new Log(elAdmin, laAccion, laFecha);

        Assert.AreEqual(elAdmin, unLog.Usuario);
        Assert.AreEqual(laAccion, unLog.Accion);
        Assert.AreEqual(laFecha, unLog.Fecha);
    }
    
    [TestMethod]
    public void Log_WhenUsuarioIsNull_ShouldThrowException()
    {
        Usuario elAdmin = null;
        string laAccion = "Cualquier Acción";
        DateTime laFecha = DateTime.Now;

        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => new Log(elAdmin, laAccion, laFecha));
    }
    
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Log_WhenAccionIsNullOrWhiteSpace_ShouldThrowException(string accionInvalida)
    {
        Usuario unUsuario = new Usuario("Admin", "Test", "admin@ort.edu.uy", new DateTime(1990, 1, 1), "Admin123@");
        DateTime laFecha = DateTime.Now;

        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => new Log(unUsuario, accionInvalida, laFecha));
    }
    
    [TestMethod]
    public void Log_WhenFechaIsFuture_ShouldThrowException()
    {
        Usuario unUsuario = new Usuario("Admin", "Test", "admin@ort.edu.uy", new DateTime(1990, 1, 1), "Admin123@");
        string unaAccion = "Login en el sistema";
        DateTime fechaFutura = DateTime.Now.AddDays(1);

        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => new Log(unUsuario, unaAccion, fechaFutura));
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void Log_WhenIdIsLessThanOrEqualToZero_ShouldThrowException(int invalidId)
    {
        Usuario unUsuario = new Usuario("Admin", "Test", "admin@ort.edu.uy", new DateTime(1990, 1, 1), "Admin123@");
        Log unLog = new Log(unUsuario, "Login en el sistema", DateTime.Now);

        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => unLog.Id = invalidId);
    }
    
    [TestMethod]
    public void Log_WhenAccionExceedsMaxLength_ShouldThrowException()
    {
        Usuario unUsuario = new Usuario("Admin", "Test", "admin@ort.edu.uy", new DateTime(1990, 1, 1), "Admin123@");
        string accionLarga = new string('A', 201);
    
        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => new Log(unUsuario, accionLarga, DateTime.Now));
    }
    
    [TestMethod]
    public void SetId_CuandoValorInvalido_LanzaExcepcion()
    {
        Usuario usuario = new Usuario("Admin", "Sist", "admin@test.com", DateTime.Now.AddYears(-20), "Admin123#", [Rol.Administrador]);
        Log log = new Log(usuario, "Accion", DateTime.Now);

        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => log.Id = 0);
        Assert.ThrowsException<ArgumentosDeLogNoValidos>(() => log.Id = -1);
    }

    [TestMethod]
    public void SetId_CuandoValorValido_AsignaId()
    {
        Usuario usuario = new Usuario("Admin", "Sist", "admin@test.com", DateTime.Now.AddYears(-20), "Admin123#", [Rol.Administrador]);
        Log log = new Log(usuario, "Accion", DateTime.Now);

        log.Id = 10;

        Assert.AreEqual(10, log.Id);
    }
}



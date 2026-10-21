using App.Dominio.Exceptions;
using App.Dominio;
namespace App.Dominio.Tests;

[TestClass]
public class ResultadoImportacionTests
{
    [TestMethod]
    public void Constructor_WhenCreated_ThenCantidadImportadaDebeSerCero()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();
        Assert.AreEqual(0, resultado.CantidadImportada);
    }
    
    [TestMethod]
    public void Constructor_WhenCreated_ThenErroresDebeEstarVacia()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        Assert.AreEqual(0, resultado.Errores.Count);
    }
    
    [TestMethod]
    public void FueExitosa_WhenNoTieneErrores_ThenDebeSerTrue()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        Assert.IsTrue(resultado.FueExitosa);
    }
    
    [TestMethod]
    public void AgregarError_WhenErrorValido_ThenSeAgregaALaLista()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        resultado.AgregarError("Fila 2: Confederación inválida.");

        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: Confederación inválida.", resultado.Errores[0]);
    }
    
    [TestMethod]
    public void FueExitosa_WhenTieneErrores_ThenDebeSerFalse()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        resultado.AgregarError("Fila 2: Nombre vacío.");

        Assert.IsFalse(resultado.FueExitosa);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentosDeResultadoImportacionNoValidos))]
    public void AgregarError_WhenErrorVacio_ThenLanzaExcepcion()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        resultado.AgregarError("");
    }
    
    [TestMethod]
    public void AumentarCantidadImportada_WhenCalled_ThenIncrementaCantidad()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        resultado.AumentarCantidadImportada();

        Assert.AreEqual(1, resultado.CantidadImportada);
    }
    
    [TestMethod]
    public void AumentarCantidadImportada_WhenCalledMultipleTimes_ThenIncrementaCorrectamente()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        resultado.AumentarCantidadImportada();
        resultado.AumentarCantidadImportada();
        resultado.AumentarCantidadImportada();

        Assert.AreEqual(3, resultado.CantidadImportada);
    }
    
    [TestMethod]
    public void AgregarError_CuandoErrorEsVacio_LanzaExcepcion()
    {
        ResultadoImportacion resultado = new ResultadoImportacion();

        Assert.ThrowsException<ArgumentosDeResultadoImportacionNoValidos>(() => resultado.AgregarError(""));
        Assert.ThrowsException<ArgumentosDeResultadoImportacionNoValidos>(() => resultado.AgregarError("   "));
    }
}



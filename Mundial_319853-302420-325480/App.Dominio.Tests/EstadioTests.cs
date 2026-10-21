using App.Dominio.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.Dominio.Tests;

[TestClass]

public class EstadioTests
{
    private string nombre = "Spotify Camp Nou";
    private string ciudad = "Barcelona";
    private string descripcion = "Estacio del FC Barcelona";
    private int capacidad = 99354;
    private List<DateTime> fechasOcupadas = new List<DateTime>();

    [TestMethod]
    public void CreateNewEstadio_WhenNombreIsEmpty_ThenThrowsException()
    {
        string nombreVacio = "";
        
        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() => 
            new Estadio(nombreVacio, ciudad, capacidad,fechasOcupadas, descripcion)
        );
        
        Assert.AreEqual("Nombre no puede ser vacío", exception.Message);
    }

    [TestMethod]
    public void CreateNewEstadio_WhenNombreIsLargerThan80Characters_ThenThrowsException()
    {
        string nombreLargo = "Estadio Internacional Metropolitano de las Glorias Eternas del Fútbol Sudamericano y la Pasión Inquebrantable de su Hinchada";

        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombreLargo, ciudad, capacidad,fechasOcupadas, descripcion)
        );
        
        Assert.AreEqual("Nombre no puede tener mas de 80 caracteres", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithValidNombre_ShouldIntegrateCorrectly()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad,fechasOcupadas, descripcion);
        Assert.AreEqual(nombre, e.Nombre);
    }

    [TestMethod]
    public void CreateNewEstadio_WhenCiudadIsEmpty_ThenThrowsException()
    {
        string ciudadVacio = "";

        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombre, ciudadVacio, capacidad,fechasOcupadas, descripcion)
        );
        
        Assert.AreEqual("Ciudad no puede ser vacío", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WhenCiudadIsLargerThan60Characters_ThenThrowsException()
    {
        string ciudadLarga = "Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch-del-valle-historico";

        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombre, ciudadLarga, capacidad,fechasOcupadas, descripcion)
        );
        
        Assert.AreEqual("Ciudad no puede tener mas de 60 caracteres", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithValidCiudad_ShouldIntegrateCorrectly()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad,fechasOcupadas, descripcion);
        Assert.AreEqual(ciudad, e.Ciudad);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WhenDescripcionIsLargerThan400Characters_ThenThrowsException()
    {
        string descripcionLarga = "El estadio se erige como un símbolo imponente de la pasión futbolera, con una arquitectura moderna que combina acero, vidrio y hormigón en perfecta armonía. Sus tribunas envolventes permiten una visión privilegiada desde cualquier ángulo, mientras que su capacidad para más de 70.000 espectadores lo convierte en un verdadero templo del deporte. Cuenta con tecnología de iluminación LED de última generación, pantallas gigantes de alta definición y un sistema acústico que amplifica el aliento de la hinchada creando una atmósfera única. En su interior, además de vestuarios de nivel internacional y zonas de prensa completamente equipadas, se destacan espacios gastronómicos, áreas VIP y sectores interactivos para los fanáticos, haciendo que la experiencia trascienda el partido y se convierta en un espectáculo total.";

        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombre, ciudad, capacidad,fechasOcupadas, descripcionLarga)
        );
        
        Assert.AreEqual("Descripcion no puede tener mas de 400 caracteres", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithValidDescription_Empty_ShouldIntegrateCorrectly()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad,fechasOcupadas);
        Assert.AreEqual("Sin descripcion", e.Descripcion);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithValidDescription_NotEmpty_ShouldIntegrateCorrectly()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad,fechasOcupadas, descripcion);
        Assert.AreEqual(descripcion, e.Descripcion);
    }
    
    [TestMethod]
    [DataRow(-10000)]
    [DataRow(0)]
    [DataRow(10000)]
    public void CreateNewEstadio_WhenCapacidadIsSmallerThan20000_ThenThrowsException(int capacidadInvalida)
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombre, ciudad, capacidadInvalida,fechasOcupadas, descripcion)
        );
        
        
        Assert.AreEqual("Capacidad no puede ser menor a 20.000 personas", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithValidCapacidad_ShouldIntegrateCorrectly()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad,fechasOcupadas, descripcion);
        Assert.AreEqual(capacidad, e.Capacidad);
    }

    [TestMethod]
    public void CreateNewEstadio_WithInvalidFechas_ThenThrowsException()
    {
        DateTime fechaPasada = new DateTime(2000, 1, 1);
        fechasOcupadas.Add(fechaPasada);

        Exception e = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombre, ciudad, capacidad, fechasOcupadas, descripcion)
        );
        
        Assert.AreEqual("No se puede ingresar fechas pasadas", e.Message);
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithExistingFechas_ThenThrowsException()
    {
        DateTime fechaExistente = new DateTime(2027, 1, 1, 14, 0, 0);
        fechasOcupadas.Clear();
        fechasOcupadas.Add(fechaExistente);
        fechasOcupadas.Add(fechaExistente);

        Exception e = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
            new Estadio(nombre, ciudad, capacidad, fechasOcupadas, descripcion)
        );

        Assert.AreEqual("No se puede ingresar fechas ocupadas", e.Message);
    }

    [TestMethod]
    public void CreateNewEstadio_WithValidFechas_WithData_ShouldIntegrateCorrectly()
    {
        DateTime fechaValida = new DateTime(2027, 1, 1, 14, 0, 0);
        fechasOcupadas.Add(fechaValida);

        Estadio e = new Estadio(nombre, ciudad, capacidad, fechasOcupadas, descripcion);

        Assert.AreEqual(1, e.FechasOcupadas.Count);
        Assert.IsTrue(e.FechasOcupadas.Any(f => f.Fecha == fechaValida));
    }
    
    [TestMethod]
    public void CreateNewEstadio_WithValidFechas_WithoutData_ShouldIntegrateCorrectly()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        Assert.AreEqual(0, e.FechasOcupadas.Count);
    }

    [TestMethod]
    public void EsFechaDisponible_WithInvalidFecha_ShouldReturnFalse()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 01, 01, 14, 0, 0);
        
        Assert.AreEqual(e.ReservaEstadioValidacion.EsFechaDisponible(date, e.FechasOcupadas), false);
    }

    [TestMethod]
    public void EsFechaDisponible_WithInvalidHour_ShouldReturnFalse()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 06, 01, 13, 0, 0);
        
        Assert.AreEqual(e.ReservaEstadioValidacion.EsFechaDisponible(date, e.FechasOcupadas), false);
    }

    [TestMethod]
    public void EsFechaDisponible_WithExistingFecha_ShouldReturnFalse()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 06, 01, 14, 0, 0);
        e.AddFecha(date);
        
        Assert.AreEqual(e.ReservaEstadioValidacion.EsFechaDisponible(date, e.FechasOcupadas), false);
    }
    

    [TestMethod]
    public void EsFechaDisponible_WithValidData_ShouldReturnTrue()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 06, 01, 14, 0, 0);
        e.AddFecha(date);
        
        Assert.AreEqual(e.ReservaEstadioValidacion.EsFechaDisponible(date.AddHours(4), e.FechasOcupadas), true);
    }

    [TestMethod]
    public void AddFecha_WithInvalidFecha_ShouldThrowException()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad);
        DateTime date = new DateTime(2026, 01, 01, 14, 00, 00);

        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
        { 
            e.AddFecha(date);
        });
        
        Assert.AreEqual("No se puede ingresar fechas pasadas", exception.Message);
    }

    [TestMethod]
    public void AddFecha_WithInvalidHour_ShouldThrowException()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 06, 01, 13, 0, 0);

        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
        {
            e.AddFecha(date);
        });
        
        Assert.AreEqual("Los horarios válidos son: 14:00, 18:00 y 22:00", exception.Message);
    }
    
    [TestMethod]
    public void AddFecha_WithExistingFecha_ShouldThrowException()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 06, 01, 14, 0, 0);
        e.AddFecha(date);
        
        Exception exception = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() =>
        { 
            e.AddFecha(date);
        });
        
        Assert.AreEqual("No se puede ingresar fechas ocupadas", exception.Message);
    }
    
    [TestMethod]
    public void AddFecha_WithValidData_ShouldAddDataToFechasOcupadas()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime date = new DateTime(2026, 06, 01, 14, 0, 0);
        e.AddFecha(date);
        e.AddFecha(date.AddHours(4));
        
        Assert.AreEqual(2, e.FechasOcupadas.Count);
    }
    
    [TestMethod]
    public void EsFechaDisponible_CuandoHorarioInvalido_DevuelveFalse()
    {
        Estadio estadio = new Estadio("A", "Ciudad", 30000, "Desc");

        Assert.IsFalse(estadio.ReservaEstadioValidacion.EsFechaDisponible(new DateTime(2026, 6, 1, 15, 0, 0), estadio.FechasOcupadas));
        Assert.IsFalse(estadio.ReservaEstadioValidacion.EsFechaDisponible(new DateTime(2026, 6, 1, 14, 30, 0), estadio.FechasOcupadas));
        Assert.IsFalse(estadio.ReservaEstadioValidacion.EsFechaDisponible(new DateTime(2026, 6, 1, 14, 0, 1), estadio.FechasOcupadas));
    }

    [TestMethod]
    public void AddFecha_CuandoHorarioInvalidoOcupado_LanzaExcepcion()
    {
        Estadio estadio = new Estadio("A", "Ciudad", 30000, "Desc");
        DateTime fecha = new DateTime(2026, 6, 1, 14, 0, 0);

        Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() => estadio.AddFecha(new DateTime(2026, 6, 1, 15, 0, 0)));

        estadio.AddFecha(fecha);

        Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() => estadio.AddFecha(fecha));
    }

    [TestMethod]
    public void FechasOcupadas_SetNull_LanzaExcepcion()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);

        var ex = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() => e.FechasOcupadas = null!);
        Assert.AreEqual("No se puede ingresar fechas nulas", ex.Message);
    }

    [TestMethod]
    public void FechasOcupadas_SetFechaPasada_LanzaExcepcion()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        var fechas = new List<FechaOcupada> { new FechaOcupada(DateTime.Now.AddDays(-1), e) };

        var ex = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() => e.FechasOcupadas = fechas);
        Assert.AreEqual("No se puede ingresar fechas pasadas", ex.Message);
    }

    [TestMethod]
    public void FechasOcupadas_SetFechaRepetida_LanzaExcepcion()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        DateTime fecha = DateTime.Now.AddDays(1);
        var fechas = new List<FechaOcupada> 
        { 
            new FechaOcupada(fecha, e), 
            new FechaOcupada(fecha, e) 
        };

        var ex = Assert.ThrowsException<ArgumentosDeEstadioNoValidos>(() => e.FechasOcupadas = fechas);
        Assert.AreEqual("No puede ingresar fechas repetidas", ex.Message);
    }

    [TestMethod]
    public void FechasOcupadas_SetValidCollection_SetsValue()
    {
        Estadio e = new Estadio(nombre, ciudad, capacidad, descripcion);
        var fechas = new List<FechaOcupada> { new FechaOcupada(DateTime.Now.AddDays(1), e) };

        e.FechasOcupadas = fechas;

        Assert.AreEqual(1, e.FechasOcupadas.Count);
    }
    }




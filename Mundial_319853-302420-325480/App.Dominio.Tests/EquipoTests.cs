using App.Dominio.Exceptions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.Dominio.Tests;

[TestClass]
public class EquipoTests
{
    [TestMethod]
    public void CreateEquipo_WhenDataIsValid_ThenEquipoIsCreatedSuccessfully()
    {
        string nombre = "Uruguay";
        string confederacion = "CONMEBOL";
        int RankingFifa = 500;
        Equipo e = new Equipo(nombre, confederacion, RankingFifa);
        Assert.AreEqual(e.Nombre, nombre);
        Assert.AreEqual(e.Confederacion, confederacion);
        Assert.AreEqual(e.RankingFifa, RankingFifa);
        Assert.AreEqual(e.Puntos, 0);
        Assert.AreEqual(e.GolesAFavor, 0);
        Assert.AreEqual(e.GolesEnContra, 0);
    }

    [TestMethod]
    public void CreateEquipo_WhenBanderaBase64IsProvided_ThenStoresBanderaBase64()
    {
        string banderaBase64 = "data:image/png;base64,iVBORw0KGgo=";

        Equipo e = new Equipo("Uruguay", "CONMEBOL", 500, banderaBase64);

        Assert.AreEqual(banderaBase64, e.BanderaBase64);
    }
    
    [TestMethod]
    public void CreateNewEquipo_WhenNombreIsEmpty_ThenThrowsException(){
        string nombre = "";
        string confederacion = "CONMEBOL";
        int RankingFifa = 500;
        
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() => 
            new Equipo(nombre, confederacion, RankingFifa)
        );
        
        Assert.AreEqual("Nombre no puede ser vacío", exception.Message);
    }

    [TestMethod]
    public void CreateNewEquipo_WhenNombreIsLargerThan60Characters_ThenThrowsException()
    {
        string nombre = "El nombre de este equipo es mayor de sesenta caracteres y ninguno es incorrecto";
        string confederacion = "CONMEBOL";
        int RankingFifa = 500;
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            new Equipo(nombre, confederacion, RankingFifa)
        );
        Assert.AreEqual("El nombre debe tener una longitud mayor o igual a 1 y menor o igual a 60 caracteres", exception.Message);
    }

    [TestMethod]
    public void CreateNewEquipo_WhenConfederacionIsEmpty_ThenThrowsException()
    {
        string nombre = "Uruguay";
        string confederacion = "";
        int RankingFifa = 500;
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            new Equipo(nombre, confederacion, RankingFifa)
        );
        Assert.AreEqual("La confederación debe ser una de las siguientes: AFC, CAF, CONCACAF, CONMEBOL, OFC o UEFA", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEquipo_WhenConfederacionIsNotInTheDomain_ThenThrowsException()
    {
        string nombre = "Uruguay";
        string confederacion = "Sudamérica";
        int RankingFifa = 500;
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            new Equipo(nombre, confederacion, RankingFifa)
        );
        Assert.AreEqual("La confederación debe ser una de las siguientes: AFC, CAF, CONCACAF, CONMEBOL, OFC o UEFA", exception.Message);
    }

    [TestMethod]
    public void CreateNewEquipo_WhenRankingFifaIsLessThan300_ThenThrowsException()
    {
        string nombre = "Uruguay";
        string confederacion = "CONMEBOL";
        int RankingFifa = 0;
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            new Equipo(nombre, confederacion, RankingFifa)
        );
        Assert.AreEqual("El ranking FIFA debe ser mayor o igual a 300 y menor o igual que 2500", exception.Message);
    }
    
    [TestMethod]
    public void CreateNewEquipo_WhenRankingFifaIsGreaterThan2500_ThenThrowsException()
    {
        string nombre = "Uruguay";
        string confederacion = "CONMEBOL";
        int RankingFifa = 3000;
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            new Equipo(nombre, confederacion, RankingFifa)
        );
        Assert.AreEqual("El ranking FIFA debe ser mayor o igual a 300 y menor o igual que 2500", exception.Message);
    }

    [TestMethod]
    public void ActualizarEquipo_WhenGolesAFavorIsNegative_ThenThrowsException()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            {
                e.ActualizarEquipo(0, -2, 2);
                
            }
        );
        Assert.AreEqual("Los goles a favor deben ser un número entero mayor o igual a 0", exception.Message);
    }
    
    [TestMethod]
    public void ActualizarEquipo_WhenGolesEnContraIsNegative_ThenThrowsException()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            {
                e.ActualizarEquipo(0, 2, -2);
                
            }
        );
        Assert.AreEqual("Los goles en contra deben ser un número entero mayor o igual a 0", exception.Message);
    }

    [TestMethod]
    public void ActualizarEquipo_WhenPuntosIsNegative_ThenThrowsException()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            {
                e.ActualizarEquipo(-2, 2, 2);
                
            }
        );
        Assert.AreEqual("Los puntos deben ser un entero entre los siguientes: {0, 1, 3}", exception.Message);
    }

    [TestMethod]
    public void ActualizarEquipo_WhenGolesAFavorGreaterThanGolesEnContraAndPuntosNotEqual3_ThenThrowsException()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            {
                e.ActualizarEquipo(0, 3, 2);
                
            }
        );
        Assert.AreEqual("Cuando los goles a favor son más que los en contra, la cantidad de puntos debe ser 3", exception.Message);
    }
    
    [TestMethod]
    public void ActualizarEquipo_WhenGolesEnContraGreaterThanGolesAFavorAndPuntosNotEqual0_ThenThrowsException()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            {
                e.ActualizarEquipo(3, 3, 4);
                
            }
        );
        Assert.AreEqual("Cuando los goles en contra son más que los a favor, la cantidad de puntos debe ser 0", exception.Message);
    }
    
    [TestMethod]
    public void ActualizarEquipo_WhenGolesEnContraEqualsGolesAFavorAndPuntosNotEqual1_ThenThrowsException()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Exception exception = Assert.ThrowsException<ArgumentosDeEquipoNoValidos>(() =>
            {
                e.ActualizarEquipo(3, 0, 0);
                
            }
        );
        Assert.AreEqual("Cuando los goles a favor son iguales a los en contra, la cantidad de puntos debe ser 1", exception.Message);
    }

    [TestMethod]
    public void ActualizarEquipo_WhenDataIsValid_ThenDataUpdatesCorrectly()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        int puntosAntes = e.Puntos;
        e.ActualizarEquipo(1, 0, 0);
        Assert.AreEqual(e.Puntos, puntosAntes + 1);
    }

    [TestMethod]
    public void ReiniciarEstadisticas_WhenEquipoTieneDatos_ThenVuelveACero()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);

        e.ActualizarEquipo(3, 2, 0);

        e.ReiniciarEstadisticas();

        Assert.AreEqual(0, e.Puntos);
        Assert.AreEqual(0, e.GolesAFavor);
        Assert.AreEqual(0, e.GolesEnContra);
    }

    [TestMethod]
    public void CompareTo_DeberiaRetornarCorrectamente()
    {
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 2000);
        Equipo e2 = new Equipo("Brasil", "CONMEBOL", 500);
        
        e1.RegistrarPartidoJugado(2,1);
        e2.RegistrarPartidoJugado(1,2);
        
        int dif = e1.CompareTo(e2);
        Assert.IsTrue(dif < 0);
    }
    
    [TestMethod]
    public void CompareTo_ConOtroNulo_DebeRetornarNegativoUno()
    {
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 2000);
        Assert.AreEqual(-1, e1.CompareTo(null));
    }

    [TestMethod]
    public void CompareTo_MismosPuntosDiferenteDiferenciaGoles_DebeOrdenarPorDiferencia()
    {
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 2000);
        Equipo e2 = new Equipo("Argentina", "CONMEBOL", 500);
        
        e1.RegistrarPartidoJugado(1, 0);
        e2.RegistrarPartidoJugado(2, 0);
        
        Assert.IsTrue(e1.CompareTo(e2) > 0);
    }

    [TestMethod]
    public void CompareTo_MismosPuntosMismaDiferenciaGolesDiferenteGolesFavor_DebeOrdenarPorGolesFavor()
    {
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 2000);
        Equipo e2 = new Equipo("Argentina", "CONMEBOL", 500);
        
        e1.RegistrarPartidoJugado(2, 1);
        e2.RegistrarPartidoJugado(1, 0);
        
        Assert.IsTrue(e1.CompareTo(e2) < 0);
    }

    [TestMethod]
    public void RegistrarPartidoJugado_Empate_SumaUnPunto()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        e.RegistrarPartidoJugado(1, 1);
        Assert.AreEqual(1, e.Puntos);
        Assert.AreEqual(1, e.GolesAFavor);
        Assert.AreEqual(1, e.GolesEnContra);
    }

    [TestMethod]
    public void RegistrarPartidoJugado_Derrota_NoSumaPuntos()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        e.RegistrarPartidoJugado(0, 1);
        Assert.AreEqual(0, e.Puntos);
        Assert.AreEqual(0, e.GolesAFavor);
        Assert.AreEqual(1, e.GolesEnContra);
    }

    [TestMethod]
    public void GetHashCode_DebeRetornarHashDelNombre()
    {
        Equipo e = new Equipo("Uruguay", "CONMEBOL", 2000);
        Assert.AreEqual("Uruguay".GetHashCode(), e.GetHashCode());
    }
}


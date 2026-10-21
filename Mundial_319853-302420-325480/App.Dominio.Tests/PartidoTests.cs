using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Dominio;
using App.Dominio.Exceptions;

namespace App.Dominio.Tests;


[TestClass]
public class PartidoTests
{
    private char grupoID = 'A';
    private int token = 1;
    private Estadio estadioValido = new Estadio("Spotify Camp Nou", "Barcelona", 99354, new List<DateTime>());
    private Equipo equipoLocal = new Equipo("España", "UEFA", 300);
    private Equipo equipoVisitante = new Equipo("Uruguay", "CONMEBOL", 317);
    private DateTime fechaValida = DateTime.Now.AddDays(10);

    [TestMethod]
    public void CreatePartido_WhenIdIsLessThan1_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(0, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        
        Assert.AreEqual("El id debe ser un entero entre 1 y 104 (inclusive)", ex.Message);
    }
    
    [TestMethod]
    public void CreatePartido_WhenIdIsMoreThan104_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(105, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        
        Assert.AreEqual("El id debe ser un entero entre 1 y 104 (inclusive)", ex.Message);
    }

    [TestMethod]
    public void CreatePartido_WhenFaseIsGruposAndGrupoIdIsNull_ThenThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, null, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        
        Assert.AreEqual("El grupo es obligatorio en partidos de Fase de Grupos", ex.Message);
    }
    
    [TestMethod]
    public void CreatePartido_WhenFaseIsGruposAndGrupoIdIsInvalid_ThenThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, 'Z', fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        
        Assert.AreEqual("El grupo debe estar compuesto por letras del A a la L", ex.Message);
    }
    
    [TestMethod]
    public void CreatePartido_WhenFaseIsNotGruposAndGrupoIdIsNotNull_ThenThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, 'B', fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Final", "20:00", 1);
        });
        
        Assert.AreEqual("El grupo no puede ser ingresado en partidos que no sean de Fase de Grupos", ex.Message);
    }
    
    [TestMethod]
    public void SetFecha_WhenFechaisNull_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, null, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        
        Assert.AreEqual("La fecha es obligatoria", ex.Message);
    }

    [TestMethod]
    public void SetEstadio_WhenEstadioIsNull_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, null, equipoLocal, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        Assert.AreEqual("El estadio es obligatorio", ex.Message);
    }

    [TestMethod]
    public void SetEquipoLocal_WhenEquipoIsNull_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, null, equipoVisitante, 0, 0, "Grupos", "20:00", 1);
        });
        Assert.AreEqual("El equipo local es obligatorio", ex.Message);
    }

    [TestMethod]
    public void SetEquipoVisitante_WhenEquipoIsNull_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, null, 0, 0, "Grupos", "20:00", 1);
        });
        Assert.AreEqual("El equipo visitante es obligatorio", ex.Message);
    }

    [TestMethod]
    public void SetFase_WhenFaseIsNull_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, null, "20:00", 1);
        });
        Assert.AreEqual("La fase es obligatoria", ex.Message);
    }

    [TestMethod]
    public void SetJornada_WhenJornadaIsNullAndFaseIsFaseDeGrupos_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", "20:00", null);
        });
        Assert.AreEqual("La jornada es obligatoria en partidos de fase de grupos", ex.Message);
    }
    
    [TestMethod]
    public void CreatePartido_ShouldAssignIncrementalId()
    {
        Partido p1 = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", "20:00", 1);
        Partido p2 = new Partido(2, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", "22:00", 1);

        Assert.IsTrue(p2.Id > p1.Id, "El ID del segundo partido debería ser mayor al del primero");
        Assert.AreEqual(p1.Id + 1, p2.Id);
    }

    [TestMethod]
    [DataRow(1,0)]
    [DataRow(4,2)]
    public void Vencedor_ShouldBeEquipoLocal_WhenGolesLocalIsGreater(int golesLoc, int golesVis)
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, golesLoc, golesVis, "Fase de grupos", "20:00", 1);
        
        Assert.AreEqual(equipoLocal.Nombre, p.Vencedor);
    }

    [TestMethod]
    [DataRow(1, 2)]
    [DataRow(0, 3)]
    public void Vencedor_ShouldBeEquipoVisitante_WhenGolesLocalIsLess(int golesLoc, int golesVis)
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, golesLoc, golesVis, "Fase de grupos", "20:00", 1);
        
        Assert.AreEqual(equipoVisitante.Nombre, p.Vencedor);
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(0, 0)]
    public void Vencedor_ShouldBeEmpate_WhenGolesLocalesSameAsGolesVis(int golesLoc, int golesVis)
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, golesLoc, golesVis, "Fase de grupos", "20:00", 1);
        
        Assert.AreEqual("Empate", p.Vencedor);
    }

    [TestMethod]
    public void EquipoGanador_CuandoGanaLocal_DevuelveEquipoLocal()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 2, 0, "Fase de grupos", "20:00", 1);

        Assert.AreSame(equipoLocal, p.EquipoGanador);
    }

    [TestMethod]
    public void EquipoGanador_CuandoGanaVisitante_DevuelveEquipoVisitante()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 2, "Fase de grupos", "20:00", 1);

        Assert.AreSame(equipoVisitante, p.EquipoGanador);
    }

    [TestMethod]
    public void EquipoGanador_CuandoEmpate_DevuelveNull()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 1, 1, "Fase de grupos", "20:00", 1);

        Assert.IsNull(p.EquipoGanador);
    }

    [TestMethod]
    public void EquipoPerdedor_CuandoGanaLocal_DevuelveEquipoVisitante()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 2, 0, "Fase de grupos", "20:00", 1);

        Assert.AreSame(equipoVisitante, p.EquipoPerdedor);
    }

    [TestMethod]
    public void EquipoPerdedor_CuandoEmpate_DevuelveNull()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 1, 1, "Fase de grupos", "20:00", 1);

        Assert.IsNull(p.EquipoPerdedor);
    }

    [TestMethod]
    public void CreatePartido_WhenEquiposAreEqual_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>((() => new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoLocal, 0, 0, "Grupos", "20:00", 1)
            ));
        Assert.AreEqual("Un equipo no puede jugar un partido contra sí mismo", ex.Message);
    }

    [TestMethod]
    [DataRow(-1, 0)]
    [DataRow(-999, -2)]
    [DataRow(0, -55)]
    public void SetGoles_WhenValueIsNegative_ThrowsException(int golesLoc, int golesVis)
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante,  golesLoc, golesVis, "Fase de grupos", "20:00", 1);
        });

        string mensajeEsperado = golesLoc < 0
            ? "La cantidad de goles del local debe ser un entero mayor o igual a 0"
            : "La cantidad de goles del visitante debe ser un entero mayor o igual a 0";
        Assert.AreEqual(mensajeEsperado, ex.Message);
    }

    [TestMethod]
    [DataRow("Texto inválido")]
    [DataRow("")]
    public void CreatePartido_WhenFaseIsInvalid_ThrowsException(string fase)
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido,  equipoLocal, equipoVisitante, 0, 0, fase, "20:00", 1);
        });
        Assert.AreEqual("Fase inválida", ex.Message);
    }

    [TestMethod]
    public void CreatePartido_WithValidData_ShouldIntegrateCorrectly()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", "20:00", 1);
        
        Assert.AreEqual(1, p.Id);
        Assert.AreEqual(token, p.Token);
        Assert.AreEqual(grupoID, p.GrupoID);
        Assert.AreEqual(fechaValida, p.Fecha);
        Assert.AreEqual(estadioValido, p.Estadio);
        Assert.AreEqual(equipoLocal, p.EquipoLocal);
        Assert.AreEqual(equipoVisitante, p.EquipoVisitante);
        Assert.AreEqual(0, p.GolesLocal);
        Assert.AreEqual(0, p.GolesVisitante);
        Assert.AreEqual("Fase de Grupos", p.Fase);
        Assert.AreEqual("20:00", p.Horario);
        Assert.AreEqual(1, p.Jornada);
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(-99)]
    public void SetJornada_WhenValueIsNegativeOr0_ThrowsException(int jornadaNegativa)
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", "20:00", jornadaNegativa);
        });
        Assert.AreEqual("La jornada no puede ser negativa o 0", ex.Message);
    }

    [TestMethod]
    public void SetHorario_WhenValueIsEmpty_ThrowsException()
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", "", 1);
        });
        Assert.AreEqual("El horario no puede ser vacio", ex.Message);
    }
    
    [TestMethod]
    [DataRow("25:00")]
    [DataRow("12:61")]
    [DataRow("9:30")]
    [DataRow("12-30")]
    [DataRow("pasado mañana")]
    public void SetHorario_WhenValueisInvalid_ThrowsException(string horarioInvalido)
    {
        Exception ex = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
        {
            new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, 0, 0, "Fase de grupos", horarioInvalido, 1);
        });
        Assert.AreEqual("El formato del horario debe ser HH:mm (24 horas)", ex.Message);
    }

    [TestMethod]
    [DataRow(2,1)]
    [DataRow(3,0)]
    public void SetPartido_WhenEquipoLocalWon_ShouldEquipoLocalScore3points(int golesLoc, int golesVis)
    {
        int puntos = equipoLocal.Puntos; 
        new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, golesLoc, golesVis, "Fase de Grupos", "20:00", 3);
        Assert.AreEqual(puntos+3, equipoLocal.Puntos);
    }
    
    [TestMethod]
    [DataRow(1,2)]
    [DataRow(0,3)]
    public void SetPartido_WhenEquipoVisitanteWon_ShouldEquipoVisitanteScore3points(int golesLoc, int golesVis)
    {
        int puntos = equipoVisitante.Puntos; 
        new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, golesLoc, golesVis, "Fase de Grupos", "20:00", 3);
        Assert.AreEqual(puntos+3, equipoVisitante.Puntos);
    }

    [TestMethod]
    [DataRow(2, 2)]
    [DataRow(0, 0)]
    public void SetPartido_WhenDraw_ShoulEquipoVisitanteAndEquipoLocalScore1point(int golesLoc, int golesVis)
    {
        int puntosLoc =  equipoLocal.Puntos;
        int puntosVis =  equipoVisitante.Puntos;
        new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, golesLoc, golesVis, "Fase de Grupos", "20:00", 3);
        Assert.AreEqual(puntosLoc+1, equipoLocal.Puntos);
        Assert.AreEqual(puntosVis+1, equipoLocal.Puntos);
    }

    [TestMethod]
    public void asignarResultado_WhenGolesLocalIsNegative_ThenThrowsException()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Fase de Grupos", 3);

        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarResultado(-2, 0)
        );
        
        Assert.AreEqual("La cantidad de goles del local debe ser un entero mayor o igual a 0", exception.Message);
    }
    
    [TestMethod]
    public void asignarResultado_WhenGolesVisitanteIsNegative_ThenThrowsException()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Fase de Grupos", 3);

        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarResultado(2, -1)
        );
        
        Assert.AreEqual("La cantidad de goles del visitante debe ser un entero mayor o igual a 0", exception.Message);
    }

    [TestMethod]
    public void asignarPenales_WhenFaseIsFaseDeGrupos_ThenThrowsException()
    {
        Partido p = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Fase de Grupos", 1);
        p.AsignarResultado(0, 0);
        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarPenales(2, 1)
        );
        
        Assert.AreEqual("Un partido de Fase de Grupos no se puede definir por penales", exception.Message);
    }

    [TestMethod]
    public void asignarPenales_WhenGolesLocalNotEqualsGolesVisitante_ThenThrowsException()
    {
        Partido p = new Partido(1, token, null, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Final", null);
        p.AsignarResultado(2, 0);
        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarPenales(2, 1)
        );
        
        Assert.AreEqual("En un partido que no terminó en empate no puede haber definición por penales.", exception.Message);
    }
    
    [TestMethod]
    public void asignarPenales_WhenPenalesLocalEqualsPenalesVisitante_ThenThrowsException()
    {
        Partido p = new Partido(1, token, null, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Final", null);
        p.AsignarResultado(0, 0);
        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarPenales(2, 2)
        );
        
        Assert.AreEqual("La cantidad de penales convertidos por ambos equipos no puede ser la misma.", exception.Message);
    }
    
    [TestMethod]
    public void asignarPenales_WhenPenalesLocalIsLessThan0_ThenThrowsException()
    {
        Partido p = new Partido(1, token, null, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Final", null);
        p.AsignarResultado(0, 0);
        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarPenales(-2, 2)
        );
        
        Assert.AreEqual("La cantidad de penales convertidos por ambos equipos debe ser mayor o igual a 0.", exception.Message);
    }
    
    [TestMethod]
    public void asignarPenales_WhenPenalesVisitanteIsLessThan0_ThenThrowsException()
    {
        Partido p = new Partido(1, token, null, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Final", null);
        p.AsignarResultado(0, 0);
        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => 
            p.AsignarPenales(2, -2)
        );
        
        Assert.AreEqual("La cantidad de penales convertidos por ambos equipos debe ser mayor o igual a 0.", exception.Message);
    }
    
    [TestMethod]
    public void PartidoSinResultado_NoModificaPuntosDeLosEquipos()
    {
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Francia", "UEFA", 1900);
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");

        Partido partido = new Partido(1, token, grupoID, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);

        Assert.AreEqual(0, local.Puntos);
        Assert.AreEqual(0, visitante.Puntos);
    }

    [TestMethod]
    public void Vencedor_CuandoEmpateEliminatoriaConPenales_DevuelveGanadorPorPenales()
    {
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Francia", "UEFA", 1900);
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");
        Partido partido = new Partido(1, token, null, new DateTime(2026, 7, 1, 14, 0, 0), estadio, local, visitante, "Final", null);

        partido.AsignarResultado(1, 1);
        partido.AsignarPenales(4, 3);

        Assert.AreEqual("Uruguay", partido.Vencedor);
    }

    [TestMethod]
    public void AsignarResultado_CuandoEsFaseEliminatoria_NoModificaPuntosNiGoles()
    {
        Equipo local = new Equipo("Brasil", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Alemania", "UEFA", 1900);
        Estadio estadio = new Estadio("Estadio Eliminatoria", "Ciudad", 30000, "Desc");
        Partido partido = new Partido(1, token, null, new DateTime(2026, 7, 1, 14, 0, 0), estadio, local, visitante, "Final", null);

        partido.AsignarResultado(2, 1);

        Assert.AreEqual(0, local.Puntos);
        Assert.AreEqual(0, visitante.Puntos);
        Assert.AreEqual(0, local.GolesAFavor);
        Assert.AreEqual(0, visitante.GolesAFavor);
        Assert.AreEqual(0, local.GolesEnContra);
        Assert.AreEqual(0, visitante.GolesEnContra);
    }

    [TestMethod]
    public void RegistrarTarjetas_CuandoCantidadesSonValidas_DebeGuardarTarjetasPorEquipo()
    {
        Partido partido = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Fase de Grupos", 1);

        partido.RegistrarTarjetas(2, 1, 3, 0);

        Assert.AreEqual(2, partido.TarjetasAmarillasLocal);
        Assert.AreEqual(1, partido.TarjetasRojasLocal);
        Assert.AreEqual(3, partido.TarjetasAmarillasVisitante);
        Assert.AreEqual(0, partido.TarjetasRojasVisitante);
    }

    [TestMethod]
    [DataRow(-1, 0, 0, 0)]
    [DataRow(0, -1, 0, 0)]
    [DataRow(0, 0, -1, 0)]
    [DataRow(0, 0, 0, -1)]
    public void RegistrarTarjetas_CuandoAlgunaCantidadEsNegativa_DebeLanzarExcepcion(
        int amarillasLocal,
        int rojasLocal,
        int amarillasVisitante,
        int rojasVisitante)
    {
        Partido partido = new Partido(1, token, grupoID, fechaValida, estadioValido, equipoLocal, equipoVisitante, "Fase de Grupos", 1);

        Exception exception = Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() =>
            partido.RegistrarTarjetas(amarillasLocal, rojasLocal, amarillasVisitante, rojasVisitante)
        );

        Assert.AreEqual("La cantidad de tarjetas debe ser mayor o igual a 0", exception.Message);
    }

    [TestMethod]
    public void SetEquipoLocalYVisitante_CuandoSonIguales_LanzaExcepcion()
    {
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Francia", "UEFA", 1900);
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");
        Partido partido = new Partido(1, token, grupoID, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);

        Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => partido.EquipoLocal = visitante);
        Assert.ThrowsException<ArgumentosDePartidoNoValidos>(() => partido.EquipoVisitante = local);
    }

    [TestMethod]
    public void SimularPartido_CuandoUsaDistintoMotorSimulacion_DevuelveDistintoResultado()
    {
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Francia", "UEFA", 500);
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");
        Partido partido1 = new Partido(1, 1, grupoID, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);
        Partido partido2 = new Partido(1, 2, grupoID, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);

        Semilla semilla1 = new Semilla(1, 1, 1, 1, 2);
        Semilla semilla2 = new Semilla(2, 1, 1, 1, 2);
        
        partido1.Simular(semilla1.RandomSimulacion, true);
        partido2.Simular(semilla2.RandomSimulacion, false);
        
        bool resutadosDistintos = partido1.GolesLocal != partido2.GolesLocal || partido1.GolesVisitante != partido2.GolesVisitante;

        Assert.IsTrue(resutadosDistintos);
    }

    [TestMethod]
    public void Simular_CuandoSeSimulaElPartido_TambienSimulaLasTarjetas()
    {
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");
        int totalTarjetas = 0;

        for (int semilla = 1; semilla <= 20; semilla++)
        {
            Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
            Equipo visitante = new Equipo("Francia", "UEFA", 500);
            Partido partido = new Partido(1, semilla, grupoID, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);

            partido.Simular(new Random(semilla), false);

            totalTarjetas += partido.TarjetasAmarillasLocal + partido.TarjetasRojasLocal
                + partido.TarjetasAmarillasVisitante + partido.TarjetasRojasVisitante;
        }

        Assert.IsTrue(totalTarjetas > 0, "Al simular un partido también deben simularse las tarjetas (incidencias).");
    }

    [TestMethod]
    public void SimularPenales_CuandoUsaDistintoMotorSimulacion_DevuelveDistintoResultado()
    {
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Francia", "UEFA", 500);
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");
        Partido partido1 = new Partido(104, 1, null, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Final", 1);
        Partido partido2 = new Partido(104, 2, null, new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Final", 1);
        
        Semilla semilla1 = new Semilla(1, 1, 1, 1, 2);
        Semilla semilla2 = new Semilla(2, 1, 1, 1, 2);
        
        partido1.AsignarResultado(1,1);
        partido2.AsignarResultado(1,1);
        
        partido1.SimularPenales(semilla1.RandomSimulacion, true);
        partido2.SimularPenales(semilla2.RandomSimulacion, false);
        
        bool resutadosDistintos = partido1.PenalesLocal != partido2.PenalesLocal || partido1.PenalesVisitante != partido2.PenalesVisitante;
        
        Assert.IsTrue(resutadosDistintos);
    }
}

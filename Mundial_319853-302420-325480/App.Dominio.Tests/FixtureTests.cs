using App.Dominio.Exceptions;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace App.Dominio.Tests;

[TestClass]
public class FixtureTests
{
    // Semilla fija para que las simulaciones sean deterministas y no dependan
    // del orden/aislamiento de ejecucion de los tests (resultados reproducibles).
    private const int SemillaSimulacion = 12345;

    private Fixture fixture;
    private List<Estadio> estadios;
    private List<Equipo> equipos;
    private List<Grupo> grupos;

    [TestInitialize]
    public void setup()
    {
        equipos = new List<Equipo>()
        {
            new Equipo("Equipo1", "CONMEBOL", 1200),
            new Equipo("Equipo2", "UEFA", 700),
            new Equipo("Equipo3", "AFC", 600),
            new Equipo("Equipo4", "CAF", 500),

            new Equipo("Equipo5", "UEFA", 1400),
            new Equipo("Equipo6", "CONMEBOL", 800),
            new Equipo("Equipo7", "CONCACAF", 650),
            new Equipo("Equipo8", "AFC", 550),

            new Equipo("Equipo9", "CONMEBOL", 1200),
            new Equipo("Equipo10", "UEFA", 700),
            new Equipo("Equipo11", "AFC", 600),
            new Equipo("Equipo12", "CAF", 500),

            new Equipo("Equipo13", "CONMEBOL", 1200),
            new Equipo("Equipo14", "UEFA", 700),
            new Equipo("Equipo15", "AFC", 600),
            new Equipo("Equipo16", "CAF", 500),

            new Equipo("Equipo17", "CONMEBOL", 1200),
            new Equipo("Equipo18", "UEFA", 700),
            new Equipo("Equipo19", "AFC", 600),
            new Equipo("Equipo20", "CAF", 500),

            new Equipo("Equipo21", "UEFA", 1400),
            new Equipo("Equipo22", "CONMEBOL", 800),
            new Equipo("Equipo23", "CONCACAF", 650),
            new Equipo("Equipo24", "AFC", 550),

            new Equipo("Equipo25", "CONMEBOL", 1200),
            new Equipo("Equipo26", "UEFA", 700),
            new Equipo("Equipo27", "AFC", 600),
            new Equipo("Equipo28", "CAF", 500),

            new Equipo("Equipo29", "CONMEBOL", 1200),
            new Equipo("Equipo30", "UEFA", 700),
            new Equipo("Equipo31", "AFC", 600),
            new Equipo("Equipo32", "CAF", 500),

            new Equipo("Equipo33", "CONMEBOL", 1200),
            new Equipo("Equipo34", "UEFA", 700),
            new Equipo("Equipo35", "AFC", 600),
            new Equipo("Equipo36", "CAF", 500),

            new Equipo("Equipo37", "UEFA", 1400),
            new Equipo("Equipo38", "CONMEBOL", 800),
            new Equipo("Equipo39", "CONCACAF", 650),
            new Equipo("Equipo40", "AFC", 550),

            new Equipo("Equipo41", "CONMEBOL", 1200),
            new Equipo("Equipo42", "UEFA", 700),
            new Equipo("Equipo43", "AFC", 600),
            new Equipo("Equipo44", "CAF", 500),

            new Equipo("Equipo45", "CONMEBOL", 1200),
            new Equipo("Equipo46", "UEFA", 700),
            new Equipo("Equipo47", "AFC", 600),
            new Equipo("Equipo48", "CAF", 500)
        };

            grupos = new List<Grupo>();

            Grupo g1 = new Grupo('A', 1);
            Grupo g2 = new Grupo('B', 1);
            Grupo g3 = new Grupo('C', 1);
            Grupo g4 = new Grupo('D', 1);
            Grupo g5 = new Grupo('E', 1);
            Grupo g6 = new Grupo('F', 1);
            Grupo g7 = new Grupo('G', 1);
            Grupo g8 = new Grupo('H', 1);
            Grupo g9 = new Grupo('I', 1);
            Grupo g10 = new Grupo('J', 1);
            Grupo g11 = new Grupo('K', 1);
            Grupo g12 = new Grupo('L', 1);
            for (int i = 0; i < 48; i++)
            {
                if (i < 4)
                {
                    g1.AddEquipo(equipos[i]);
                }
                else if (i < 8)
                {
                    g2.AddEquipo(equipos[i]);
                }
                else if (i < 12)
                {
                    g3.AddEquipo(equipos[i]);
                }
                else if (i < 16)
                {
                    g4.AddEquipo(equipos[i]);
                }
                else if (i < 20)
                {
                    g5.AddEquipo(equipos[i]);
                }
                else if (i < 24)
                {
                    g6.AddEquipo(equipos[i]);
                }
                else if (i < 28)
                {
                    g7.AddEquipo(equipos[i]);
                }
                else if (i < 32)
                {
                    g8.AddEquipo(equipos[i]);
                }
                else if (i < 36)
                {
                    g9.AddEquipo(equipos[i]);
                }
                else if (i < 40)
                {
                    g10.AddEquipo(equipos[i]);
                }
                else if (i < 44)
                {
                    g11.AddEquipo(equipos[i]);
                }
                else
                {
                    g12.AddEquipo(equipos[i]);
                }
            }

            grupos.Add(g1);
            grupos.Add(g2);
            grupos.Add(g3);
            grupos.Add(g4);
            grupos.Add(g5);
            grupos.Add(g6);
            grupos.Add(g7);
            grupos.Add(g8);
            grupos.Add(g9);
            grupos.Add(g10);
            grupos.Add(g11);
            grupos.Add(g12);


            estadios = new List<Estadio>()
            {
                new Estadio("Estadio1", "Ciudad", 40000, "Descripcion"),
                new Estadio("Estadio2", "Ciudad", 42000, "Descripcion"),
                new Estadio("Estadio3", "Ciudad", 45000, "Descripcion"),
                new Estadio("Estadio4", "Ciudad", 50000, "Descripcion"),
                new Estadio("Estadio5", "Ciudad", 55000, "Descripcion")
                
            };
            Semilla semilla = new Semilla(1, 123, 456, 789, 0);
            fixture = new Fixture(1, grupos, estadios, semilla, false);
        
    }

    [TestMethod]
    public void CrearFase_WhenCalled_ThenGeneratesEveryPartidoWithSameToken()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();
        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
        fixture.ArmarFase("Octavos de Final");
        simularOctavos();
        fixture.ArmarFase("Cuartos de Final");
        simularCuartos();
        fixture.ArmarFase("Semifinal");
        simularSemis();
        fixture.ArmarFase("Tercer y Cuarto Puesto");
        fixture.ArmarFase("Final");

        for (int i=0; i<104; i++)
        {
            Assert.AreEqual(fixture.Partidos[i].Token, fixture.Token);
        }
    }

    [TestMethod]
    public void CrearFase_WhenCalled_GeneratesIncreasingIdsWithinSameFixture()
    {
        fixture.ArmarFase("Fase de Grupos");
        for (int i=0; i<71; i++)
        {
            Assert.AreEqual(fixture.Partidos[i].Id + 1, fixture.Partidos[i + 1].Id);
        }
    }

    [TestMethod] 
    public void ArmarFase_WhenCalledIfEstadiosLengthIsLessThan4_ThenThrowsError(){
        List<Estadio> tresEstadios = new List<Estadio>()
        {
            new Estadio("Estadio5", "Ciudad", 40000, "Descripcion"),
            new Estadio("Estadio6", "Ciudad", 42000, "Descripcion"),
            new Estadio("Estadio7", "Ciudad", 45000, "Descripcion"),
        };
        Semilla semilla = new Semilla(2, 123, 456, 789, 0);

        Fixture f = new Fixture(1, grupos, tresEstadios, semilla, false);
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() => 
            f.ArmarFase("Grupos")
        );

        Assert.AreEqual("Debe haber al menos 4 estadios", exception.Message);
    }
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsFaseDeGrupos_ThenGeneratesCorrectAmountOfPartidos(){
        fixture.ArmarFase("Fase de Grupos");
        
        Assert.AreEqual(72, fixture.Partidos.Count);
    }

    [TestMethod]
    public void ArmarFase_WhenFaseIsDieciseisavosDeFinalAndPartidosJugadosIsLessThan72_ThenThrowsError()
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() =>
            fixture.ArmarFase("Dieciseisavos de Final")
        );
        
        Assert.AreEqual("No se puede armar los dieciseisavos de final si no se ha terminado la fase de grupos", exception.Message);

    }

    [TestMethod]
    public void ArmarFase_WhenFaseIsDieciseisavosDeFinal_ThenGeneratesCorrectAmountOfPartidos()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();

        fixture.ArmarFase("Dieciseisavos de Final");
        
        Assert.AreEqual(fixture.Partidos.Count, 88);
    }
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsOctavosDeFinalAndPartidosJugadosIsLessThan88_ThenThrowsError()
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() =>
            fixture.ArmarFase("Octavos de Final")
        );
        
        Assert.AreEqual("No se puede armar los octavos de final si no se han terminado los dieciseisavos de final", exception.Message);

    }
    
    
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsOctavosDeFinal_ThenGeneratesCorrectAmountOfPartidos()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();

        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
        
        fixture.ArmarFase("Octavos de Final");
        
        Assert.AreEqual(96, fixture.Partidos.Count);

    }

    [TestMethod]
    public void ArmarFase_WhenFaseIsCuartosDeFinalAndPartidosJugadosIsLessThan96_ThenThrowsError()
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() =>
            fixture.ArmarFase("Cuartos de Final")
        );
        
        Assert.AreEqual("No se puede armar los cuartos de final si no se han terminado los octavos de final", exception.Message);
    }

        
    [TestMethod]
    public void ArmarFase_WhenFaseIsCuartosDeFinal_ThenGeneratesCorrectAmountOfPartidos()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();

        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
      
        fixture.ArmarFase("Octavos de Final");
        simularOctavos();
        
        fixture.ArmarFase("Cuartos de Final");
        Assert.AreEqual(100, fixture.Partidos.Count);
    }
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsSemifinalAndPartidosJugadosIsLessThan100_ThenThrowsError()
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() =>
            fixture.ArmarFase("Semifinal")
        );
        
        Assert.AreEqual("No se puede armar las semifinales si no se han terminado los cuartos de final", exception.Message);
    }
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsSemifinal_ThenGeneratesCorrectAmountOfPartidos()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();

        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
        
        fixture.ArmarFase("Octavos de Final");
        simularOctavos();
        
        fixture.ArmarFase("Cuartos de Final");
        simularCuartos();
        
        fixture.ArmarFase("Semifinal");
        Assert.AreEqual(102, fixture.Partidos.Count);
    }

    [TestMethod]
    public void ArmarFase_WhenFaseIsTercerYCuartoPuestoAndPartidosJugadosIsLessThan102_ThenThrowsError()
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() =>
            fixture.ArmarFase("Tercer y Cuarto Puesto")
        );
        
        Assert.AreEqual("No se puede armar el Tercer y Cuarto Puesto si no se han terminado las semifinales", exception.Message);
    }
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsTercerYCuartoPuesto_ThenGeneratesCorrectAmountOfPartidos()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();


        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
        
        fixture.ArmarFase("Octavos de Final");
        simularOctavos();

        
        fixture.ArmarFase("Cuartos de Final");
        simularCuartos();
        
        fixture.ArmarFase("Semifinal");
        simularSemis();
        
        fixture.ArmarFase("Tercer y Cuarto Puesto");
        Assert.AreEqual(103, fixture.Partidos.Count);
    }
    
    [TestMethod]
    public void ArmarFase_WhenFaseIsFinalAndPartidosJugadosIsLessThan102_ThenThrowsError()
    {
        Exception exception = Assert.ThrowsException<ArgumentosDeFixtureNoValidos>(() =>
            fixture.ArmarFase("Final")
        );
        
        Assert.AreEqual("No se puede armar la Final si no se han terminado las semifinales", exception.Message);
    }
    
     [TestMethod]
    public void ArmarFase_WhenFaseIsFinal_ThenGeneratesCorrectAmountOfPartidos()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();
        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
        fixture.ArmarFase("Octavos de Final");
        simularOctavos();
        fixture.ArmarFase("Cuartos de Final");
        simularCuartos();
        fixture.ArmarFase("Semifinal");
        simularSemis();
        
        fixture.ArmarFase("Tercer y Cuarto Puesto");
        fixture.ArmarFase("Final");
        Assert.AreEqual(104, fixture.Partidos.Count);
    }

    [TestMethod]
    public void ArmarFase_WhenFaseIsFaseDeGruposAndJornada3_ThenAllMatchesInSameGroupoHaveSameFecha()
    {
        fixture.ArmarFase("Fase de Grupos");
        
        Assert.AreEqual(fixture.Grupos[0].Partidos[4].Fecha, fixture.Grupos[0].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[1].Partidos[4].Fecha, fixture.Grupos[1].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[2].Partidos[4].Fecha, fixture.Grupos[2].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[3].Partidos[4].Fecha, fixture.Grupos[3].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[4].Partidos[4].Fecha, fixture.Grupos[4].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[5].Partidos[4].Fecha, fixture.Grupos[5].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[6].Partidos[4].Fecha, fixture.Grupos[6].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[7].Partidos[4].Fecha, fixture.Grupos[7].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[8].Partidos[4].Fecha, fixture.Grupos[8].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[9].Partidos[4].Fecha, fixture.Grupos[9].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[10].Partidos[4].Fecha, fixture.Grupos[10].Partidos[5].Fecha);
        Assert.AreEqual(fixture.Grupos[11].Partidos[4].Fecha, fixture.Grupos[11].Partidos[5].Fecha);
    }

    [TestMethod]
    public void ArmarFase_WhenEquipoJuegaMultiplesPartidos_ThenExisteMinimoTresDiasEntreCadaPartido()
    {
        fixture.ArmarFase("Fase de Grupos");
        simularGrupos();
        fixture.ArmarFase("Dieciseisavos de Final");
        simularDieciseisavos();
        fixture.ArmarFase("Octavos de Final");
        simularOctavos();
        fixture.ArmarFase("Cuartos de Final");
        simularCuartos();
        fixture.ArmarFase("Semifinal");
        simularSemis();
        
        fixture.ArmarFase("Tercer y Cuarto Puesto");
        fixture.ArmarFase("Final");

        List<Equipo> finalistas = new List<Equipo>()
            { fixture.Partidos[103].EquipoLocal, fixture.Partidos[103].EquipoVisitante };

        foreach (Equipo equipo in finalistas)
        {
            List<Partido> partidosEquipo = fixture.Partidos
                .Where(p => p.EquipoLocal == equipo || p.EquipoVisitante == equipo)
                .OrderBy(p => p.Fecha)
                .ToList();

            for (int i = 1; i < partidosEquipo.Count; i++)
            {
                DateTime fechaAnterior = partidosEquipo[i - 1].Fecha.Value.Date;
                DateTime fechaActual = partidosEquipo[i].Fecha.Value.Date;

                int diferencia = (fechaActual - fechaAnterior).Days;
                Assert.IsTrue(diferencia >= 3);
            }
        }
    }

    public void simularGrupos()
    {
        Random rnd = new Random(SemillaSimulacion);
        for (int i = 0; i < 72; i++)
        {
            int golesL = rnd.Next(0, 4);
            int golesV = rnd.Next(0, 4);
            fixture.Partidos[i].AsignarResultado(golesL, golesV);
        }
    }

    public void simularDieciseisavos()
    {
        Random rnd = new Random(SemillaSimulacion);
        for (int i = 72; i < 88; i++)
        {
            int golesL = rnd.Next(0, 4);
            int golesV = rnd.Next(0, 4);
            while (golesV == golesL)
            {
                golesV = rnd.Next(0, 4);
            }
            fixture.Partidos[i].AsignarResultado(golesL, golesV);
        }
    }

    public void simularOctavos()
    {
        Random rnd = new Random(SemillaSimulacion);
        for (int i = 88; i < 96; i++)
        {
            int golesL = rnd.Next(0, 4);
            int golesV = rnd.Next(0, 4);
            while (golesV == golesL)
            {
                golesV = rnd.Next(0, 4);
            }
            fixture.Partidos[i].AsignarResultado(golesL, golesV);
        }
    }

    public void simularCuartos()
    {
        Random rnd = new Random(SemillaSimulacion);
        for (int i = 96; i < 100; i++)
        {
            int golesL = rnd.Next(0, 4);
            int golesV = rnd.Next(0, 4);
            while (golesV == golesL)
            {
                golesV = rnd.Next(0, 4);
            }
            fixture.Partidos[i].AsignarResultado(golesL, golesV);
        }
    }

    public void simularSemis()
    {
        Random rnd = new Random(SemillaSimulacion);
        for (int i = 100; i < 102; i++)
        {
            int golesL = rnd.Next(0, 4);
            int golesV = rnd.Next(0, 4);
            while (golesV == golesL)
            {
                golesV = rnd.Next(0, 4);
            }
            fixture.Partidos[i].AsignarResultado(golesL, golesV);
        }
    }
    
    [TestMethod]
    public void ObtenerGanadorYPerdedor_CuandoEmpateSinPenales_DevuelveNull()
    {
        Fixture fixture = new Fixture(1,new List<Grupo>(), CrearEstadios(), new Semilla(1, 1, 1, 1, 1), false);
        Partido partido = CrearPartidoEliminatorio();

        partido.AsignarResultado(1, 1);

        Assert.IsNull(fixture.ObtenerGanador(partido));
        Assert.IsNull(fixture.ObtenerPerdedor(partido));
    }

    [TestMethod]
    public void PartidosJugados_CuentaSoloPartidosTerminados()
    {
        Fixture fixture = new Fixture(1,new List<Grupo>(), CrearEstadios(), new Semilla(1, 1, 1, 1, 1), false);

        Partido grupoTerminado = CrearPartidoGrupo();
        grupoTerminado.AsignarResultado(1, 0);

        Partido finalEmpatadaSinPenales = CrearPartidoEliminatorio();
        finalEmpatadaSinPenales.AsignarResultado(1, 1);

        Partido finalConPenales = CrearPartidoEliminatorio();
        finalConPenales.AsignarResultado(1, 1);
        finalConPenales.AsignarPenales(4, 3);

        fixture.Partidos.Add(grupoTerminado);
        fixture.Partidos.Add(finalEmpatadaSinPenales);
        fixture.Partidos.Add(finalConPenales);
        
        Assert.AreEqual(2, fixture.PartidosJugados);
    }

    private List<Estadio> CrearEstadios()
    {
        return new List<Estadio>
        {
            new Estadio("E1", "C", 30000, "D"),
            new Estadio("E2", "C", 30000, "D"),
            new Estadio("E3", "C", 30000, "D"),
            new Estadio("E4", "C", 30000, "D")
        };
    }

    private Partido CrearPartidoGrupo()
    {
        return new Partido(1, 1, 'A', new DateTime(2026, 6, 1, 14, 0, 0), new Estadio("EG", "C", 30000, "D"),
            new Equipo(Guid.NewGuid().ToString(), "UEFA", 1800),
            new Equipo(Guid.NewGuid().ToString(), "CONMEBOL", 1700),
            "Fase de Grupos", 1);
    }

    private Partido CrearPartidoEliminatorio()
    {
        return new Partido(104, 1, null, new DateTime(2026, 7, 1, 14, 0, 0), new Estadio(Guid.NewGuid().ToString(), "C", 30000, "D"),
            new Equipo(Guid.NewGuid().ToString(), "UEFA", 1800),
            new Equipo(Guid.NewGuid().ToString(), "CONMEBOL", 1700),
            "Final", null);
    }
}



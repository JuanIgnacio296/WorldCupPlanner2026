using App.Dominio;
using Services;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class FixtureServiceTests
{
    private Fixture fixture;
    private FixtureService service;
    private List<Grupo> grupos;
    private List<Estadio> estadios;
    private List<Equipo> equipos;
    private Usuario editor = new Usuario("Editor", "Sist", "editor@ort.edu.uy", DateTime.Now.AddYears(-20), "Editor123#", new List<Rol> { Rol.Editor });
    private IAuditoriaService auditoriaService;
    private AppDbContext _context;
    
    [TestInitialize]
    public void setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

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

        grupos = new List<Grupo>()
        {
            g1, g2, g3, g4, g5, g6,
            g7, g8, g9, g10, g11, g12
        };

        estadios = new List<Estadio>()
        {
            new Estadio("Estadio1", "Ciudad", 40000, "Descripcion"),
            new Estadio("Estadio2", "Ciudad", 42000, "Descripcion"),
            new Estadio("Estadio3", "Ciudad", 45000, "Descripcion"),
            new Estadio("Estadio4", "Ciudad", 50000, "Descripcion"),
            new Estadio("Estadio5", "Ciudad", 55000, "Descripcion")
        };
        int tokenFixture = 123;
        Semilla semilla = new Semilla(1, tokenFixture,  456, 789, 0);
        fixture = new Fixture(1, grupos, estadios, semilla, true);
        InMemoryFixture memoryFixture = new InMemoryFixture();
        memoryFixture.AgregarFixture(tokenFixture ,fixture);
        InMemoryPartido inMemoryPartido = new InMemoryPartido();

        foreach (Partido partido in fixture.Partidos)
        {
            inMemoryPartido.AgregarPartido(partido);
        }

        InMemoryLog inMemoryLog = new InMemoryLog();
        auditoriaService = new AuditoriaService(inMemoryLog);
        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        service = new FixtureService(memoryFixture, partidoService, auditoriaService);
    }

    [TestMethod]
    public void GuardarFixture_WhenFixtureIsValid_ThenCanObtainFixtureByToken()
    {
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryLog inMemoryLog = new InMemoryLog();
        
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        FixtureService fixtureService = new FixtureService(inMemoryFixture, partidoService, auditoriaService);

        int token = 1;

        fixtureService.GuardarFixture(token, fixture);

        Fixture fixtureObtenido = fixtureService.ObtenerFixturePorToken(token);

        Assert.AreSame(fixture, fixtureObtenido);
    }

    [TestMethod]
    public void GuardarFixture_WhenTokenAlreadyExists_ThenDoesNotReplaceFixture()
    {
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryLog inMemoryLog = new InMemoryLog();

        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        FixtureService fixtureService = new FixtureService(inMemoryFixture, partidoService, auditoriaService);

        int token = 1;

        Semilla semilla1 = new Semilla(1,111, 222, 333, 444);
        Semilla semilla2 = new Semilla(2,555, 666, 777, 888);

        Fixture fixtureOriginal = new Fixture(1, grupos, estadios, semilla1, true);
        Fixture otroFixture = new Fixture(2, grupos, estadios, semilla2, true);

        fixtureService.GuardarFixture(token, fixtureOriginal);
        fixtureService.GuardarFixture(token, otroFixture);

        Fixture fixtureObtenido = fixtureService.ObtenerFixturePorToken(token);

        Assert.AreSame(fixtureOriginal, fixtureObtenido);
        Assert.AreEqual(token, fixtureService.ObtenerTokenActual());
    }

    [TestMethod]
    public void ObtenerFixturePorToken_WhenTokenDoesNotExist_ThenThrowsException()
    {
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryLog inMemoryLog = new InMemoryLog();
        
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        FixtureService fixtureService = new FixtureService(inMemoryFixture, partidoService, auditoriaService);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            fixtureService.ObtenerFixturePorToken(99);
        });
    }

    [TestMethod]
    public void ObtenerFixturePorTokenSemilla_WhenFixtureExists_ThenReturnsFixtureAndUpdatesCurrentToken()
    {
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryLog inMemoryLog = new InMemoryLog();

        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        FixtureService fixtureService = new FixtureService(inMemoryFixture, partidoService, auditoriaService);

        Fixture fixtureSemilla1 = new Fixture(1 ,grupos, estadios, new Semilla(1, 111, 1, 1, 1), true);
        Fixture fixtureSemilla2 = new Fixture(2, grupos, estadios, new Semilla(2, 222, 2, 2, 2), true);
        fixtureSemilla1.ArmarFase("Fase de Grupos");
        fixtureSemilla2.ArmarFase("Fase de Grupos");

        fixtureService.GuardarFixture(10, fixtureSemilla1);
        fixtureService.GuardarFixture(20, fixtureSemilla2);
        Assert.AreEqual(fixtureSemilla2.Partidos.First().Id, partidoService.ObtenerPartidos().First().Id);

        Fixture fixtureObtenido = fixtureService.ObtenerFixturePorTokenSemilla(1);

        Assert.AreSame(fixtureSemilla1, fixtureObtenido);
        Assert.AreEqual(10, fixtureService.ObtenerTokenActual());
        Assert.AreEqual(72, partidoService.ObtenerPartidos().Count);
        Assert.AreEqual(fixtureSemilla1.Partidos.First().Id, partidoService.ObtenerPartidos().First().Id);
    }

    [TestMethod]
    public void ObtenerFixture_WhenFixtureWasSaved_ThenReturnsCurrentFixture()
    {
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryLog inMemoryLog = new InMemoryLog();
        
        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        FixtureService fixtureService = new FixtureService(inMemoryFixture, partidoService, auditoriaService);

        int token = 1;

        fixtureService.GuardarFixture(token, fixture);

        Fixture fixtureObtenido = fixtureService.ObtenerFixture();

        Assert.AreSame(fixture, fixtureObtenido);
    }

    [TestMethod]
    public void GuardarFixture_WhenNewFixtureIsSaved_ThenResetsPartidosInMemory()
    {
        InMemoryFixture inMemoryFixture = new InMemoryFixture();
        InMemoryPartido inMemoryPartido = new InMemoryPartido();
        InMemoryLog inMemoryLog = new InMemoryLog();

        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IPartidoService partidoService = new PartidoService(auditoriaService, inMemoryPartido, new NotificacionService(_context, auditoriaService));
        FixtureService fixtureService = new FixtureService(inMemoryFixture, partidoService, auditoriaService);

        Partido partidoAnterior = new Partido(1, 2, 'A',
            new DateTime(2026, 6, 1, 14, 0, 0),
            estadios[0],
            equipos[0],
            equipos[1],
            null,
            null,
            "Fase de Grupos",
            "14:00",
            1
        );
        inMemoryPartido.AgregarPartido(partidoAnterior);

        fixture.ArmarFase("Fase de Grupos");

        fixtureService.GuardarFixture(1, fixture);

        List<Partido> partidosGuardados = partidoService.ObtenerPartidos();

        Assert.AreEqual(72, partidosGuardados.Count);
        Assert.IsFalse(partidosGuardados.Contains(partidoAnterior));
        CollectionAssert.AreEquivalent(fixture.Partidos, partidosGuardados);
    }

    [TestMethod]
    public void GenerarFaseDeGrupos_WhenCalled_ThenGeneratesPartidos()
    {
        service.GenerarFaseDeGrupos();

        Assert.IsTrue(service.ObtenerPartidos().Count > 0);
    }
    
    [TestMethod]
    public void ObtenerPartidosPorFase_WhenFaseExists_ThenReturnsCorrectPartidos()
    {
        service.GenerarFaseDeGrupos();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Fase de Grupos");

        Assert.AreEqual(72, partidos.Count);
    }
    
    [TestMethod]
    public void ObtenerFixture_WhenCalled_ThenReturnsFixture()
    {
        Fixture retorno = service.ObtenerFixture();

        Assert.AreEqual(fixture, retorno);
    }
    
    [TestMethod]
    public void MostrarCruces_WhenNoCrucesExist_ThenReturnsEmptyString()
    {
        string resultado = service.MostrarCruces();

        Assert.AreEqual("", resultado);
    }
    
    [TestMethod]
    public void MostrarCruces_WhenDieciseisavosExist_ThenShowsCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < fixture.Partidos.Count; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        string resultado = service.MostrarCruces();

        Assert.IsTrue(resultado.Contains("DIECISEISAVOS DE FINAL"));
    }
    
    [TestMethod]
    public void GenerarDieciseisavos_WhenFaseDeGruposFinished_ThenGeneratesCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Dieciseisavos de Final");

        Assert.IsTrue(partidos.Count > 0);
    }
    
    [TestMethod]
    public void GenerarOctavos_WhenDieciseisavosFinished_ThenGeneratesCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        for (int i = 72; i < 88; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarOctavos();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Octavos de Final");

        Assert.AreEqual(8, partidos.Count);
    }
    
    [TestMethod]
    public void GenerarCuartos_WhenOctavosFinished_ThenGeneratesCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        for (int i = 72; i < 88; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }
        
        service.GenerarOctavos();

        for (int i = 88; i < 96; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarCuartos();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Cuartos de Final");

        Assert.AreEqual(4, partidos.Count);
    }
    
    [TestMethod]
    public void GenerarSemifinal_WhenCuartosFinished_ThenGeneratesCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        for (int i = 72; i < 88; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }
        
        service.GenerarOctavos();

        for (int i = 88; i < 96; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarCuartos();

        for (int i = 96; i < 100; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarSemifinal();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Semifinal");

        Assert.AreEqual(2, partidos.Count);
    }
    
    [TestMethod]
    public void GenerarTercerYCuartoPuesto_WhenSemifinalFinished_ThenGeneratesCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        for (int i = 72; i < 88; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }
        
        service.GenerarOctavos();

        for (int i = 88; i < 96; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarCuartos();

        for (int i = 96; i < 100; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarSemifinal();

        for (int i = 100; i < 102; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarTercerYCuartoPuesto();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Tercer y Cuarto Puesto");

        Assert.AreEqual(1, partidos.Count);
    }
    
    [TestMethod]
    public void GenerarFinal_WhenSemifinalFinished_ThenGeneratesCruces()
    {
        service.GenerarFaseDeGrupos();

        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarDieciseisavos();

        for (int i = 72; i < 88; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }
        
        service.GenerarOctavos();

        for (int i = 88; i < 96; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarCuartos();

        for (int i = 96; i < 100; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarSemifinal();

        for (int i = 100; i < 102; i++)
        {
            fixture.Partidos[i].AsignarResultado(1, 0);
        }

        service.GenerarFinal();

        List<Partido> partidos = service.ObtenerPartidosPorFase("Final");

        Assert.AreEqual(1, partidos.Count);
    }

    [TestMethod]
    public void SimularFecha_WhenAllPreviousDateTimesAreNotPlayedYet_ThenThrowsException()
    {
        service.GenerarFaseDeGrupos();
        DateTime date = new DateTime(2026, 06, 10, 18, 0, 0);
        Assert.ThrowsException<InvalidOperationException>(() => { service.SimularFecha(date, editor, fixture.Semilla.RandomSimulacion);});
    }

    [TestMethod]
    public void SimularFecha_WhenDateTimeDoesNotExist_ThenThrowsException()
    {
        service.GenerarFaseDeGrupos();
        DateTime date = new DateTime(2026, 06, 01, 10, 25, 53);
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.SimularFecha(date, editor, fixture.Semilla.RandomSimulacion);
        });
    }

    [TestMethod]
    public void SimularFecha_WhenPartidoAlreadyHasResult_ThenDoesNotModifyResult()
    {
        service.GenerarFaseDeGrupos();
        fixture.Partidos[0].AsignarResultado(10, 1);
        DateTime date = fixture.Partidos[0].Fecha.Value;
        service.SimularFecha(date, editor, fixture.Semilla.RandomSimulacion);
        Assert.AreEqual(10, fixture.Partidos[0].GolesLocal);
        Assert.AreEqual(1, fixture.Partidos[0].GolesVisitante);
    }
    
    [TestMethod]
    public void SimularFecha_WhenDateExists_ThenSimulatesMatches()
    {
        service.GenerarFaseDeGrupos();

        DateTime date = fixture.Partidos[10].Fecha.Value;

        foreach (Partido partido in fixture.Partidos)
        {
            if (partido.Fecha < date)
            {
                partido.AsignarResultado(1, 0);
            }
        }

        service.SimularFecha(date, editor, fixture.Semilla.RandomSimulacion);

        List<Partido> partidosEnFecha = fixture.Partidos
            .Where(p => p.Fecha == date)
            .ToList();

        foreach (Partido partido in partidosEnFecha)
        {
            Assert.IsNotNull(partido.GolesLocal);
            Assert.IsNotNull(partido.GolesVisitante);
        }
    }
    
    [TestMethod]
    public void SimularJornada_WhenAllPreviousJornadasAreNotPlayedYet_ThenThrowsException()
    {
        service.GenerarFaseDeGrupos();
        Assert.ThrowsException<InvalidOperationException>(() => { service.SimularJornada(2, editor, fixture.Semilla.RandomSimulacion);});
    }
    
    [TestMethod]
    public void SimularJornada_WhenJornadaDoesNotExist_ThenThrowsException()
    {
        service.GenerarFaseDeGrupos();
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.SimularJornada(4, editor, fixture.Semilla.RandomSimulacion);
        });
    }

    [TestMethod]
    public void SimularJornada_WhenPartidoAlreadyHasResult_ThenDoesNotModifyResult()
    {
        service.GenerarFaseDeGrupos();
        fixture.Partidos[0].AsignarResultado(10, 1);
        int jornada = fixture.Partidos[0].Jornada.Value;
        service.SimularJornada(jornada, editor, fixture.Semilla.RandomSimulacion);
        Assert.AreEqual(10, fixture.Partidos[0].GolesLocal);
        Assert.AreEqual(1, fixture.Partidos[0].GolesVisitante);
    }
    
    [TestMethod]
    public void SimularJornada_WhenJornadaExists_ThenSimulatesMatches()
    {
        service.GenerarFaseDeGrupos();

        int jornada = fixture.Partidos[30].Jornada.Value;

        foreach (Partido partido in fixture.Partidos)
        {
            if (partido.Jornada.Value < jornada)
            {
                partido.AsignarResultado(1, 0);
            }
        }

        service.SimularJornada(jornada, editor, fixture.Semilla.RandomSimulacion);

        List<Partido> partidosEnFecha = fixture.Partidos
            .Where(p => p.Fase == "Fase de Grupos" &&  p.Jornada == jornada)
            .ToList();

        foreach (Partido partido in partidosEnFecha)
        {
            Assert.IsNotNull(partido.GolesLocal);
            Assert.IsNotNull(partido.GolesVisitante);
        }
    }
    
    [TestMethod]
    public void SimularFase_WhenAllPreviousFasesAreNotPlayedYet_ThenThrowsException()
    {
        service.GenerarFaseDeGrupos();
        Assert.ThrowsException<InvalidOperationException>(() => { service.SimularFase("Dieciseisavos de Final", editor, fixture.Semilla.RandomSimulacion);});
    }
    
    [TestMethod]
    public void SimularFase_WhenFaseDoesNotExist_ThenThrowsException()
    {
        service.GenerarFaseDeGrupos();
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.SimularFase("Fase que no existe", editor, fixture.Semilla.RandomSimulacion);
        });
    }

    [TestMethod]
    public void SimularFase_WhenPartidoAlreadyHasResult_ThenDoesNotModifyResult()
    {
        service.GenerarFaseDeGrupos();
        fixture.Partidos[0].AsignarResultado(10, 1);
        string fase = fixture.Partidos[0].Fase;
        service.SimularFase(fase, editor, fixture.Semilla.RandomSimulacion);
        Assert.AreEqual(10, fixture.Partidos[0].GolesLocal);
        Assert.AreEqual(1, fixture.Partidos[0].GolesVisitante);
    }
    
    [TestMethod]
    public void SimularFase_WhenFaseExists_ThenSimulatesMatches()
    {
        service.GenerarFaseDeGrupos();
        string fase = fixture.Partidos[0].Fase;

        foreach (Partido partido in fixture.Partidos)
        {
            if (partido.Fase == fase)
            {
                partido.AsignarResultado(1, 0);
            }
        }
        
        service.GenerarDieciseisavos();
        fase = fixture.Partidos[75].Fase;
        
        service.SimularFase(fase, editor, fixture.Semilla.RandomSimulacion);

        List<Partido> partidosDeFase = fixture.Partidos
            .Where(p => p.Fase == fase)
            .ToList();

        foreach (Partido partido in partidosDeFase)
        {
            Assert.IsNotNull(partido.GolesLocal);
            Assert.IsNotNull(partido.GolesVisitante);
        }
    }
}



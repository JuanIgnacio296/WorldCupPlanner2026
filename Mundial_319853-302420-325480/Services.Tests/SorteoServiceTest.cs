using App.Dominio;
using Services.Interfaces;
using Services;
using DataAccess;

namespace Services.Tests;

[TestClass]
public class SorteoServiceTests
{
    private InMemorySorteo _inMemorySorteo = null!;
    private InMemoryFixture _inMemoryFixture = null!;
    private InMemoryLog _inMemoryLog = null!;
    private InMemoryEquipo _inMemoryEquipo = null!;
    private InMemoryPartido _inMemoryPartido = null!;
    private InMemoryEstadio _inMemoryEstadio = null!;
    private InMemorySemilla _inMemorySemilla = null!;
    private IAuditoriaService _auditoriaService = null!;
    private Usuario _editor = null!;

    [TestInitialize]
    public void Setup()
    {
        _inMemoryLog = new InMemoryLog();
        _inMemorySorteo = new InMemorySorteo();
        _inMemoryFixture = new InMemoryFixture();
        _inMemoryEquipo = new InMemoryEquipo();
        _inMemoryPartido = new InMemoryPartido();
        _inMemoryEstadio = new InMemoryEstadio();
        _inMemorySemilla = new InMemorySemilla();

        _auditoriaService = new AuditoriaService(_inMemoryLog);

        _editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );
        
        _inMemorySemilla.AgregarSemilla(new Semilla(1, 1, 1, 1, 1));
        _inMemorySemilla.AgregarSemilla(new Semilla(2, 2, 2, 2, 2));
    }

    
    private SorteoService CrearService()
    {
        return new SorteoService(
            _auditoriaService,
            _inMemorySorteo,
            _inMemoryFixture,
            _inMemoryEquipo,
            _inMemoryPartido,
            _inMemoryEstadio,
            _inMemorySemilla
        );
    }

    [TestMethod]
    public void CrearSorteo_WhenCalled_ThenCanObtainSorteoByToken()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = new List<Equipo>();
        List<Estadio> estadios = new List<Estadio>();

        Sorteo sorteoCreado = service.CrearSorteo(_editor, equipos, estadios, 1);

        Sorteo sorteoObtenido = service.ObtenerSorteoPorToken(1);

        Assert.AreSame(sorteoCreado, sorteoObtenido);
    }

    [TestMethod]
    public void CrearSorteo_WhenCalledTwice_ThenAssignsDifferentTokens()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos1 = new List<Equipo>();
        List<Estadio> estadios1 = new List<Estadio>();

        List<Equipo> equipos2 = new List<Equipo>();
        List<Estadio> estadios2 = new List<Estadio>();

        Sorteo sorteo1 = service.CrearSorteo(_editor, equipos1, estadios1, 1);
        Sorteo sorteo2 = service.CrearSorteo(_editor, equipos2, estadios2, 1);

        Sorteo sorteoObtenido1 = service.ObtenerSorteoPorToken(1);
        Sorteo sorteoObtenido2 = service.ObtenerSorteoPorToken(2);

        Assert.AreSame(sorteo1, sorteoObtenido1);
        Assert.AreSame(sorteo2, sorteoObtenido2);
        Assert.AreNotSame(sorteoObtenido1, sorteoObtenido2);
    }

    [TestMethod]
    public void ObtenerSorteoPorToken_WhenTokenExists_ThenReturnsSorteo()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = new List<Equipo>();
        List<Estadio> estadios = new List<Estadio>();

        Sorteo sorteoCreado = service.CrearSorteo(_editor, equipos, estadios, 1);

        Sorteo sorteoObtenido = service.ObtenerSorteoPorToken(1);

        Assert.AreSame(sorteoCreado, sorteoObtenido);
    }

    [TestMethod]
    public void ObtenerSorteoPorToken_WhenTokenDoesNotExist_ThenThrowsException()
    {
        SorteoService service = CrearService();

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.ObtenerSorteoPorToken(99);
        });
    }

    [TestMethod]
    public void SortearEquipos_WhenSorteoExists_ThenReturns12Groups()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        List<Grupo> grupos = service.SortearEquipos(_editor, 1);

        Assert.AreEqual(12, grupos.Count);
    }

    [TestMethod]
    public void SortearEquipos_WhenSorteoExists_ThenEachGroupHas4Teams()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        List<Grupo> grupos = service.SortearEquipos(_editor, 1);

        foreach (Grupo grupo in grupos)
        {
            Assert.AreEqual(4, grupo.Equipos.Count);
        }
    }

    [TestMethod]
    public void SortearEquipos_WhenTokenDoesNotExist_ThenThrowsException()
    {
        SorteoService service = CrearService();

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.SortearEquipos(_editor, 99);
        });
    }

    [TestMethod]
    public void CrearFixture_WhenSorteoTieneGruposYEstadios_ThenCrea72Partidos()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixture = service.CrearFixture(_editor, 1, true);

        Assert.AreEqual(72, fixture.Partidos.Count);
    }

    [TestMethod]
    public void CrearFixture_AlAsignarResultadoEnFaseDeGrupos_ActualizaEstadisticasDeLosEquipos()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);
        service.SortearEquipos(_editor, 1);
        Fixture fixture = service.CrearFixture(_editor, 1, true);

        fixture.Partidos[0].AsignarResultado(3, 0);

        Assert.IsTrue(fixture.Grupos.SelectMany(grupo => grupo.Equipos).Any(equipo => equipo.Puntos > 0));
    }

    [TestMethod]
    public void CrearFixture_WhenTokenDoesNotExist_ThenThrowsException()
    {
        SorteoService service = CrearService();

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearFixture(_editor, 99, true);
        });
    }

    [TestMethod]
    public void CrearFixture_WhenEquiposNoFueronSorteados_ThenThrowsException()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearFixture(_editor, 1, true);
        });
    }

    [TestMethod]
    public void CrearFixture_WhenGruposNoTienen48Equipos_ThenThrowsException()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        Sorteo sorteo = service.CrearSorteo(_editor, equipos, estadios, 1);

        sorteo.CrearGrupos();

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearFixture(_editor, 1, true);
        });
    }

    [TestMethod]
    public void CrearFixture_WhenFixtureAlreadyExists_ThenThrowsException()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);
        service.CrearFixture(_editor, 1, true);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearFixture(_editor, 1, true);
        });
    }

    [TestMethod]
    public void SortearEquipos_WhenEquiposAlreadySorted_ThenThrowsException()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.SortearEquipos(_editor, 1);
        });
    }

    [TestMethod]
    public void ObtenerFixturePorToken_WhenFixtureExists_ThenReturnsFixture()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixtureCreado = service.CrearFixture(_editor, 1, true);

        Fixture fixtureObtenido = service.ObtenerFixturePorToken(_editor, 1);

        Assert.AreSame(fixtureCreado, fixtureObtenido);
    }

    [TestMethod]
    public void ObtenerFixturePorToken_WhenFixtureDoesNotExist_ThenThrowsException()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.ObtenerFixturePorToken(_editor, 1);
        });
    }

    [TestMethod]
    public void CrearSorteo_ConSemillaRepetida_DebeLanzarExcepcion()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos1 = new List<Equipo>();
        List<Estadio> estadios1 = new List<Estadio>();

        List<Equipo> equipos2 = new List<Equipo>();
        List<Estadio> estadios2 = new List<Estadio>();

        Semilla semilla = new Semilla(1,10, 20, 30, 40);

        service.CrearSorteo(_editor, equipos1, estadios1, semilla);

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearSorteo(_editor, equipos2, estadios2, semilla);
        });
    }

    [TestMethod]
    public void CrearSorteo_SinSemilla_DebeGenerarSemillasIncrementales()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos1 = new List<Equipo>();
        List<Estadio> estadios1 = new List<Estadio>();

        List<Equipo> equipos2 = new List<Equipo>();
        List<Estadio> estadios2 = new List<Estadio>();

        Sorteo sorteo1 = service.CrearSorteo(_editor, equipos1, estadios1, 1);
        Sorteo sorteo2 = service.CrearSorteo(_editor, equipos2, estadios2, 2);

        Assert.AreEqual(1, sorteo1.Semilla.SemillaFixture);
        Assert.AreEqual(1, sorteo1.Semilla.SemillaCompletar);
        Assert.AreEqual(1, sorteo1.Semilla.SemillaCrucesFase);
        Assert.AreEqual(1, sorteo1.Semilla.SemillaSimulacion);

        Assert.AreEqual(2, sorteo2.Semilla.SemillaFixture);
        Assert.AreEqual(2, sorteo2.Semilla.SemillaCompletar);
        Assert.AreEqual(2, sorteo2.Semilla.SemillaCrucesFase);
        Assert.AreEqual(2, sorteo2.Semilla.SemillaSimulacion);
    }

    private List<Equipo> Crear48Equipos()
    {
        List<Equipo> equiposCreados = new List<Equipo>();

        AgregarEquipos(equiposCreados, "UEFA", 16, 2000);
        AgregarEquipos(equiposCreados, "CONMEBOL", 7, 1800);
        AgregarEquipos(equiposCreados, "CONCACAF", 7, 1600);
        AgregarEquipos(equiposCreados, "CAF", 9, 1400);
        AgregarEquipos(equiposCreados, "AFC", 8, 1200);
        AgregarEquipos(equiposCreados, "OFC", 1, 1000);

        return equiposCreados;
    }

    private void AgregarEquipos(List<Equipo> equipos, string confederacion, int cantidad, int rankingInicial)
    {
        for (int i = 1; i <= cantidad; i++)
        {
            equipos.Add(new Equipo($"{confederacion}_{i}", confederacion, rankingInicial + i));
        }
    }

    private List<Estadio> Crear4Estadios()
    {
        return new List<Estadio>
        {
            new Estadio("Estadio A", "Ciudad A", 30000, "Descripcion A"),
            new Estadio("Estadio B", "Ciudad B", 35000, "Descripcion B"),
            new Estadio("Estadio C", "Ciudad C", 40000, "Descripcion C"),
            new Estadio("Estadio D", "Ciudad D", 45000, "Descripcion D")
        };
    }
    
    [TestMethod]
    public void CrearFixture_WhenSorteoNoTieneEstadios_ThenCompleta4EstadiosPorDefecto()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = new List<Estadio>();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixture = service.CrearFixture(_editor, 1, true);

        Assert.AreEqual(4, fixture.Estadios.Count);
        Assert.AreEqual(4, _inMemoryEstadio.ObtenerEstadios().Count);
    }
    
    [TestMethod]
    public void CrearFixture_WhenSorteoTiene2Estadios_ThenCompletaHasta4Estadios()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();

        List<Estadio> estadios = new List<Estadio>
        {
            new Estadio("Estadio Existente 1", "Ciudad A", 30000, "Descripcion A"),
            new Estadio("Estadio Existente 2", "Ciudad B", 35000, "Descripcion B")
        };

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixture = service.CrearFixture(_editor, 1, true);

        Assert.AreEqual(4, fixture.Estadios.Count);
        Assert.AreEqual(2, fixture.Estadios.Count(e => e.Nombre.StartsWith("Estadio Existente")));
        Assert.AreEqual(2, fixture.Estadios.Count(e => e.Nombre.StartsWith("Estadio generado")));
    }
    
    [TestMethod]
    public void CrearFixture_WhenYaHay4EstadiosEnMemoria_ThenUsaEsosEstadiosYNoGeneraNuevos()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadiosDelSorteo = new List<Estadio>();

        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 1", "Ciudad A", 30000, "Descripcion A"));
        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 2", "Ciudad B", 35000, "Descripcion B"));
        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 3", "Ciudad C", 40000, "Descripcion C"));
        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 4", "Ciudad D", 45000, "Descripcion D"));

        service.CrearSorteo(_editor, equipos, estadiosDelSorteo, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixture = service.CrearFixture(_editor, 1, true);

        Assert.AreEqual(4, fixture.Estadios.Count);
        Assert.AreEqual(4, _inMemoryEstadio.ObtenerEstadios().Count);

        Assert.IsTrue(fixture.Estadios.Any(e => e.Nombre == "Estadio Memoria 1"));
        Assert.IsTrue(fixture.Estadios.Any(e => e.Nombre == "Estadio Memoria 2"));
        Assert.IsTrue(fixture.Estadios.Any(e => e.Nombre == "Estadio Memoria 3"));
        Assert.IsTrue(fixture.Estadios.Any(e => e.Nombre == "Estadio Memoria 4"));
    }
    
    [TestMethod]
    public void CrearFixture_WhenSorteoYaTiene4Estadios_ThenNoAgregaEstadiosPorDefecto()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();
        List<Estadio> estadios = Crear4Estadios();

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixture = service.CrearFixture(_editor, 1, true);

        Assert.AreEqual(4, fixture.Estadios.Count);
        Assert.AreEqual(0, _inMemoryEstadio.ObtenerEstadios().Count);

        Assert.IsFalse(fixture.Estadios.Any(e => e.Nombre.StartsWith("Estadio generado")));
    }
    
    [TestMethod]
    public void CrearFixture_WhenEstadioYaEstaEnSorteoYMemoria_ThenNoDuplicaEstadio()
    {
        SorteoService service = CrearService();

        List<Equipo> equipos = Crear48Equipos();

        Estadio estadioCompartido = new Estadio(
            "Estadio Compartido",
            "Ciudad A",
            30000,
            "Descripcion A"
        );

        List<Estadio> estadios = new List<Estadio>
        {
            estadioCompartido
        };

        _inMemoryEstadio.AgregarEstadio(estadioCompartido);
        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 2", "Ciudad B", 35000, "Descripcion B"));
        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 3", "Ciudad C", 40000, "Descripcion C"));
        _inMemoryEstadio.AgregarEstadio(new Estadio("Estadio Memoria 4", "Ciudad D", 45000, "Descripcion D"));

        service.CrearSorteo(_editor, equipos, estadios, 1);

        service.SortearEquipos(_editor, 1);

        Fixture fixture = service.CrearFixture(_editor, 1, true);

        Assert.AreEqual(4, fixture.Estadios.Count);
        Assert.AreEqual(1, fixture.Estadios.Count(e => e.Nombre == "Estadio Compartido"));
    }
    
    [TestMethod]
    public void CrearSorteo_ConTokenSemilla_CuandoUsuarioNoEsEditor_DebeLanzarExcepcion()
    {
        SorteoService service = CrearService();

        Usuario noEditor = new Usuario(
            "Usuario",
            "SinRol",
            "sinrol@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Sinrol123#",
            [Rol.Ninguno]
        );

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearSorteo(noEditor, new List<Equipo>(), new List<Estadio>(), 1);
        });

        Assert.AreEqual("Solo pueden crear sorteos los editores.", ex.Message);
    }

    [TestMethod]
    public void CrearSorteo_ConTokenSemilla_CuandoSemillaNoExiste_DebeCrearYGuardarSemilla()
    {
        SorteoService service = CrearService();

        int tokenSemillaInexistente = 99;

        Sorteo sorteo = service.CrearSorteo(
            _editor,
            new List<Equipo>(),
            new List<Estadio>(),
            tokenSemillaInexistente
        );

        Semilla semillaGuardada = _inMemorySemilla.ObtenerSemilla(tokenSemillaInexistente);

        Assert.AreEqual(tokenSemillaInexistente, sorteo.Semilla.Token);
        Assert.AreEqual(tokenSemillaInexistente, sorteo.Semilla.SemillaFixture);
        Assert.AreEqual(tokenSemillaInexistente, sorteo.Semilla.SemillaCompletar);
        Assert.AreEqual(tokenSemillaInexistente, sorteo.Semilla.SemillaCrucesFase);
        Assert.AreEqual(tokenSemillaInexistente, sorteo.Semilla.SemillaSimulacion);
        Assert.AreSame(sorteo.Semilla, semillaGuardada);
    }
    [TestMethod]
    public void CrearSorteo_ConSemilla_CuandoUsuarioNoEsEditor_DebeLanzarExcepcion()
    {
        SorteoService service = CrearService();

        Usuario noEditor = new Usuario(
            "Usuario",
            "SinRol",
            "sinrol2@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Sinrol123#",
            [Rol.Ninguno]
        );

        Semilla semilla = new Semilla(10, 10, 20, 30, 40);

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearSorteo(noEditor, new List<Equipo>(), new List<Estadio>(), semilla);
        });

        Assert.AreEqual("Solo pueden crear sorteos los editores.", ex.Message);
    }

    [TestMethod]
    public void SortearEquipos_CuandoUsuarioNoEsEditor_DebeLanzarExcepcion()
    {
        SorteoService service = CrearService();

        Usuario noEditor = new Usuario(
            "Usuario",
            "SinRol",
            "sinrol3@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Sinrol123#",
            [Rol.Ninguno]
        );

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.SortearEquipos(noEditor, 1);
        });

        Assert.AreEqual("Solo pueden sortear equipos los editores.", ex.Message);
    }
    [TestMethod]
    public void CrearFixture_CuandoUsuarioNoEsEditor_DebeLanzarExcepcion()
    {
        SorteoService service = CrearService();

        Usuario noEditor = new Usuario(
            "Usuario",
            "SinRol",
            "sinrol4@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Sinrol123#",
            [Rol.Ninguno]
        );

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.CrearFixture(noEditor, 1, true);
        });

        Assert.AreEqual("Solo pueden crear fixture los editores.", ex.Message);
    }

    [TestMethod]
    public void ObtenerFixturePorToken_CuandoUsuarioNoEsEditor_DebeLanzarExcepcion()
    {
        SorteoService service = CrearService();

        Usuario noEditor = new Usuario(
            "Usuario",
            "SinRol",
            "sinrol5@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Sinrol123#",
            [Rol.Ninguno]
        );

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.ObtenerFixturePorToken(noEditor, 1);
        });

        Assert.AreEqual("Solo pueden consultar fixture editores.", ex.Message);
    }
    
}



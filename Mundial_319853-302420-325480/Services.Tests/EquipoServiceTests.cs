using App.Dominio;
using DataAccess;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class EquipoServiceTests
{
    private IAuditoriaService _auditoriaService;
    private InMemoryEquipo _inMemoryEquipo;
    private Usuario _admin;
        
    [TestInitialize]
    public void Setup()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        _auditoriaService = new AuditoriaService(inMemoryLog);
        _inMemoryEquipo = new InMemoryEquipo();
        _admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );
    }
    
    [TestMethod]
    public void CrearEquipo_CuandoEsAdmin_DebeAgregarALaLista()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        
        Equipo nuevoEquipo = new Equipo("Uruguay", "CONMEBOL", 400);

        servicio.CrearEquipo(admin, nuevoEquipo);

        Assert.AreEqual(1, servicio.ObtenerEquipos().Count);
        Assert.AreEqual("Uruguay", servicio.ObtenerEquipos()[0].Nombre);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void CrearEquipo_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario editor = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", new List<Rol> { Rol.Editor });
        Equipo nuevoEquipo = new Equipo("Brasil", "CONMEBOL", 330);

        servicio.CrearEquipo(editor, nuevoEquipo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearEquipo_NombreDuplicado_DebeLanzarExcepcion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
    
        Equipo e1 = new Equipo("Argentina", "CONMEBOL", 450);
        Equipo e2 = new Equipo("Argentina", "CONMEBOL", 465);

        servicio.CrearEquipo(admin, e1);
        servicio.CrearEquipo(admin, e2);
    }
    
    [TestMethod]
    public void EliminarEquipo_CuandoEsAdminYExiste_DebeQuitarloDeLaLista()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Equipo equipoAEliminar = new Equipo("Argentina", "CONMEBOL", 1500);
    
        servicio.CrearEquipo(admin, equipoAEliminar);

        servicio.EliminarEquipo(admin, "Argentina");

        Assert.AreEqual(0, servicio.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void EliminarEquipo_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario noAutorizado = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", new List<Rol> { Rol.Ninguno });

        servicio.EliminarEquipo(noAutorizado, "Argentina");
    }
    
    [TestMethod]
    public void ModificarEquipo_CuandoExiste_DebeActualizarLosDatos()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Equipo original = new Equipo("Argentina", "CONMEBOL", 1500);
    
        servicio.CrearEquipo(admin, original);

        Equipo modificado = new Equipo("Argentina", "CONMEBOL", 2000);

        servicio.ModificarEquipo(admin, "Argentina", modificado);

        Equipo equipoEnLista = servicio.ObtenerEquipos().First(e => e.Nombre == "Argentina");
        Assert.AreEqual(2000, equipoEnLista.RankingFifa);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void ModificarEquipo_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario noAutorizado = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", new List<Rol> { Rol.Ninguno });
        Equipo modificado = new Equipo("Argentina", "CONMEBOL", 2000);

        servicio.ModificarEquipo(noAutorizado, "Argentina", modificado);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ModificarEquipo_NombreDuplicado_DebeLanzarExcepcion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        EquipoService servicio = new EquipoService(_auditoriaService, inMemoryEquipo);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Equipo e1 = new Equipo("Argentina", "CONMEBOL", 1500);
        Equipo e2 = new Equipo("Brasil", "CONMEBOL", 1600);

        servicio.CrearEquipo(admin, e1);
        servicio.CrearEquipo(admin, e2);

        Equipo modificado = new Equipo("Argentina", "CONMEBOL", 1600);

        servicio.ModificarEquipo(admin, "Brasil", modificado);
    }
    
    [TestMethod]
    public void CompletarEquiposFaltantes_CuandoNoHayEquipos_DebeGenerar48Equipos()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        
        equipoService.CompletarEquiposFaltantes(_admin);
        Assert.AreEqual(48, equipoService.ObtenerCantidadTotal());
    }
    
    [TestMethod]
    public void CompletarEquiposFaltantes_DebeRespetarCuposPorConfederacion()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        
        equipoService.CompletarEquiposFaltantes(_admin);

        Assert.AreEqual(16, equipoService.ContarPorConfederacion("UEFA"), "Falla en UEFA");
        Assert.AreEqual(7, equipoService.ContarPorConfederacion("CONMEBOL"), "Falla en CONMEBOL");
        Assert.AreEqual(7, equipoService.ContarPorConfederacion("CONCACAF"), "Falla en CONCACAF");
        Assert.AreEqual(9, equipoService.ContarPorConfederacion("CAF"), "Falla en CAF");
        Assert.AreEqual(8, equipoService.ContarPorConfederacion("AFC"), "Falla en AFC");
        Assert.AreEqual(1, equipoService.ContarPorConfederacion("OFC"), "Falla en OFC");
    }
    
    [TestMethod]
    public void CompletarEquiposFaltantes_SiSeEjecutaDosVeces_NoDebeAgregarEquiposExtra()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        
        equipoService.CompletarEquiposFaltantes(_admin);
        equipoService.CompletarEquiposFaltantes(_admin);

        Assert.AreEqual(48, equipoService.ObtenerCantidadTotal());
    }
    
    [TestMethod]
    public void CompletarEquiposFaltantes_ConEquiposPrevios_SoloAgregaLosFaltantes()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        
        equipoService.CrearEquipo(_admin, new Equipo("Uruguay", "CONMEBOL", 2000));
        equipoService.CrearEquipo(_admin, new Equipo("Argentina", "CONMEBOL", 2100));

        equipoService.CompletarEquiposFaltantes(_admin);

        Assert.AreEqual(7, equipoService.ContarPorConfederacion("CONMEBOL"));
        Assert.AreEqual(48, equipoService.ObtenerCantidadTotal());
    }
    
    [TestMethod]
    public void CompletarEquiposFaltantes_GeneraRankingsDentroDelRangoPermitido()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        
        equipoService.CompletarEquiposFaltantes(_admin);
        var todos = equipoService.ObtenerEquipos();

        foreach (var eq in todos)
        {
            Assert.IsTrue(eq.RankingFifa >= 300 && eq.RankingFifa <= 2500,
                $"El equipo {eq.Nombre} tiene un ranking fuera de rango: {eq.RankingFifa}");
        }
    }

    [TestMethod]
    public void CompletarEquiposFaltantes_ConNombreGeneradoYaEnUso_AlcanzaElCupoIgual()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        equipoService.CrearEquipo(_admin, new Equipo("CONMEBOL_03", "CONMEBOL", 1800));

        equipoService.CompletarEquiposFaltantes(_admin);

        Assert.AreEqual(7, equipoService.ContarPorConfederacion("CONMEBOL"));
        Assert.AreEqual(48, equipoService.ObtenerCantidadTotal());
    }
}



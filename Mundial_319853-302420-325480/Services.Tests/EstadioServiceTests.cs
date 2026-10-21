using App.Dominio;
using Services.Interfaces;
using Services;
using DataAccess;

namespace Services.Tests;

[TestClass]
public class EstadioServiceTest
{
    IAuditoriaService _auditoriaService;

    [TestInitialize]
    public void Inicializar()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        _auditoriaService = new AuditoriaService(inMemoryLog);
    }
    
    [TestMethod]
    public void CrearEstadio_CuandoEsAdmin_DebeAgregarALaLista()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Estadio nuevoEstadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");

        servicio.CrearEstadio(admin, nuevoEstadio);

        Assert.AreEqual(1, servicio.ObtenerEstadios().Count);
        Assert.AreEqual("Centenario", servicio.ObtenerEstadios()[0].Nombre);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void CrearEstadio_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService service = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario noAdmin = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", new List<Rol> { Rol.Editor });
        Estadio nuevoEstadio = new Estadio("Centenario", "Montevideo", 60000, "Estadio Historico");

        service.CrearEstadio(noAdmin, nuevoEstadio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearEstadio_NombreDuplicado_DebeLanzarExcepcion()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
    
        Estadio e1 = new Estadio("Centenario", "Montevideo", 60000, "Historico");
        Estadio e2 = new Estadio("Centenario", "Canelones", 20000, "Moderno");

        servicio.CrearEstadio(admin, e1);
        servicio.CrearEstadio(admin, e2);
    }
    
    [TestMethod]
    public void EliminarEstadio_CuandoEsAdminYExiste_DebeQuitarloDeLaLista()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Estadio estadioAEliminar = new Estadio("Centenario", "Montevideo", 60000, "Historico");
    
        servicio.CrearEstadio(admin, estadioAEliminar);

        servicio.EliminarEstadio(admin, "Centenario");

        Assert.AreEqual(0, servicio.ObtenerEstadios().Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void EliminarEstadio_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario noAdmin = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", new List<Rol> { Rol.Editor });

        servicio.EliminarEstadio(noAdmin, "Centenario");
    }
    
    [TestMethod]
    public void ModificarEstadio_CuandoExiste_DebeActualizarLosDatos()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Estadio original = new Estadio("Centenario", "Montevideo", 60000, "Original");
        servicio.CrearEstadio(admin, original);

        Estadio modificado = new Estadio("Centenario", "Montevideo", 65000, "Remodelado");

        servicio.ModificarEstadio(admin, "Centenario", modificado);

        Estadio estadioEnLista = servicio.ObtenerEstadios().First(e => e.Nombre == "Centenario");
        Assert.AreEqual(65000, estadioEnLista.Capacidad);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void ModificarEstadio_CuandoUsuarioNoEsAdmin_DebeLanzarExcepcion()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario noAdmin = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", new List<Rol> { Rol.Editor });
        Estadio modificado = new Estadio("Centenario", "Montevideo", 65000, "Remodelacion");

        servicio.ModificarEstadio(noAdmin, "Centenario", modificado);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ModificarEstadio_NombreDuplicado_DebeLanzarExcepcion()
    {
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();
        EstadioService servicio = new EstadioService(_auditoriaService, inMemoryEstadio);
        Usuario admin = new Usuario("Admin", "Sist", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Admin123#", new List<Rol> { Rol.Administrador });
        Estadio e1 = new Estadio("Estadio A", "Ciudad A", 30000, "Desc");
        Estadio e2 = new Estadio("Estadio B", "Ciudad B", 35000, "Desc");
        servicio.CrearEstadio(admin, e1);
        servicio.CrearEstadio(admin, e2);

        Estadio intentoDuplicado = new Estadio("Estadio A", "Ciudad A", 40000, "Desc");

        servicio.ModificarEstadio(admin, "Estadio B", intentoDuplicado);
    }
    
    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_WhenNoHayEstadios_ThenCrea4Estadios()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();

        IEstadioService service = new EstadioService(auditoriaService, inMemoryEstadio);

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        service.CompletarEstadiosMinimosParaFixture(admin);

        List<Estadio> estadios = service.ObtenerEstadios();

        Assert.AreEqual(4, estadios.Count);
    }
    
    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_WhenHay2Estadios_ThenCompletaHasta4()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();

        IEstadioService service = new EstadioService(auditoriaService, inMemoryEstadio);

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        Estadio estadio1 = new Estadio("Estadio Existente 1", "Ciudad A", 30000, "Descripcion A");
        Estadio estadio2 = new Estadio("Estadio Existente 2", "Ciudad B", 35000, "Descripcion B");

        service.CrearEstadio(admin, estadio1);
        service.CrearEstadio(admin, estadio2);

        service.CompletarEstadiosMinimosParaFixture(admin);

        List<Estadio> estadios = service.ObtenerEstadios();

        Assert.AreEqual(4, estadios.Count);
        Assert.AreEqual(2, estadios.Count(e => e.Nombre.StartsWith("Estadio Existente")));
        Assert.AreEqual(2, estadios.Count(e => e.Nombre.StartsWith("Estadio generado")));
    }
    
    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_WhenYaHay4Estadios_ThenNoCreaNuevos()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();

        IEstadioService service = new EstadioService(auditoriaService, inMemoryEstadio);

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        service.CrearEstadio(admin, new Estadio("Estadio 1", "Ciudad A", 30000, "Descripcion A"));
        service.CrearEstadio(admin, new Estadio("Estadio 2", "Ciudad B", 35000, "Descripcion B"));
        service.CrearEstadio(admin, new Estadio("Estadio 3", "Ciudad C", 40000, "Descripcion C"));
        service.CrearEstadio(admin, new Estadio("Estadio 4", "Ciudad D", 45000, "Descripcion D"));

        service.CompletarEstadiosMinimosParaFixture(admin);

        List<Estadio> estadios = service.ObtenerEstadios();

        Assert.AreEqual(4, estadios.Count);
        Assert.AreEqual(0, estadios.Count(e => e.Nombre.StartsWith("Estadio generado")));
    }
    
    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_WhenUsuarioNoEsAdministrador_ThenLanzaExcepcion()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();

        IEstadioService service = new EstadioService(auditoriaService, inMemoryEstadio);

        Usuario noAdmin = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            service.CompletarEstadiosMinimosParaFixture(noAdmin);
        });

        Assert.AreEqual(0, service.ObtenerEstadios().Count);
    }
    
    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_WhenCreaEstadios_ThenRegistraLogs()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();

        IEstadioService service = new EstadioService(auditoriaService, inMemoryEstadio);

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        service.CompletarEstadiosMinimosParaFixture(admin);

        List<Log> logs = auditoriaService.ObtenerLogs(admin);

        Assert.AreEqual(4, logs.Count);
        Assert.IsTrue(logs[0].Accion.Contains("Creación de estadio por defecto"));
    }
    
    [TestMethod]
    public void CompletarEstadiosMinimosParaFixture_WhenYaExisteEstadioGenerado_ThenNoDuplica()
    {
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        InMemoryEstadio inMemoryEstadio = new InMemoryEstadio();

        IEstadioService service = new EstadioService(auditoriaService, inMemoryEstadio);

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        service.CrearEstadio(
            admin,
            new Estadio(
                "Estadio generado 1",
                "Montevideo",
                60000,
                "Estadio generado automáticamente para completar el fixture"
            )
        );

        service.CompletarEstadiosMinimosParaFixture(admin);

        List<Estadio> estadios = service.ObtenerEstadios();

        Assert.AreEqual(4, estadios.Count);
        Assert.AreEqual(1, estadios.Count(e => e.Nombre == "Estadio generado 1"));
    }
    
    [TestMethod]
    public void CrearEquipo_CuandoYaHay48Equipos_DebeLanzarExcepcion()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());
        
        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        AgregarEquiposHastaCompletarCupos(equipoService, admin);

        Equipo equipoExtra = new Equipo("EquipoExtra", "UEFA", 1500);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() =>
        {
            equipoService.CrearEquipo(admin, equipoExtra);
        });

        Assert.AreEqual("No se pueden registrar más de 48 equipos.", ex.Message);
        Assert.AreEqual(48, equipoService.ObtenerCantidadTotal());
    }

    [TestMethod]
    public void CrearEquipo_CuandoConfederacionYaTieneCupoMaximo_DebeLanzarExcepcion()
    {
        EquipoService equipoService = new EquipoService(_auditoriaService, new InMemoryEquipo());

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );
        
        for (int i = 1; i <= 16; i++)
        {
            equipoService.CrearEquipo(admin, new Equipo($"UEFA_{i}", "UEFA", 1500 + i));
        }

        Equipo equipoExtra = new Equipo("UEFA_Extra", "UEFA", 1800);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() =>
        {
            equipoService.CrearEquipo(admin, equipoExtra);
        });

        Assert.AreEqual("No se pueden registrar más equipos de UEFA.", ex.Message);
        Assert.AreEqual(16, equipoService.ContarPorConfederacion("UEFA"));
    }

    private void AgregarEquiposHastaCompletarCupos(EquipoService equipoService, Usuario admin)
    {
        AgregarEquipos(equipoService, admin, "UEFA", 16);
        AgregarEquipos(equipoService, admin, "CONMEBOL", 7);
        AgregarEquipos(equipoService, admin, "CONCACAF", 7);
        AgregarEquipos(equipoService, admin, "CAF", 9);
        AgregarEquipos(equipoService, admin, "AFC", 8);
        AgregarEquipos(equipoService, admin, "OFC", 1);
    }

    private void AgregarEquipos(EquipoService equipoService, Usuario admin, string confederacion, int cantidad)
    {
        for (int i = 1; i <= cantidad; i++)
        {
            equipoService.CrearEquipo(
                admin,
                new Equipo($"{confederacion}_{i}", confederacion, 1000 + i)
            );
        }
    }
}



using App.Dominio;
using Services.Interfaces;
using DataAccess;

namespace Services.Tests;

[TestClass]
public class UsuarioServiceTest
{
    private IUsuarioService _servicio;
    private IAuditoriaService _auditoriaService;
    private InMemoryUsuario _inMemoryUsuario;
    private InMemoryLog _inMemoryLog;
    private Usuario _admin;
    private Usuario _noAdmin;

    [TestInitialize]
    public void Setup()
    {
        _inMemoryUsuario = new InMemoryUsuario();
        _inMemoryLog = new InMemoryLog();

        _auditoriaService = new AuditoriaService(_inMemoryLog);
        _servicio = new UsuarioService(_auditoriaService, _inMemoryUsuario);

        _admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            new List<Rol> { Rol.Administrador }
        );

        _noAdmin = new Usuario(
            "Juan",
            "Perez",
            "juan@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Juan123#",
            new List<Rol> { Rol.Ninguno }
        );
    }

    [TestMethod]
    public void CrearUsuario_CuandoEsAdmin_DebeAgregarALaLista()
    {
        Usuario nuevo = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#"
        );

        _servicio.CrearUsuario(_admin, nuevo);

        Assert.AreEqual(5, _servicio.ObtenerUsuarios(_admin).Count);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void CrearUsuario_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        Usuario nuevoUsuario = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#",
            new List<Rol> { Rol.Ninguno }
        );

        _servicio.CrearUsuario(_noAdmin, nuevoUsuario);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearUsuario_ConEmailRepetido_DebeLanzarExcepcion()
    {
        Usuario u1 = new Usuario(
            "Ana",
            "Gomez",
            "repetido@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#"
        );

        Usuario u2 = new Usuario(
            "Pedro",
            "Sosa",
            "repetido@test.com",
            DateTime.Now.AddYears(-20),
            "Pedro123#"
        );

        _servicio.CrearUsuario(_admin, u1);
        _servicio.CrearUsuario(_admin, u2);
    }

    [TestMethod]
    public void EliminarUsuario_CuandoEsAdminYExiste_DebeQuitarloDeLaLista()
    {
        Usuario usuarioAEliminar = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#"
        );

        _servicio.CrearUsuario(_admin, usuarioAEliminar);

        _servicio.EliminarUsuario(_admin, "ana@test.com");
        
        Assert.AreEqual(4, _servicio.ObtenerUsuarios(_admin).Count);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void EliminarUsuario_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        _servicio.EliminarUsuario(_noAdmin, "cualquier@correo.com");
    }

    [TestMethod]
    public void ModificarUsuario_CuandoExiste_DebeActualizarLosDatos()
    {
        Usuario original = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#"
        );

        _servicio.CrearUsuario(_admin, original);

        Usuario modificado = new Usuario(
            "Ana",
            "Perez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#"
        );

        _servicio.ModificarUsuario(_admin, "ana@test.com", modificado);

        Usuario usuarioEnLista = _servicio
            .ObtenerUsuarios(_admin)
            .First(u => u.Correo == "ana@test.com");

        Assert.AreEqual("Perez", usuarioEnLista.Apellido);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void ModificarUsuario_CuandoNoEsAdmin_DebeLanzarExcepcion()
    {
        Usuario ana = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#",
            new List<Rol> { Rol.Ninguno }
        );

        _servicio.CrearUsuario(_admin, ana);

        Usuario anaModificadaConRol = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#",
            new List<Rol> { Rol.Editor }
        );

        _servicio.ModificarUsuario(_noAdmin, "ana@test.com", anaModificadaConRol);
    }

    [TestMethod]
    public void CrearUsuario_DebeGenerarUnLogDeAuditoria()
    {
        Usuario nuevo = new Usuario(
            "Ana",
            "Gomez",
            "ana@test.com",
            DateTime.Now.AddYears(-20),
            "Ana1234#"
        );

        _servicio.CrearUsuario(_admin, nuevo);

        List<Log> logs = _auditoriaService.ObtenerLogs(_admin);

        Assert.AreEqual(1, logs.Count);
        Assert.IsTrue(logs.Any(l => l.Accion.Contains("Creado") || l.Accion.Contains("Creación")));
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void ModificarUsuario_CuandoEditorIntentaDarseRolAdmin_DebeLanzarExcepcion()
    {
        Usuario editor = new Usuario(
            "Carlos",
            "Editor",
            "carlos@test.com",
            DateTime.Now.AddYears(-25),
            "Editor123#",
            new List<Rol> { Rol.Editor }
        );

        _servicio.CrearUsuario(_admin, editor);

        Usuario intentoHack = new Usuario(
            "Carlos",
            "Editor",
            "carlos@test.com",
            DateTime.Now.AddYears(-25),
            "Editor123#",
            new List<Rol> { Rol.Editor, Rol.Administrador }
        );

        _servicio.ModificarUsuario(editor, "carlos@test.com", intentoHack);
    }

    [TestMethod]
    public void ModificarUsuario_CuandoEsEditorYSeModificaASiMismo_DebePermitir()
    {
        Usuario editor = new Usuario(
            "Carlos",
            "Editor",
            "carlos@test.com",
            DateTime.Now.AddYears(-25),
            "Editor123#",
            new List<Rol> { Rol.Editor }
        );

        _servicio.CrearUsuario(_admin, editor);

        Usuario datosNuevos = new Usuario(
            "Carlos",
            "Modificado",
            "carlos@test.com",
            DateTime.Now.AddYears(-25),
            "Editor123#",
            new List<Rol> { Rol.Editor }
        );

        _servicio.ModificarUsuario(editor, "carlos@test.com", datosNuevos);

        Usuario actualizado = _servicio
            .ObtenerUsuarios(_admin)
            .First(u => u.Correo == "carlos@test.com");

        Assert.AreEqual("Modificado", actualizado.Apellido);
    }
    
    [TestMethod]
    public void ModificarUsuario_CuandoCorreoCambiaAYaExistente_LanzaExcepcion()
    {
        UsuarioService service = new UsuarioService(_auditoriaService, _inMemoryUsuario);

        Usuario nuevo = new Usuario("Juan", "Perez", "juan@test.com", DateTime.Now.AddYears(-20), "Juan123#", [Rol.Editor]);
        service.CrearUsuario(_admin, nuevo);

        Usuario modificado = new Usuario("Juan", "Perez", "admin@ort.edu.uy", DateTime.Now.AddYears(-20), "Juan123#", [Rol.Editor]);

        Assert.ThrowsException<ArgumentException>(() =>
            service.ModificarUsuario(_admin, "juan@test.com", modificado));
    }

    [TestMethod]
    public void ObtenerLogs_CuandoEsAdministrador_DevuelveLogs()
    {
        _auditoriaService.RegistrarLog(_admin, "Accion");

        UsuarioService service = new UsuarioService(_auditoriaService, _inMemoryUsuario);

        List<Log> logs = service.ObtenerLogs(_admin);

        Assert.IsTrue(logs.Count > 0);
    }

    [TestMethod]
    public void ObtenerLogs_CuandoNoEsAdministrador_LanzaExcepcion()
    {
        _auditoriaService.RegistrarLog(_noAdmin, "Accion");
        UsuarioService service = new UsuarioService(_auditoriaService, _inMemoryUsuario);
        
        Assert.ThrowsException<UnauthorizedAccessException>(() => 
            service.ObtenerLogs(_noAdmin)
        );
    }
    
    [TestMethod]
    public void ModificarUsuario_CuandoUsuarioNoAdminModificaAOtroUsuario_DebeLanzarExcepcion()
    {
        
        Usuario usuario = new Usuario(
            "Ana", "Gomez", "ana@test.com", DateTime.Now.AddYears(-20),
            "Ana1234#", new List<Rol> { Rol.Ninguno }
        );

        _servicio.CrearUsuario(_admin, usuario);

        Usuario datosModificados = new Usuario(
            "Ana", "Perez", "ana@test.com", DateTime.Now.AddYears(-20),
            "Ana1234#", new List<Rol> { Rol.Ninguno }
        );

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _servicio.ModificarUsuario(_noAdmin, "ana@test.com", datosModificados));
    }

    [TestMethod]
    public void ModificarUsuario_CuandoAdminModificaRoles_DebeActualizarRoles()
    {
        Usuario usuario = new Usuario("Carlos", "Perez", "carlos@test.com",
            DateTime.Now.AddYears(-25), "Carlos123#", new List<Rol> { Rol.Ninguno }
        );

        _servicio.CrearUsuario(_admin, usuario);

        Usuario usuarioConRolesModificados = new Usuario("Carlos", "Perez", "carlos@test.com",
            DateTime.Now.AddYears(-25), "Carlos123#", new List<Rol> { Rol.Editor }
        );

        _servicio.ModificarUsuario(_admin, "carlos@test.com", usuarioConRolesModificados);

        Usuario usuarioActualizado = _servicio.ObtenerUsuarios(_admin).First(u => u.Correo == "carlos@test.com");

        CollectionAssert.AreEquivalent(
            new List<Rol> { Rol.Editor },
            usuarioActualizado.Roles
        );
    }
}




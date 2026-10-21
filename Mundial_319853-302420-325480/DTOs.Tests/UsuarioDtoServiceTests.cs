using App.Dominio;
using DTOs;
using DataAccess;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class UsuarioDtoServiceTests
{
    private IUsuarioService _servicio = null!;
    private IAuditoriaService _auditoriaService = null!;
    private InMemoryUsuario _inMemoryUsuario = null!;
    private UsuarioReadDto _admin = null!;

    [TestInitialize]
    public void Setup()
    {
        _inMemoryUsuario = new InMemoryUsuario();
        _auditoriaService = new AuditoriaService(new InMemoryLog());
        _servicio = new UsuarioService(_auditoriaService, _inMemoryUsuario);

        _admin = new UsuarioReadDto
        {
            Nombre = "Admin",
            Apellido = "Sist",
            Correo = "admin@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Administrador"]
        };
    }

    [TestMethod]
    public void CrearUsuario_ConUsuarioCreateDto_DebeAgregarYDevolverUsuarioReadDto()
    {
        UsuarioCreateDto nuevo = new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        };

        UsuarioReadDto creado = _servicio.CrearUsuario(_admin, nuevo);

        Assert.AreEqual("ana@test.com", creado.Correo);
        Assert.AreEqual("Editor", creado.Roles.Single());
        AssertNoExponeDatosSensibles(typeof(UsuarioReadDto));
    }

    [TestMethod]
    public void ObtenerUsuarios_DebeDevolverUsuarioReadDtoYNoEntidadUsuario()
    {
        List<UsuarioReadDto> usuarios = _servicio.ObtenerUsuarios(_admin);

        Assert.IsTrue(usuarios.Count >= 4);
        Assert.AreNotEqual(typeof(Usuario), usuarios[0].GetType());
        AssertNoExponeDatosSensibles(usuarios[0].GetType());
    }

    [TestMethod]
    public void ModificarUsuario_ConUsuarioUpdateDto_NoDebeModificarContrasena()
    {
        UsuarioCreateDto nuevo = new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        };
        _servicio.CrearUsuario(_admin, nuevo);

        UsuarioUpdateDto modificado = new UsuarioUpdateDto
        {
            Nombre = "Ana",
            Apellido = "Perez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-21),
            Roles = ["Editor"]
        };

        UsuarioReadDto actualizado = _servicio.ModificarUsuario(_admin, "ana@test.com", modificado);

        Assert.AreEqual("Perez", actualizado.Apellido);
        Assert.AreEqual("Ana1234#", _inMemoryUsuario.ObtenerUsuario("ana@test.com").Contrasena);
    }

    [TestMethod]
    public void CambiarPassword_ConCambiarPasswordDto_DebeActualizarContrasena()
    {
        CambiarPasswordDto cambio = new CambiarPasswordDto
        {
            Correo = "admin@ort.edu.uy",
            NuevaContrasena = "Nueva123#",
            ConfirmacionNuevaContrasena = "Nueva123#"
        };

        _servicio.CambiarPassword(_admin, cambio);

        Assert.AreEqual("Nueva123#", _inMemoryUsuario.ObtenerUsuario("admin@ort.edu.uy").Contrasena);
    }

    [TestMethod]
    public void ObtenerLogs_ConUsuarioReadDtoAdministrador_DebeDevolverLogReadDto()
    {
        _servicio.CrearUsuario(_admin, new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        });

        List<LogReadDto> logs = _servicio.ObtenerLogs(_admin);

        Assert.AreEqual(1, logs.Count);
        Assert.IsTrue(logs[0].Accion.Contains("Creación de usuario"));
        Assert.AreEqual("admin@ort.edu.uy", logs[0].Usuario.Correo);
        Assert.AreNotEqual(typeof(Log), logs[0].GetType());
    }

    private static void AssertNoExponeDatosSensibles(Type tipo)
    {
        string[] nombresSensibles = ["Password", "PasswordHash", "Contrasena", "Contrasenia", "Hash"];

        foreach (string nombre in nombresSensibles)
        {
            bool existe = tipo.GetProperties()
                .Any(propiedad => propiedad.Name.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            Assert.IsFalse(existe, $"{tipo.Name} expone {nombre}.");
        }
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void CrearUsuario_ConUsuarioReadDtoNoAdministrador_DebeLanzarExcepcion()
    {
        UsuarioReadDto usuario = new UsuarioReadDto
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Correo = "edito@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Editor"]
        };

        UsuarioCreateDto nuevo = new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        };
        
        _servicio.CrearUsuario(usuario, nuevo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void ModificarUsuario_CuandoUsuarioModificaSusRoles_SiendoNoAdministrador_DebeLanzarExcepcion()
    {
        _servicio.CrearUsuario(_admin, new UsuarioCreateDto
        {
            Nombre = "Editor",
            Apellido = "Uno",
            Correo = "editor@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Editor123#",
            Roles = ["Editor"]
        });

        UsuarioReadDto editor = new UsuarioReadDto
        {
            Nombre = "Editor",
            Apellido = "Uno",
            Correo = "editor@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Editor"]
        };

        UsuarioUpdateDto modificado = new UsuarioUpdateDto
        {
            Nombre = "Editor",
            Apellido = "Uno",
            Correo = "editor@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Editor", "Administrador"]
        };

        _servicio.ModificarUsuario(editor, "editor@test.com", modificado);
    }
    
    [TestMethod]
    public void ModificarUsuario_CuandoAdministradorModificaRoles_DebeActualizarRoles()
    {
        _servicio.CrearUsuario(_admin, new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        });

        UsuarioUpdateDto modificado = new UsuarioUpdateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Administrador"]
        };

        UsuarioReadDto resultado = _servicio.ModificarUsuario(_admin, "ana@test.com", modificado);

        Assert.IsTrue(resultado.EsAdministrador);
    }
    
    [TestMethod]
    public void CambiarPassword_CuandoAdministradorCambiaPasswordDeOtro_DebePermitir()
    {
        _servicio.CrearUsuario(_admin, new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        });

        CambiarPasswordDto cambio = new CambiarPasswordDto
        {
            Correo = "ana@test.com",
            NuevaContrasena = "Nueva123#",
            ConfirmacionNuevaContrasena = "Nueva123#"
        };

        _servicio.CambiarPassword(_admin, cambio);

        Assert.AreEqual(
            "Nueva123#",
            _inMemoryUsuario.ObtenerUsuario("ana@test.com").Contrasena
        );
    }
    
    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void CambiarPassword_CuandoUsuarioIntentaCambiarPasswordDeOtro_DebeLanzarExcepcion()
    {
        _servicio.CrearUsuario(_admin, new UsuarioCreateDto
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Correo = "ana@test.com",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Contrasena = "Ana1234#",
            Roles = ["Editor"]
        });

        UsuarioReadDto editor = new UsuarioReadDto
        {
            Nombre = "Editor",
            Apellido = "Uno",
            Correo = "editor@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = ["Editor"]
        };

        CambiarPasswordDto cambio = new CambiarPasswordDto
        {
            Correo = "ana@test.com",
            NuevaContrasena = "Nueva123#",
            ConfirmacionNuevaContrasena = "Nueva123#"
        };

        _servicio.CambiarPassword(editor, cambio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CambiarPassword_CuandoConfirmacionNoCoincide_DebeLanzarExcepcion()
    {
        CambiarPasswordDto cambio = new CambiarPasswordDto
        {
            Correo = "admin@ort.edu.uy",
            NuevaContrasena = "Nueva123#",
            ConfirmacionNuevaContrasena = "Otra123#"
        };

        _servicio.CambiarPassword(_admin, cambio);
    }
    
    [TestMethod]
    public void UsuarioReadDto_EsAdministrador_CuandoTieneRolAdministrador_RetornaTrue()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Administrador"]
        };

        Assert.IsTrue(dto.EsAdministrador);
    }
    
    [TestMethod]
    public void UsuarioReadDto_EsAdministrador_CuandoNoTieneRolAdministrador_RetornaFalse()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Editor"]
        };

        Assert.IsFalse(dto.EsAdministrador);
    }
    
    [TestMethod]
    public void UsuarioReadDto_EsEditor_CuandoTieneRolEditor_RetornaTrue()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Editor"]
        };

        Assert.IsTrue(dto.EsEditor);
    }
    
    [TestMethod]
    public void UsuarioReadDto_EsEditor_CuandoNoTieneRolEditor_RetornaFalse()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Administrador"]
        };

        Assert.IsFalse(dto.EsEditor);
    }
    
    [TestMethod]
    public void UsuarioReadDto_EsPeriodista_CuandoTieneRolPeriodista_RetornaTrue()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Periodista"]
        };

        Assert.IsTrue(dto.EsPeriodista);
    }
    
    [TestMethod]
    public void UsuarioReadDto_EsPeriodista_CuandoNoTieneRolPeriodista_RetornaFalse()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Editor"]
        };

        Assert.IsFalse(dto.EsPeriodista);
    }
    
    [TestMethod]
    public void UsuarioReadDto_ConMultiplesRoles_DetectaTodosCorrectamente()
    {
        UsuarioReadDto dto = new UsuarioReadDto
        {
            Roles = ["Administrador", "Editor", "Periodista"]
        };

        Assert.IsTrue(dto.EsAdministrador);
        Assert.IsTrue(dto.EsEditor);
        Assert.IsTrue(dto.EsPeriodista);
    }
}

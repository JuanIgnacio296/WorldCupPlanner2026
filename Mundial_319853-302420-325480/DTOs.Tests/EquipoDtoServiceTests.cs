using App.Dominio;
using DTOs;
using DataAccess;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class EquipoDtoServiceTests
{
    private IEquipoService _servicio = null!;
    private UsuarioReadDto _admin = null!;

    [TestInitialize]
    public void Setup()
    {
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        _servicio = new EquipoService(auditoriaService, new InMemoryEquipo());
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
    public void CrearEquipo_ConEquipoCreateDto_DebeAgregarYDevolverEquipoReadDto()
    {
        EquipoCreateDto nuevoEquipo = new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "base64-bandera"
        };

        EquipoReadDto creado = _servicio.CrearEquipo(_admin, nuevoEquipo);

        Assert.AreEqual("Uruguay", creado.Nombre);
        Assert.AreEqual("CONMEBOL", creado.Confederacion);
        Assert.AreEqual(400, creado.RankingFifa);
        Assert.AreEqual("base64-bandera", creado.BanderaBase64);
        Assert.AreNotEqual(typeof(Equipo), creado.GetType());
    }

    [TestMethod]
    public void CrearEquipo_ConBanderaBase64_DebeGuardarBanderaEnEntidadEquipo()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "base64-bandera"
        });

        Equipo equipo = _servicio.ObtenerEquipos().Single(equipo => equipo.Nombre == "Uruguay");

        Assert.AreEqual("base64-bandera", equipo.BanderaBase64);
    }

    [TestMethod]
    public void ObtenerEquiposListado_DebeDevolverEquipoListItemDtoSinBanderaBase64()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "base64-bandera"
        });

        List<EquipoListItemDto> equipos = _servicio.ObtenerEquiposListado();

        Assert.AreEqual(1, equipos.Count);
        Assert.AreEqual("Uruguay", equipos[0].Nombre);
        Assert.AreEqual("CONMEBOL", equipos[0].Confederacion);
        Assert.AreEqual(400, equipos[0].RankingFifa);
        Assert.IsTrue(equipos[0].TieneBandera);
        Assert.AreNotEqual(typeof(Equipo), equipos[0].GetType());
        Assert.IsFalse(typeof(EquipoListItemDto).GetProperties().Any(propiedad => propiedad.Name == "BanderaBase64"));
    }

    [TestMethod]
    public void ModificarEquipo_ConEquipoUpdateDto_DebeActualizarYDevolverEquipoReadDto()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "base64-bandera"
        });
        EquipoUpdateDto equipoModificado = new EquipoUpdateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 450,
            BanderaBase64 = "base64-bandera-actualizada"
        };

        EquipoReadDto actualizado = _servicio.ModificarEquipo(_admin, "Uruguay", equipoModificado);

        Assert.AreEqual("Uruguay", actualizado.Nombre);
        Assert.AreEqual("CONMEBOL", actualizado.Confederacion);
        Assert.AreEqual(450, actualizado.RankingFifa);
        Assert.AreEqual("base64-bandera-actualizada", actualizado.BanderaBase64);
        Assert.AreNotEqual(typeof(Equipo), actualizado.GetType());
    }

    [TestMethod]
    public void ModificarEquipo_CambiandoNombreYBandera_DebeActualizarNombreYRemoverBanderaAnterior()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "base64-bandera-original"
        });
        EquipoUpdateDto equipoModificado = new EquipoUpdateDto
        {
            Nombre = "Uruguay Actualizado",
            Confederacion = "CONMEBOL",
            RankingFifa = 450,
            BanderaBase64 = "base64-bandera-nueva"
        };

        EquipoReadDto actualizado = _servicio.ModificarEquipo(_admin, "Uruguay", equipoModificado);

        Assert.AreEqual("Uruguay Actualizado", actualizado.Nombre);
        Assert.AreEqual("base64-bandera-nueva", actualizado.BanderaBase64);
        Assert.AreEqual(1, _servicio.ObtenerEquiposListado().Count);
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.ObtenerEquipo("Uruguay"));
    }

    [TestMethod]
    public void CrearEquipo_ConUsuarioReadDtoSinRoles_DebeLanzarUnauthorizedAccessException()
    {
        UsuarioReadDto usuarioSinRoles = new UsuarioReadDto
        {
            Nombre = "Usuario",
            Apellido = "SinRol",
            Correo = "usuario@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-20),
            Roles = []
        };
        EquipoCreateDto nuevoEquipo = new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400
        };

        Assert.ThrowsException<UnauthorizedAccessException>(() => _servicio.CrearEquipo(usuarioSinRoles, nuevoEquipo));
    }

    [TestMethod]
    public void ObtenerEquipo_ConNombreExistente_DebeDevolverEquipoReadDto()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400,
            BanderaBase64 = "base64-bandera"
        });

        EquipoReadDto equipo = _servicio.ObtenerEquipo("Uruguay");

        Assert.AreEqual("Uruguay", equipo.Nombre);
        Assert.AreEqual("CONMEBOL", equipo.Confederacion);
        Assert.AreEqual(400, equipo.RankingFifa);
        Assert.AreEqual("base64-bandera", equipo.BanderaBase64);
        Assert.AreNotEqual(typeof(Equipo), equipo.GetType());
    }

    [TestMethod]
    public void ObtenerRankingEquipos_DebeDevolverRankingEquipoDtoOrdenadoPorRankingFifa()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400
        });
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Francia",
            Confederacion = "UEFA",
            RankingFifa = 900
        });

        List<RankingEquipoDto> ranking = _servicio.ObtenerRankingEquipos();

        Assert.AreEqual(2, ranking.Count);
        Assert.AreEqual("Francia", ranking[0].Nombre);
        Assert.AreEqual(900, ranking[0].RankingFifa);
        Assert.AreEqual("Uruguay", ranking[1].Nombre);
        Assert.AreEqual(400, ranking[1].RankingFifa);
        Assert.AreNotEqual(typeof(Equipo), ranking[0].GetType());
    }

    [TestMethod]
    public void EliminarEquipo_ConUsuarioReadDto_DebeEliminarEquipo()
    {
        _servicio.CrearEquipo(_admin, new EquipoCreateDto
        {
            Nombre = "Uruguay",
            Confederacion = "CONMEBOL",
            RankingFifa = 400
        });

        _servicio.EliminarEquipo(_admin, "Uruguay");

        Assert.AreEqual(0, _servicio.ObtenerEquiposListado().Count);
    }

    [TestMethod]
    public void CompletarEquiposFaltantes_ConUsuarioReadDto_DebeGenerarEquiposYListarlosComoDto()
    {
        _servicio.CompletarEquiposFaltantes(_admin);

        List<EquipoListItemDto> equipos = _servicio.ObtenerEquiposListado();

        Assert.AreEqual(48, equipos.Count);
        Assert.AreNotEqual(typeof(Equipo), equipos[0].GetType());
    }
}

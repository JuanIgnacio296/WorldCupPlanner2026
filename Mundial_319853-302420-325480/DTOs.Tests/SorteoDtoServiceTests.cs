using App.Dominio;
using DTOs;
using DataAccess;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class SorteoDtoServiceTests
{
    private ISorteoService _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        InMemorySemilla inMemorySemilla = new InMemorySemilla();
        inMemorySemilla.AgregarSemilla(new Semilla(1, 1, 1, 1, 1));

        _servicio = new SorteoService(
            auditoriaService,
            new InMemorySorteo(),
            new InMemoryFixture(),
            new InMemoryEquipo(),
            new InMemoryPartido(),
            new InMemoryEstadio(),
            inMemorySemilla
        );
    }

    [TestMethod]
    public void CrearSorteo_ConDtosYTokenSemilla_DebeCrearYDevolverSorteoReadDto()
    {
        UsuarioReadDto editor = CrearEditor();
        List<EquipoReadDto> equipos = new List<EquipoReadDto>
        {
            new EquipoReadDto { Nombre = "Uruguay", Confederacion = "CONMEBOL", RankingFifa = 400 }
        };
        List<EstadioReadDto> estadios = new List<EstadioReadDto>
        {
            new EstadioReadDto
            {
                Nombre = "Centenario",
                Ciudad = "Montevideo",
                Capacidad = 60000,
                Descripcion = "Estadio Historico"
            }
        };

        SorteoReadDto creado = _servicio.CrearSorteo(editor, equipos, estadios, 1);

        Assert.AreEqual(1, creado.Token);
        Assert.AreEqual(1, creado.Semilla.Token);
        Assert.AreEqual(1, creado.Equipos.Count);
        Assert.AreEqual("Uruguay", creado.Equipos[0].Nombre);
        Assert.AreEqual(1, creado.Estadios.Count);
        Assert.AreEqual("Centenario", creado.Estadios[0].Nombre);
        Assert.AreNotEqual(typeof(Sorteo), creado.GetType());
    }

    [TestMethod]
    public void CrearSorteo_ConEquipoConBandera_DebePreservarBanderaBase64()
    {
        UsuarioReadDto editor = CrearEditor();
        List<EquipoReadDto> equipos = new List<EquipoReadDto>
        {
            new EquipoReadDto
            {
                Nombre = "Uruguay",
                Confederacion = "CONMEBOL",
                RankingFifa = 400,
                BanderaBase64 = "bandera-uy-base64"
            }
        };

        SorteoReadDto creado = _servicio.CrearSorteo(editor, equipos, CrearEstadiosDto(), 1);

        Assert.AreEqual("bandera-uy-base64", creado.Equipos[0].BanderaBase64);
    }

    [TestMethod]
    public void ObtenerSorteoPorToken_ConTokenExistente_DebeDevolverSorteoReadDto()
    {
        UsuarioReadDto editor = CrearEditor();
        _servicio.CrearSorteo(editor, CrearEquiposDto(), CrearEstadiosDto(), 1);

        SorteoReadDto obtenido = _servicio.ObtenerSorteoPorTokenDto(1);

        Assert.AreEqual(1, obtenido.Token);
        Assert.AreEqual(1, obtenido.Semilla.Token);
        Assert.AreEqual("Uruguay", obtenido.Equipos[0].Nombre);
        Assert.AreNotEqual(typeof(Sorteo), obtenido.GetType());
    }

    [TestMethod]
    public void SortearEquipos_ConUsuarioReadDto_DebeSortearYDevolverGruposReadDto()
    {
        UsuarioReadDto editor = CrearEditor();
        _servicio.CrearSorteo(editor, CrearEquiposParaSorteoDto(), CrearEstadiosDto(), 1);

        List<GrupoReadDto> grupos = _servicio.SortearEquipos(editor, 1);

        Assert.AreEqual(12, grupos.Count);
        Assert.AreEqual(4, grupos[0].Equipos.Count);
        Assert.AreNotEqual(typeof(Grupo), grupos[0].GetType());
    }

    [TestMethod]
    public void CrearFixture_ConUsuarioReadDto_DebeCrearYDevolverFixtureReadDto()
    {
        UsuarioReadDto editor = CrearEditor();
        _servicio.CrearSorteo(editor, CrearEquiposParaSorteoDto(), CrearEstadiosParaFixtureDto(), 1);
        _servicio.SortearEquipos(editor, 1);

        FixtureReadDto fixture = _servicio.CrearFixture(editor, 1, true);

        Assert.AreEqual(72, fixture.Partidos.Count);
        Assert.AreEqual(0, fixture.PartidosJugados);
        Assert.AreNotEqual(typeof(Fixture), fixture.GetType());
    }

    [TestMethod]
    public void ObtenerFixturePorToken_ConUsuarioReadDto_DebeDevolverFixtureReadDto()
    {
        UsuarioReadDto editor = CrearEditor();
        _servicio.CrearSorteo(editor, CrearEquiposParaSorteoDto(), CrearEstadiosParaFixtureDto(), 1);
        _servicio.SortearEquipos(editor, 1);
        _servicio.CrearFixture(editor, 1, true);

        FixtureReadDto fixture = _servicio.ObtenerFixturePorToken(editor, 1);

        Assert.AreEqual(1, fixture.TokenActual);
        Assert.AreEqual(72, fixture.Partidos.Count);
        Assert.AreNotEqual(typeof(Fixture), fixture.GetType());
    }

    private static UsuarioReadDto CrearEditor()
    {
        return new UsuarioReadDto
        {
            Nombre = "Editor",
            Apellido = "Sorteo",
            Correo = "editor@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            Roles = ["Editor"]
        };
    }

    private static List<EquipoReadDto> CrearEquiposDto()
    {
        return new List<EquipoReadDto>
        {
            new EquipoReadDto { Nombre = "Uruguay", Confederacion = "CONMEBOL", RankingFifa = 400 }
        };
    }

    private static List<EquipoReadDto> CrearEquiposParaSorteoDto()
    {
        Dictionary<string, int> cuposPorConfederacion = new()
        {
            { "UEFA", 16 },
            { "CONMEBOL", 7 },
            { "CONCACAF", 7 },
            { "CAF", 9 },
            { "AFC", 8 },
            { "OFC", 1 }
        };

        List<EquipoReadDto> equipos = [];
        int ranking = 2400;
        foreach (KeyValuePair<string, int> cupo in cuposPorConfederacion)
        {
            for (int i = 1; i <= cupo.Value; i++)
            {
                equipos.Add(new EquipoReadDto
                {
                    Nombre = $"{cupo.Key}_{i:00}",
                    Confederacion = cupo.Key,
                    RankingFifa = ranking--
                });
            }
        }

        return equipos;
    }

    private static List<EstadioReadDto> CrearEstadiosParaFixtureDto()
    {
        List<EstadioReadDto> estadios = [];
        for (int i = 1; i <= 12; i++)
        {
            estadios.Add(new EstadioReadDto
            {
                Nombre = $"Estadio {i:00}",
                Ciudad = $"Ciudad {i:00}",
                Capacidad = 50000 + i,
                Descripcion = $"Estadio para fixture {i:00}"
            });
        }

        return estadios;
    }

    private static List<EstadioReadDto> CrearEstadiosDto()
    {
        return new List<EstadioReadDto>
        {
            new EstadioReadDto
            {
                Nombre = "Centenario",
                Ciudad = "Montevideo",
                Capacidad = 60000,
                Descripcion = "Estadio Historico"
            }
        };
    }
}

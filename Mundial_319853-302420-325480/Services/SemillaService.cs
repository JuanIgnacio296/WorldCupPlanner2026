using Services.Interfaces;
using App.Dominio;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;

namespace Services;

public class SemillaService : ISemillaService
{
    private IinMemorySemilla _IinMemorySemilla;

    public SemillaService(IinMemorySemilla IinMemorySemilla)
    {
        _IinMemorySemilla = IinMemorySemilla;
    }

    public Semilla ObtenerSemilla(int token)
    {
        if (!_IinMemorySemilla.ExisteSemilla(token))
        {
            throw new ArgumentException("No existe semilla con el token");
        }
        return _IinMemorySemilla.ObtenerSemilla(token);
    }

    public SemillaReadDto ObtenerSemillaDto(int token)
    {
        return mapearSemilla(ObtenerSemilla(token));
    }

    public void AgregarSemilla(Semilla semilla)
    {
        if (_IinMemorySemilla.ExisteSemilla(semilla.Token))
        {
            throw new ArgumentException("Ya existe una semilla con ese mismo token");
        }

        if (_IinMemorySemilla.ExisteSemilla(semilla))
        {
            int token = _IinMemorySemilla.ObtenerSemilla(semilla).Token;
            throw new ArgumentException("Ya existe una semilla igual con el token " + token);
        }
        
        _IinMemorySemilla.AgregarSemilla(semilla);
    }

    public SemillaReadDto AgregarSemilla(SemillaCreateDto semilla)
    {
        Semilla semillaDominio = new Semilla(
            semilla.Token,
            semilla.SemillaFixture,
            semilla.SemillaCompletar,
            semilla.SemillaCrucesFase,
            semilla.SemillaSimulacion
        );

        AgregarSemilla(semillaDominio);

        return mapearSemilla(semillaDominio);
    }

    public List<Semilla> ObtenerSemillas()
    {
        return _IinMemorySemilla.ObtenerSemillas();
    }

    public List<SemillaReadDto> ObtenerSemillasListado()
    {
        return _IinMemorySemilla.ObtenerSemillas()
            .Select(mapearSemilla)
            .ToList();
    }

    public int ObtenerSiguienteTokenDisponible()
    {
        List<Semilla> semillas = _IinMemorySemilla.ObtenerSemillas();

        if (semillas.Count == 0)
        {
            return 1;
        }

        return semillas.Max(s => s.Token) + 1;
    }

    public void AsegurarSemillaExiste(Semilla semilla)
    {
        if (!_IinMemorySemilla.ExisteSemilla(semilla.Token))
        {
            _IinMemorySemilla.AgregarSemilla(semilla);
        }
    }

    private static SemillaReadDto mapearSemilla(Semilla semilla)
    {
        return new SemillaReadDto
        {
            Token = semilla.Token,
            SemillaFixture = semilla.SemillaFixture,
            SemillaCompletar = semilla.SemillaCompletar,
            SemillaCrucesFase = semilla.SemillaCrucesFase,
            SemillaSimulacion = semilla.SemillaSimulacion
        };
    }
}


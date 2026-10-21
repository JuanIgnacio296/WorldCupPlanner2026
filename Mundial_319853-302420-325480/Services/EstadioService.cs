using App.Dominio;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;

namespace Services;

public class EstadioService : IEstadioService
{
    private IinMemoryEstadio _IinMemoryEstadio;
    private IAuditoriaService _auditoriaService;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();
    private static readonly EstadioMapeador _estadioMapeador = new EstadioMapeador();

    public EstadioService(IAuditoriaService auditoriaService, IinMemoryEstadio IinMemoryEstadio)
    {
        _auditoriaService = auditoriaService;
        _IinMemoryEstadio = IinMemoryEstadio;
    }

    public void CrearEstadio(Usuario administrador, Estadio nuevoEstadio)
    {
        if (!administrador.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede gestionar estadios.");
        }

        if (_IinMemoryEstadio.ExisteEstadio(nuevoEstadio.Nombre))
        {
            throw new ArgumentException("Ya existe un estadio con ese nombre.");
        }
        
        _IinMemoryEstadio.AgregarEstadio(nuevoEstadio);

        _auditoriaService.RegistrarLog(administrador, $"Creado el estadio {nuevoEstadio.Nombre}");
    }

    public EstadioReadDto CrearEstadio(UsuarioReadDto administrador, EstadioCreateDto nuevoEstadio)
    {
        Estadio estadio = new Estadio(
            nuevoEstadio.Nombre,
            nuevoEstadio.Ciudad,
            nuevoEstadio.Capacidad,
            nuevoEstadio.Descripcion
        );

        CrearEstadio(_usuarioMapeador.ADominio(administrador), estadio);

        return _estadioMapeador.ADto(estadio);
    }
    
    public void EliminarEstadio(Usuario administrador, string nombre)
    {
        if (!administrador.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede eliminar estadios.");
        }
    
        Estadio estadio = _IinMemoryEstadio.ObtenerEstadio(nombre);
        if (estadio == null)
        {
            return;
        }

        if (estadio.FechasOcupadas.Any())
        {
            throw new InvalidOperationException("No se puede eliminar un estadio que está siendo usado en un fixture.");
        }

        _IinMemoryEstadio.EliminarEstadio(estadio);
        _auditoriaService.RegistrarLog(administrador, $"El estadio {nombre} fue eliminado");
    }

    public void EliminarEstadio(UsuarioReadDto administrador, string nombre)
    {
        EliminarEstadio(_usuarioMapeador.ADominio(administrador), nombre);
    }
    
    public void ModificarEstadio(Usuario administrador, string nombreOriginal, Estadio estadioModificado)
    {
        if (!administrador.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede modificar estadios.");
        }
        
        Estadio estadio = _IinMemoryEstadio.ObtenerEstadio(nombreOriginal);

        if (estadio == null)
        {
            throw new ArgumentException("Estadio no encontrado");
        }

        bool cambiaNombre = nombreOriginal != estadioModificado.Nombre;

        if (cambiaNombre && _IinMemoryEstadio.ExisteEstadio(estadioModificado.Nombre))
        {
            throw new ArgumentException("Ya existe otro estadio con ese nombre.");
        }

        if (cambiaNombre)
        {
            if (estadio.FechasOcupadas.Any())
            {
                throw new InvalidOperationException("No se puede cambiar el nombre de un estadio que está siendo usado en un fixture.");
            }

            _IinMemoryEstadio.EliminarEstadio(estadio);
            _IinMemoryEstadio.AgregarEstadio(new Estadio(
                estadioModificado.Nombre, estadioModificado.Ciudad, estadioModificado.Capacidad, estadioModificado.Descripcion));
        }
        else
        {
            estadio.Ciudad = estadioModificado.Ciudad;
            estadio.Capacidad = estadioModificado.Capacidad;
            estadio.Descripcion = estadioModificado.Descripcion;
        }

        _auditoriaService.RegistrarLog(administrador, $"Modificación del estadio {nombreOriginal}");
    }

    public EstadioReadDto ModificarEstadio(UsuarioReadDto administrador, string nombreOriginal, EstadioUpdateDto estadioModificado)
    {
        Estadio estadioDominio = new Estadio(
            estadioModificado.Nombre,
            estadioModificado.Ciudad,
            estadioModificado.Capacidad,
            estadioModificado.Descripcion
        );

        ModificarEstadio(_usuarioMapeador.ADominio(administrador), nombreOriginal, estadioDominio);

        Estadio actualizado = _IinMemoryEstadio.ObtenerEstadio(estadioDominio.Nombre);
        return _estadioMapeador.ADto(actualizado);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _IinMemoryEstadio.ObtenerEstadios();
    }

    public List<EstadioListItemDto> ObtenerEstadiosListado()
    {
        return _IinMemoryEstadio.ObtenerEstadios()
            .Select(mapearEstadioListItem)
            .ToList();
    }

    public List<EstadioReadDto> ObtenerEstadiosDetalle()
    {
        return _IinMemoryEstadio.ObtenerEstadios()
            .Select(_estadioMapeador.ADto)
            .ToList();
    }
    
    public void CompletarEstadiosMinimosParaFixture(Usuario administrador)
    {
        if (!administrador.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede completar estadios.");
        }

        List<Estadio> estadiosPorDefecto = crearEstadiosPorDefecto();

        foreach (Estadio estadio in estadiosPorDefecto)
        {
            if (_IinMemoryEstadio.ObtenerEstadios().Count >= 4)
            {
                return;
            }

            if (!_IinMemoryEstadio.ExisteEstadio(estadio.Nombre))
            {
                _IinMemoryEstadio.AgregarEstadio(estadio);

                _auditoriaService.RegistrarLog(
                    administrador,
                    $"Creación de estadio por defecto: {estadio.Nombre}"
                );
            }
        }
    }

    public void CompletarEstadiosMinimosParaFixture(UsuarioReadDto administrador)
    {
        CompletarEstadiosMinimosParaFixture(_usuarioMapeador.ADominio(administrador));
    }

    private List<Estadio> crearEstadiosPorDefecto()
    {
        return EstadiosPorDefecto.Crear();
    }

    private static EstadioListItemDto mapearEstadioListItem(Estadio estadio)
    {
        return new EstadioListItemDto
        {
            Nombre = estadio.Nombre,
            Ciudad = estadio.Ciudad,
            Capacidad = estadio.Capacidad
        };
    }
}


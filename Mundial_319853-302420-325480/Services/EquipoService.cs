using App.Dominio;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;

namespace Services;

public class EquipoService : IEquipoService
{
    private IinMemoryEquipo _IinMemoryEquipo;
    private IAuditoriaService _auditoriaService;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();
    private readonly EquipoMapeador _equipoMapeador = new EquipoMapeador();

    public EquipoService(IAuditoriaService auditoriaService, IinMemoryEquipo IinMemoryEquipo)
    {
        _auditoriaService = auditoriaService;
        _IinMemoryEquipo = IinMemoryEquipo;
    }

    public void CrearEquipo(Usuario usuario, Equipo nuevoEquipo)
    {
        if (!usuario.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el administrador puede crear equipos.");
        }
        if (_IinMemoryEquipo.ExisteEquipo(nuevoEquipo.Nombre))
        {
            throw new ArgumentException("Ya existe un equipo con ese nombre.");
        }
        
        validarCupoDisponible(nuevoEquipo);
        
        _IinMemoryEquipo.AgregarEquipo(nuevoEquipo);
        
        _auditoriaService.RegistrarLog(usuario, $"Creado el equipo {nuevoEquipo.Nombre}");
    }

    public EquipoReadDto CrearEquipo(UsuarioReadDto usuario, EquipoCreateDto nuevoEquipo)
    {
        Usuario usuarioDominio = _usuarioMapeador.ADominio(usuario);
        Equipo equipo = new Equipo(nuevoEquipo.Nombre, nuevoEquipo.Confederacion, nuevoEquipo.RankingFifa, nuevoEquipo.BanderaBase64);

        CrearEquipo(usuarioDominio, equipo);

        return mapearEquipo(equipo);
    }
    
    public void EliminarEquipo(Usuario usuario, string nombre)
    {
        if (!usuario.TieneRol(Rol.Editor) && !usuario.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el editor o administrador puede eliminar equipos.");
        }
    
        if (_IinMemoryEquipo.ObtenerEquipo(nombre) == null)
        {
            return;
        }

        if (_IinMemoryEquipo.EstaEnUso(nombre))
        {
            throw new InvalidOperationException("No se puede eliminar un equipo que ya fue sorteado o que juega en un fixture.");
        }

        _IinMemoryEquipo.EliminarEquipo(_IinMemoryEquipo.ObtenerEquipo(nombre));
        _auditoriaService.RegistrarLog(usuario, $"El equipo {nombre} fue eliminado");
    }

    public void EliminarEquipo(UsuarioReadDto usuario, string nombre)
    {
        Usuario usuarioDominio = _usuarioMapeador.ADominio(usuario);

        EliminarEquipo(usuarioDominio, nombre);
    }
    
    public void ModificarEquipo(Usuario usuario, string nombreOriginal, Equipo equipoModificado)
    {
        if (!usuario.TieneRol(Rol.Editor) && !usuario.TieneRol(Rol.Administrador))
        {
            throw new UnauthorizedAccessException("Solo el editor o administrador puede modificar equipos.");
        }
        
        var equipo = _IinMemoryEquipo.ObtenerEquipo(nombreOriginal);

        if (equipo == null) throw new ArgumentException("Equipo no encontrado");

        bool cambiaNombre = nombreOriginal != equipoModificado.Nombre;

        if (cambiaNombre && _IinMemoryEquipo.ExisteEquipo(equipoModificado.Nombre))
        {
            throw new ArgumentException("Ya existe otro equipo con ese nombre.");
        }

        int rankingViejo = equipo.RankingFifa;

        if (cambiaNombre)
        {
            if (_IinMemoryEquipo.EstaEnUso(nombreOriginal))
            {
                throw new InvalidOperationException("No se puede cambiar el nombre de un equipo que ya fue sorteado o que juega en un fixture.");
            }

            _IinMemoryEquipo.EliminarEquipo(equipo);
            _IinMemoryEquipo.AgregarEquipo(new Equipo(
                equipoModificado.Nombre, equipoModificado.Confederacion, equipoModificado.RankingFifa, equipoModificado.BanderaBase64));
        }
        else
        {
            equipo.Confederacion = equipoModificado.Confederacion;
            equipo.RankingFifa = equipoModificado.RankingFifa;
            equipo.BanderaBase64 = equipoModificado.BanderaBase64;
        }

        _auditoriaService.RegistrarLog(usuario, $"Modificacion del equipo {nombreOriginal}");
        if (rankingViejo != equipoModificado.RankingFifa)
        {
            _auditoriaService.RegistrarLog(usuario, $"Ranking del equipo {nombreOriginal} cambiado de {rankingViejo} a {equipoModificado.RankingFifa}");
        }
    }

    public EquipoReadDto ModificarEquipo(UsuarioReadDto usuario, string nombreOriginal, EquipoUpdateDto equipoModificado)
    {
        Usuario usuarioDominio = _usuarioMapeador.ADominio(usuario);
        Equipo equipoDominio = new Equipo(equipoModificado.Nombre, equipoModificado.Confederacion, equipoModificado.RankingFifa, equipoModificado.BanderaBase64);

        ModificarEquipo(usuarioDominio, nombreOriginal, equipoDominio);

        Equipo actualizado = _IinMemoryEquipo.ObtenerEquipo(equipoDominio.Nombre);
        return mapearEquipo(actualizado);
    }

    public List<Equipo> ObtenerEquipos()
    {
        return _IinMemoryEquipo.ObtenerEquipos();
    }

    public EquipoReadDto ObtenerEquipo(string nombre)
    {
        Equipo equipo = _IinMemoryEquipo.ObtenerEquipo(nombre);

        return mapearEquipo(equipo);
    }

    public List<EquipoListItemDto> ObtenerEquiposListado()
    {
        return _IinMemoryEquipo.ObtenerEquipos()
            .Select(mapearEquipoListItem)
            .ToList();
    }

    public List<EquipoReadDto> ObtenerEquiposDetalle()
    {
        return _IinMemoryEquipo.ObtenerEquipos()
            .Select(_equipoMapeador.ADto)
            .ToList();
    }

    public List<RankingEquipoDto> ObtenerRankingEquipos()
    {
        return _IinMemoryEquipo.ObtenerEquipos()
            .OrderByDescending(equipo => equipo.RankingFifa)
            .Select(mapearRankingEquipo)
            .ToList();
    }
    
    public void CompletarEquiposFaltantes(Usuario administrador)
    {
        Random rnd = new Random();

        foreach (var cupo in CuposConfederacion.Todos)
        {
            procesarCupoConfederacion(administrador, cupo.Key, cupo.Value, rnd);
        }

        Console.WriteLine($"Total de equipos tras completar: {ObtenerCantidadTotal()}");
    }

    public void CompletarEquiposFaltantes(UsuarioReadDto administrador)
    {
        CompletarEquiposFaltantes(_usuarioMapeador.ADominio(administrador));
    }

    private void procesarCupoConfederacion(Usuario admin, string confederacion, int maximo, Random rnd)
    {
        int indice = 1;

        while (ContarPorConfederacion(confederacion) < maximo)
        {
            string nombreSugerido = $"{confederacion}_{indice:00}";
            indice++;

            if (!ExisteEquipo(nombreSugerido))
            {
                var nuevo = generarEquipoAleatorio(nombreSugerido, confederacion, rnd);
                this.CrearEquipo(admin, nuevo);
            }
        }
    }

    private Equipo generarEquipoAleatorio(string nombre, string confederacion, Random rnd)
    {
        int ranking = rnd.Next(300, 2501);
        return new Equipo(nombre, confederacion, ranking);
    }

    public int ContarPorConfederacion(string conf)
    {
        return _IinMemoryEquipo.ObtenerEquipos().Count(e => e.Confederacion == conf);
    }

    private bool ExisteEquipo(string nombre)
    {
        return _IinMemoryEquipo.ObtenerEquipos().Any(e => e.Nombre == nombre);
    }

    public int ObtenerCantidadTotal()
    {
        return _IinMemoryEquipo.ObtenerEquipos().Count;
    }
    
    private void validarCupoDisponible(Equipo equipo)
    {
        if (ObtenerCantidadTotal() >= 48)
        {
            throw new ArgumentException("No se pueden registrar más de 48 equipos.");
        }

        int cupoMaximo = CuposConfederacion.Maximo(equipo.Confederacion);

        if (ContarPorConfederacion(equipo.Confederacion) >= cupoMaximo)
        {
            throw new ArgumentException($"No se pueden registrar más equipos de {equipo.Confederacion}.");
        }
    }

    private EquipoReadDto mapearEquipo(Equipo equipo)
    {
        return _equipoMapeador.ADto(equipo);
    }

    private EquipoListItemDto mapearEquipoListItem(Equipo equipo)
    {
        return new EquipoListItemDto
        {
            Nombre = equipo.Nombre,
            Confederacion = equipo.Confederacion,
            RankingFifa = equipo.RankingFifa,
            TieneBandera = !string.IsNullOrEmpty(equipo.BanderaBase64)
        };
    }

    private static RankingEquipoDto mapearRankingEquipo(Equipo equipo)
    {
        return new RankingEquipoDto
        {
            Nombre = equipo.Nombre,
            Confederacion = equipo.Confederacion,
            RankingFifa = equipo.RankingFifa
        };
    }
}



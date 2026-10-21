using App.Dominio.Exceptions;

namespace App.Dominio;

public class Grupo
{
    private const int CantidadMaximaEquipos = 4;
    private const int CantidadMaximaPartidos = 6;
    private const int MaximoEquiposUefa = 2;
    private const int MaximoEquiposMismaConfederacion = 1;

    private char? _id;
    private int _token; 
    private List<Equipo> _listaEquipos = [];
    private List<Partido> _listaPartidos = [];

    public Grupo(char? id, int token, List<Partido> partidos)
    {
        Id = id;
        Token = token;
        Partidos = partidos;
    }

    public Grupo(char? id, int token)
    {
        Id = id;
        Token = token;
    }

    private Grupo() { }

    public char? Id
    {
        get => _id;
        set
        {
            ValidarId(value);
            _id = char.ToUpper(value!.Value);
        }
    }

    public int Token
    {
        get => _token;
        set
        {
            _token = value;
        }
    }

    public List<Equipo> Equipos => _listaEquipos.ToList();

    public List<Partido> Partidos
    {
        get => _listaPartidos.ToList();
        set
        {
            ValidarPartidos(value);
            _listaPartidos = value.ToList();
        }
    }

    public List<Estadio> Estadios
    {
        get
        {
            return _listaPartidos
                .Select(partido => partido.Estadio)
                .OfType<Estadio>()
                .Distinct()
                .ToList();
        }
    }

    public void AddEquipo(Equipo? equipo)
    {
        ValidarEquipoParaAgregar(equipo);
        ValidarConfederacion(equipo!);
        _listaEquipos.Add(equipo!);
    }

    public void AddPartido(Partido? partido)
    {
        if (partido == null)
        {
            throw new ArgumentosDeGrupoNoValidos("Partido no puede ser nulo");
        }

        _listaPartidos.Add(partido);
    }

    public void RemoveEquipo(Equipo equipo)
    {
        if (!_listaEquipos.Contains(equipo))
        {
            throw new ArgumentosDeGrupoNoValidos("No se puede eliminar un equipo de un grupo al que no pertenece");
        }

        _listaEquipos.Remove(equipo);
    }

    public void SortEquipos()
    {
        _listaEquipos.Sort((equipo, otroEquipo) => equipo.CompareTo(otroEquipo));
    }

    private static void ValidarId(char? id)
    {
        if (!id.HasValue || id == ' ')
        {
            throw new ArgumentosDeGrupoNoValidos("Id no puede ser vacio ni nulo");
        }

        char idNormalizado = char.ToUpper(id.Value);
        if (idNormalizado < 'A' || idNormalizado > 'L')
        {
            throw new ArgumentosDeGrupoNoValidos("Id debe ser una letra A-L");
        }
    }

    private void ValidarEquipoParaAgregar(Equipo? equipo)
    {
        if (equipo == null)
        {
            throw new ArgumentosDeGrupoNoValidos("No se pueden agregar equipos nulos");
        }

        if (_listaEquipos.Count == CantidadMaximaEquipos)
        {
            throw new ArgumentosDeGrupoNoValidos("No se pueden agregar mas de 4 equipos");
        }

        if (_listaEquipos.Contains(equipo))
        {
            throw new ArgumentosDeGrupoNoValidos("No se pueden agregar dos equipos iguales");
        }
    }

    private void ValidarConfederacion(Equipo nuevoEquipo)
    {
        int cantidadMismaConfederacion = _listaEquipos.Count(
            equipo => equipo.Confederacion == nuevoEquipo.Confederacion);

        if (nuevoEquipo.Confederacion == "UEFA")
        {
            if (cantidadMismaConfederacion >= MaximoEquiposUefa)
            {
                throw new ArgumentosDeGrupoNoValidos("No pueden haber 3 equipos de UEFA en un mismo grupo");
            }

            return;
        }

        if (cantidadMismaConfederacion >= MaximoEquiposMismaConfederacion)
        {
            throw new ArgumentosDeGrupoNoValidos(
                "No puede haber más de un equipo de " + nuevoEquipo.Confederacion + " en el grupo");
        }
    }

    private static void ValidarPartidos(List<Partido> partidos)
    {
        if (partidos == null)
        {
            throw new ArgumentosDeGrupoNoValidos("Partidos no puede ser nulo");
        }

        if (partidos.Count > CantidadMaximaPartidos)
        {
            throw new ArgumentosDeGrupoNoValidos("No pueden haber mas de 6 partidos");
        }
    }
}





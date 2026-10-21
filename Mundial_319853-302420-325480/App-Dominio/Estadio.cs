using App.Dominio.Exceptions;

namespace App.Dominio;

public class Estadio
{
    private const string DescripcionPorDefecto = "Sin descripcion";

    public int Id { get; set; }
    private string _nombre = string.Empty;
    private string _ciudad = string.Empty;
    private string _descripcion = string.Empty;
    private int _capacidad;
    private ICollection<FechaOcupada> _fechasOcupadas = new List<FechaOcupada>();
    private EstadioValidacion _estadioValidacion;
    private ReservaEstadioValidacion _reservaEstadioValidacion;

    public Estadio(string nombre, string ciudad, int capacidad, List<DateTime> fechasOcupadas, string descripcion)
    {
        _estadioValidacion = new();
        _reservaEstadioValidacion = new(new PoliticaReservaMundial());
        Nombre = nombre;
        Ciudad = ciudad;
        Descripcion = descripcion;
        Capacidad = capacidad;
        foreach (var fecha in fechasOcupadas)
        {
            AddFecha(fecha);
        }
    }

    private Estadio() { }

    public Estadio(string nombre, string ciudad, int capacidad, string descripcion)
        : this(nombre, ciudad, capacidad, [], descripcion)
    {
    }

    public Estadio(string nombre, string ciudad, int capacidad, List<DateTime> fechasOcupadas)
        : this(nombre, ciudad, capacidad, fechasOcupadas, DescripcionPorDefecto)
    {
    }

    public Estadio(string nombre, string ciudad, int capacidad)
        : this(nombre, ciudad, capacidad, [], DescripcionPorDefecto)
    {
    }

    public string Nombre
    {
        get => _nombre;
        set
        {
            _estadioValidacion.ValidarNombre(value);
            _nombre = value;
        }
    }

    public string Ciudad
    {
        get => _ciudad;
        set
        {
            _estadioValidacion.ValidarCiudad(value);
            _ciudad = value;
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        set
        {
            _estadioValidacion.ValidarDescripcion(value);
            _descripcion = value;
        }
    }

    public int Capacidad
    {
        get => _capacidad;
        set
        {
            _estadioValidacion.ValidarCapacidad(value);
            _capacidad = value;
        }
    }

    public virtual ICollection<FechaOcupada> FechasOcupadas
    {
        get => _fechasOcupadas;
        set
        {
            _reservaEstadioValidacion.ValidarFechasOcupadas(value);
            _fechasOcupadas = value;
        }
    }
    
    public EstadioValidacion EstadioValidacion => _estadioValidacion;
    public ReservaEstadioValidacion ReservaEstadioValidacion => _reservaEstadioValidacion;

    public void AddFecha(DateTime fecha)
    {
        _reservaEstadioValidacion.ValidarFechaReservable(fecha, _fechasOcupadas);
        _fechasOcupadas.Add(new FechaOcupada (fecha, this));
    }

    public void AddFecha(DateTime fecha, int fixtureToken)
    {
        _reservaEstadioValidacion.ValidarFechaReservable(fecha, _fechasOcupadas);
        _fechasOcupadas.Add(new FechaOcupada(fecha, this, fixtureToken));
    }


    public override bool Equals(object? obj)
    {
        return this.Nombre == (obj as Estadio)?.Nombre;
    }
}

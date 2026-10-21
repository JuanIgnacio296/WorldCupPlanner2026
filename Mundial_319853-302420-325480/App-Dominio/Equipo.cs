namespace App.Dominio;

using App.Dominio.Exceptions;

public class Equipo
{
    private const int LargoMaximoNombre = 60;
    private const int RankingMinimoFifa = 300;
    private const int RankingMaximoFifa = 2500;
    private const string MensajeConfederacionInvalida =
        "La confederación debe ser una de las siguientes: AFC, CAF, CONCACAF, CONMEBOL, OFC o UEFA";

    private static readonly HashSet<string> ConfederacionesValidas =
    [
        "AFC",
        "CAF",
        "CONCACAF",
        "CONMEBOL",
        "OFC",
        "UEFA"
    ];

    private string _nombre = string.Empty;
    private string _confederacion = string.Empty;
    private int _RankingFifa;
    private int _puntos;
    private int _golesAFavor;
    private int _golesEnContra;
    private string _banderaBase64 = string.Empty;

    public Equipo(string nombre, string confederacion, int RankingFifa)
    {
        Nombre = nombre;
        Confederacion = confederacion;
        this.RankingFifa = RankingFifa;
    }

    private Equipo() { }

    public Equipo(string nombre, string confederacion, int RankingFifa, string banderaBase64)
        : this(nombre, confederacion, RankingFifa)
    {
        BanderaBase64 = banderaBase64;
    }

    public string Nombre
    {
        get => _nombre;
        set
        {
            ValidarNombre(value);
            _nombre = value;
        }
    }

    public string Confederacion
    {
        get => _confederacion;
        set
        {
            string confederacionNormalizada = NormalizarConfederacion(value);
            ValidarConfederacion(confederacionNormalizada);
            _confederacion = confederacionNormalizada;
        }
    }

    public int RankingFifa
    {
        get => _RankingFifa;
        set
        {
            ValidarRankingFifa(value);
            _RankingFifa = value;
        }
    }

    public int Puntos
    {
        get => _puntos;
        set => _puntos += value;
    }

    public int GolesAFavor
    {
        get => _golesAFavor;
        set => _golesAFavor += value;
    }

    public int GolesEnContra
    {
        get => _golesEnContra;
        set => _golesEnContra += value;
    }

    public string BanderaBase64
    {
        get => _banderaBase64;
        set => _banderaBase64 = value;
    }

    public void ActualizarEquipo(int puntos, int golesAFavor, int golesEnContra)
    {
        ValidarEstadisticas(puntos, golesAFavor, golesEnContra);

        Puntos = puntos;
        GolesAFavor = golesAFavor;
        GolesEnContra = golesEnContra;
    }

    public void ReiniciarEstadisticas()
    {
        _puntos = 0;
        _golesAFavor = 0;
        _golesEnContra = 0;
    }

    public void RevertirEstadisticas(int puntos, int golesAFavor, int golesEnContra)
    {
        _puntos -= puntos;
        _golesAFavor -= golesAFavor;
        _golesEnContra -= golesEnContra;
    }

    public override bool Equals(object? obj)
    {
        return obj is Equipo equipo && Nombre == equipo.Nombre;
    }

    public override int GetHashCode()
    {
        return Nombre.GetHashCode();
    }

    public int CompareTo(Equipo? other)
    {
        if (other == null)
        {
            return -1;
        }

        if (Puntos != other.Puntos)
        {
            return other.Puntos.CompareTo(Puntos);
        }

        int diferenciaGoles = GolesAFavor - GolesEnContra;
        int diferenciaGolesOtro = other.GolesAFavor - other.GolesEnContra;

        if (diferenciaGoles != diferenciaGolesOtro)
        {
            return diferenciaGolesOtro.CompareTo(diferenciaGoles);
        }

        return other.GolesAFavor.CompareTo(GolesAFavor);
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            throw new ArgumentosDeEquipoNoValidos("Nombre no puede ser vacío");
        }

        if (nombre.Length > LargoMaximoNombre)
        {
            throw new ArgumentosDeEquipoNoValidos(
                "El nombre debe tener una longitud mayor o igual a 1 y menor o igual a 60 caracteres");
        }
    }

    private static string NormalizarConfederacion(string confederacion)
    {
        if (string.IsNullOrEmpty(confederacion))
        {
            throw new ArgumentosDeEquipoNoValidos(MensajeConfederacionInvalida);
        }

        return confederacion.ToUpper();
    }

    private static void ValidarConfederacion(string confederacion)
    {
        if (!ConfederacionesValidas.Contains(confederacion))
        {
            throw new ArgumentosDeEquipoNoValidos(MensajeConfederacionInvalida);
        }
    }

    private static void ValidarRankingFifa(int RankingFifa)
    {
        if (RankingFifa < RankingMinimoFifa || RankingFifa > RankingMaximoFifa)
        {
            throw new ArgumentosDeEquipoNoValidos("El ranking FIFA debe ser mayor o igual a 300 y menor o igual que 2500");
        }
    }

    private static void ValidarEstadisticas(int puntos, int golesAFavor, int golesEnContra)
    {
        if (golesAFavor < 0)
        {
            throw new ArgumentosDeEquipoNoValidos("Los goles a favor deben ser un número entero mayor o igual a 0");
        }

        if (golesEnContra < 0)
        {
            throw new ArgumentosDeEquipoNoValidos("Los goles en contra deben ser un número entero mayor o igual a 0");
        }

        if (puntos < 0)
        {
            throw new ArgumentosDeEquipoNoValidos("Los puntos deben ser un entero entre los siguientes: {0, 1, 3}");
        }

        ValidarPuntosConsistentesConResultado(puntos, golesAFavor, golesEnContra);
    }

    private static void ValidarPuntosConsistentesConResultado(int puntos, int golesAFavor, int golesEnContra)
    {
        if (golesAFavor > 
            golesEnContra && puntos != 3)
        {
            throw new ArgumentosDeEquipoNoValidos(
                "Cuando los goles a favor son más que los en contra, la cantidad de puntos debe ser 3");
        }

        if (golesEnContra > golesAFavor && puntos != 0)
        {
            throw new ArgumentosDeEquipoNoValidos(
                "Cuando los goles en contra son más que los a favor, la cantidad de puntos debe ser 0");
        }

        if (golesAFavor == golesEnContra && puntos != 1)
        {
            throw new ArgumentosDeEquipoNoValidos(
                "Cuando los goles a favor son iguales a los en contra, la cantidad de puntos debe ser 1");
        }
    }
    
    public void RegistrarPartidoJugado(int golesAFavor, int golesEnContra)
    {
        GolesAFavor += golesAFavor;
        GolesEnContra += golesEnContra;

        if (golesAFavor > golesEnContra)
        {
            Puntos += 3;
        }
        else if (golesAFavor == golesEnContra)
        {
            Puntos += 1;
        }
        else
        {
            Puntos += 0;
        }
    }
}


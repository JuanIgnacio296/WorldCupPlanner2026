namespace App.Dominio;

public class Semilla
{
    private int _semillaFixture;
    private int _semillaCompletar;
    private int _semillaCrucesFase;
    private int _semillaSimulacion;
    private int _token;
    private Random _randomSimulacion = new();

    public Semilla(int token, int fixture, int completar, int crucesFase, int simulacion)
    {
        Token = token;
        SemillaFixture = fixture;
        SemillaCompletar = completar;
        SemillaCrucesFase = crucesFase;
        SemillaSimulacion = simulacion;
    }

    private Semilla() { }

    public int SemillaFixture
    {
        get => _semillaFixture;
        set => _semillaFixture = value;
    }

    public int SemillaCompletar
    {
        get => _semillaCompletar;
        set => _semillaCompletar = value;
    }

    public int SemillaCrucesFase
    {
        get => _semillaCrucesFase;
        set => _semillaCrucesFase = value;
    }

    public int SemillaSimulacion
    {
        get => _semillaSimulacion;
        set
        {
            _semillaSimulacion = value;
            _randomSimulacion = new Random(_semillaSimulacion);
        }
    }

    public int Token
    {
        get => _token;
        set => _token = value;
    }

    public Random RandomSimulacion => _randomSimulacion;

    public override bool Equals(object? obj)
    {
        return obj is Semilla semilla
               && SemillaCompletar == semilla.SemillaCompletar
               && SemillaCrucesFase == semilla.SemillaCrucesFase
               && SemillaFixture == semilla.SemillaFixture
               && SemillaSimulacion == semilla.SemillaSimulacion;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            SemillaCompletar,
            SemillaCrucesFase,
            SemillaFixture,
            SemillaSimulacion);
    }
}




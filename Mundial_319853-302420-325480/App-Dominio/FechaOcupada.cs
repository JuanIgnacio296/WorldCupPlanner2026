namespace App.Dominio;

public class FechaOcupada
{
    public FechaOcupada(DateTime fecha, Estadio estadio)
    {
        Fecha = fecha;
        Estadio = estadio;
    }

    public FechaOcupada(DateTime fecha, Estadio estadio, int fixtureToken)
    {
        Fecha = fecha;
        Estadio = estadio;
        FixtureToken = fixtureToken;
    }

    private FechaOcupada() { }

    public int EstadioId { get; private set; }
    public DateTime Fecha { get; set; }
    public int FixtureToken { get; set; }
    public virtual Estadio Estadio { get; set; } = null!;
}

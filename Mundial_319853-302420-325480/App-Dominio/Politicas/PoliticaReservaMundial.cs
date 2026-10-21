namespace App.Dominio;

public class PoliticaReservaMundial : IPoliticaReservaEstadio
{
    private static readonly DateTime FechaMinimaDisponible = new(2026, 6, 1,14,0,0);
    private static readonly DateTime FechaMinimaReserva = new DateTime(2026, 6, 1);
    private static readonly int[] HorasValidas = [14, 18, 22];
    
    public bool EsHorario(DateTime fecha)
    {
        return HorasValidas.Contains(fecha.Hour) && fecha.Minute == 0 && fecha.Second == 0;
    }
    
    public bool EsFechaDisponible(DateTime fecha, ICollection<FechaOcupada> fechasOcupadas)
    {
        return fecha >= FechaMinimaDisponible && EsHorario(fecha) && !fechasOcupadas.Any(fechaOcupada => fechaOcupada.Fecha == fecha);
    }
    
    public bool EsFechaValida(DateTime fecha)
    {
        return fecha >= FechaMinimaReserva;
    }
}
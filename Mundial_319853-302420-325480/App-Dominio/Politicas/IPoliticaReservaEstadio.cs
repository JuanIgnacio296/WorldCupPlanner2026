namespace App.Dominio;

public interface IPoliticaReservaEstadio
{
    bool EsHorario(DateTime fecha);
    bool EsFechaValida(DateTime fecha);
    bool EsFechaDisponible(DateTime fecha, ICollection<FechaOcupada> fechasOcupadas);
}
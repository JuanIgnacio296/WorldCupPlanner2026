using App.Dominio.Exceptions;

namespace App.Dominio;

public class ReservaEstadioValidacion
{
    private readonly IPoliticaReservaEstadio _politicaReservaEstadio;

    public ReservaEstadioValidacion(IPoliticaReservaEstadio politica)
    {
        _politicaReservaEstadio = politica;
    }
    
    public bool EsFechaDisponible(DateTime fecha, ICollection<FechaOcupada> fechasOcupadas)
    {
        return _politicaReservaEstadio.EsFechaDisponible(fecha, fechasOcupadas);
    }
    
    public void ValidarFechasOcupadas(ICollection<FechaOcupada> fechasOcupadas)
    {
        if (fechasOcupadas == null)
        {
            throw new ArgumentosDeEstadioNoValidos("No se puede ingresar fechas nulas");
        }

        foreach (FechaOcupada fo in fechasOcupadas)
        {
            if (fo.Fecha < DateTime.Now)
            {
                throw new ArgumentosDeEstadioNoValidos("No se puede ingresar fechas pasadas");
            }

            if (fechasOcupadas.Count(f => f.Fecha == fo.Fecha) >= 2)
            {
                throw new ArgumentosDeEstadioNoValidos("No puede ingresar fechas repetidas");
            }
        }
    }

    public void ValidarFechaReservable(DateTime fecha, ICollection<FechaOcupada> fechasOcupadas)
    {
        if (!_politicaReservaEstadio.EsFechaValida(fecha))
        {
            throw new ArgumentosDeEstadioNoValidos("No se puede ingresar fechas pasadas");
        }

        if (!_politicaReservaEstadio.EsHorario(fecha))
        {
            throw new ArgumentosDeEstadioNoValidos("Los horarios válidos son: 14:00, 18:00 y 22:00");
        }

        if (fechasOcupadas.Any(fo => fo.Fecha == fecha))
        {
            throw new ArgumentosDeEstadioNoValidos("No se puede ingresar fechas ocupadas");
        }
    }

    public bool EsHorarioValido(DateTime fecha)
    {
        return _politicaReservaEstadio.EsHorario(fecha);
    }
}
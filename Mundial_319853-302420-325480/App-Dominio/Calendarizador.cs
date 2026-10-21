namespace App.Dominio;

public class Calendarizador
{
    private const int MaximoPartidosPorDia = 3;

    private static readonly string[] Horarios = ["14:00", "18:00", "22:00"];
    private static readonly DateTime FechaBaseMundial = new(2026, 5, 28);

    private readonly List<Partido> _partidos;
    private readonly List<Estadio> _estadios;
    private readonly int _token;
    private int _estadiosDesignados;

    public Calendarizador(List<Partido> partidos, List<Estadio> estadios, int token)
    {
        _partidos = partidos;
        _estadios = estadios;
        _token = token;
    }

    public Partido CrearPartido(Equipo equipo1, Equipo equipo2, string fase, int? jornada, char? grupoId)
    {
        DateTime fecha = DesignarPrimerFechaDisponible(equipo1, equipo2);
        Estadio estadio = DesignarEstadio();
        _estadiosDesignados++;

        while (true)
        {
            foreach (string hora in Horarios)
            {
                fecha = new DateTime(2026, fecha.Month, fecha.Day);
                TimeSpan horario = TimeSpan.Parse(hora);
                DateTime fechaConHorario = fecha + horario;

                if (estadio.ReservaEstadioValidacion.EsFechaDisponible(fechaConHorario, estadio.FechasOcupadas))
                {
                    estadio.AddFecha(fechaConHorario, _token);
                    Partido partido = new(_partidos.Count + 1, _token, grupoId, fechaConHorario, estadio, equipo1, equipo2, null, null, fase, hora, jornada);
                    _partidos.Add(partido);
                    return partido;
                }
            }

            fecha = fecha.AddDays(1);
        }
    }

    public List<Partido> CrearJornada3(Grupo grupo)
    {
        DateTime fecha = DesignarPrimerFechaDisponible(grupo.Equipos[0], grupo.Equipos[1]);
        DateTime fechaSegundoPartido = DesignarPrimerFechaDisponible(grupo.Equipos[2], grupo.Equipos[3]);

        if (fecha < fechaSegundoPartido)
        {
            fecha = fechaSegundoPartido;
        }

        Estadio estadio1 = DesignarEstadio();
        _estadiosDesignados++;
        Estadio estadio2 = DesignarEstadio();
        _estadiosDesignados++;

        while (true)
        {
            foreach (string hora in Horarios)
            {
                fecha = new DateTime(2026, fecha.Month, fecha.Day);
                TimeSpan horario = TimeSpan.Parse(hora);
                DateTime fechaConHorario = fecha + horario;

                if (PuedeAsignarJornada3(estadio1, estadio2, fecha, fechaConHorario))
                {
                    estadio1.AddFecha(fechaConHorario, _token);
                    estadio2.AddFecha(fechaConHorario, _token);

                    Partido partido1 = new(_partidos.Count + 1, _token, grupo.Id, fechaConHorario, estadio1, grupo.Equipos[0], grupo.Equipos[1], null, null, "Fase de Grupos", hora, 3);
                    Partido partido2 = new(_partidos.Count + 2, _token, grupo.Id, fechaConHorario, estadio2, grupo.Equipos[2], grupo.Equipos[3], null, null, "Fase de Grupos", hora, 3);

                    _partidos.Add(partido1);
                    _partidos.Add(partido2);

                    return [partido1, partido2];
                }
            }

            fecha = fecha.AddDays(1);
        }
    }

    public Estadio DesignarEstadio()
    {
        return _estadios[_estadiosDesignados % _estadios.Count];
    }

    public DateTime DesignarPrimerFechaDisponible(Equipo local, Equipo visitante)
    {
        DateTime fechaLocal = UltimoPartido(local);
        DateTime fechaVisitante = UltimoPartido(visitante);
        DateTime ultimaFecha = fechaLocal < fechaVisitante ? fechaVisitante : fechaLocal;

        return ultimaFecha.AddDays(3);
    }

    public DateTime UltimoPartido(Equipo equipo)
    {
        for (int indice = _partidos.Count - 1; indice >= 0; indice--)
        {
            Partido partido = _partidos[indice];
            if (partido.EquipoLocal!.Equals(equipo) || partido.EquipoVisitante!.Equals(equipo))
            {
                return partido.Fecha!.Value;
            }
        }

        return FechaBaseMundial;
    }

    private bool PuedeAsignarJornada3(
        Estadio estadio1,
        Estadio estadio2,
        DateTime fecha,
        DateTime fechaConHorario)
    {
        return estadio1.ReservaEstadioValidacion.EsFechaDisponible(fechaConHorario, estadio1.FechasOcupadas)
               && estadio2.ReservaEstadioValidacion.EsFechaDisponible(fechaConHorario, estadio2.FechasOcupadas)
               && ContarPartidosEnDia(fecha) < MaximoPartidosPorDia;
    }

    private int ContarPartidosEnDia(DateTime fecha)
    {
        return _partidos.Count(partido => partido.Fecha!.Value.Date == fecha.Date);
    }
}
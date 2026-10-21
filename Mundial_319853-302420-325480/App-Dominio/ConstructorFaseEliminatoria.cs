using App.Dominio.Exceptions;

namespace App.Dominio;

public class ConstructorFaseEliminatoria
{
    private readonly Fixture _fixture;
    private readonly Calendarizador _calendarizador;

    public ConstructorFaseEliminatoria(Fixture fixture, Calendarizador calendarizador)
    {
        _fixture = fixture;
        _calendarizador = calendarizador;
    }

    public void Armar(string fase)
    {
        if (fase == "Dieciseisavos de Final")
        {
            ArmarDieciseisavos();
        }
        else if (fase == "Octavos de Final")
        {
            ArmarOctavos();
        }
        else if (fase == "Cuartos de Final")
        {
            ArmarCuartos();
        }
        else if (fase == "Semifinal")
        {
            ArmarSemifinales();
        }
        else if (fase == "Tercer y Cuarto Puesto")
        {
            ArmarTercerYCuartoPuesto();
        }
        else if (fase == "Final")
        {
            ArmarFinal();
        }
    }

    private void ArmarDieciseisavos()
    {
        if (_fixture.PartidosJugados < 72)
        {
            throw new ArgumentosDeFixtureNoValidos(
                "No se puede armar los dieciseisavos de final si no se ha terminado la fase de grupos");
        }

        _fixture.OrganizarListas();
        int indicePrimeros = 0;
        int indiceSegundos = 0;

        for (int indiceTerceros = 7; indiceTerceros >= 0; indiceTerceros--)
        {
            Equipo equipo1 = _fixture.ListaPrimeros[indicePrimeros];
            Grupo grupo = ObtenerGrupo(equipo1)!;
            Equipo equipo2 = _fixture.ListaTerceros[indiceTerceros];

            if (grupo.Equipos.Contains(equipo2))
            {
                equipo2 = ReubicarTerceroParaEvitarMismoGrupo(grupo, equipo2, indiceTerceros);
            }

            _calendarizador.CrearPartido(equipo1, equipo2, "Dieciseisavos de Final", null, null);
            indicePrimeros++;
        }

        for (int indiceUltimoSegundo = 11; indiceUltimoSegundo > indiceSegundos; indiceUltimoSegundo--)
        {
            Equipo equipo1;
            if (indicePrimeros < 12)
            {
                equipo1 = _fixture.ListaPrimeros[indicePrimeros];
                indicePrimeros++;
            }
            else
            {
                equipo1 = _fixture.ListaSegundos[indiceSegundos];
                indiceSegundos++;
            }

            Equipo equipo2 = _fixture.ListaSegundos[indiceUltimoSegundo];
            Grupo grupo = ObtenerGrupo(equipo1)!;
            if (grupo.Equipos.Contains(equipo2))
            {
                Equipo equipo3 = _fixture.ListaTerceros[indiceUltimoSegundo - 1];
                _fixture.ListaTerceros[indiceUltimoSegundo - 1] = equipo2;
                _fixture.ListaTerceros[indiceUltimoSegundo] = equipo3;
                equipo2 = equipo3;
            }

            _calendarizador.CrearPartido(equipo1, equipo2, "Dieciseisavos de Final", null, null);
        }
    }

    private void ArmarOctavos()
    {
        if (_fixture.PartidosJugados < 88)
        {
            throw new ArgumentosDeFixtureNoValidos(
                "No se puede armar los octavos de final si no se han terminado los dieciseisavos de final");
        }

        for (int indice = 72; indice < 80; indice++)
        {
            _calendarizador.CrearPartido(_fixture.ObtenerGanador(_fixture.Partidos[indice])!, _fixture.ObtenerGanador(_fixture.Partidos[indice + 8])!, "Octavos de Final", null, null);
        }
    }

    private void ArmarCuartos()
    {
        if (_fixture.PartidosJugados < 96)
        {
            throw new ArgumentosDeFixtureNoValidos(
                "No se puede armar los cuartos de final si no se han terminado los octavos de final");
        }

        for (int indice = 88; indice < 96; indice += 2)
        {
            _calendarizador.CrearPartido(_fixture.ObtenerGanador(_fixture.Partidos[indice])!, _fixture.ObtenerGanador(_fixture.Partidos[indice + 1])!, "Cuartos de Final", null, null);
        }
    }

    private void ArmarSemifinales()
    {
        if (_fixture.PartidosJugados < 100)
        {
            throw new ArgumentosDeFixtureNoValidos(
                "No se puede armar las semifinales si no se han terminado los cuartos de final");
        }

        for (int indice = 96; indice < 100; indice += 2)
        {
            _calendarizador.CrearPartido(_fixture.ObtenerGanador(_fixture.Partidos[indice])!, _fixture.ObtenerGanador(_fixture.Partidos[indice + 1])!, "Semifinal", null, null);
        }
    }

    private void ArmarTercerYCuartoPuesto()
    {
        if (_fixture.PartidosJugados < 102)
        {
            throw new ArgumentosDeFixtureNoValidos(
                "No se puede armar el Tercer y Cuarto Puesto si no se han terminado las semifinales");
        }

        _calendarizador.CrearPartido(_fixture.ObtenerPerdedor(_fixture.Partidos[100])!, _fixture.ObtenerPerdedor(_fixture.Partidos[101])!, "Tercer y Cuarto Puesto", null, null);
    }

    private void ArmarFinal()
    {
        if (_fixture.PartidosJugados < 102)
        {
            throw new ArgumentosDeFixtureNoValidos("No se puede armar la Final si no se han terminado las semifinales");
        }

        _calendarizador.CrearPartido(_fixture.ObtenerGanador(_fixture.Partidos[100])!, _fixture.ObtenerGanador(_fixture.Partidos[101])!, "Final", null, null);
    }

    private Equipo ReubicarTerceroParaEvitarMismoGrupo(Grupo grupo, Equipo equipo2, int indiceTerceros)
    {
        if (indiceTerceros == 0)
        {
            int indiceSegundo = 11;
            Equipo equipo3 = _fixture.ListaSegundos[indiceSegundo];
            if (grupo.Equipos.Contains(equipo3))
            {
                indiceSegundo = 10;
                equipo3 = _fixture.ListaSegundos[indiceSegundo];
            }

            _fixture.ListaTerceros[indiceTerceros] = equipo3;
            _fixture.ListaSegundos[indiceSegundo] = equipo2;
            return equipo3;
        }

        Equipo terceroAnterior = _fixture.ListaTerceros[indiceTerceros - 1];
        _fixture.ListaTerceros[indiceTerceros - 1] = equipo2;
        _fixture.ListaTerceros[indiceTerceros] = terceroAnterior;
        return terceroAnterior;
    }

    private Grupo? ObtenerGrupo(Equipo equipo)
    {
        return _fixture.Grupos.FirstOrDefault(grupo => grupo.Equipos.Contains(equipo));
    }
}
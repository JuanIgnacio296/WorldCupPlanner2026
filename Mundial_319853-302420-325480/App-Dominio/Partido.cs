using System.Text.RegularExpressions;
using App.Dominio.Exceptions;

namespace App.Dominio;

public class Partido
{
    private static readonly List<string> FasesMundial =
    [
        "Fase de Grupos",
        "Dieciseisavos de Final",
        "Octavos de Final",
        "Cuartos de Final",
        "Semifinal",
        "Tercer y Cuarto Puesto",
        "Final"
    ];

    public static List<string> Fases => new List<string>(FasesMundial);

    private static readonly Regex FormatoHorario = new(
        @"^([01]\d|2[0-3]):([0-5]\d)$",
        RegexOptions.Compiled);
    
    private int _id;
    private int _token;
    private char? _grupoID;
    private DateTime? _fecha;
    private Estadio? _estadio;
    private Equipo? _equipoLocal;
    private Equipo? _equipoVisitante;
    private int? _golesLocal;
    private int? _golesVisitante;
    private string? _fase;
    private string? _horario;
    private int? _jornada;
    private int? _penalesLocal;
    private int? _penalesVisitante;
    private int _tarjetasAmarillasLocal;
    private int _tarjetasRojasLocal;
    private int _tarjetasAmarillasVisitante;
    private int _tarjetasRojasVisitante;

    public Partido(int id, int token, char? grupoID, DateTime? fecha, Estadio? estadio, Equipo? equipoLocal, Equipo? equipoVisitante, int? golesLocal, int? golesVisitante, string? fase, string? horario, int? jornada)
    {
        Id = id;
        Token = token;
        Fase = fase!;
        GrupoID = grupoID;
        Fecha = fecha;
        Estadio = estadio;
        EquipoLocal = equipoLocal;
        EquipoVisitante = equipoVisitante;
        Horario = horario!;
        Jornada = jornada;
        
        if (golesLocal != null && golesVisitante != null)
        {
            AsignarResultado(golesLocal.Value, golesVisitante.Value);
        }
    }

    public Partido(int id, int token, char? grupoID, DateTime? fecha, Estadio? estadio, Equipo? equipoLocal, Equipo? equipoVisitante, string? fase, int? jornada)
    {
        Id = id;
        Token = token;
        Fase = fase!;
        GrupoID = grupoID;
        Fecha = fecha;
        Estadio = estadio;
        EquipoLocal = equipoLocal;
        EquipoVisitante = equipoVisitante;
        Jornada = jornada;
    }

    private Partido() { }

    public int Id
    {
        get => _id;
        set
        {
            ValidarId(value);
            _id = value;
        }
    }

    public int Token
    {
        get => _token;
        set => _token = value;
    }
    
    public char? GrupoID
    {
        get => _grupoID;
        set
        {
            ValidarGrupo(value);
            _grupoID = value;
        }
    }

    public DateTime? Fecha
    {
        get => _fecha;
        set
        {
            if (value == null)
                throw new ArgumentosDePartidoNoValidos("La fecha es obligatoria");

            _fecha = value;
        }
    }

    public Estadio? Estadio
    {
        get => _estadio;
        set
        {
            if (value == null)
                throw new ArgumentosDePartidoNoValidos("El estadio es obligatorio");

            _estadio = value;
        }
    }

    public Equipo? EquipoLocal
    {
        get => _equipoLocal;
        set
        {
            ValidarEquipoLocal(value);
            _equipoLocal = value;
        }
    }

    public Equipo? EquipoVisitante
    {
        get => _equipoVisitante;
        set
        {
            ValidarEquipoVisitante(value);
            _equipoVisitante = value;
        }
    }

    public int? GolesLocal
    {
        get => _golesLocal;
        private set
        {
            ValidarGoles(value);
            _golesLocal = value;
        }
    }

    public int? GolesVisitante
    {
        get => _golesVisitante;
        private set
        {
            ValidarGoles(value);
            _golesVisitante = value;
        }
    }

    public string Fase
    {
        get => _fase!;
        set
        {
            string faseNormalizada = NormalizarFase(value);
            ValidarFase(faseNormalizada);
            _fase = faseNormalizada;
        }
    }

    public string Horario
    {
        get => _horario!;
        set
        {
            ValidarHorario(value);
            _horario = value;
        }
    }

    public int? Jornada
    {
        get => _jornada;
        set
        {
            ValidarJornada(value);
            _jornada = value;
        }
    }

    public int? PenalesLocal => _penalesLocal;
    public int? PenalesVisitante => _penalesVisitante;
    public int TarjetasAmarillasLocal => _tarjetasAmarillasLocal;
    public int TarjetasRojasLocal => _tarjetasRojasLocal;
    public int TarjetasAmarillasVisitante => _tarjetasAmarillasVisitante;
    public int TarjetasRojasVisitante => _tarjetasRojasVisitante;

    public string Vencedor
    {
        get
        {
            if (_golesLocal > _golesVisitante) return _equipoLocal!.Nombre;
            if (_golesLocal < _golesVisitante) return _equipoVisitante!.Nombre;

            if (_fase != "Fase de Grupos" && _penalesLocal != null && _penalesVisitante != null)
            {
                return ObtenerVencedorPorPenales();
            }

            return "Empate";
        }
    }

    public Equipo? EquipoGanador
    {
        get
        {
            string vencedor = Vencedor;

            if (vencedor == _equipoLocal?.Nombre) return _equipoLocal;
            if (vencedor == _equipoVisitante?.Nombre) return _equipoVisitante;

            return null;
        }
    }

    public Equipo? EquipoPerdedor
    {
        get
        {
            string vencedor = Vencedor;

            if (vencedor == _equipoLocal?.Nombre) return _equipoVisitante;
            if (vencedor == _equipoVisitante?.Nombre) return _equipoLocal;

            return null;
        }
    }

    public void AsignarResultado(int golesLocal, int golesVisitante)
    {
        ValidarGolesResultado(golesLocal, golesVisitante);

        if (Fase == "Fase de Grupos" && EquipoLocal != null && EquipoVisitante != null
            && GolesLocal != null && GolesVisitante != null)
        {
            RevertirEstadisticasEquipos(GolesLocal.Value, GolesVisitante.Value);
        }

        GolesLocal = golesLocal;
        GolesVisitante = golesVisitante;

        if (Fase == "Fase de Grupos" && EquipoLocal != null && EquipoVisitante != null)
        {
            ActualizarEstadisticasEquipos(golesLocal, golesVisitante);
        }
    }

    public void AsignarPenales(int penalesLocal, int penalesVisitante)
    {
        ValidarPenales(penalesLocal, penalesVisitante);
        _penalesLocal = penalesLocal;
        _penalesVisitante = penalesVisitante;
    }

    public void Simular(Random rnd, bool usaMotorProbabilistico)
    {
        Equipo local = EquipoLocal!;
        Equipo visitante = EquipoVisitante!;

        double probabilidadEmpate = 0.2;
        double probabilidadRestante = 0.8;
        double probabilidadLocal = 0.5;

        if (usaMotorProbabilistico)
        {
            probabilidadEmpate = 0.15;
            probabilidadRestante = 0.85;
            int totalRankingFifa = local.RankingFifa + visitante.RankingFifa;
            probabilidadLocal = (double)local.RankingFifa / totalRankingFifa;
        }

        double valor = rnd.NextDouble();
        int maxGoles = 6;
        if (usaMotorProbabilistico && Math.Abs(local.RankingFifa - visitante.RankingFifa) > 1000)
        {
            maxGoles = 9;
        }

        if (valor < probabilidadEmpate)
        {
            int goles = rnd.Next(0, 4);
            AsignarResultado(goles, goles);
            if (Fase != "Fase de Grupos")
            {
                SimularPenales(rnd, usaMotorProbabilistico);
            }
        }
        else if (valor < probabilidadEmpate + probabilidadLocal * probabilidadRestante)
        {
            int golesLocal = rnd.Next(1, maxGoles);
            int golesVisitante = rnd.Next(0, golesLocal);
            AsignarResultado(golesLocal, golesVisitante);
        }
        else
        {
            int golesVisitante = rnd.Next(1, maxGoles);
            int golesLocal = rnd.Next(0, golesVisitante);
            AsignarResultado(golesLocal, golesVisitante);
        }

        SimularTarjetas(rnd);
    }

    private void SimularTarjetas(Random rnd)
    {
        int amarillasLocal = rnd.Next(0, 6);
        int amarillasVisitante = rnd.Next(0, 6);
        int rojasLocal = rnd.Next(0, 2);
        int rojasVisitante = rnd.Next(0, 2);

        RegistrarTarjetas(amarillasLocal, rojasLocal, amarillasVisitante, rojasVisitante);
    }

    public void SimularPenales(Random rnd, bool usaMotorProbabilistico)
    {
        double probabilidadLocal = 0.5;
        double probabilidadVisitante = 0.5;

        if (usaMotorProbabilistico)
        {
            probabilidadLocal = Math.Min(0.50 + ((double)EquipoLocal!.RankingFifa / 2500), 0.90);
            probabilidadVisitante = Math.Min(0.50 + ((double)EquipoVisitante!.RankingFifa / 2500), 0.90);
        }

        int golesLocal = 0;
        int golesVisitante = 0;
        int penalesPateados = 0;
        bool serieDefinida = false;

        while (!serieDefinida)
        {
            penalesPateados++;

            double valorLocal = rnd.NextDouble();
            double valorVisitante = rnd.NextDouble();

            if (valorLocal < probabilidadLocal)
            {
                golesLocal++;
            }

            if (valorVisitante < probabilidadVisitante)
            {
                golesVisitante++;
            }

            int diferencia = Math.Abs(golesLocal - golesVisitante);

            if (diferencia > 0 && penalesPateados >= 3 && diferencia > (5 - penalesPateados))
            {
                serieDefinida = true;
            }

            if (penalesPateados >= 5 && golesLocal != golesVisitante)
            {
                serieDefinida = true;
            }
        }

        AsignarPenales(golesLocal, golesVisitante);
    }

    public void RegistrarTarjetas(int amarillasLocal, int rojasLocal, int amarillasVisitante, int rojasVisitante)
    {
        ValidarTarjetas(amarillasLocal, rojasLocal, amarillasVisitante, rojasVisitante);

        _tarjetasAmarillasLocal = amarillasLocal;
        _tarjetasRojasLocal = rojasLocal;
        _tarjetasAmarillasVisitante = amarillasVisitante;
        _tarjetasRojasVisitante = rojasVisitante;
    }

    private static void ValidarId(int id)
    {
        if (id < 1 || id > 104)
        {
            throw new ArgumentosDePartidoNoValidos("El id debe ser un entero entre 1 y 104 (inclusive)");
        }
    }
    
    private void ValidarGrupo(char? grupoID)
    {
        if (grupoID == null && Fase == "Fase de Grupos")
        {
            throw new ArgumentosDePartidoNoValidos("El grupo es obligatorio en partidos de Fase de Grupos");
        } 
        if (grupoID != null && Fase == "Fase de Grupos" && (grupoID < 'A' || grupoID > 'L'))
        {
            throw new ArgumentosDePartidoNoValidos("El grupo debe estar compuesto por letras del A a la L");
        } 
        if (grupoID != null && Fase != "Fase de Grupos")
        {
            throw new ArgumentosDePartidoNoValidos("El grupo no puede ser ingresado en partidos que no sean de Fase de Grupos");
        }
    }

    private void ValidarEquipoLocal(Equipo? equipoLocal)
    {
        if (equipoLocal == null)
        {
            throw new ArgumentosDePartidoNoValidos("El equipo local es obligatorio");
        }

        if (_equipoVisitante != null && equipoLocal.Nombre == _equipoVisitante.Nombre)
        {
            throw new ArgumentosDePartidoNoValidos("Un equipo no puede jugar un partido contra sí mismo");
        }
    }

    private void ValidarEquipoVisitante(Equipo? equipoVisitante)
    {
        if (equipoVisitante == null)
        {
            throw new ArgumentosDePartidoNoValidos("El equipo visitante es obligatorio");
        }

        if (_equipoLocal != null && equipoVisitante.Nombre == _equipoLocal.Nombre)
        {
            throw new ArgumentosDePartidoNoValidos("Un equipo no puede jugar un partido contra sí mismo");
        }
    }

    private static void ValidarGoles(int? goles)
    {
        if (goles != null && goles < 0)
        {
            throw new ArgumentosDePartidoNoValidos("No se pueden ingresar goles negativos");
        }
    }

    private static void ValidarGolesResultado(int golesLocal, int golesVisitante)
    {
        if (golesLocal < 0)
        {
            throw new ArgumentosDePartidoNoValidos("La cantidad de goles del local debe ser un entero mayor o igual a 0");
        }

        if (golesVisitante < 0)
        {
            throw new ArgumentosDePartidoNoValidos("La cantidad de goles del visitante debe ser un entero mayor o igual a 0");
        }
    }

    private static string NormalizarFase(string? fase)
    {
        if (fase == null)
        {
            throw new ArgumentosDePartidoNoValidos("La fase es obligatoria");
        }

        return fase is "Grupos" or "Fase de grupos" ? "Fase de Grupos" : fase;
    }

    private static void ValidarFase(string fase)
    {
        if (string.IsNullOrEmpty(fase) || !FasesMundial.Contains(fase))
        {
            throw new ArgumentosDePartidoNoValidos("Fase inválida");
        }
    }

    private static void ValidarHorario(string? horario)
    {
        if (string.IsNullOrEmpty(horario))
        {
            throw new ArgumentosDePartidoNoValidos("El horario no puede ser vacio");
        }

        if (!FormatoHorario.IsMatch(horario))
        {
            throw new ArgumentosDePartidoNoValidos("El formato del horario debe ser HH:mm (24 horas)");
        }
    }

    private void ValidarJornada(int? jornada)
    {
        if (jornada == null && Fase == "Fase de Grupos")
        {
            throw new ArgumentosDePartidoNoValidos("La jornada es obligatoria en partidos de fase de grupos");
        }

        if (jornada <= 0)
        {
            throw new ArgumentosDePartidoNoValidos("La jornada no puede ser negativa o 0");
        }
    }

    private void ValidarPenales(int penalesLocal, int penalesVisitante)
    {
        if (_fase == "Fase de Grupos")
        {
            throw new ArgumentosDePartidoNoValidos("Un partido de Fase de Grupos no se puede definir por penales");
        }

        if (_golesLocal != null && _golesVisitante != null && _golesLocal != _golesVisitante)
        {
            throw new ArgumentosDePartidoNoValidos("En un partido que no terminó en empate no puede haber definición por penales.");
        }

        if (penalesLocal == penalesVisitante)
        {
            throw new ArgumentosDePartidoNoValidos("La cantidad de penales convertidos por ambos equipos no puede ser la misma.");
        }

        if (penalesLocal < 0 || penalesVisitante < 0)
        {
            throw new ArgumentosDePartidoNoValidos("La cantidad de penales convertidos por ambos equipos debe ser mayor o igual a 0.");
        }
    }

    private static void ValidarTarjetas(int amarillasLocal, int rojasLocal, int amarillasVisitante, int rojasVisitante)
    {
        if (amarillasLocal < 0 || rojasLocal < 0 || amarillasVisitante < 0 || rojasVisitante < 0)
        {
            throw new ArgumentosDePartidoNoValidos("La cantidad de tarjetas debe ser mayor o igual a 0");
        }
    }

    private void ActualizarEstadisticasEquipos(int golesLocal, int golesVisitante)
    {
        int puntosLocal = golesLocal > golesVisitante ? 3 : (golesLocal == golesVisitante ? 1 : 0);
        int puntosVisitante = golesVisitante > golesLocal ? 3 : (golesLocal == golesVisitante ? 1 : 0);

        _equipoLocal!.ActualizarEquipo(puntosLocal, golesLocal, golesVisitante);
        _equipoVisitante!.ActualizarEquipo(puntosVisitante, golesVisitante, golesLocal);
    }

    private void RevertirEstadisticasEquipos(int golesLocal, int golesVisitante)
    {
        int puntosLocal = golesLocal > golesVisitante ? 3 : (golesLocal == golesVisitante ? 1 : 0);
        int puntosVisitante = golesVisitante > golesLocal ? 3 : (golesLocal == golesVisitante ? 1 : 0);

        _equipoLocal!.RevertirEstadisticas(puntosLocal, golesLocal, golesVisitante);
        _equipoVisitante!.RevertirEstadisticas(puntosVisitante, golesVisitante, golesLocal);
    }

    private string ObtenerVencedorPorPenales()
    {
        if (_penalesLocal > _penalesVisitante) return _equipoLocal!.Nombre;
        if (_penalesLocal < _penalesVisitante) return _equipoVisitante!.Nombre;
        return "Empate";
    }
}
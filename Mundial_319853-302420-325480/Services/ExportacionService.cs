using App.Dominio;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;

namespace Services;

public class ExportacionService : IExportacionService
{
    private readonly IinMemoryLog _logs;
    private readonly IFixtureService _fixtureService;
    private readonly IXlsxExportador _xlsxExportador;
    private readonly ICsvExportador _csvExportador;
    private readonly IAuditoriaService _auditoriaService;
    private readonly AuditoriaFilaMapeador _auditoriaFilaMapeador = new AuditoriaFilaMapeador();
    private readonly FixtureFilaMapeador _fixtureFilaMapeador = new FixtureFilaMapeador();
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();

    public ExportacionService(IinMemoryLog logs, IFixtureService fixtureService, IAuditoriaService auditoriaService)
        : this(logs, fixtureService, new XlsxExportador(), new CsvExportador(), auditoriaService)
    {
    }

    public ExportacionService(IinMemoryLog logs, IFixtureService fixtureService, IXlsxExportador xlsxExportador, IAuditoriaService auditoriaService)
        : this(logs, fixtureService, xlsxExportador, new CsvExportador(), auditoriaService)
    {
    }

    public ExportacionService(IinMemoryLog logs, IFixtureService fixtureService, IXlsxExportador xlsxExportador, ICsvExportador csvExportador, IAuditoriaService auditoriaService)
    {
        _logs = logs;
        _fixtureService = fixtureService;
        _xlsxExportador = xlsxExportador;
        _csvExportador = csvExportador;
        _auditoriaService = auditoriaService;
    }

    public byte[] ExportarAuditoriaCsv(Usuario solicitante, DateTime inicio, DateTime fin)
    {
        ValidarAutorizado(solicitante);
        ValidarRango(inicio, fin);

        List<string[]> filas = _auditoriaFilaMapeador.Mapear(_logs.ObtenerLogs(inicio, fin));
        ValidarHayDatos(filas, "No hay registros de auditoría en el rango de fechas seleccionado.");

        string[] encabezados = new string[] { "Fecha", "Usuario", "Correo", "Accion" };
        byte[] archivo = _csvExportador.CrearArchivo(encabezados, filas);

        _auditoriaService.RegistrarLog(solicitante, "Exportación de archivo Auditoria.csv ");

        return archivo;
    }

    public byte[] ExportarFixtureFinalCsv(Usuario solicitante, DateTime inicio, DateTime fin)
    {
        ValidarAutorizado(solicitante);
        ValidarRango(inicio, fin);

        List<string[]> filas = _fixtureFilaMapeador.Mapear(ObtenerPartidosFinal(inicio, fin));
        ValidarHayDatos(filas, "No hay partidos finales en el rango de fechas seleccionado.");

        string[] encabezados = new string[] { "Fecha", "Fase", "Jornada", "Local", "GolesLocal", "GolesVisitante", "Visitante", "Estadio" };
        byte[] archivo = _csvExportador.CrearArchivo(encabezados, filas);

        _auditoriaService.RegistrarLog(solicitante, "Exportación de archivo FixtureFinal.csv ");
        return archivo;
    }

    public byte[] ExportarAuditoriaXlsx(Usuario solicitante, DateTime inicio, DateTime fin)
    {
        ValidarAutorizado(solicitante);
        ValidarRango(inicio, fin);

        List<string[]> filas = _auditoriaFilaMapeador.Mapear(_logs.ObtenerLogs(inicio, fin));
        ValidarHayDatos(filas, "No hay registros de auditoría en el rango de fechas seleccionado.");

        string[] encabezados = new string[] { "Fecha", "Usuario", "Correo", "Accion" };
        byte[] archivo = _xlsxExportador.CrearArchivo(encabezados, filas);

        _auditoriaService.RegistrarLog(solicitante, "Exportación de archivo Auditoria.xlsx");

        return archivo;
    }

    public byte[] ExportarFixtureFinalXlsx(Usuario solicitante, DateTime inicio, DateTime fin)
    {
        ValidarAutorizado(solicitante);
        ValidarRango(inicio, fin);

        List<string[]> filas = _fixtureFilaMapeador.Mapear(ObtenerPartidosFinal(inicio, fin));
        ValidarHayDatos(filas, "No hay partidos finales en el rango de fechas seleccionado.");

        string[] encabezados = new string[] { "Fecha", "Fase", "Jornada", "Local", "GolesLocal", "GolesVisitante", "Visitante", "Estadio" };
        byte[] archivo = _xlsxExportador.CrearArchivo(encabezados, filas);

        _auditoriaService.RegistrarLog(solicitante, "Exportación de archivo FixtureFinal.xlsx");

        return archivo;
    }

    public byte[] ExportarAuditoriaCsv(UsuarioReadDto solicitante, DateTime inicio, DateTime fin)
        => ExportarAuditoriaCsv(_usuarioMapeador.ADominio(solicitante), inicio, fin);

    public byte[] ExportarFixtureFinalCsv(UsuarioReadDto solicitante, DateTime inicio, DateTime fin)
        => ExportarFixtureFinalCsv(_usuarioMapeador.ADominio(solicitante), inicio, fin);

    public byte[] ExportarAuditoriaXlsx(UsuarioReadDto solicitante, DateTime inicio, DateTime fin)
        => ExportarAuditoriaXlsx(_usuarioMapeador.ADominio(solicitante), inicio, fin);

    public byte[] ExportarFixtureFinalXlsx(UsuarioReadDto solicitante, DateTime inicio, DateTime fin)
        => ExportarFixtureFinalXlsx(_usuarioMapeador.ADominio(solicitante), inicio, fin);

    private List<Partido> ObtenerPartidosFinal(DateTime inicio, DateTime fin)
    {
        Fixture fixture = _fixtureService.ObtenerFixture();
        if (fixture == null)
        {
            return new List<Partido>();
        }

        return fixture.ObtenerPartidosFinalPorFecha(inicio, fin);
    }

    private static void ValidarAutorizado(Usuario solicitante)
    {
        if (!solicitante.TieneRol(Rol.Administrador) && !solicitante.TieneRol(Rol.Periodista))
        {
            throw new UnauthorizedAccessException("Solo Administrador o Periodista pueden exportar auditorias.");
        }
    }

    private static void ValidarRango(DateTime inicio, DateTime fin)
    {
        if (inicio > fin)
        {
            throw new ArgumentException("La fecha 'desde' no puede ser posterior a la fecha 'hasta'.");
        }
    }

    private static void ValidarHayDatos(List<string[]> filas, string mensaje)
    {
        if (filas.Count == 0)
        {
            throw new InvalidOperationException(mensaje);
        }
    }
}
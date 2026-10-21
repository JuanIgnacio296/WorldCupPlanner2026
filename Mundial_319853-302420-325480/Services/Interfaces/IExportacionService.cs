using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IExportacionService
{
    byte[] ExportarAuditoriaCsv(Usuario solicitante, DateTime inicio, DateTime fin);
    byte[] ExportarFixtureFinalCsv(Usuario solicitante, DateTime inicio, DateTime fin);
    byte[] ExportarAuditoriaXlsx(Usuario solicitante, DateTime inicio, DateTime fin);
    byte[] ExportarFixtureFinalXlsx(Usuario solicitante, DateTime inicio, DateTime fin);

    byte[] ExportarAuditoriaCsv(UsuarioReadDto solicitante, DateTime inicio, DateTime fin);
    byte[] ExportarFixtureFinalCsv(UsuarioReadDto solicitante, DateTime inicio, DateTime fin);
    byte[] ExportarAuditoriaXlsx(UsuarioReadDto solicitante, DateTime inicio, DateTime fin);
    byte[] ExportarFixtureFinalXlsx(UsuarioReadDto solicitante, DateTime inicio, DateTime fin);
}

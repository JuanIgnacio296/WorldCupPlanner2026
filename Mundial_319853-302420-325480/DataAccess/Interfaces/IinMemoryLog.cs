using App.Dominio;

namespace DataAccess.Interfaces;

public interface IinMemoryLog
{
    public void AgregarLog(Log log);
    public List<Log> ObtenerLogs();
    public List<Log> ObtenerLogs(DateTime inicio, DateTime fin);
}

using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess;

public class InMemoryLog : IinMemoryLog
{
    private List<Log> _listaLogs;
    private int _proximId;

    public InMemoryLog()
    {
        _listaLogs = new List<Log>();
        _proximId = 1;
    }

    public void AgregarLog(Log log)
    {
        log.Id = _proximId;
        _proximId++;
        _listaLogs.Add(log);
    }

    public List<Log> ObtenerLogs()
    {
        return _listaLogs.ToList();
    }

    public List<Log> ObtenerLogs(DateTime inicio, DateTime fin)
    {
        return _listaLogs.Where(l => l.Fecha >= inicio && l.Fecha <= fin).ToList();
    }
    
}

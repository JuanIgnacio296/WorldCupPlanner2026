using App.Dominio;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class LogEFRepository : IinMemoryLog
{
    private readonly AppDbContext _context;


    public LogEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AgregarLog(Log log)
    {
        Usuario? trackeado = _context.Usuarios.Local.FirstOrDefault(u => u.Correo == log.Usuario.Correo);
        if (trackeado != null)
        {
            log.Usuario = trackeado;
        }
        else
        {
            _context.Usuarios.Attach(log.Usuario);
        }

        _context.Logs.Add(log);
        _context.SaveChanges();
    }

    public List<Log> ObtenerLogs()
    {
        return _context.Logs.ToList();
    }

    public List<Log> ObtenerLogs(DateTime inicio, DateTime fin)
    {
        return _context.Logs.Where(l => l.Fecha >= inicio && l.Fecha <= fin).ToList();   
    }
}
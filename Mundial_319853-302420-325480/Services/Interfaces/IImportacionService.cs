using App.Dominio;
using DTOs;

namespace Services.Interfaces;

public interface IImportacionService
{
    ResultadoImportacion ImportarEquipos(Usuario editor, string contenidoCsv);
    ResultadoImportacionReadDto ImportarEquipos(UsuarioReadDto editor, string contenidoCsv);
}


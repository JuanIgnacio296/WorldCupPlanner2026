using App.Dominio.Exceptions;

namespace App.Dominio;

public class ResultadoImportacion
{
    private readonly List<string> _listaErrores = [];
    private int _cantidadImportada;

    public int CantidadImportada => _cantidadImportada;

    public List<string> Errores => _listaErrores.ToList();

    public bool FueExitosa => _listaErrores.Count == 0;

    public void AgregarError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentosDeResultadoImportacionNoValidos("El error no puede ser vacío.");
        }

        _listaErrores.Add(error);
    }

    public void AumentarCantidadImportada()
    {
        _cantidadImportada++;
    }
}




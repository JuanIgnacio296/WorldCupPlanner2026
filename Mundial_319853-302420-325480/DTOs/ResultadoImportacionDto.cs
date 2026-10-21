namespace DTOs;

public class ResultadoImportacionReadDto
{
    public int CantidadImportada { get; set; }
    public List<string> Errores { get; set; } = [];
    public bool FueExitosa => Errores.Count == 0;
}

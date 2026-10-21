using App.Dominio;
using DataAccess;
using DTOs;
using Services;
using Services.Interfaces;

namespace DTOs.Tests;

[TestClass]
public class ImportacionDtoServiceTests
{
    private IImportacionService _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        _servicio = new ImportacionService(new InMemoryEquipo(), auditoriaService);
    }

    [TestMethod]
    public void ImportarEquipos_ConUsuarioReadDtoYCsvValido_DebeImportarYDevolverResultadoImportacionReadDto()
    {
        UsuarioReadDto editor = CrearEditor();
        string csv = """
                     Nombre,Confederacion,RankingFifa
                     Uruguay,CONMEBOL,500
                     """;

        ResultadoImportacionReadDto resultado = _servicio.ImportarEquipos(editor, csv);

        Assert.AreEqual(1, resultado.CantidadImportada);
        Assert.AreEqual(0, resultado.Errores.Count);
        Assert.AreNotEqual(typeof(ResultadoImportacion), resultado.GetType());
    }

    [TestMethod]
    public void ImportarEquipos_ConCsvInvalido_DebeDevolverResultadoImportacionReadDtoConErrores()
    {
        UsuarioReadDto editor = CrearEditor();
        string csv = """
                     Nombre,Confederacion,RankingFifa
                     Uruguay,INVALIDA,500
                     """;

        ResultadoImportacionReadDto resultado = _servicio.ImportarEquipos(editor, csv);

        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.IsTrue(resultado.Errores[0].Contains("confederación es inválida"));
        Assert.AreNotEqual(typeof(ResultadoImportacion), resultado.GetType());
    }
    
    [TestMethod]
    public void ResultadoImportacionReadDto_FueExitosa_DebeFuncionarSegunErrores()
    {
        ResultadoImportacionReadDto exitoso = new ResultadoImportacionReadDto { Errores = [] };
        ResultadoImportacionReadDto fallido = new ResultadoImportacionReadDto { Errores = ["Error"] };

        Assert.IsTrue(exitoso.FueExitosa);
        Assert.IsFalse(fallido.FueExitosa);
    }

    private static UsuarioReadDto CrearEditor()
    {
        return new UsuarioReadDto
        {
            Nombre = "Editor",
            Apellido = "Importacion",
            Correo = "editor.importacion@ort.edu.uy",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            Roles = ["Editor"]
        };
    }
}

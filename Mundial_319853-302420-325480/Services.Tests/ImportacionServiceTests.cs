using App.Dominio;
using DataAccess;
using Services.Interfaces;

namespace Services.Tests;

[TestClass]
public class ImportacionServiceTests
{
    [TestMethod]
    public void Constructor_WhenCreated_ThenServiceIsNotNull()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Assert.IsNotNull(service);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenCalled_ThenReturnsResultadoImportacion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        ResultadoImportacion resultado = service.ImportarEquipos(editor, "");

        Assert.IsNotNull(resultado);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenUsuarioNoEsEditor_ThenLanzaExcepcion()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario noEditor = new Usuario(
            "Juan",
            "Perez",
            "juan@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Juan123#",
            [Rol.Ninguno]
        );

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            service.ImportarEquipos(noEditor, "");
        });
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenContenidoCsvVacio_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        ResultadoImportacion resultado = service.ImportarEquipos(editor, "");

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("El contenido del CSV no puede estar vacío.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenEncabezadoIncorrecto_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv = "Nombre,Continente,RankingFIFA";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("El encabezado del CSV es incorrecto.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenFilaValida_ThenImportaUnEquipo()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1800";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsTrue(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.CantidadImportada);
        Assert.AreEqual(0, resultado.Errores.Count);

        List<Equipo> equipos = inMemoryEquipo.ObtenerEquipos();

        Assert.AreEqual(1, equipos.Count);
        Assert.AreEqual("Uruguay", equipos[0].Nombre);
        Assert.AreEqual("CONMEBOL", equipos[0].Confederacion);
        Assert.AreEqual(1800, equipos[0].RankingFifa);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenVariasFilasValidas_ThenImportaTodosLosEquipos()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1800\n" +
            "Francia,UEFA,1900\n" +
            "Japon,AFC,1500";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsTrue(resultado.FueExitosa);
        Assert.AreEqual(3, resultado.CantidadImportada);
        Assert.AreEqual(0, resultado.Errores.Count);

        List<Equipo> equipos = inMemoryEquipo.ObtenerEquipos();

        Assert.AreEqual(3, equipos.Count);

        Assert.AreEqual("Uruguay", equipos[0].Nombre);
        Assert.AreEqual("CONMEBOL", equipos[0].Confederacion);
        Assert.AreEqual(1800, equipos[0].RankingFifa);

        Assert.AreEqual("Francia", equipos[1].Nombre);
        Assert.AreEqual("UEFA", equipos[1].Confederacion);
        Assert.AreEqual(1900, equipos[1].RankingFifa);

        Assert.AreEqual("Japon", equipos[2].Nombre);
        Assert.AreEqual("AFC", equipos[2].Confederacion);
        Assert.AreEqual(1500, equipos[2].RankingFifa);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenFilaTieneCantidadIncorrectaDeColumnas_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: cantidad incorrecta de columnas.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenRankingFifaNoEsNumero_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,ABC";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: RankingFIFA debe ser numérico.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenConfederacionInvalida_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,SUDAMERICA,1800";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: La confederación es inválida.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenRankingFifaFueraDeRango_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv = "Nombre,Confederacion,RankingFifa\n" + "Uruguay,CONMEBOL,200";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: RankingFIFA fuera de rango.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenNombreVacio_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            ",CONMEBOL,1800";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: El nombre no puede ser vacío.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenNombreRepetidoEnCsv_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1800\n" +
            "Uruguay,CONMEBOL,1900";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 3: Ya existe un equipo con ese nombre.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenEquipoYaExisteEnMemoria_ThenResultadoTieneError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        Equipo equipoExistente = new Equipo("Uruguay", "CONMEBOL", 1800);
        inMemoryEquipo.AgregarEquipo(equipoExistente);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1900";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: Ya existe un equipo con ese nombre.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(1, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenUnaFilaValidaYUnaInvalida_ThenNoImportaNadaYRegistraError()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1800\n" +
            "EquipoInvalido,SUDAMERICA,1700";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 3: La confederación es inválida.", resultado.Errores[0]);

        List<Equipo> equipos = inMemoryEquipo.ObtenerEquipos();

        Assert.AreEqual(0, equipos.Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenImportacionExitosa_ThenRegistraLog()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1800\n" +
            "Francia,UEFA,1900";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        List<Log> logs = auditoriaService.ObtenerLogs(admin);

        Assert.IsTrue(resultado.FueExitosa);
        Assert.AreEqual(2, resultado.CantidadImportada);
        Assert.AreEqual(1, logs.Count);
        Assert.IsTrue(logs[0].Accion.Contains("Importación de equipos exitosa"));
        Assert.IsTrue(logs[0].Accion.Contains("2"));
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenImportacionTieneErrores_ThenRegistraLog()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        Usuario admin = new Usuario(
            "Admin",
            "Sist",
            "admin@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Admin123#",
            [Rol.Administrador]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,SUDAMERICA,1800";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        List<Log> logs = auditoriaService.ObtenerLogs(admin);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(1, resultado.Errores.Count);

        Assert.AreEqual(1, logs.Count);
        Assert.IsTrue(logs[0].Accion.Contains("Importación de equipos con errores"));
        Assert.IsTrue(logs[0].Accion.Contains("1"));
    }
    
    [TestMethod]
    public void ImportarEquipos_WhenHayVariasValidasPeroUnaInvalida_ThenNoImportaNinguna()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);

        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "Uruguay,CONMEBOL,1800\n" +
            "Francia,UEFA,1900\n" +
            "EquipoInvalido,SUDAMERICA,1700\n" +
            "Japon,AFC,1500";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 4: La confederación es inválida.", resultado.Errores[0]);
        Assert.AreEqual(0, inMemoryEquipo.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void ImportarEquipos_CuandoYaHay48Equipos_ThenResultadoTieneErrorYNoImporta()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        CargarEquiposHastaCompletarCupos(inMemoryEquipo);

        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = CrearEditor();

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "EquipoExtra,UEFA,1800";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: Se supera el máximo de 48 equipos.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(48, inMemoryEquipo.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void ImportarEquipos_CuandoConfederacionYaTieneCupoMaximo_ThenResultadoTieneErrorYNoImporta()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();

        for (int i = 1; i <= 16; i++)
        {
            inMemoryEquipo.AgregarEquipo(new Equipo($"UEFA_{i}", "UEFA", 1000 + i));
        }

        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = CrearEditor();

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "UEFA_Extra,UEFA,1900";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 2: Se supera el cupo máximo de UEFA.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(16, inMemoryEquipo.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void ImportarEquipos_CuandoCsvSuperaCupoPorConfederacion_ThenNoImportaNingunEquipo()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();

        for (int i = 1; i <= 15; i++)
        {
            inMemoryEquipo.AgregarEquipo(new Equipo($"UEFA_{i}", "UEFA", 1000 + i));
        }

        InMemoryLog inMemoryLog = new InMemoryLog();
        IAuditoriaService auditoriaService = new AuditoriaService(inMemoryLog);
        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);

        Usuario editor = CrearEditor();

        string contenidoCsv =
            "Nombre,Confederacion,RankingFifa\n" +
            "UEFA_16,UEFA,1800\n" +
            "UEFA_17,UEFA,1810";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, contenidoCsv);

        Assert.IsFalse(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual("Fila 3: Se supera el cupo máximo de UEFA.", resultado.Errores[0]);
        Assert.AreEqual(0, resultado.CantidadImportada);
        Assert.AreEqual(15, inMemoryEquipo.ObtenerEquipos().Count);
    }

    private Usuario CrearEditor()
    {
        return new Usuario(
            "Editor",
            "Sist",
            "editor@ort.edu.uy",
            DateTime.Now.AddYears(-20),
            "Editor123#",
            [Rol.Editor]
        );
    }

    private void CargarEquiposHastaCompletarCupos(InMemoryEquipo inMemoryEquipo)
    {
        CargarEquipos(inMemoryEquipo, "UEFA", 16);
        CargarEquipos(inMemoryEquipo, "CONMEBOL", 7);
        CargarEquipos(inMemoryEquipo, "CONCACAF", 7);
        CargarEquipos(inMemoryEquipo, "CAF", 9);
        CargarEquipos(inMemoryEquipo, "AFC", 8);
        CargarEquipos(inMemoryEquipo, "OFC", 1);
    }

    private void CargarEquipos(InMemoryEquipo inMemoryEquipo, string confederacion, int cantidad)
    {
        for (int i = 1; i <= cantidad; i++)
        {
            inMemoryEquipo.AgregarEquipo(
                new Equipo($"{confederacion}_{i}", confederacion, 1000 + i)
            );
        }
    }
    
    [TestMethod]
    public void ImportarEquipos_CuandoHayLineaVacia_LaIgnora()
    {
        InMemoryEquipo inMemoryEquipo = new InMemoryEquipo();
        IAuditoriaService auditoriaService = new AuditoriaService(new InMemoryLog());
        IImportacionService service = new ImportacionService(inMemoryEquipo, auditoriaService);
        Usuario editor = new Usuario("Editor", "Sist", "editor@test.com", DateTime.Now.AddYears(-20), "Editor123#", [Rol.Editor]);

        string csv = "Nombre,Confederacion,RankingFifa\n\nUruguay,CONMEBOL,1800";

        ResultadoImportacion resultado = service.ImportarEquipos(editor, csv);

        Assert.IsTrue(resultado.FueExitosa);
        Assert.AreEqual(1, resultado.CantidadImportada);
    }
}




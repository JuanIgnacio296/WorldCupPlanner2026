using App.Dominio.Exceptions;
namespace App.Dominio.Tests;


[TestClass]
public class GrupoTests
{
    private char idValido = 'B';
    private int token = 1;
    private List<Equipo> listaEquipos = new List<Equipo>();
    private List<Partido> listaPartidos = new List<Partido>();
    private List<Estadio> listaEstadios = new List<Estadio>();
    
    private Equipo e1 = new Equipo("España", "UEFA", 2000);
    private Equipo e2 = new Equipo("Francia", "UEFA", 2000);
    private Equipo e3 = new Equipo("Uruguay", "CONMEBOL", 2000);
    private Equipo e4 = new Equipo("Argelia", "AFC", 2000);
    private Equipo e5 = new Equipo("Bosnia", "OFC", 2000);
    
    private Grupo gh = new Grupo('H', 1);
    private static Equipo uru = new Equipo("Uruguay", "CONMEBOL", 2000);
    private static Equipo esp = new Equipo("España", "UEFA", 2000);
    private static Equipo ara = new Equipo("Arabia Saudi", "UEFA", 2000);
    private static Equipo cav = new Equipo("Cabo Verde", "AFC", 2000);
        
    private static Estadio escn = new Estadio("Spotify Camp Nou", "Barcelona", 99354, new List<DateTime>());
    private static Estadio metLife = new Estadio("MetLife Stadium", "East Rutherford, New Jersey", 82500, new List<DateTime>());
        
        
    Partido p1 = new Partido(1, 1, 'A', DateTime.Now.AddDays(10), escn, esp, uru, 0, 2, "Fase de Grupos", "20:00", 1);
    Partido p2 = new Partido(2, 1, 'A', DateTime.Now.AddDays(8), metLife, ara, cav, 1, 1, "Fase de Grupos", "18:00", 1);
    
    [TestMethod]
    [DataRow(' ')]
    [DataRow(null)]
    public void CreateGrupo_idCanNotBeNull(char? idInvalido)
    {
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
        {
            new Grupo(idInvalido, token, listaPartidos);
        });
        
        Assert.AreEqual("Id no puede ser vacio ni nulo", ex.Message);
    }

    [TestMethod]
    [DataRow('Z')]
    [DataRow('3')]
    [DataRow('M')]
    public void CreateGrupo_IdIsFromAToL(char? idInvalido)
    {
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() => 
            new Grupo(idInvalido, token, listaPartidos)
        );
        
        Assert.AreEqual("Id debe ser una letra A-L", ex.Message);
    }

    

    [TestMethod]
    public void AddEquipoOnEquipos_WhenLengthIs4_ShouldThrowException()
    {
        Grupo g = new Grupo('B', token);
        
        

        g.AddEquipo(e1);
        g.AddEquipo(e2);
        g.AddEquipo(e3);
        g.AddEquipo(e4);
        
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
        {
            g.AddEquipo(e5);
        });
        
        Assert.AreEqual("No se pueden agregar mas de 4 equipos", ex.Message);
    }

    [TestMethod]
    public void AddEquipoOnEquipos_WhenEquiposContainsEquipo_ShouldThrowException()
    {
        Grupo g = new Grupo('C', token);
        Equipo e1 = new Equipo("Bosnia", "UEFA", 2000);
        Equipo e2 = new Equipo("Bosnia", "UEFA", 2000);

        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
        {
            g.AddEquipo(e1);
            g.AddEquipo(e2);
        });
        
        Assert.AreEqual("No se pueden agregar dos equipos iguales", ex.Message);
    }

    [TestMethod]
    public void AddEquipoOnEquipos_WhenEquipoIsNull_ShouldThrowException()
    {
        Grupo g = new Grupo('D', token);
        Equipo e = null;
        
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
        {
            g.AddEquipo(e);
        });
        
        Assert.AreEqual("No se pueden agregar equipos nulos", ex.Message);
        
    }

    [TestMethod]
    public void AddEquipo_WhenThreeUEFAExist_ThrowsException()
    {
        Grupo g = new Grupo('A', token);
        Equipo e = new Equipo("Grecia", "UEFA", 2000);
        
        g.AddEquipo(e1);
        g.AddEquipo(e2);

        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
        {
            g.AddEquipo(e);
        });
        
        Assert.AreEqual("No pueden haber 3 equipos de UEFA en un mismo grupo", ex.Message);
    }
    
    [TestMethod]
    [DataRow("AFC")]
    [DataRow("CAF")]
    [DataRow("CONCACAF")]
    [DataRow("CONMEBOL")]
    [DataRow("OFC")]
    public void AddEquipo_WhenSecondSameConferacionIsAdded_ShouldThrowException(string confederacion)
    {
        Grupo g = new Grupo('A', token);
        
        Equipo ea =  new Equipo("EquipoAFC", "AFC", 2000);
        Equipo eca = new Equipo("EquipoCAF", "CAF", 2000);
        Equipo econc = new Equipo("EquipoCONCACAF", "CONCACAF", 2000);
        Equipo econm = new Equipo("EquipoCONM", "CONMEBOL", 2000);
        Equipo eofc = new Equipo("EquipoOFC", "OFC", 2000);
        
        Equipo e = new Equipo("Equipo", confederacion,  2000);

        switch (confederacion)
        {
            case "AFC":
                g.AddEquipo(ea);
                break;
            case "CAF":
                g.AddEquipo(eca);
                break;
            case "CONCACAF":
                g.AddEquipo(econc);
                break;
            case "CONMEBOL":
                g.AddEquipo(econm);
                break;
            case "OFC":
                g.AddEquipo(eofc);
                break;
            
        }
        
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
        {
            g.AddEquipo(e);
        });
        
        Assert.AreEqual("No puede haber más de un equipo de " + confederacion + " en el grupo",  ex.Message);
    }
    
    [TestMethod]
    public void SetId_ShouldGetCorrectValue()
    {
        Grupo g = new Grupo('A', token);
        Assert.AreEqual('A', g.Id);
    }
    
    [TestMethod]
    public void GetEquipos_ShouldReturnAllTeams()
    {
        Grupo g = new Grupo('A', token);
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 1000);
        g.AddEquipo(e1);

        List<Equipo> listaObtenida = g.Equipos;

        Assert.AreEqual(1, listaObtenida.Count);
        Assert.AreEqual("Uruguay", listaObtenida[0].Nombre);
    }
    
    [TestMethod]
    public void GetEquipos_ShouldReturnACopy_ToProtectEncapsulation()
    {
        Grupo g = new Grupo('A', token);
        g.AddEquipo(new Equipo("Uruguay", "CONMEBOL", 1000));

        List<Equipo> listaExterna = g.Equipos;
        listaExterna.Add(new Equipo("Infiltrado", "UEFA", 1000));

        Assert.AreNotEqual(g.Equipos.Count, listaExterna.Count);
        Assert.AreEqual(1, g.Equipos.Count);
    }
    
    [TestMethod]
    public void GetPartidos_ShouldReturnAddedMatches()
    {
        Grupo g = new Grupo('A', token);
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 1000);
        Equipo e2 = new Equipo("Rusia", "UEFA", 800);
        g.AddEquipo(e1);
        g.AddEquipo(e2);
    
        Partido p = new Partido(3, g.Token, g.Id, DateTime.Now, escn, e1, e2, 0, 0, "Fase de Grupos", "20:00", 1);

        g.AddPartido(p);
        var lista = g.Partidos;

        Assert.AreEqual(1, lista.Count);
        Assert.IsTrue(lista.Contains(p));
    }
    
    [TestMethod]
    public void AddPartido_WhenIsNull_ShouldThrowException()
    {
        Grupo g = new Grupo('A', token);
        Partido p = null;
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() => g.AddPartido(p));
        Assert.AreEqual("Partido no puede ser nulo", ex.Message);
    }
    
    [TestMethod]
    public void GetEstadios_ShouldReturnUniqueStadiumsFromMatches()
    {
        Grupo g = new Grupo('A', token);
        Equipo e1 = new Equipo("Uruguay", "CONMEBOL", 1000);
        Equipo e2 = new Equipo("Corea", "AFC", 900);
        g.AddEquipo(e1);
        g.AddEquipo(e2);

        Estadio centenario = new Estadio("Centenario", "Montevideo", 60000, new List<DateTime>());
    
        Partido p1 = new Partido(3, g.Token, g.Id, DateTime.Now, centenario, e1, e2, 0, 0, "Fase de Grupos", "20:00", 1);
        Partido p2 = new Partido(4, g.Token, g.Id, DateTime.Now.AddDays(1), centenario, e2, e1, 0, 0, "Fase de Grupos", "18:00", 2);

        g.AddPartido(p1);
        g.AddPartido(p2);

        List<Estadio> estadiosDelGrupo = g.Estadios;

        Assert.AreEqual(1, estadiosDelGrupo.Count, "La lista de estadios no debería tener duplicados");
        Assert.AreEqual("Centenario", estadiosDelGrupo[0].Nombre);
    }
    
    [TestMethod]
    public void SetPartidos_WithValidList_ShouldStoreMatches()
    {
        Grupo g = new Grupo('A', token);
        var nuevaLista = new List<Partido> { 
            new Partido(3, g.Token, g.Id, DateTime.Now, escn, e1, e2, 0, 0, "Fase de Grupos", "20:00", 1) 
        };

        g.Partidos = nuevaLista;

        Assert.AreEqual(1, g.Partidos.Count);
        Assert.AreEqual(nuevaLista[0], g.Partidos[0]);
    }
    
    [TestMethod]
    public void SetPartidos_WithMoreThan6Matches_ThrowsException()
    {
        Grupo g = new Grupo('A', token);
        var soFi = new Estadio("SoFi Stadium", "Inglewood", 70000, new List<DateTime>());
        var usa = new Equipo("USA", "CONCACAF", 950);
        var arg = new Equipo("Argentina", "CONMEBOL", 1100);
        var fra = new Equipo("Francia", "UEFA", 1050);
        var jpn = new Equipo("Japón", "AFC", 900);
        
        List<Partido> partidosEjemplo = new List<Partido>
        {
            new Partido(3, g.Token, g.Id, new DateTime(2026, 06, 15), metLife, uru, usa, 2, 1, "Fase de Grupos", "20:00", 1),

            new Partido(4, g.Token, g.Id, new DateTime(2026, 06, 16), soFi, arg, fra, 1, 1, "Fase de Grupos", "18:00", 1),

            new Partido(5, g.Token, g.Id, new DateTime(2026, 06, 30), metLife, esp, jpn, 3, 0, "Fase de Grupos", "21:00", 2),

            new Partido(6, g.Token, g.Id, new DateTime(2026, 07, 05), soFi, uru, esp, 1, 0, "Fase de Grupos", "17:00", 2),

            new Partido(7, g.Token, g.Id, new DateTime(2026, 07, 10), metLife, arg, jpn, 2, 0, "Fase de Grupos", "20:00", 3),

            new Partido(8, g.Token, g.Id, new DateTime(2026, 07, 14), soFi, fra, esp, 1, 2, "Fase de Grupos", "21:00", 3),

            new Partido(9, g.Token, g.Id, new DateTime(2026, 07, 19), metLife, uru, arg, 2, 1, "Fase de Grupos", "19:00", 1)
        };
        
        Exception ex = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() => {
            g.Partidos = partidosEjemplo;
        });
        
        Assert.AreEqual("No pueden haber mas de 6 partidos",ex.Message);
    }
    
    [TestMethod]
    public void SortEquipos_ShouldHandlePointsAndGoalDifferenceTieBreak()
    {
        Grupo g = new Grupo('A', token);
    
        Equipo br = new Equipo("Brasil", "CONMEBOL", 2000);
        Equipo mar = new Equipo("Marruecos", "CAF", 1200);
        Equipo nz = new Equipo("Nueva Zelanda", "OFC", 800);
        Equipo cur = new Equipo("Curazao", "CONCACAF", 500);
        g.AddEquipo(br);
        g.AddEquipo(mar);
        g.AddEquipo(nz);
        g.AddEquipo(cur);

        Partido p1 = new Partido(3, g.Token, g.Id, DateTime.Now, metLife, br, cur, null, null, "Fase de Grupos", "20:00", 1);
    
        Partido p2 = new Partido(4, g.Token, g.Id, DateTime.Now, metLife, mar, nz, null, null, "Fase de Grupos", "18:00", 1);

        p1.AsignarResultado(3, 0);
        p2.AsignarResultado(1, 0);
        
        g.AddPartido(p1);
        g.AddPartido(p2);

        g.SortEquipos();
        List<Equipo> ranking = g.Equipos;

        Assert.AreEqual("Brasil", ranking[0].Nombre, "Brasil debe ser 1° (3 pts, +3 DG)");
        Assert.AreEqual("Marruecos", ranking[1].Nombre, "Marruecos debe ser 2° (3 pts, +1 DG)");
    }

    [TestMethod]
    public void RemoveEquipo_WhenGrupoNotContainsEquipo_ThenThrowsException()
    {
        Grupo g = new Grupo('A', token);
        g.AddEquipo(new Equipo("Equipo1", "CONMEBOL", 1200));
        g.AddEquipo(new Equipo("Equipo2", "UEFA", 700));
        g.AddEquipo(new Equipo("Equipo3", "AFC", 600));
        g.AddEquipo(new Equipo("Equipo4", "CAF", 500));
        Equipo e = new Equipo("Equipo5", "UEFA", 700);

        Exception exception = Assert.ThrowsException<ArgumentosDeGrupoNoValidos>(() =>
            { g.RemoveEquipo(e); }    
        );
        
        Assert.AreEqual("No se puede eliminar un equipo de un grupo al que no pertenece", exception.Message);
    }

    [TestMethod]
    public void RemoveEquipo_WhenGrupoContainsEquipo_ThenGrupoRemovesEquipo()
    {
        Grupo g = new Grupo('A', token);
        Equipo e = new Equipo("Equipo2", "UEFA", 700);
        g.AddEquipo(new Equipo("Equipo1", "CONMEBOL", 1200));
        g.AddEquipo(e);
        g.AddEquipo(new Equipo("Equipo3", "AFC", 600));
        g.AddEquipo(new Equipo("Equipo4", "CAF", 500));

        g.RemoveEquipo(e);

        Assert.AreEqual(3, g.Equipos.Count);
    }
    
    [TestMethod]
    public void Constructor_ConListaDePartidos_InicializaPartidos()
    {
        Equipo local = new Equipo("Uruguay", "CONMEBOL", 1800);
        Equipo visitante = new Equipo("Francia", "UEFA", 1900);
        Estadio estadio = new Estadio("Estadio", "Ciudad", 30000, "Desc");
        Partido partido = new Partido(3, token, 'A', new DateTime(2026, 6, 1, 14, 0, 0), estadio, local, visitante, "Fase de Grupos", 1);

        Grupo grupo = new Grupo('A', token, new List<Partido> { partido });

        Assert.AreEqual('A', grupo.Id);
        Assert.AreEqual(1, grupo.Partidos.Count);
    }
}



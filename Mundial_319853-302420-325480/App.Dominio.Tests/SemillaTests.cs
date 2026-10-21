namespace App.Dominio.Tests;

[TestClass]
public class SemillaTests
{
    private Semilla _semilla1 = new Semilla(1, 123, 456, 789, 0);
    private Semilla _semilla2 = new Semilla(2, 111, 222, 333, 444);

    [TestMethod]
    public void CompletarEquipos_WhenMismaSemilla_ThenMismosResultados()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Equipo> equipos2 = generarEquipos();
        
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1);
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla1);

        sorteo1.CompletarEquiposSiFaltan();
        sorteo2.CompletarEquiposSiFaltan();
        
        Assert.AreEqual(48, equipos1.Count);
        Assert.AreEqual(48, equipos2.Count);

        for (int i = 0; i < 48; i++)
        {
            Equipo e1 = equipos1[i];
            Equipo e2 = equipos2[i];
            
            Assert.AreEqual(e1.Nombre, e2.Nombre);
            Assert.AreEqual(e1.Confederacion, e2.Confederacion);
            Assert.AreEqual(e1.RankingFifa, e2.RankingFifa);
        }
    }
   
    [TestMethod]
    public void CompletarEquipos_WhenDistintaSemilla_ThenDistintosResultados()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Equipo> equipos2 = generarEquipos();
        
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1);
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla2);

        sorteo1.CompletarEquiposSiFaltan();
        sorteo2.CompletarEquiposSiFaltan();
        
        Assert.AreEqual(48, equipos1.Count);
        Assert.AreEqual(48, equipos2.Count);

        bool sonDistintos = false;

        for (int i = 3; i < 48 && !sonDistintos; i++)
        {
            Equipo e1 = equipos1[i];
            Equipo e2 = equipos2[i];

            if (e1.RankingFifa != e2.RankingFifa)
            {
                sonDistintos = true;
            }
        }
        
        Assert.IsTrue(sonDistintos);
    }
    
     
    [TestMethod]
    public void OrganizarEquiposPorRanking_WhenMismaSemilla_ThenMismoOrden()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Equipo> equipos2 = generarEquipos();
        
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1);
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla1);
        sorteo1.CompletarEquiposSiFaltan();
        sorteo2.CompletarEquiposSiFaltan();
        
        equipos1 = sorteo1.OrdenarEquiposPorRanking(new Random(_semilla1.SemillaFixture));
        equipos2 = sorteo2.OrdenarEquiposPorRanking(new Random(_semilla1.SemillaFixture));

        for (int i = 0; i < 48; i++)
        {
            Equipo e1 = equipos1[i];
            Equipo e2 = equipos2[i];
            
            Assert.AreEqual(e1, e2);
        }
    }

    [TestMethod]
    public void OrganizarEquiposPorRanking_WhenDistintaSemilla_ThenDistintoOrden()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Equipo> equipos2 = generarEquipos();
        
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1);
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla2);
        sorteo1.CompletarEquiposSiFaltan();
        sorteo2.CompletarEquiposSiFaltan();

        equipos1 = sorteo1.OrdenarEquiposPorRanking(new Random(_semilla1.SemillaFixture));
        equipos2 = sorteo2.OrdenarEquiposPorRanking(new Random(_semilla2.SemillaFixture));

        bool sonDistintos = false;
        
        for (int i = 0; i < 48 && !sonDistintos; i++)
        {
            Equipo e1 = equipos1[i];
            Equipo e2 = equipos2[i];
            
            if (!e1.Equals(e2))
            {
                sonDistintos = true;
            }
        }
        Assert.IsTrue(sonDistintos);
    }

    [TestMethod]
    public void SortearEquipos_WhenMismaSemilla_ThenMismosGrupos()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Equipo> equipos2 = generarEquipos();
        
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1);
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla1);

        List<Grupo> grupos1 = sorteo1.SortearEquipos();
        List<Grupo> grupos2 = sorteo2.SortearEquipos();
        
        Assert.AreEqual(48, equipos1.Count);
        Assert.AreEqual(48, equipos2.Count);

        for (int i = 0; i < 12; i++)
        {
            Grupo g1 = grupos1[i];
            Grupo g2 = grupos2[i];

            for (int j = 0; j < 4; j++)
            {
                Equipo e1 = g1.Equipos[j];
                Equipo e2 = g2.Equipos[j];
                
                Assert.AreEqual(e1, e2);
            }
                
        }
    }

    [TestMethod]
    public void SortearEquipos_WhenDistintaSemilla_ThenDistintosGrupos()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Equipo> equipos2 = generarEquipos();
        
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1);
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla2);

        List<Grupo> grupos1 = sorteo1.SortearEquipos();
        List<Grupo> grupos2 = sorteo2.SortearEquipos();
        
        Assert.AreEqual(48, equipos1.Count);
        Assert.AreEqual(48, equipos2.Count);

        bool sonDistintos = false;
        
        for (int i = 0; i < 12 && !sonDistintos; i++)
        {
            Grupo g1 = grupos1[i];
            Grupo g2 = grupos2[i];

            for (int j = 0; j < 4 && !sonDistintos; j++)
            {
                Equipo e1 = g1.Equipos[j];
                Equipo e2 = g2.Equipos[j];

                if (!e1.Equals(e2))
                {
                    sonDistintos = true;
                }
            }
        }
        
        Assert.IsTrue(sonDistintos);
    }
    
    
    [TestMethod]
    public void OrganizarListas_WhenMismaSemilla_ThenMismasListas()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Estadio> estadios1 = generarEstadios();
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1, estadios1);
        List<Grupo> grupos1 = sorteo1.SortearEquipos();
        Fixture fixture1 = new Fixture(sorteo1.Token, grupos1, estadios1, _semilla1, false);
        fixture1.ArmarFase("Fase de Grupos");
        simularGrupos(fixture1);
        
        List<Equipo> equipos2 = generarEquipos();
        List<Estadio> estadios2 = generarEstadios();
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla1, estadios2);
        List<Grupo> grupos2 = sorteo2.SortearEquipos();
        Fixture fixture2 = new Fixture(sorteo2.Token, grupos2, estadios2, _semilla1, false);
        fixture2.ArmarFase("Fase de Grupos");
        simularGrupos(fixture2);

        fixture1.OrganizarListas();
        fixture2.OrganizarListas();

        for (int i = 0; i < 12; i++)
        {
            Assert.AreEqual(fixture1.ListaPrimeros[i], fixture2.ListaPrimeros[i]);
            Assert.AreEqual(fixture1.ListaSegundos[i], fixture2.ListaSegundos[i]);
            Assert.AreEqual(fixture1.ListaTerceros[i], fixture2.ListaTerceros[i]);
        }
    }
    
    [TestMethod]
    public void OrganizarListas_WhenDistintaSemilla_ThenDistintasListas()
    {
        List<Equipo> equipos1 = generarEquipos();
        List<Estadio> estadios1 = generarEstadios();
        Sorteo sorteo1 = new Sorteo(equipos1, _semilla1, estadios1);
        List<Grupo> grupos1 = sorteo1.SortearEquipos();
        Fixture fixture1 = new Fixture(sorteo1.Token, grupos1, estadios1, _semilla1, false);
        fixture1.ArmarFase("Fase de Grupos");
        simularGrupos(fixture1);
        
        List<Equipo> equipos2 = generarEquipos();
        List<Estadio> estadios2 = generarEstadios();
        Sorteo sorteo2 = new Sorteo(equipos2, _semilla2, estadios2);
        List<Grupo> grupos2 = sorteo2.SortearEquipos();
        Fixture fixture2 = new Fixture(sorteo2.Token, grupos2, estadios2, _semilla2, false);
        fixture2.ArmarFase("Fase de Grupos");
        simularGrupos(fixture2);

        fixture1.OrganizarListas();
        fixture2.OrganizarListas();
        bool sonDistintas = false;

        for (int i = 0; i < 12 && !false; i++)
        {
            if (fixture1.ListaPrimeros[i] != fixture2.ListaPrimeros[i] ||
                fixture1.ListaSegundos[i] != fixture2.ListaSegundos[i] ||
                fixture1.ListaTerceros[i] != fixture2.ListaTerceros[i])
            {
                sonDistintas = true;
            }
        }
        Assert.IsTrue(sonDistintas);
    }


    private List<Equipo> generarEquipos()
    {
        return new List<Equipo>()
        {
            new Equipo("Uruguay", "CONMEBOL", 1300),
            new Equipo("Argentina", "CONMEBOL", 2200),
            new Equipo("Brasil", "CONMEBOL", 1900)
        };
    }

    private List<Estadio> generarEstadios()
    {
        return new List<Estadio>()
        {
            new Estadio("Estadio1", "Ciudad", 40000, "Descripcion"),
            new Estadio("Estadio2", "Ciudad", 42000, "Descripcion"),
            new Estadio("Estadio3", "Ciudad", 45000, "Descripcion"),
            new Estadio("Estadio4", "Ciudad", 50000, "Descripcion"),
            new Estadio("Estadio5", "Ciudad", 55000, "Descripcion")
                
        };
    }
    
    public void simularGrupos(Fixture fixture)
    {
        for (int i = 0; i < 72; i++)
        {
            fixture.Partidos[i].AsignarResultado(2, 0);
        }
    }
    
    [TestMethod]
    public void Equals_CuandoObjetoEsNullUOtroTipo_DevuelveFalse()
    {
        Semilla semilla = new Semilla(1, 1, 2, 3, 4);

        Assert.IsFalse(semilla.Equals(null));
        Assert.IsFalse(semilla.Equals("texto"));
    }

    [TestMethod]
    public void RandomSimulacion_CuandoSeCreaSemilla_NoEsNull()
    {
        Semilla semilla = new Semilla(1, 1, 2, 3, 4);

        Assert.IsNotNull(semilla.RandomSimulacion);
    }
}



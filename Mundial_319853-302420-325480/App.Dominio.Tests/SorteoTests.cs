namespace App.Dominio.Tests;

[TestClass]
public class SorteoTests
{
    private Semilla _semilla = new Semilla(1, 1, 1, 1, 1);
    private List<Equipo> equipos;

    [TestInitialize]
    public void TestInitialize()
    {
        equipos = new List<Equipo>();
    }

    [TestMethod]
    public void CrearGrupos_WhenCalled_ThenCreates12Groups()
    {
        Sorteo sorteo = new Sorteo(equipos, _semilla);

        List<Grupo> grupos = sorteo.CrearGrupos();

        Assert.AreEqual(12, grupos.Count);
    }

    [TestMethod]
    public void CrearGrupos_WhenCalled_ThenGroupsGoFromAToL()
    {
        Sorteo sorteo = new Sorteo(equipos, _semilla);

        List<Grupo> grupos = sorteo.CrearGrupos();

        Assert.AreEqual('A', grupos[0].Id);
        Assert.AreEqual('B', grupos[1].Id);
        Assert.AreEqual('C', grupos[2].Id);
        Assert.AreEqual('D', grupos[3].Id);
        Assert.AreEqual('E', grupos[4].Id);
        Assert.AreEqual('F', grupos[5].Id);
        Assert.AreEqual('G', grupos[6].Id);
        Assert.AreEqual('H', grupos[7].Id);
        Assert.AreEqual('I', grupos[8].Id);
        Assert.AreEqual('J', grupos[9].Id);
        Assert.AreEqual('K', grupos[10].Id);
        Assert.AreEqual('L', grupos[11].Id);
    }

    [TestMethod]
    public void SortearEquipos_WhenHay48Equipos_ThenCadaGrupoTiene4Equipos()
    {
        Sorteo sorteo = new Sorteo(crear48Equipos(), _semilla);

        List<Grupo> grupos = sorteo.SortearEquipos();

        foreach (Grupo grupo in grupos)
        {
            Assert.AreEqual(4, grupo.Equipos.Count);
        }
    }

    [TestMethod]
    public void SortearEquipos_WhenHayMenosDe48Equipos_ThenCompletaHasta48()
    {
        List<Equipo> equiposIncompletos = crear48Equipos().Take(47).ToList();
        Sorteo sorteo = new Sorteo(equiposIncompletos, _semilla);

        List<Grupo> grupos = sorteo.SortearEquipos();

        int cantidadEquiposSorteados = grupos.Sum(grupo => grupo.Equipos.Count);

        Assert.AreEqual(48, cantidadEquiposSorteados);
    }

    [TestMethod]
    public void SortearEquipos_WhenCompletaEquipos_ThenRespetaCuposPorConfederacion()
    {
        List<Equipo> equiposIncompletos = new List<Equipo>();

        for (int i = 1; i <= 15; i++)
        {
            equiposIncompletos.Add(new Equipo($"UEFA_{i}", "UEFA", 1000 + i));
        }

        for (int i = 1; i <= 7; i++)
        {
            equiposIncompletos.Add(new Equipo($"CONMEBOL_{i}", "CONMEBOL", 900 + i));
        }

        for (int i = 1; i <= 7; i++)
        {
            equiposIncompletos.Add(new Equipo($"CONCACAF_{i}", "CONCACAF", 800 + i));
        }

        for (int i = 1; i <= 9; i++)
        {
            equiposIncompletos.Add(new Equipo($"CAF_{i}", "CAF", 700 + i));
        }

        for (int i = 1; i <= 8; i++)
        {
            equiposIncompletos.Add(new Equipo($"AFC_{i}", "AFC", 600 + i));
        }

        Sorteo sorteo = new Sorteo(equiposIncompletos, _semilla);

        List<Grupo> grupos = sorteo.SortearEquipos();

        List<Equipo> equiposFinales = grupos
            .SelectMany(grupo => grupo.Equipos)
            .ToList();

        Assert.AreEqual(16, equiposFinales.Count(e => e.Confederacion == "UEFA"));
        Assert.AreEqual(7, equiposFinales.Count(e => e.Confederacion == "CONMEBOL"));
        Assert.AreEqual(7, equiposFinales.Count(e => e.Confederacion == "CONCACAF"));
        Assert.AreEqual(9, equiposFinales.Count(e => e.Confederacion == "CAF"));
        Assert.AreEqual(8, equiposFinales.Count(e => e.Confederacion == "AFC"));
        Assert.AreEqual(1, equiposFinales.Count(e => e.Confederacion == "OFC"));
    }

    [TestMethod]
    public void OrdenarEquiposPorRanking_DebeRetornarEquiposDeMayorAMenor()
    {
        List<Equipo> equipos = crear48Equipos();
        Sorteo sorteo = new Sorteo(equipos, _semilla);
        Random rnd = new Random(_semilla.SemillaFixture);

        List<Equipo> resultado = sorteo.OrdenarEquiposPorRanking(rnd);
        
        for (int i = 0; i < resultado.Count - 1; i++)
        {
            Assert.IsTrue(resultado[i].RankingFifa >= resultado[i + 1].RankingFifa, 
                $"El equipo en posiciÃ³n {i} tiene menos ranking que el siguiente.");
        }
    }

    [TestMethod]
    public void SortearEquipos_WhenHayEmpateDeRankingYMismoToken_ThenResultadoEsIgual()
    {
        List<Equipo> equiposEmpatados1 = crear48EquiposConEmpatesDeRanking();
        List<Equipo> equiposEmpatados2 = crear48EquiposConEmpatesDeRanking();

        Sorteo sorteo1 = new Sorteo(equiposEmpatados1, _semilla);
        Sorteo sorteo2 = new Sorteo(equiposEmpatados2, _semilla);

        List<Grupo> grupos1 = sorteo1.SortearEquipos();
        List<Grupo> grupos2 = sorteo2.SortearEquipos();

        for (int i = 0; i < grupos1.Count; i++)
        {
            for (int j = 0; j < grupos1[i].Equipos.Count; j++)
            {
                Assert.AreEqual(
                    grupos1[i].Equipos[j].Nombre,
                    grupos2[i].Equipos[j].Nombre
                );
            }
        }
    }

    [TestMethod]
    public void SortearEquipos_WhenHayEmpateDeRankingYTokensDistintos_ThenResultadoEsDistinto()
    {
        List<Equipo> equiposEmpatados1 = crear48EquiposConEmpatesDeRanking();
        List<Equipo> equiposEmpatados2 = crear48EquiposConEmpatesDeRanking();
        Semilla semilla2 = new Semilla(2,2, 2, 2, 2);
        Sorteo sorteo1 = new Sorteo(equiposEmpatados1, _semilla);
        Sorteo sorteo2 = new Sorteo(equiposEmpatados2, semilla2);

        List<Grupo> grupos1 = sorteo1.SortearEquipos();
        List<Grupo> grupos2 = sorteo2.SortearEquipos();

        bool hayAlgunaDiferencia = false;

        for (int i = 0; i < grupos1.Count; i++)
        {
            for (int j = 0; j < grupos1[i].Equipos.Count; j++)
            {
                if (grupos1[i].Equipos[j].Nombre != grupos2[i].Equipos[j].Nombre)
                {
                    hayAlgunaDiferencia = true;
                }
            }
        }

        Assert.IsTrue(hayAlgunaDiferencia);
    }

    [TestMethod]
    public void SortearEquipos_WhenCalled_ThenNoGrupoTieneConfederacionesRepetidasExceptoUefa()
    {
        Sorteo sorteo = new Sorteo(crear48Equipos(), _semilla);

        List<Grupo> grupos = sorteo.SortearEquipos();

        foreach (Grupo grupo in grupos)
        {
            int cantidadUefa = grupo.Equipos.Count(e => e.Confederacion == "UEFA");
            int cantidadConmebol = grupo.Equipos.Count(e => e.Confederacion == "CONMEBOL");
            int cantidadConcacaf = grupo.Equipos.Count(e => e.Confederacion == "CONCACAF");
            int cantidadCaf = grupo.Equipos.Count(e => e.Confederacion == "CAF");
            int cantidadAfc = grupo.Equipos.Count(e => e.Confederacion == "AFC");
            int cantidadOfc = grupo.Equipos.Count(e => e.Confederacion == "OFC");

            Assert.IsTrue(cantidadUefa <= 2);
            Assert.IsTrue(cantidadConmebol <= 1);
            Assert.IsTrue(cantidadConcacaf <= 1);
            Assert.IsTrue(cantidadCaf <= 1);
            Assert.IsTrue(cantidadAfc <= 1);
            Assert.IsTrue(cantidadOfc <= 1);
        }
    }

    [TestMethod]
    public void SortearEquipos_WhenCalled_ThenNoPierdeNiDuplicaEquipos()
    {
        List<Equipo> equiposOriginales = crear48Equipos();
        Sorteo sorteo = new Sorteo(equiposOriginales, _semilla);

        List<Grupo> grupos = sorteo.SortearEquipos();

        List<Equipo> equiposSorteados = grupos
            .SelectMany(grupo => grupo.Equipos)
            .ToList();

        Assert.AreEqual(48, equiposSorteados.Count);
        Assert.AreEqual(48, equiposSorteados.Select(equipo => equipo.Nombre).Distinct().Count());

        foreach (Equipo equipo in equiposOriginales)
        {
            Assert.IsTrue(equiposSorteados.Any(e => e.Nombre == equipo.Nombre));
        }
    }
    private List<Equipo> crear48Equipos()
    {
        List<Equipo> equiposCreados = new List<Equipo>();

        agregarEquipos(equiposCreados, "UEFA", 16, 2000);
        agregarEquipos(equiposCreados, "CONMEBOL", 7, 1800);
        agregarEquipos(equiposCreados, "CONCACAF", 7, 1600);
        agregarEquipos(equiposCreados, "CAF", 9, 1400);
        agregarEquipos(equiposCreados, "AFC", 8, 1200);
        agregarEquipos(equiposCreados, "OFC", 1, 1000);

        return equiposCreados;
    }

    private List<Equipo> crear48EquiposConEmpatesDeRanking()
    {
        List<Equipo> equiposCreados = new List<Equipo>();

        equiposCreados.Add(new Equipo("UEFA_Empatado_1", "UEFA", 2500));
        equiposCreados.Add(new Equipo("UEFA_Empatado_2", "UEFA", 2500));
        equiposCreados.Add(new Equipo("CONMEBOL_Empatado_1", "CONMEBOL", 2500));
        equiposCreados.Add(new Equipo("CONCACAF_Empatado_1", "CONCACAF", 2500));
        equiposCreados.Add(new Equipo("CAF_Empatado_1", "CAF", 2500));
        equiposCreados.Add(new Equipo("AFC_Empatado_1", "AFC", 2500));

        agregarEquipos(equiposCreados, "UEFA", 14, 2000);
        agregarEquipos(equiposCreados, "CONMEBOL", 6, 1800);
        agregarEquipos(equiposCreados, "CONCACAF", 6, 1600);
        agregarEquipos(equiposCreados, "CAF", 8, 1400);
        agregarEquipos(equiposCreados, "AFC", 7, 1200);
        agregarEquipos(equiposCreados, "OFC", 1, 1000);

        return equiposCreados;
    }

    private void agregarEquipos(List<Equipo> equipos, string confederacion, int cantidad, int rankingInicial)
    {
        for (int i = 1; i <= cantidad; i++)
        {
            equipos.Add(new Equipo($"{confederacion}_{i}", confederacion, rankingInicial + i));
        }
    }

    private List<Estadio> crear4Estadios()
    {
        return new List<Estadio>
        {
            new Estadio("Estadio A", "Ciudad A", 30000, "Descripcion A"),
            new Estadio("Estadio B", "Ciudad B", 35000, "Descripcion B"),
            new Estadio("Estadio C", "Ciudad C", 40000, "Descripcion C"),
            new Estadio("Estadio D", "Ciudad D", 45000, "Descripcion D")
        };
    }
    
    [TestMethod]
    public void Constructores_InicializanCampos()
    {
        List<Equipo> equipos = new List<Equipo> { new Equipo("Uruguay", "CONMEBOL", 1800) };
        List<Estadio> estadios = new List<Estadio> { new Estadio("Estadio", "Ciudad", 30000, "Desc") };
        Semilla semilla = new Semilla(1, 1, 1, 1, 1);

        Sorteo sinEstadios = new Sorteo(equipos, semilla);
        Sorteo conEstadios = new Sorteo(equipos, semilla, estadios);
        Sorteo conToken = new Sorteo(equipos, 99, semilla, estadios);

        Assert.AreEqual(0, sinEstadios.Token);
        Assert.AreEqual(0, sinEstadios.Estadios.Count);
        Assert.AreEqual(1, conEstadios.Estadios.Count);
        Assert.AreEqual(99, conToken.Token);
    }

    [TestMethod]
    public void CrearGrupos_GeneraDoceGrupos()
    {
        Sorteo sorteo = new Sorteo(new List<Equipo>(), new Semilla(1, 1, 1, 1, 1));

        List<Grupo> grupos = sorteo.CrearGrupos();

        Assert.AreEqual(12, grupos.Count);
        Assert.AreEqual('A', grupos.First().Id);
        Assert.AreEqual('L', grupos.Last().Id);
    }
}



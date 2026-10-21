namespace App.Dominio;

public class Barajador
{
    public List<Equipo> FisherYates(List<Equipo> equipos, Random random)
    {
        for (int indice = equipos.Count - 1; indice > 0; indice--)
        {
            int indiceAleatorio = random.Next(indice + 1);
            (equipos[indice], equipos[indiceAleatorio]) = (equipos[indiceAleatorio], equipos[indice]);
        }

        return equipos;
    }

    public void OrdenarPorTablaYDesempatar(List<Equipo> lista, Random random)
    {
        lista.Sort(CompararEquiposPorTabla);
        AplicarFisherYatesEnEmpates(lista, random);
    }

    private void AplicarFisherYatesEnEmpates(List<Equipo> lista, Random random)
    {
        int indice = 0;
        while (indice < lista.Count)
        {
            int indiceFinEmpate = indice + 1;
            while (indiceFinEmpate < lista.Count && EstanEmpatados(lista[indice], lista[indiceFinEmpate]))
            {
                indiceFinEmpate++;
            }

            if (indiceFinEmpate - indice > 1)
            {
                List<Equipo> empatados = lista.GetRange(indice, indiceFinEmpate - indice);
                FisherYates(empatados, random);
                for (int offset = 0; offset < empatados.Count; offset++)
                {
                    lista[indice + offset] = empatados[offset];
                }
            }

            indice = indiceFinEmpate;
        }
    }

    private static int CompararEquiposPorTabla(Equipo equipo, Equipo otroEquipo)
    {
        int comparacion = otroEquipo.Puntos.CompareTo(equipo.Puntos);
        if (comparacion != 0)
        {
            return comparacion;
        }

        int diferenciaGoles = equipo.GolesAFavor - equipo.GolesEnContra;
        int diferenciaGolesOtro = otroEquipo.GolesAFavor - otroEquipo.GolesEnContra;
        comparacion = diferenciaGolesOtro.CompareTo(diferenciaGoles);
        if (comparacion != 0)
        {
            return comparacion;
        }

        return otroEquipo.GolesAFavor.CompareTo(equipo.GolesAFavor);
    }

    private static bool EstanEmpatados(Equipo equipo, Equipo otroEquipo)
    {
        return equipo.Puntos == otroEquipo.Puntos
               && equipo.GolesAFavor - equipo.GolesEnContra == otroEquipo.GolesAFavor - otroEquipo.GolesEnContra
               && equipo.GolesAFavor == otroEquipo.GolesAFavor;
    }
}
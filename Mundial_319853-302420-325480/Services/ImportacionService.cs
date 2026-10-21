using App.Dominio;
using App.Dominio.Exceptions;
using DataAccess;
using DataAccess.Interfaces;
using DTOs;
using Services.Interfaces;

namespace Services;

public class ImportacionService : IImportacionService
{
    private IinMemoryEquipo _IinMemoryEquipo;
    private IAuditoriaService _auditoriaService;
    private readonly UsuarioMapeador _usuarioMapeador = new UsuarioMapeador();

    public ImportacionService(IinMemoryEquipo IinMemoryEquipo, IAuditoriaService auditoriaService)
    {
        _IinMemoryEquipo = IinMemoryEquipo;
        _auditoriaService = auditoriaService;
    }

    public ResultadoImportacion ImportarEquipos(Usuario editor, string contenidoCsv)
    {
        if (!editor.TieneRol(Rol.Editor))
        {
            throw new InvalidOperationException("Solo pueden importar equipos los editores.");
        }

        ResultadoImportacion resultado = new ResultadoImportacion();

        if (string.IsNullOrWhiteSpace(contenidoCsv))
        {
            resultado.AgregarError("El contenido del CSV no puede estar vacío.");
            return resultado;
        }

        string[] lineas = contenidoCsv.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None
        );

        string encabezado = lineas[0].Trim().TrimStart('﻿');

        if (!EncabezadoEsValido(encabezado))
        {
            resultado.AgregarError("El encabezado del CSV es incorrecto.");
            return resultado;
        }
        
        List<string> nombresImportados = new List<string>();
        List<Equipo> equiposAImportar = new List<Equipo>();

        for (int i = 1; i < lineas.Length; i++)
        {
            string linea = lineas[i];

            if (string.IsNullOrWhiteSpace(linea))
            {
                continue;
            }

            string[] columnas = linea.Split(',');

            if (columnas.Length != 3)
            {
                resultado.AgregarError($"Fila {i + 1}: cantidad incorrecta de columnas.");
                continue;
            }

            string nombre = columnas[0].Trim();
            string confederacion = columnas[1].Trim();
            string rankingTexto = columnas[2].Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                resultado.AgregarError($"Fila {i + 1}: El nombre no puede ser vacío.");
                continue;
            }

            if (nombresImportados.Contains(nombre))
            {
                resultado.AgregarError($"Fila {i + 1}: Ya existe un equipo con ese nombre.");
                continue;
            }

            if (_IinMemoryEquipo.ObtenerEquipos().Any(e => e.Nombre == nombre))
            {
                resultado.AgregarError($"Fila {i + 1}: Ya existe un equipo con ese nombre.");
                continue;
            }

            if (!int.TryParse(rankingTexto, out int rankingFifa))
            {
                resultado.AgregarError($"Fila {i + 1}: RankingFIFA debe ser numérico.");
                continue;
            }

            if (rankingFifa < 300 || rankingFifa > 2500)
            {
                resultado.AgregarError($"Fila {i + 1}: RankingFIFA fuera de rango.");
                continue;
            }

            try
            {
                Equipo equipo = new Equipo(nombre, confederacion, rankingFifa);

                int totalActual = _IinMemoryEquipo.ObtenerEquipos().Count;
                int totalImportacion = equiposAImportar.Count;

                if (totalActual + totalImportacion >= 48)
                {
                    resultado.AgregarError($"Fila {i + 1}: Se supera el máximo de 48 equipos.");
                    continue;
                }

                int cantidadActualConfederacion = _IinMemoryEquipo.ObtenerEquipos()
                    .Count(e => e.Confederacion == equipo.Confederacion);

                int cantidadImportadaConfederacion = equiposAImportar
                    .Count(e => e.Confederacion == equipo.Confederacion);

                int maximoConfederacion = CuposConfederacion.Maximo(equipo.Confederacion);

                if (cantidadActualConfederacion + cantidadImportadaConfederacion >= maximoConfederacion)
                {
                    resultado.AgregarError($"Fila {i + 1}: Se supera el cupo máximo de {equipo.Confederacion}.");
                    continue;
                }

                nombresImportados.Add(nombre);
                equiposAImportar.Add(equipo);
            }
            catch (ArgumentException)
            {
                resultado.AgregarError($"Fila {i + 1}: La confederación es inválida.");
            }
        }

        if (!resultado.FueExitosa)
        {
            _auditoriaService.RegistrarLog(
                editor,
                $"Importación de equipos con errores. Cantidad de errores: {resultado.Errores.Count}"
            );
            return resultado;
        }

        foreach (Equipo equipo in equiposAImportar)
        {
            _IinMemoryEquipo.AgregarEquipo(equipo);
            resultado.AumentarCantidadImportada();
        }
        
        _auditoriaService.RegistrarLog(
            editor,
            $"Importación de equipos exitosa. Cantidad importada: {resultado.CantidadImportada}"
        );

        return resultado;
    }

    public ResultadoImportacionReadDto ImportarEquipos(UsuarioReadDto editor, string contenidoCsv)
    {
        return mapearResultadoImportacion(ImportarEquipos(_usuarioMapeador.ADominio(editor), contenidoCsv));
    }

    private static ResultadoImportacionReadDto mapearResultadoImportacion(ResultadoImportacion resultado)
    {
        return new ResultadoImportacionReadDto
        {
            CantidadImportada = resultado.CantidadImportada,
            Errores = resultado.Errores
        };
    }

    private static bool EncabezadoEsValido(string encabezado)
    {
        string[] columnas = encabezado.Split(',');

        if (columnas.Length != 3)
        {
            return false;
        }

        return NormalizarColumna(columnas[0]) == "nombre"
            && NormalizarColumna(columnas[1]) == "confederacion"
            && NormalizarColumna(columnas[2]) == "rankingfifa";
    }

    private static string NormalizarColumna(string columna)
    {
        return columna.Trim().ToLowerInvariant()
            .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u");
    }

}


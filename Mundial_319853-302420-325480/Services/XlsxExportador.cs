using System.IO.Compression;
using System.Security;
using System.Text;
using Services.Interfaces;

namespace Services;

public class XlsxExportador : IXlsxExportador
{
    public byte[] CrearArchivo(string[] encabezados, List<string[]> filas)
    {
        using MemoryStream stream = new MemoryStream();
        using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            AgregarEntrada(zip, "[Content_Types].xml", CrearContentTypes());
            AgregarEntrada(zip, "_rels/.rels", CrearRelacionesRaiz());
            AgregarEntrada(zip, "xl/workbook.xml", CrearWorkbook());
            AgregarEntrada(zip, "xl/_rels/workbook.xml.rels", CrearRelacionesWorkbook());
            AgregarEntrada(zip, "xl/worksheets/sheet1.xml", CrearWorksheet(encabezados, filas));
        }

        return stream.ToArray();
    }

    private static void AgregarEntrada(ZipArchive zip, string nombre, string contenido)
    {
        ZipArchiveEntry entry = zip.CreateEntry(nombre);
        using StreamWriter writer = new StreamWriter(entry.Open(), Encoding.UTF8);
        writer.Write(contenido);
    }

    private static string CrearContentTypes()
    {
        return """
               <?xml version="1.0" encoding="UTF-8"?>
               <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                 <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                 <Default Extension="xml" ContentType="application/xml"/>
                 <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
                 <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
               </Types>
               """;
    }

    private static string CrearRelacionesRaiz()
    {
        return """
               <?xml version="1.0" encoding="UTF-8"?>
               <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                 <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
               </Relationships>
               """;
    }

    private static string CrearWorkbook()
    {
        return """
               <?xml version="1.0" encoding="UTF-8"?>
               <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
                 <sheets>
                   <sheet name="Exportacion" sheetId="1" r:id="rId1"/>
                 </sheets>
               </workbook>
               """;
    }

    private static string CrearRelacionesWorkbook()
    {
        return """
               <?xml version="1.0" encoding="UTF-8"?>
               <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                 <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
               </Relationships>
               """;
    }

    private static string CrearWorksheet(string[] encabezados, List<string[]> filas)
    {
        StringBuilder xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">""");
        xml.AppendLine("<sheetData>");
        AgregarFilaXlsx(xml, 1, encabezados);

        for (int i = 0; i < filas.Count; i++)
        {
            AgregarFilaXlsx(xml, i + 2, filas[i]);
        }

        xml.AppendLine("</sheetData>");
        xml.AppendLine("</worksheet>");

        return xml.ToString();
    }

    private static void AgregarFilaXlsx(StringBuilder xml, int numeroFila, string[] valores)
    {
        xml.Append($"""<row r="{numeroFila}">""");

        for (int i = 0; i < valores.Length; i++)
        {
            string referencia = $"{ObtenerColumna(i)}{numeroFila}";
            string valor = SecurityElement.Escape(valores[i]) ?? "";
            xml.Append($"""<c r="{referencia}" t="inlineStr"><is><t>{valor}</t></is></c>""");
        }

        xml.AppendLine("</row>");
    }

    private static string ObtenerColumna(int indice)
    {
        return ((char)('A' + indice)).ToString();
    }
}

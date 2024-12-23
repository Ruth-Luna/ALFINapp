using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

public class SubidaDeArchivosDTO
{
    public string? COD_CANAL { get; set; }
    public string? CANAL { get; set; }
    public float DNI { get; set; }
    public DateTime? FECHA_ENVIO { get; set; }
    public DateTime? FECHA_GESTION { get; set; }
    public string? HORA_GESTION { get; set; }
    public int? TELEFONO { get; set; }
    public string? ORIGEN_TELEFONO { get; set; }
    public string? COD_CAMPAÑA { get; set; }
    public int? COD_TIP { get; set; }
    public float? OFERTA { get; set; }
    public string? DNI_ASESOR { get; set; }
}
public class SubidaDeArchivosDTOMapping : ClassMap<SubidaDeArchivosDTO>
{
    public SubidaDeArchivosDTOMapping()
    {
        Map(m => m.COD_CANAL);
        Map(m => m.CANAL);
        Map(m => m.DNI);
        Map(m => m.FECHA_ENVIO)
            .TypeConverterOption.Format("yyyyMMdd")
            .TypeConverterOption.NullValues("");
        Map(m => m.FECHA_GESTION)
            .TypeConverterOption.Format("yyyyMMdd")
            .TypeConverterOption.NullValues("");
        Map(m => m.HORA_GESTION)
            .TypeConverterOption.Format("MM/dd/yy HH:mm");
        Map(m => m.TELEFONO);
        Map(m => m.ORIGEN_TELEFONO);
        Map(m => m.COD_CAMPAÑA);
        Map(m => m.COD_TIP);
        Map(m => m.OFERTA);
        Map(m => m.DNI_ASESOR);
    }
}

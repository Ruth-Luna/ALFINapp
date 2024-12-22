using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

public class SubidaDeArchivosDTO
{
    public string? COD_CANAL { get; set; }
    public string? CANAL { get; set; }
    public string? DNI { get; set; }
    public DateOnly? FECHA_ENVIO { get; set; }
    public DateOnly? FECHA_GESTION { get; set; }
    public DateTime? HORA_GESTION { get; set; }
    public int? TELEFONO { get; set; }
    public string? ORIGEN_TELEFONO { get; set; }
    public string? COD_CAMPAÑA { get; set; }
    public int? COD_TIP { get; set; }
    public decimal? OFERTA { get; set; }
    public string? DNI_ASESOR { get; set; }
}

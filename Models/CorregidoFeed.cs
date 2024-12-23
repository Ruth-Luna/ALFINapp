using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;
public partial class CorregidoFeed
{
    [Column("CANAL")]
    public string? Canal { get; set; }

    [Column("CANAL_1")]
    public string? Canal1 { get; set; }
    [Key]
    [Column("DNI")]
    public float Dni { get; set; } // Ajusta el tipo de dato según tu base de datos

    [Column("FECHA_ENVIO")]
    public DateTime? FechaEnvio { get; set; }

    [Column("FECHA_GESTION")]
    public DateTime? FechaGestion { get; set; }

    [Column("HORA_GESTION")]
    public string? HoraGestion { get; set; }

    [Column("TELEFONO1")]
    public float? Telefono1 { get; set; }

    [Column("ORIGEN_TELEFONO")]
    public string? OrigenTelefono { get; set; }

    [Column("CAMPAÑA")]
    public string? Campaña { get; set; }

    [Column("COD_TIPO")]
    public float? CodTipo { get; set; }

    [Column("OFERTA")]
    public float? Oferta { get; set; } // Ajusta según el tipo en la base de datos

    [Column("DNI_ASESOR")]
    public string? DniAsesor { get; set; }
}

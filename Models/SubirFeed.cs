using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;
public partial class SubirFeed
{
    [Key]
    [Column("ID_FEED")]
    public int IdFeed { get; set; }

    [Column("COD_CANAL")]
    public string? CodCanal { get; set; }

    [Column("CANAL")]
    public string? Canal { get; set; }
    [Column("DNI")]
    public string? Dni { get; set; } // Ajusta el tipo de dato según tu base de datos

    [Column("FECHA_ENVIO")]
    public DateTime? FechaEnvio { get; set; }

    [Column("FECHA_GESTION")]
    public DateTime? FechaGestion { get; set; }

    [Column("HORA_GESTION")]
    public string? HoraGestion { get; set; }

    [Column("TELEFONO")]
    public string? Telefono { get; set; }

    [Column("ORIGEN_TELEFONO")]
    public string? OrigenTelefono { get; set; }

    [Column("COD_CAMPANA")]
    public string? CodCampana { get; set; }

    [Column("COD_TIP")]
    public float? CodTip { get; set; }

    [Column("OFERTA")]
    public float? Oferta { get; set; } // Ajusta según el tipo en la base de datos

    [Column("DNI_ASESOR")]
    public string? DniAsesor { get; set; }
}

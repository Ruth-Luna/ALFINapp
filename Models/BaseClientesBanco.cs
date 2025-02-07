using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models;
public partial class BaseClientesBanco
{
    [Key]
    [Column("id_base_banco")]
    public int IdBaseBanco { get; set; }

    [Column("dni")]
    public string? Dni { get; set; }

    [Column("tasa_1")]
    public decimal? Tasa1 { get; set; }

    [Column("tasa_2")]
    public decimal? Tasa2 { get; set; }

    [Column("tasa_3")]
    public decimal? Tasa3 { get; set; }

    [Column("tasa_4")]
    public decimal? Tasa4 { get; set; }

    [Column("tasa_5")]
    public decimal? Tasa5 { get; set; }

    [Column("tasa_6")]
    public decimal? Tasa6 { get; set; }

    [Column("tasa_7")]
    public decimal? Tasa7 { get; set; }

    [Column("oferta_max")]
    public decimal? OfertaMax { get; set; }

    [Column("id_plazo_banco")]
    public int? IdPlazoBanco { get; set; }

    [Column("CAPACIDAD_PAGO_MEN")]
    public decimal? CapacidadPagoMen { get; set; }

    [Column("id_campana_grupo_banco")]
    public int? IdCampanaGrupoBanco { get; set; }

    [Column("id_color_banco")]
    public int? IdColorBanco { get; set; }

    [Column("id_usuario_banco")]
    public int? IdUsuarioBanco { get; set; }

    [Column("id_rango_deuda")]
    public int? IdRangoDeuda { get; set; }

    [Column("num_entidades")]
    public int? NumEntidades { get; set; }

    [Column("frescura")]
    public bool? Frescura { get; set; }

    [Column("reenganche")]
    public decimal? Reenganche { get; set; }

    [Column("tasas_especiales")]
    public string? TasasEspeciales { get; set; }
    [Column("fecha_subida")]
    public DateTime? FechaSubida { get; set; }
    [Column("PATERNO")]
    public string? PATERNO { get; set; }
    [Column("NOMBRES")]
    public string? NOMBRES { get; set; }
    [Column("MATERNO")]
    public string? MATERNO { get; set; }
}

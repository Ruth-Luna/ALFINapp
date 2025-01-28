using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

public partial class ClientesEnriquecido
{
    [Key]
    [Column("id_cliente")]
    public int IdCliente { get; set; }
    [Column("id_base")]

    public int? IdBase { get; set; }
    [Column("id_base_banco")]
    public int? IdBaseBanco { get; set; }
    [Column("telefono_1")]

    public string? Telefono1 { get; set; }
    [Column("telefono_2")]

    public string? Telefono2 { get; set; }
    [Column("telefono_3")]

    public string? Telefono3 { get; set; }
    [Column("telefono_4")]

    public string? Telefono4 { get; set; }
    [Column("telefono_5")]

    public string? Telefono5 { get; set; }
    [Column("email_1")]

    public string? Email1 { get; set; }
    [Column("email_2")]

    public string? Email2 { get; set; }
    [Column("fecha_enriquecimiento")]
    public DateTime? FechaEnriquecimiento { get; set; }
    [Column("comentario_telefono_1")]
    public string? ComentarioTelefono1 { get; set; }
    [Column("comentario_telefono_2")]
    public string? ComentarioTelefono2 { get; set; }
    [Column("comentario_telefono_3")]
    public string? ComentarioTelefono3 { get; set; }
    [Column("comentario_telefono_4")]
    public string? ComentarioTelefono4 { get; set; }
    [Column("comentario_telefono_5")]
    public string? ComentarioTelefono5 { get; set; }

    [Column("ultima_tipificacion_telefono_1")]
    public string? UltimaTipificacionTelefono1 { get; set; }
    [Column("ultima_tipificacion_telefono_2")]
    public string? UltimaTipificacionTelefono2 { get; set; }
    [Column("ultima_tipificacion_telefono_3")]
    public string? UltimaTipificacionTelefono3 { get; set; }
    [Column("ultima_tipificacion_telefono_4")]
    public string? UltimaTipificacionTelefono4 { get; set; }
    [Column("ultima_tipificacion_telefono_5")]
    public string? UltimaTipificacionTelefono5 { get; set; }
    [Column("fecha_ultima_tipificacion_telefono_1")]
    public DateTime? FechaUltimaTipificacionTelefono1 { get; set; }
    [Column("fecha_ultima_tipificacion_telefono_2")]
    public DateTime? FechaUltimaTipificacionTelefono2 { get; set; }
    [Column("fecha_ultima_tipificacion_telefono_3")]
    public DateTime? FechaUltimaTipificacionTelefono3 { get; set; }
    [Column("fecha_ultima_tipificacion_telefono_4")]
    public DateTime? FechaUltimaTipificacionTelefono4 { get; set; }
    [Column("fecha_ultima_tipificacion_telefono_5")]
    public DateTime? FechaUltimaTipificacionTelefono5 { get; set; }
    [Column("id_clientetip_telefono_1")]
    public int? IdClientetipTelefono1 { get; set; }
    [Column("id_clientetip_telefono_2")]
    public int? IdClientetipTelefono2 { get; set; }
    [Column("id_clientetip_telefono_3")]
    public int? IdClientetipTelefono3 { get; set; }
    [Column("id_clientetip_telefono_4")]
    public int? IdClientetipTelefono4 { get; set; }
    [Column("id_clientetip_telefono_5")]
    public int? IdClientetipTelefono5 { get; set; }
}

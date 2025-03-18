using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models;

public partial class TelefonosAgregados
{
    [Column("id_cliente")]
    public int IdCliente { get; set; }
    [Column("telefono")]
    public string? Telefono { get; set; }
    [Column("comentario")]
    public string? Comentario { get; set; }
    [Column("agregado_por")]
    public string? AgregadoPor { get; set; }
    [Column("ultima_tipificacion")]
    public string? UltimaTipificacion { get; set; }
    [Key]
    [Column("id_telefono")]
    public int IdTelefono { get; set; }
    [Column("fecha_ultima_tipificacion")]
    public DateTime? FechaUltimaTipificacion { get; set; }
    [Column("id_clientetip")]
    public int? IdClienteTip { get; set; }
}

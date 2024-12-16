using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

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
    [Column("tipificacion_mayor_peso")]
    public string? TipificacionMayorPeso { get; set; }
    [Key]
    [Column("id_telefono")]
    public int IdTelefono { get; set; }
}

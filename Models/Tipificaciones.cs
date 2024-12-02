using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

public partial class Tipificaciones
{
    [Key]
    [Column("id_tipificacion")]
    public int IdTipificacion { get; set; }
    [Column("COD_ESTADO")]
    public string CodEstado { get; set; } = null!;
    [Column("ESTADO")]
    public string Estado { get; set; } = null!;
    [Column("COD_SUB")]
    public string CodSub { get; set; } = null!;
    [Column("SUB_ESTADO")]
    public string SubEstado { get; set; } = null!;
    [Column("COD_TIP")]
    public string CodTip { get; set; } = null!;
    [Column("descripcion_tipificacion")]
    public string DescripcionTipificacion { get; set; } = null!;
    [Column("PESO")]
    public int? Peso { get; set; }

}

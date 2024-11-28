using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public partial class Tipificacione
{
    [Key]
    public int IdTipificacion { get; set; }

    public string CodEstado { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string CodSub { get; set; } = null!;

    public string SubEstado { get; set; } = null!;

    public string CodTip { get; set; } = null!;

    public string DescripcionTipificacion { get; set; } = null!;

    public int? Peso { get; set; }

}

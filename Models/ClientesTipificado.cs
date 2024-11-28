using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public partial class ClientesTipificado
{
    [Key]
    public int IdClientetip { get; set; }

    public int IdAsignacion { get; set; }

    public int IdTipificacion { get; set; }

    public DateTime? FechaTipificacion { get; set; }

    public string Origen { get; set; } = null!;

    public virtual ClientesAsignado IdAsignacionNavigation { get; set; } = null!;

    public virtual Tipificacione IdTipificacionNavigation { get; set; } = null!;
}

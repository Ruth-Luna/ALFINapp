using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public partial class Retiro
{
    [Key]
    public int IdRetiro { get; set; }

    public int? IdBase { get; set; }

    public string? Dni { get; set; }

    public DateTime? FechaRetiro { get; set; }

    public string? MotivoRetiro { get; set; }

}

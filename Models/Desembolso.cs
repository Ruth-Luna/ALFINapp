using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public partial class Desembolso
{
    [Key]
    public int IdDesembolsos { get; set; }

    public int? IdBase { get; set; }

    public string? Dni { get; set; }

    public decimal? MontoDesembolsado { get; set; }

    public DateTime? FechaDesembolso { get; set; }

    public string? Canal { get; set; }

}

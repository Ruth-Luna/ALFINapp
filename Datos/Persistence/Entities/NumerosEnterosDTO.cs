using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ALFINapp.Infrastructure.Persistence.Models;

public class NumerosEnterosDTO
{
    [Key]
    public int? NumeroEntero { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models;

public partial class BaseCliente
{
    [Key]
    [Column("id_base")]
    public int IdBase { get; set; }
    [Column("dni")]
    public string? Dni { get; set; }
    [Column("x_appaterno")]
    public string? XAppaterno { get; set; }
    [Column("x_apmaterno")]
    public string? XApmaterno { get; set; }
    [Column("x_nombre")]
    public string? XNombre { get; set; }
    [Column("edad")]
    public int? Edad { get; set; }
    [Column("departamento")]
    public string? Departamento { get; set; }
    [Column("provincia")]
    public string? Provincia { get; set; }
    [Column("distrito")]
    public string? Distrito { get; set; }
    [Column("id_base_banco")]
    public int? IdBaseBanco { get; set; }
}

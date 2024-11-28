using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

public partial class ClientesAsignado
{
    [Key]
    [Column("id_asignacion")]
    public int IdAsignacion { get; set; }
    [Column("id_cliente")]

    public int IdCliente { get; set; }
    [Column("id_usuario")]

    public int IdUsuario { get; set; }
    [Column("fecha_asignacion")]

    public DateTime? FechaAsignacion { get; set; }
    [Column("origen")]

    public string? Origen { get; set; }

}

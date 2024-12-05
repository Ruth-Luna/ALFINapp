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
    [Column("id_usuarioS")]
    public int IdUsuarioS { get; set; }    
    [Column("fecha_asignacion_sup")]
    public DateTime? FechaAsignacionSup { get; set; }
    [Column("id_usuarioV")]
    public int IdUsuarioV { get; set; }
    [Column("fecha_asignacion_vendedor")]
    public DateTime? FechaAsignacionVendedor { get; set; }
}

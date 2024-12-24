using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

public partial class CargaManualCsv
{
    [Key]
    [Column("id_carga")]
    public int IdCarga { get; set; }
    [Column("id_usuario")]
    public int? IdUsuario { get; set; }
    [Column("fecha_de_carga")]
    public DateTime? FechaDeCarga { get; set; }
    [Column("dni_usuario_agregado")]
    public string DniUsuarioAgregado { get; set; }
}

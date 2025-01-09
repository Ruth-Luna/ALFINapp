using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

public partial class AsesoresSecundariosAsignacion
{
    [Key]
    [Column("id_secundario_asignacion")]
    public int id_secundario_asignacion { get; set; }
    [Column("id_asignacion")]
    public int id_asignacion { get; set; }
    [Column("id_usuarioV")]
    public int id_usuarioV { get; set; }
    [Column("fecha_asignacion_secundarioV")]
    public DateTime fecha_asignacion_secundarioV { get; set; }
    [Column("tipo_asesor_secundario")]
    public string tipo_asesor_secundario { get; set; }
    [Column("comentario_general_asesor_secundario")]
    public string comentario_general_asesor_secundario { get; set; }
    [Column("tipificacion_mayor_peso_secundaria")]
    public string tipificacion_mayor_peso_secundaria { get; set; }
    [Column("peso_tipificacion_mayor_secundaria")]
    public int peso_tipificacion_mayor_secundaria { get; set; }

    [Column("fecha_tipificacion_mayor_peso_secundaria")]
    public DateTime fecha_tipificacion_mayor_peso_secundaria { get; set; }
    [Column("ultima_tipificacion_general_secundaria")]
    public string ultima_tipificacion_general_secundaria { get; set; }
    [Column("fecha_ultima_tipificacion_secundaria")]
    public DateTime fecha_ultima_tipificacion_secundaria { get; set; }
    
}

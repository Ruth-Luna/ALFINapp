﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models;

public partial class ClientesTipificado
{
    [Key]
    [Column("id_clientetip")]
    public int IdClientetip { get; set; }
    [Column("id_asignacion")]
    public int IdAsignacion { get; set; }
    [Column("id_tipificacion")]
    public int IdTipificacion { get; set; }
    [Column("fecha_tipificacion")]
    public DateTime? FechaTipificacion { get; set; }
    [Column("origen")]
    public string? Origen { get; set; }
    [Column("telefono_tipificado")]
    public string? TelefonoTipificado { get; set; }
    [Column("derivacion_fecha")]
    public DateTime? DerivacionFecha { get; set; }
    
}

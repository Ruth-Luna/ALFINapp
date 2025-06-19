﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models;

public partial class ClientesAsignado
{
    [Key]
    [Column("id_asignacion")]
    public int IdAsignacion { get; set; }
    [Column("id_cliente")]
    public int IdCliente { get; set; }
    [Column("id_usuarioS")]
    public int? IdUsuarioS { get; set; }
    [Column("fecha_asignacion_sup")]
    public DateTime? FechaAsignacionSup { get; set; }
    [Column("id_usuarioV")]
    public int? IdUsuarioV { get; set; }
    [Column("fecha_asignacion_vendedor")]
    public DateTime? FechaAsignacionVendedor { get; set; }
    [Column("fuente_base")]
    public string? FuenteBase { get; set; }

    [Column("finalizar_tipificacion")]
    public bool FinalizarTipificacion { get; set; }

    [Column("comentario_general")]
    public string? ComentarioGeneral { get; set; }

    [Column("tipificacion_mayor_peso")]
    public string? TipificacionMayorPeso { get; set; }
    [Column("peso_tipificacion_mayor")]
    public int? PesoTipificacionMayor { get; set; }
    [Column("cliente_desembolso")]
    public bool? ClienteDesembolso { get; set; }
    [Column("cliente_retirado")]
    public bool? ClienteRetirado { get; set; }
    [Column("fecha_tipificacion_mayor_peso")]
    public DateTime? FechaTipificacionMayorPeso { get; set; }
    [Column("identificador_base")]
    public string? IdentificadorBase { get; set; }
    [Column("destino")]
    public string? Destino { get; set; }
    [Column("id_NombreBase")]
    public int? IdNombreBase { get; set; }
    [Column("id_detalle_base")]
    public int? IdDetalleBase { get; set; }
    [Column("id_lista")]
    public int? IdLista { get; set; }
    [Column("nom_base")]
    public string? NomBase { get; set; }
}

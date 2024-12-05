using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models;

public partial class DetalleBase
{
    [Key]
    [Column("id_detalle")]
    public int IdDetalle { get; set; }
    
    [Column("id_base")]
    public int IdBase { get; set; }

    [Column("sucursal")]
    public string? Sucursal { get; set; }

    [Column("tienda")]
    public string? Tienda { get; set; }
    
    [Column("oferta_max")]
    public decimal? OfertaMax { get; set; }

    [Column("tasa_minima")]
    public decimal? TasaMinima { get; set; }

    [Column("tipo_verificacion")]
    public int? TipoVerificacion { get; set; }

    [Column("canal")]
    public string? Canal { get; set; }

    [Column("tipovisita")]
    public string? Tipovisita { get; set; }
    
    [Column("tasa_1")]
    public decimal? Tasa1 { get; set; }

    [Column("tasa_2")]
    public decimal? Tasa2 { get; set; }

    [Column("tasa_3")]
    public decimal? Tasa3 { get; set; }

    [Column("tasa_4")]
    public decimal? Tasa4 { get; set; }

    [Column("tasa_5")]
    public decimal? Tasa5 { get; set; }

    [Column("tasa_6")]
    public decimal? Tasa6 { get; set; }

    [Column("tasa_7")]
    public decimal? Tasa7 { get; set; }

    [Column("segmento")]
    public string? Segmento { get; set; }

    [Column("plazo")]
    public int? Plazo { get; set; }

    [Column("campaña")]
    public string? Campaña { get; set; }

    [Column("tem")]
    public string? Tem { get; set; }

    [Column("desgravamen")]
    public decimal? Desgravamen { get; set; }

    [Column("cuota")]
    public decimal? Cuota { get; set; }

    [Column("oferta_12M")]
    public decimal? Oferta12m { get; set; }

    [Column("tasa_12M")]
    public decimal? Tasa12m { get; set; }

    [Column("desgravamen_12M")]
    public decimal? Desgravamen12m { get; set; }

    [Column("cuota_12M")]
    public decimal? Cuota12m { get; set; }

    [Column("oferta_18M")]
    public decimal? Oferta18m { get; set; }

    [Column("tasa_18M")]
    public decimal? Tasa18m { get; set; }

    [Column("desgravamen_18M")]
    public decimal? Desgravamen18m { get; set; }

    [Column("cuota_18M")]
    public decimal? Cuota18m { get; set; }

    [Column("oferta_24M")]
    public decimal? Oferta24m { get; set; }

    [Column("tasa_24M")]
    public decimal? Tasa24m { get; set; }

    [Column("desgravamen_24M")]
    public decimal? Desgravamen24m { get; set; }

    [Column("cuota_24M")]
    public decimal? Cuota24m { get; set; }

    [Column("oferta_36M")]
    public decimal? Oferta36m { get; set; }

    [Column("tasa_36M")]
    public decimal? Tasa36m { get; set; }

    [Column("desgravamen_36M")]
    public decimal? Desgravamen36m { get; set; }

    [Column("cuota_36M")]
    public decimal? Cuota36m { get; set; }

    [Column("validador_telefono")]
    public string? ValidadorTelefono { get; set; }

    [Column("prioridad")]
    public string? Prioridad { get; set; }

    [Column("nombre_prioridad")]
    public string? NombrePrioridad { get; set; }

    [Column("deuda_1")]
    public decimal? Deuda1 { get; set; }

    [Column("entidad_1")]
    public string? Entidad1 { get; set; }

    [Column("deuda_2")]
    public decimal? Deuda2 { get; set; }

    [Column("entidad_2")]
    public string? Entidad2 { get; set; }

    [Column("deuda_3")]
    public decimal? Deuda3 { get; set; }

    [Column("entidad_3")]
    public string? Entidad3 { get; set; }

    [Column("sucursal_comercial")]
    public string? SucursalComercial { get; set; }

    [Column("agencia_comercial")]
    public string? AgenciaComercial { get; set; }

    [Column("region_comercial")]
    public string? RegionComercial { get; set; }

    [Column("ubicacion")]
    public string? Ubicacion { get; set; }

    [Column("oferta_maxima_sin_seguro")]
    public decimal? OfertaMaximaSinSeguro { get; set; }

    [Column("color_gestion")]
    public string? ColorGestion { get; set; }

    [Column("oferta_final")]
    public decimal? OfertaFinal { get; set; }

    [Column("garantia")]
    public string? Garantia { get; set; }

    [Column("oferta_minima_paperless")]
    public decimal? OfertaMinimaPaperless { get; set; }

    [Column("rango_edad")]
    public string? RangoEdad { get; set; }

    [Column("rango_oferta")]
    public string? RangoOferta { get; set; }

    [Column("rango_sueldo")]
    public string? RangoSueldo { get; set; }

    [Column("capacidad_max")]
    public decimal? CapacidadMax { get; set; }

    [Column("peer")]
    public string? Peer { get; set; }

    [Column("tipo_gest")]
    public string? TipoGest { get; set; }

    [Column("tipo_cliente")]
    public string? TipoCliente { get; set; }

    [Column("cliente_nuevo")]
    public string? ClienteNuevo { get; set; }

    [Column("grupo_tasa")]
    public string? GrupoTasa { get; set; }

    [Column("grupo_monto")]
    public string? GrupoMonto { get; set; }

    [Column("tasa_vs_monto")]
    public string? TasaVsMonto { get; set; }

    [Column("grupo_tasa_reenganche")]
    public string? GrupoTasaReenganche { get; set; }

    [Column("saldo_diferencial_reeng")]
    public decimal? SaldoDiferencialReeng { get; set; }

    [Column("flag_reeng")]
    public string? FlagReeng { get; set; }

    [Column("color")]
    public string? Color { get; set; }

    [Column("color_final")]
    public string? ColorFinal { get; set; }

    [Column("propension")]
    public int? Propension { get; set; }

    [Column("marca_base")]
    public string? MarcaBase { get; set; }

    [Column("segmento_user")]
    public string? SegmentoUser { get; set; }

    [Column("usuario")]
    public string? Usuario { get; set; }

    [Column("tipo_cliente_riegos")]
    public string? TipoClienteRiegos { get; set; }

    [Column("incremento_monto_riesgos")]
    public int? IncrementoMontoRiesgos { get; set; }

    [Column("fecha_carga")]
    public DateTime? FechaCarga { get; set; }

    [Column("tipo_base")]
    public string? TipoBase { get; set; }
}

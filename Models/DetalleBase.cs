using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALFINapp.Models;

public partial class DetalleBase
{
    [Key]
    public int IdDetalle { get; set; }

    public int IdBase { get; set; }

    public string? Sucursal { get; set; }

    public string? Tienda { get; set; }

    public decimal? OfertaMax { get; set; }

    public decimal? TasaMinima { get; set; }

    public int? TipoVerificacion { get; set; }

    public string? Canal { get; set; }

    public string? Tipovisita { get; set; }

    public decimal? Tasa1 { get; set; }

    public decimal? Tasa2 { get; set; }

    public decimal? Tasa3 { get; set; }

    public decimal? Tasa4 { get; set; }

    public decimal? Tasa5 { get; set; }

    public decimal? Tasa6 { get; set; }

    public decimal? Tasa7 { get; set; }

    public string? Segmento { get; set; }

    public int? Plazo { get; set; }

    public string? Campaña { get; set; }

    public string? Tem { get; set; }

    public decimal? Desgravamen { get; set; }

    public decimal? Cuota { get; set; }

    public decimal? Oferta12m { get; set; }

    public decimal? Tasa12m { get; set; }

    public decimal? Desgravamen12m { get; set; }

    public decimal? Cuota12m { get; set; }

    public decimal? Oferta18m { get; set; }

    public decimal? Tasa18m { get; set; }

    public decimal? Desgravamen18m { get; set; }

    public decimal? Cuota18m { get; set; }

    public decimal? Oferta24m { get; set; }

    public decimal? Tasa24m { get; set; }

    public decimal? Desgravamen24m { get; set; }

    public decimal? Cuota24m { get; set; }

    public decimal? Oferta36m { get; set; }

    public decimal? Tasa36m { get; set; }

    public decimal? Desgravamen36m { get; set; }

    public decimal? Cuota36m { get; set; }

    public string? ValidadorTelefono { get; set; }

    public string? Prioridad { get; set; }

    public string? NombrePrioridad { get; set; }

    public decimal? Deuda1 { get; set; }

    public string? Entidad1 { get; set; }

    public decimal? Deuda2 { get; set; }

    public string? Entidad2 { get; set; }

    public decimal? Deuda3 { get; set; }

    public string? Entidad3 { get; set; }

    public string? SucursalComercial { get; set; }

    public string? AgenciaComercial { get; set; }

    public string? RegionComercial { get; set; }

    public string? Ubicacion { get; set; }

    public decimal? OfertaMaximaSinSeguro { get; set; }

    public string? ColorGestion { get; set; }

    public decimal? OfertaFinal { get; set; }

    public string? Garantia { get; set; }

    public decimal? OfertaMinimaPaperless { get; set; }

    public string? RangoEdad { get; set; }

    public string? RangoOferta { get; set; }

    public string? RangoSueldo { get; set; }

    public decimal? CapacidadMax { get; set; }

    public string? Peer { get; set; }

    public string? TipoGest { get; set; }

    public string? TipoCliente { get; set; }

    public string? ClienteNuevo { get; set; }

    public string? GrupoTasa { get; set; }

    public string? GrupoMonto { get; set; }

    public string? TasaVsMonto { get; set; }

    public string? GrupoTasaReenganche { get; set; }

    public decimal? SaldoDiferencialReeng { get; set; }

    public string? FlagReeng { get; set; }

    public string? Color { get; set; }

    public string? ColorFinal { get; set; }

    public int? Propension { get; set; }

    public string? MarcaBase { get; set; }

    public string? SegmentoUser { get; set; }

    public string? Usuario { get; set; }

    public string? TipoClienteRiegos { get; set; }

    public int? IncrementoMontoRiesgos { get; set; }

    public DateTime? FechaCarga { get; set; }

    public string? TipoBase { get; set; }

}

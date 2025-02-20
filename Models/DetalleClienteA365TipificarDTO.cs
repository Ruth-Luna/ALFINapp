using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class DetalleClienteA365TipificarDTO
    {
        // Propiedades de la tabla BaseCliente
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public int? Edad { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
        public int? IdBase { get; set; }

        //Propiedades de la tabla DetalleBase
        public string? Campaña { get; set; }
        public string? Sucursal { get; set; }
        public string? AgenciaComercial { get; set; }
        public int? Plazo { get; set; }
        public decimal? Cuota { get; set; }
        public string? GrupoTasa { get; set; }
        public string? GrupoMonto { get; set; }
        public int? Propension { get; set; }
        public string? TipoCliente { get; set; }
        public string? ClienteNuevo { get; set; }
        public string? Color { get; set; }
        public string? ColorFinal { get; set; }
        public string? Usuario { get; set; }
        public string? SegmentoUser { get; set; }

        //Propiedades de la Tabla ClientsEnriquecido
        public int? IdCliente { get; set; }
        public string? Telefono1 { get; set; }
        public string? Telefono2 { get; set; }
        public string? Telefono3 { get; set; }
        public string? Telefono4 { get; set; }
        public string? Telefono5 { get; set; }
        public string? ComentarioTelefono1 { get; set; }
        public string? ComentarioTelefono2 { get; set; }
        public string? ComentarioTelefono3 { get; set; }
        public string? ComentarioTelefono4 { get; set; }
        public string? ComentarioTelefono5 { get; set; }
        public string? UltimaTipificacionTelefono1 { get; set; }
        public string? UltimaTipificacionTelefono2 { get; set; }
        public string? UltimaTipificacionTelefono3 { get; set; }
        public string? UltimaTipificacionTelefono4 { get; set; }
        public string? UltimaTipificacionTelefono5 { get; set; }
        public DateTime? FechaUltimaTipificacionTelefono1 { get; set; }
        public DateTime? FechaUltimaTipificacionTelefono2 { get; set; }
        public DateTime? FechaUltimaTipificacionTelefono3 { get; set; }
        public DateTime? FechaUltimaTipificacionTelefono4 { get; set; }
        public DateTime? FechaUltimaTipificacionTelefono5 { get; set; }

        //Propiedas de Tasa
        public decimal? OfertaMax { get; set; }
        public decimal? TasaMinima { get; set; }
        public decimal? Oferta12m { get; set; }
        public decimal? Tasa12m { get; set; }
        public decimal? Cuota12m { get; set; }
        public decimal? Oferta18m { get; set; }
        public decimal? Tasa18m { get; set; }
        public decimal? Cuota18m { get; set; }
        public decimal? Oferta24m { get; set; }
        public decimal? Tasa24m { get; set; }
        public decimal? Cuota24m { get; set; }
        public decimal? Oferta36m { get; set; }
        public decimal? Tasa36m { get; set; }
        public decimal? Cuota36m { get; set; }

        //Propiedades de la tabla ClientesAsignados
        public int? IdAsignacion { get; set; }
        public string? FuenteBase { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    [Table("desembolsos")]
    public class Desembolsos
    {
        [Key]
        [Column("id_desembolsos")]
        public int IdDesembolsos { get; set; }

        [Column("dni_desembolso")]
        public string? DniDesembolso { get; set; }

        [Column("CUENTA_BT")]
        public string? CuentaBT { get; set; }

        [Column("N_OPER")]
        public string? NOper { get; set; }

        [Column("SUCURSAL")]
        public string? Sucursal { get; set; }

        [Column("MONTO_FINANCIADO")]
        public decimal? MontoFinanciado { get; set; }

        [Column("FECHA_SOL")]
        public DateTime? FechaSol { get; set; }

        [Column("FECHA_DESEMBOLSOS")]
        public DateTime? FechaDesembolsos { get; set; }

        [Column("FECHA_GEST")]
        public DateTime? FechaGest { get; set; }

        [Column("CANAL")]
        public string? Canal { get; set; }

        [Column("TIPO_DESEM")]
        public string? TipoDesem { get; set; }

        [Column("fecha_proporcion")]
        public DateTime? FechaProporcion { get; set; }

        [Column("Observacion")]
        public string? Observacion { get; set; }

        [Column("id_NombreBase")]
        public int? IdNombreBase { get; set; }
        [Column("doc_asesor")]
        public string? DocAsesor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Entities
{
    public class Desembolso
    {
        public int IdDesembolsos { get; set; }
        public string? DniDesembolso { get; set; }
        public string? CuentaBT { get; set; }
        public string? NOper { get; set; }
        public string? Sucursal { get; set; }
        public decimal? MontoFinanciado { get; set; }
        public DateTime? FechaSol { get; set; }
        public DateTime? FechaDesembolsos { get; set; }
        public DateTime? FechaGest { get; set; }
        public string? Canal { get; set; }
        public string? TipoDesem { get; set; }
        public DateTime? FechaProporcion { get; set; }
        public string? Observacion { get; set; }
        public int? IdNombreBase { get; set; }
        public string? DocAsesor { get; set; }
    }
}
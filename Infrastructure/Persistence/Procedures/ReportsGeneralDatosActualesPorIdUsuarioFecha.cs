using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsGeneralDatosActualesPorIdUsuarioFecha
    {
        public int? tiene_derivacion { get; set; }
        public string? dni_cliente { get; set; }
        public string? nombre_agencia { get; set; }
        public decimal? oferta_max { get; set; }
        public string? supervisor { get; set; }
        public string? dni_asesor { get; set; }
        public string? cod_campaña { get; set; }
        public int? cod_tip { get; set; }
        public decimal? oferta { get; set; }
        public string? telefono { get; set; }
        public decimal? MONTO_FINANCIADO { get; set; }
        public string? SUCURSAL { get; set; }
        public int? tiene_desembolso { get; set; }
        public int? tiene_asignacion { get; set; }
        public string? nombre_cliente { get; set; }
        public string? nombre_asesor { get; set; }
    }
}
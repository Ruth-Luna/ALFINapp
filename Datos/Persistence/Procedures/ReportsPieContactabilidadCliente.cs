using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsPieContactabilidadCliente
    {
        public string estado { get; set; } = "";
        public int cantidad { get; set; } = 0;
        public decimal porcentaje { get; set; } = 0;
    }
    public class ReportsSupervisorGestionFecha
    {
        [Column("doc_cliente")]
        public string DocCliente { get; set; } = "";
        [Column("doc_asesor")]
        public string DocAsesor { get; set; } = "";
        [Column("fecha_gestion")]
        public DateTime FechaGestion { get; set; } = DateTime.MinValue;
        [Column("cod_tip")]
        public int CodTip { get; set; }
    }
    public class ReportsAsesorTipificacionesTop
    {
        public int IdTipificacion { get; set; } = 0;
        public string? DescripcionTipificaciones { get; set; } = "";
        public int ContadorTipificaciones { get; set; } = 0;
    }
}
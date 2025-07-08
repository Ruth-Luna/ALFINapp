using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsSupervisorGestion
    {
        [Column("id_derivacion")]
        public int IdDerivacion { get; set; }
        [Column("doc_asesor")]
        public string? DocAsesor { get; set; }
        [Column("doc_cliente")]
        public string? DocCliente { get; set; }
        [Column("cod_tip")]
        public int CodTip { get; set; }
    }
}
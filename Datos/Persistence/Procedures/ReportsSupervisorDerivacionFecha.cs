using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsSupervisorDerivacionFecha
    {
        [Column ("fecha")]
        public DateTime? fecha { get; set; }
        [Column ("contador")]
        public int contador { get; set; }
    }
}
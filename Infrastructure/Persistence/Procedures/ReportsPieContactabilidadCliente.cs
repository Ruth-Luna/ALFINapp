using System;
using System.Collections.Generic;
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
}
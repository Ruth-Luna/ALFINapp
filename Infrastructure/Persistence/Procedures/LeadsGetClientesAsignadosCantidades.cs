using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class LeadsGetClientesAsignadosCantidades
    {
        public int Total { get; set; }
        public int Gestionados { get; set; }
        public int Pendientes { get; set; }
    }
}
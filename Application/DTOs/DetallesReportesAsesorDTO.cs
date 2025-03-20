using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesAsesorDTO
    {
        public Usuario? Usuario { get; set; }
        public List<ClientesAsignado>? ClientesAsignados { get; set; }
        public List<DerivacionesAsesores>? DerivacionesDelAsesor { get; set; }
        public List<ClientesTipificado>? UltimaTipificacionXAsignacion { get; set; }
    }
}
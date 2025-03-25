using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesAsesorDTO
    {
        public Usuario Usuario { get; set; } = new Usuario();
        public List<ClientesAsignado> ClientesAsignados { get; set; } = new List<ClientesAsignado>();
        public List<DerivacionesAsesores> DerivacionesDelAsesor { get; set; } = new List<DerivacionesAsesores>();
        public List<ClientesTipificado> UltimaTipificacionXAsignacion { get; set; } = new List<ClientesTipificado>();
    }
}
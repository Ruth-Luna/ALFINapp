using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class InsertarAsignacionRequest
    {
        public List<int>? IdClientes { get; set; }
        public List<InsertarClientesASupervisoresDTO>? SupervisoresData { get; set; }
        public string? FuenteBase { get; set; }
        public string? Destino { get; set; }
    }
    public class InsertarClientesASupervisoresDTO
    {
        public int? idUsuario { get; set; }
        public string? nombresCompletos { get; set; }
        public int? numeroClientesAsignados { get; set; }
    }
}
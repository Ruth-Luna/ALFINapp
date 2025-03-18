using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class GenerarDerivacionDTO
    {
        public DateTime? FechaVisitaDerivacion { get; set; }
        public string? AgenciaDerivacion { get; set; }
        public string? NombreAsesorDerivacion { get; set; }
        public decimal? OfertaEnviadaDerivacion { get; set; }
        public string? DNIAsesorDerivacion { get; set; }
        public string? TelefonoDerivacion { get; set; }
        public string? DNIClienteDerivacion { get; set; }
        public string? NombreClienteDerivacion { get; set; }
        public int IdReferido { get; set; }
    }
}
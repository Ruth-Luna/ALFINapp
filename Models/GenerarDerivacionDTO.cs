using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class GenerarDerivacionDTO
    {
        public DateTime? FechaVisitaDerivacion { get; set; }
        public string? AgenciaDerivacion { get; set; }
        public string? AsesorDerivacion { get; set; }
        public string? DNIAsesorDerivacion { get; set; }
        public string? TelefonoDerivacion { get; set; }
        public string? DNIClienteDerivacion { get; set; }
        public string? NombreClienteDerivacion { get; set; }
    }
}
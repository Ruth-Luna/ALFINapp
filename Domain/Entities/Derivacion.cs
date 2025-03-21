using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.ValueObjects;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Entities
{
    public class Derivacion
    {
        public int IdDerivacion { get; set; }
        public DateTime FechaDerivacion { get; set; }
        public string? DniAsesor { get; set; }
        public string? DniCliente { get; set; }
        public int? IdCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? TelefonoCliente { get; set; }
        public string? NombreAgencia { get; set; }
        public string? NumAgencia { get; set; }
        public bool? FueProcesado { get; set; }
        public DateTime? FechaVisita { get; set; }
        public string? EstadoDerivacion { get; set; }
        public int? IdAsignacion { get; set; }
        public string? ObservacionDerivacion { get; set; }
        public bool? FueEnviadoEmail { get; set; }
    }
}
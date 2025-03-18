using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class TelefonosAgregadosDTO
    {
        public string? TelefonoTipificado { get; set; }
        public string? ComentarioTelefono { get; set; }
        public string? DescripcionTipificacion { get; set; }
        public DateTime? FechaTipificacionSup { get; set; }
    }
}
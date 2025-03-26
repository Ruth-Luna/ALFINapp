using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewCliente
    {
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public int? Edad { get; set; }
        public decimal? OfertaMax { get; set; }
        public string? Tipificacion { get; set; }
        public int? IdTipificacion { get; set; }
        public string? Campaña { get; set; }
        public int IdCliente { get; set; }
        public int IdBase { get; set; }
        public DateTime FechaCarga { get; set; }
    }
}
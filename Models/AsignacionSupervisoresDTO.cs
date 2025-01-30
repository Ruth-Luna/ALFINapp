using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class AsignacionSupervisoresDTO
    {
        public List<StringDTO>? UCampanas { get; set; }
        public List<StringDTO>? UUsuario { get; set; }
        public List<StringDTO>? UTipoBase { get; set; }
    }
}
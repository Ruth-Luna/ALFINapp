using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class AsignacionSupervisoresDTO
    {
        public List<StringDTO>? UCampanas { get; set; }
        public List<StringDTO>? UClienteEstado { get; set; }
        public List<StringDTO>? UColor { get; set; }
        public List<StringDTO>? UColorFinal { get; set; }
        public List<NumerosEnterosDTO>? UFrescura { get; set; }
        public List<StringDTO>? UGrupoMonto { get; set; }
        public List<StringDTO>? UGrupoTasa { get; set; }
        public List<NumerosEnterosDTO>? UPropension { get; set; }
        public List<StringDTO>? URangoEdad { get; set; }
        public List<StringDTO>? URangoOferta { get; set; }
        public List<StringDTO>? URangoTasas { get; set; }
        public List<StringDTO>? UTipoCliente { get; set; }
        public List<StringDTO>? UUsuario { get; set; }
        public List<StringDTO>? UTipoBase { get; set; }
        public List<NumerosEnterosDTO>? UFlgDeudaPlus { get; set; }
    }
}
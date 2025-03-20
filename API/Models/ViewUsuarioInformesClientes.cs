using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewUsuarioInformesClientes
    {
        public ViewUsuario Usuario { get; set; } = new ViewUsuario();
        public int? NumTipificaciones { get; set; }
        public int? NumClientes { get; set; }
        public int? NumClientesGestionados { get; set; }
        public int? NumClientesNoGestionados { get; set; }
        public int? NumClientesDerivados { get; set; }
        public int? NumClientesPendientes { get; set; }
        public int? NumClientesQueNoDesean { get; set; }
        public int? NumClientesQueNoContestan { get; set; }
        public int? NumClientesDesembolsados { get; set; }
        public int? NumClientesRetirados { get; set; }
        public List<ViewCliente>? Clientes { get; set; }
    }
}
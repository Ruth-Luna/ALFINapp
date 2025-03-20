using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewReportesGeneral
    {
        public List<ViewUsuarioInformesClientes> Usuarios { get; set; } = new List<ViewUsuarioInformesClientes>();
        public List<ViewSupervisorInformesAsesores> Supervisores { get; set; } = new List<ViewSupervisorInformesAsesores>();
    }
}
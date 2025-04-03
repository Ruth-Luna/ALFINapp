using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.DTOs
{
    public class DetallesInicioSupervisorDTO
    {
        public List<DetallesClienteDTO> DetallesClientes { get; set; } = new List<DetallesClienteDTO>();
    }
}
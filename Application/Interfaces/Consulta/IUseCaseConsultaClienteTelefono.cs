using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Consulta
{
    public interface IUseCaseConsultaClienteTelefono
    {
        public Task<(bool IsSuccess, string Message, ViewClienteDetalles Data)> exec(string telefono);
    }
}
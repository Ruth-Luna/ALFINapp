using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Interfaces.Asignacion
{
    public interface IUseCaseAsignarClienteManual
    {
        public Task<(bool success, string message)> exec(string dniCliente, int IdUsuarioV, string baseTipo);
    }
}
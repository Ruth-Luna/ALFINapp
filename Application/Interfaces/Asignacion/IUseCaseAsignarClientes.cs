using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.DTOs;

namespace ALFINapp.Application.Interfaces.Asignacion
{
    public interface IUseCaseAsignarClientes
    {
        public Task<(bool success, string message)> exec(List<DtoVAsignarClientes> asignacionAsesor, string selectBase, int idSupervisor);
    }
}
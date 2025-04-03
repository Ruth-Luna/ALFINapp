using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Asignacion
{
    public interface IUseCaseGetAsignacion
    {
        public Task<(bool IsSuccess, string Message, ViewAsignacionSupervisor Data)> Execute(int idUsuario);
    }
}
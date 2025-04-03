using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Supervisor
{
    public interface IUseCaseGetInicio
    {
        public Task<(bool IsSuccess, string Message, ViewInicioSupervisor Data)> Execute(int idUsuario);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesMetas
    {
        public Task<(bool IsSuccess, string Message, object Data)> Execute(int idUsuario);
    }
}
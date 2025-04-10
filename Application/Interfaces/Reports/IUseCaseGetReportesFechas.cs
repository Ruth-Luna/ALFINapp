using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesFechas
    {
        public Task<(bool IsSuccess, string Message, ViewReportesFecha Data)> Execute(string fecha, int idUsuario, int rol);
    }
}
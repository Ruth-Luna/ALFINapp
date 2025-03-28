using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesGeneralAsesor
    {
        public Task<(bool success, string message, ViewReportesGeneral? data)> Execute(int idUsuario);
    }
}
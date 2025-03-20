using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesAsesor
    {
        public Task<(bool IsSuccess, string Message, Reporte? Data)> Execute(int idUsuario);
    }
}
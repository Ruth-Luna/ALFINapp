using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesAsesor
    {
        public Task<(bool IsSuccess, string Message, ViewReportesAsesores? Data)> Execute(int idUsuario);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesAdministrador
    {
        public Task<(bool IsSuccess, string Message, ViewReportesGeneral? Data)> Execute(int idUsuario);
    }
}
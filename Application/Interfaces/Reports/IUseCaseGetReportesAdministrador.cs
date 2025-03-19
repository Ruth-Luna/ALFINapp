using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Reports
{
    public interface IUseCaseGetReportesAdministrador
    {
        public Task<(bool IsSuccess, string Message, Reportes? Data)> Execute(string? email, int idUsuario);
    }
}
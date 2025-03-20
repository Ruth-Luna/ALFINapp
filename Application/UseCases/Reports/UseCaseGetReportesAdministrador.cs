using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.Interfaces.Reports;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.UseCases.Reports
{
    public class UseCaseGetReportesAdministrador : IUseCaseGetReportesAdministrador
    {
        public async Task<(bool IsSuccess, string Message, Reportes? Data)> Execute(string? email, int idUsuario)
        {
            try
            {
                
                return (true, "Reportes obtenidos correctamente", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
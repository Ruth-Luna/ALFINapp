using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.Interfaces.Excel;

namespace ALFINapp.Application.UseCases.Excel
{
    public class UseCaseDescargarInforme : IUseCaseDescargarInforme
    {
        public async Task<(bool IsSuccess, string Message, object? Data)> exec(DateTime fechaInicio, DateTime fechaFin, int idSupervisor)
        {
            try
            {
                
                return (true, "Non Implemented", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
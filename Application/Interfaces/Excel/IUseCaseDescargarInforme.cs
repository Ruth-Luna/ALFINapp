using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Interfaces.Excel
{
    public interface IUseCaseDescargarInforme
    {
        public Task <(bool IsSuccess, string Message, object? Data)> exec(DateTime fechaInicio, DateTime fechaFin, int idSupervisor);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Leads
{
    public interface IUseCaseGetOrderingLeadsGeneral
    {
        public Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(int usuarioId, string filter, int paginaInicio, int paginaFinal);
    }
}
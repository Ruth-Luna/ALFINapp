using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Leads
{
    public interface IUseCaseGetFilterLeadsGeneral
    {
        public Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(
            int idusuario,
            string filter,
            string searchfield,
            int paginaInicio = 0,
            int paginaFinal = 1);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Leads
{
    public interface IUseCaseGetFilterLeadsGeneral
    {
        public Task<(bool IsSuccess, string Message, List<ViewClienteDetalles> Data)> Execute(
            int idusuario,
            int paginaInicio,
            int paginaFinal,
            string filter,
            string searchfield);
    }
}
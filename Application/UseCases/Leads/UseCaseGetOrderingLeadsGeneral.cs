using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Leads;

namespace ALFINapp.Application.UseCases.Leads
{
    public class UseCaseGetOrderingLeadsGeneral : IUseCaseGetOrderingLeadsGeneral
    {
        public async Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(int usuarioId, string filter, int paginaInicio, int paginaFinal)
        {
            try
            {
                var filtrosValidos = new HashSet<string> { "nombres", "campana", 
                    "oferta", "comentario", "tipificacion", "dni", "fecha" };
                return (true, "Non Implemented", new ViewGestionLeads());
            }
            catch (System.Exception ex)
            {
                return (false, "Error al obtener los clientes", new ViewGestionLeads());
            }
        }
    }
}
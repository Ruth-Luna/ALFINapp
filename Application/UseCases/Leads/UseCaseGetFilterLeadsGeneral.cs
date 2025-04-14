using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Leads;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Leads
{
    public class UseCaseGetFilterLeadsGeneral : IUseCaseGetFilterLeadsGeneral
    {
        private readonly IRepositoryVendedor _repositoryVendedor;
        public UseCaseGetFilterLeadsGeneral(IRepositoryVendedor repositoryVendedor)
        {
            _repositoryVendedor = repositoryVendedor;
        }
        public async Task<(bool IsSuccess, string Message, List<ViewClienteDetalles> Data)> Execute(
            string filter, 
            string searchfield)
        {
            try
            {
                if (searchfield != "nombre" || searchfield != "campana" || searchfield != "dni")
                {
                    return (false, "Se ha enviado un filtro invalido", new List<ViewClienteDetalles>());
                }
                var clientes = await _repositoryVendedor.GetClientesFiltradoPaginadoFromVendedor()
                return (true, "Non Implemented", new List<ViewClienteDetalles>());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new List<ViewClienteDetalles>());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Leads;
using ALFINapp.Domain.Entities;
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
        public async Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(
            int idusuario, 
            string filter, 
            string searchfield,
            int paginaInicio = 0, 
            int paginaFinal = 1)
        {
            try
            {
                if (searchfield != "nombre" || searchfield != "campana" || searchfield != "dni")
                {
                    return (false, "Se ha enviado un filtro invalido", new ViewGestionLeads());
                }
                var clientes = await _repositoryVendedor.GetClientesFiltradoPaginadoFromVendedor(
                    idusuario,
                    filter,
                    searchfield,
                    paginaInicio,
                    paginaFinal);
                var clientesView = clientes.Select(c => c.DtoToCliente()).ToList();
                var view = new ViewGestionLeads 
                {
                    ClientesA365 = clientesView,
                };
                
                return (true, "Se encontraron los siguiente clientes", view);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewGestionLeads());
            }
        }
    }
}
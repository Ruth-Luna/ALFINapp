using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
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
                var filtrosValidos = new HashSet<string> { "nombres", "campana", 
                    "oferta", "comentario", "tipificacion", "dni", "fecha" };

                if (!filtrosValidos.Contains(filter))
                {
                    return (false, "El filtro no es válido", new ViewGestionLeads());
                }
                if (string.IsNullOrWhiteSpace(searchfield))
                {
                    return (false, "El campo de búsqueda no puede estar vacío", new ViewGestionLeads());
                }
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                var usuario = await _repositoryVendedor.GetVendedor(idusuario);
                if (usuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewGestionLeads());
                }
                var usuarioDTO = new DetallesUsuarioDTO(usuario);
                var cantidades = await _repositoryVendedor.GetCantidadClientesGeneralTotalFromVendedor (idusuario);
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
                    Vendedor = usuarioDTO.ToEntityVendedor(),
                    ClientesAlfin = new List<Cliente>(),
                    clientesTotal = cantidades.Total,
                    clientesPendientes = cantidades.Pendientes,
                    clientesTipificados = cantidades.Tipificados,
                    filtro = filter,
                    searchfield = searchfield,
                };
                return (true, "Se encontraron los siguientes clientes", view);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewGestionLeads());
            }
        }
    }
}
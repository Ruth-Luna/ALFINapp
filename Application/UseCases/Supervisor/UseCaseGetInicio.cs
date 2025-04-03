using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Supervisor;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Supervisor
{
    public class UseCaseGetInicio : IUseCaseGetInicio
    {
        private readonly IRepositorySupervisor _repositorySupervisor;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetInicio(
            IRepositorySupervisor repositorySupervisor,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositorySupervisor = repositorySupervisor;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool IsSuccess, string Message, ViewInicioSupervisor Data)> Execute(int idUsuario)
        {
            try
            {
                var usuario = await _repositoryUsuarios.GetUser(idUsuario);
                if (usuario == null)
                {
                    return (false, "No se encontraron datos para el usuario.", new ViewInicioSupervisor());
                }
                var supervisorData = await _repositorySupervisor.GetInicioSupervisor(idUsuario);
                if (supervisorData == null)
                {
                    return (true, "No se encontraron datos para el supervisor.", new ViewInicioSupervisor());
                }
                int clientesPendientesSupervisor = supervisorData.DetallesClientes.Count(cliente => cliente.idUsuarioV == null);
                int totalClientes = supervisorData.DetallesClientes.Count();
                int clientesAsignadosSupervisor = supervisorData.DetallesClientes.Count(cliente => cliente.idUsuarioV != null);
                
                var DestinoBases = supervisorData.DetallesClientes
                                    .Where(ca => ca.Destino != null)
                                    .Select(ca => ca.Destino)
                                    .Distinct()
                                    .ToList();
                var supervisorList = new List<ViewClienteSupervisor>();
                foreach (var cliente in supervisorData.DetallesClientes)
                {
                    supervisorList.Add(cliente.toView());
                }

                var supervisorDataFinal = new ViewInicioSupervisor
                {
                    DetallesClientes = supervisorList.Take(300).ToList(),
                    nombreSupervisor = $"{usuario.NombresCompletos}",
                    clientesPendientesSupervisor = clientesPendientesSupervisor,
                    clientesAsignadosSupervisor = clientesAsignadosSupervisor,
                    totalClientes = totalClientes,
                    DestinoBases = DestinoBases.Where(destino => destino != null).Cast<string>().ToList()
                };

                return (true, "Correcto", supervisorDataFinal);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewInicioSupervisor());
            }
        }
    }
}
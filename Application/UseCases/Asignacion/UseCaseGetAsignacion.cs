using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Asignacion
{
    public class UseCaseGetAsignacion : IUseCaseGetAsignacion
    {
        private readonly IRepositorySupervisor _repositorySupervisor;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetAsignacion(
            IRepositorySupervisor repositorySupervisor,
            IRepositoryUsuarios repositoryUsuarios)
        {
            _repositorySupervisor = repositorySupervisor;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool IsSuccess, string Message, ViewAsignacionSupervisor Data)> Execute(int idUsuario)
        {
            try
            {
                var GetAsesoresAsignados = await _repositoryUsuarios.GetAllAsesoresBySupervisor(idUsuario);
                if (GetAsesoresAsignados == null)
                {
                    return (true, "No se encontraron asesores asignados.", new ViewAsignacionSupervisor());
                }
                var supervisorData = await _repositorySupervisor.GetContadorAllAsignacionesFromVendedor(GetAsesoresAsignados.Select(x => x.IdUsuario).ToList(), idUsuario);
                
                int totalClientes = supervisorData.DetallesClientesAsignados.Count();
                int clientesPendientesSupervisor = supervisorData.DetallesClientesAsignados.Count(cliente => cliente.IdUsuarioV == null);
                int clientesAsignadosSupervisor = supervisorData.DetallesClientesAsignados.Count(cliente => cliente.IdUsuarioV != null);

                var asesores = supervisorData.DetallesAsignacionContadorFromVendedor
                    .Select(x => new ViewAsignacionAsesor
                    {
                        NombresCompletos = x.NombresCompletos,
                        IdUsuario = x.IdUsuario,
                        NumeroClientes = x.NumeroClientes,
                        NumeroClientesGestionados = x.NumeroClientesGestionados,
                        NumeroClientesPendientes = x.NumeroClientesPendientes,
                        estaActivado = x.estaActivado
                    })
                    .ToList();

                var asignacionSupervisor = new ViewAsignacionSupervisor
                {
                    Asesores = asesores,
                    TotalClientes = totalClientes,
                    TotalClientesAsignados = clientesAsignadosSupervisor,
                    TotalClientesPendientes = clientesPendientesSupervisor,
                    Destinos = supervisorData.Destinos
                };
                return (true, "Consulta correcta", asignacionSupervisor);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al obtener la asignación: {ex.Message}", new ViewAsignacionSupervisor());
            }
        }
    }
}
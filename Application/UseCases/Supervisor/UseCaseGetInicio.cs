using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Supervisor;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Supervisor
{
    public class UseCaseGetInicio : IUseCaseGetInicio
    {
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetInicio(
            IRepositoryUsuarios repositoryUsuarios)
        {
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
                var supervisorData = new DetallesUsuarioDTO(usuario).ToEntitySupervisor();
                var supervisorDataFinal = new ViewInicioSupervisor
                {
                    Supervisor = supervisorData,
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
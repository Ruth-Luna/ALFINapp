using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Perfil;

namespace ALFINapp.Application.UseCases.Perfil
{
    public class UseCaseGetPerfil : IUseCaseGetPerfil
    {
        private readonly ALFINapp.Domain.Interfaces.IRepositoryUsuarios _repositoryUsuarios;
        public UseCaseGetPerfil(ALFINapp.Domain.Interfaces.IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<(bool success, string message, ViewUsuario data)> exec(int idUsuario)
        {
            try
            {
                var user = await _repositoryUsuarios.GetUser(idUsuario);
                if (user == null)
                {
                    return (false, "No se ha encontrado el usuario.", new ViewUsuario());
                }
                var userDto = new DetallesUsuarioDTO(user);
                return (true, "OK", userDto.ToView());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewUsuario());
            }
        }
    }
}
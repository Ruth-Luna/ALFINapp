using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.Interfaces.Email;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Email
{
    public class UseCaseRegisterEmail : IUseCaseRegisterEmail
    {
        private readonly IRepositoryUsuarios _repositoryUsuarios;

        public UseCaseRegisterEmail(IRepositoryUsuarios repositoryUsuarios)
        {
            _repositoryUsuarios = repositoryUsuarios;
        }

        public async Task<(bool IsSuccess, string Message)> Execute(string? email, int idUsuario)
        {
            try
            {
                var result = await _repositoryUsuarios.RegisterEmail(email, idUsuario);
                if (!result)
                {
                    return (false, "Error al registrar el email");
                }
                return (true, "Email registrado correctamente");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }
    }
}
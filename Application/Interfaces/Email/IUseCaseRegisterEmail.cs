using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Interfaces.Email
{
    public interface IUseCaseRegisterEmail
    {
        public Task<(bool IsSuccess, string Message)> Execute(string? email, int idUsuario);
    }
}
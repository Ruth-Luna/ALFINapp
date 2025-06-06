using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Perfil
{
    public interface IUseCaseGetPerfil
    {
        public Task<(bool success, string message, ViewUsuario data)> exec(int idUsuario);
    }
}
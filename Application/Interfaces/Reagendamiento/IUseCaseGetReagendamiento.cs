using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Reagendamiento
{
    public interface IUseCaseGetReagendamiento
    {
        public Task<(bool IsSuccess, string Message, ViewReagendamiento Data)> exec(
            int IdDerivacion);
    }
}
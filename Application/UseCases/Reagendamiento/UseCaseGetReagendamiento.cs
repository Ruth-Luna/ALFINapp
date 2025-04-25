using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reagendamiento;

namespace ALFINapp.Application.UseCases.Reagendamiento
{
    public class UseCaseGetReagendamiento : IUseCaseGetReagendamiento
    {
        public async Task<(bool IsSuccess, string Message, ViewReagendamiento Data)> exec(int IdDerivacion)
        {
            try
            {
                
                return (true, "Non Implemented", new ViewReagendamiento());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewReagendamiento());
            }
        }
    }
}
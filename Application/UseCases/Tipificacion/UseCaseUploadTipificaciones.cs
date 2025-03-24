using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.Interfaces.Tipificacion;

namespace ALFINapp.Application.UseCases.Tipificacion
{
    public class UseCaseUploadTipificaciones : IUseCaseUploadTipificaciones
    {
        public async Task<(bool, string)> execute()
        {
            try
            {
                return (false, "No implementado");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
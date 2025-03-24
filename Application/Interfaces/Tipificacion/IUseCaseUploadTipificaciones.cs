using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Interfaces.Tipificacion
{
    public interface IUseCaseUploadTipificaciones
    {
        public Task<(bool, string)> execute();
    }
}
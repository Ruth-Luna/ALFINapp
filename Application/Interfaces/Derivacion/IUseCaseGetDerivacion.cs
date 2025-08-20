using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;

namespace ALFINapp.Application.Interfaces.Derivacion
{
    public interface IUseCaseGetDerivacion
    {
        public Task<(bool success, string message, ViewDerivacionesVistaGeneral data)> Execute(
            int idUsuario,
            int idRol);
    }
}
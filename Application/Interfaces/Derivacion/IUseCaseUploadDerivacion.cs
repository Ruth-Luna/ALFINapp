using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Interfaces.Derivacion
{
    public interface IUseCaseUploadDerivacion
    {
        public Task<(bool success, string message)> Execute(
            string agenciaComercial, 
            DateTime FechaVisita, 
            string Telefono, 
            int idBase, 
            int idUsuario,
            int idAsignacion,
            int type);
    }
}
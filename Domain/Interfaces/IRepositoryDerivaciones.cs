using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryDerivaciones
    {
        public Task<List<DerivacionesAsesores>?> getDerivaciones(int idCliente, string docAsesor);
        public Task<GESTIONDETALLE?> getGestionDerivacion(string docCliente, string docAsesor);
        public Task<bool> uploadDerivacion(DerivacionesAsesores derivacion);
        public Task<(bool success, string message)> uploadNuevaDerivacion(
            DerivacionesAsesores derivacion, 
            int idBase,
            int idUsuario);
        public Task<(bool success, string message)> verDerivacion(string Dni);
        public Task<(bool success, string message)> verDisponibilidad(string DniCliente, string DniAsesor);
        public Task<(bool success, string message)> verDisponibilidad(int idBase);
    }
}
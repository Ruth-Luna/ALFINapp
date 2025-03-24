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
    }
}
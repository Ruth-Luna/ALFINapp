using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryTelefonos
    {
        public Task<TelefonosAgregados?> GetTelefono(string telefono, int IdCliente);
        public Task<bool> UpdateTelefono(TelefonosAgregados telefono);
    }
}
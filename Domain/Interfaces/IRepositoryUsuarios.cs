using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryUsuarios
    {
        public Task<bool> RegisterEmail(string? email, int idUsuario);
        public Task<bool> RegisterPassword(string password, int idUsuario);
    }
}
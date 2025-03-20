using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Vendedor
{
    public interface IUseCaseGetInicio
    {
        public Task<(bool IsSuccess, string Message, ViewInicioVendedor? Data)> Execute(int idUsuario);
    }
}
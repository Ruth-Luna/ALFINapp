using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryAsignaciones
    {
        public Task<(bool IsSuccess, string Message)> CrossAssignmentsAsync(DetallesAssignmentsMasive clientes);
        public Task<(bool IsSuccess, string Message, string NombreLista)> CreateListName(string dni_supervisor);
    }
}
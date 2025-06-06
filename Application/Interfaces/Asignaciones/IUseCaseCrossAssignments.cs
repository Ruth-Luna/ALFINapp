using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.Interfaces.Asignaciones
{
    public interface IUseCaseCrossAssignments
    {
        public Task<(bool IsSuccess, string Message, List<Cliente> Data)> Execute(DetallesAssignmentsMasive clientes);
    }
}
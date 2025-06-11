using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryAsignaciones
    {
        public Task<(bool IsSuccess, string Message)> CrossAssignments(DetallesAssignmentsMasive asignaciones);
        public Task<(bool IsSuccess, string Message, string NombreLista)> CreateListName(string dni_supervisor);
        public Task<(bool IsSuccess, string Message)> AssignLeads(string dni_supervisor, string nombre_lista);
    }
}
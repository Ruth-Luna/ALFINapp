using ALFINapp.Application.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryAsignaciones
    {
        public Task<(bool IsSuccess, string Message)> CrossAssignments(DetallesAssignmentsMasive asignaciones);
        public Task<(bool IsSuccess, string Message, string NombreLista)> CreateListName(string dni_supervisor);
        public Task<(bool IsSuccess, string Message, int numAsignaciones)> AssignLeads(string dni_supervisor, string nombre_lista);
        public Task<List<ClienteCruceDTO>> GetCrossed(int page = 1);
        public Task<(bool IsSuccess, string Message, List<DetallesAsignacionesDelSupDTO> asignaciones)> GetAllAssignmentsFromSupervisor();
        public Task<(bool IsSuccess, string Message, DetallesAsignacionesDescargaSupDTO asignaciones)> GetDetailedAssignmentsFromSupervisor(string nombre_lista, int page = -1);
    }
}
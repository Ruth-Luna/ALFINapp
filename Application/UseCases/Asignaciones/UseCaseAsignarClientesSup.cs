using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Asignaciones;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Asignaciones
{
    public class UseCaseAsignarClientesSup : IUseCaseAsignarClientesSup
    {
        private readonly IRepositoryAsignaciones _repositoryAsignaciones;
        public UseCaseAsignarClientesSup(IRepositoryAsignaciones repositoryAsignaciones)
        {
            _repositoryAsignaciones = repositoryAsignaciones ?? throw new ArgumentNullException(nameof(repositoryAsignaciones));
        }
        public async Task<(bool IsSuccess, string Message)> AsignarMasivoAsync(DetallesAssignmentsMasive clientes)
        {
            try
            {
                foreach (var supervisor in clientes.SupervisoresConClientes)
                {
                    if (supervisor.Clientes.Count == 0)
                    {
                        return (false, "No se pueden asignar clientes a un supervisor sin clientes.");
                    }
                    if (supervisor.DniSupervisor == null || string.IsNullOrWhiteSpace(supervisor.DniSupervisor))
                    {
                        return (false, "El DNI del supervisor no puede ser nulo o vacío.");
                    }
                    if (supervisor.NombreLista == null || string.IsNullOrWhiteSpace(supervisor.NombreLista))
                    {
                        return (false, "El nombre de la lista no puede ser nulo o vacío.");
                    }
                }
                foreach (var supervisor in clientes.SupervisoresConClientes)
                {
                    var assignLeadsResult = await _repositoryAsignaciones.AssignLeads(supervisor.DniSupervisor
                        ?? string.Empty, supervisor.NombreLista ?? string.Empty);
                    if (!assignLeadsResult.IsSuccess)
                    {
                        return (false, assignLeadsResult.Message);
                    }
                }
                return (true, "Clientes asignados correctamente.");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al asignar clientes: {ex.Message}");
            }
        }
    }
}
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Asignaciones;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Asignaciones
{
    public class UseCaseCrossAssignments : IUseCaseCrossAssignments
    {
        private readonly IRepositoryAsignaciones _repositoryAsignaciones;
        public UseCaseCrossAssignments(IRepositoryAsignaciones repositoryAsignaciones)
        {
            _repositoryAsignaciones = repositoryAsignaciones;
        }
        public async Task<(bool IsSuccess, string Message, DetallesAssignmentsMasive? ClientesCruzados)> Execute(DetallesAssignmentsMasive clientes)
        {
            try
            {
                if (clientes == null || clientes.SupervisoresConClientes.Count == 0)
                {
                    return (false, "No se proporcionaron asignaciones para procesar.", null);
                }
                foreach (var supervisor in clientes.SupervisoresConClientes)
                {
                    if (string.IsNullOrEmpty(supervisor.DniSupervisor))
                    {
                        return (false, "Todos los supervisores deben tener un DNI válido.", null);
                    }
                    if (supervisor.DniSupervisor.Length > 8)
                    {
                        return (false, $"Hay un DNI de un supervisor que tiene mas de 8 caracteres {supervisor.DniSupervisor}.", null);
                    }
                    if (supervisor.DniSupervisor.Length != 8)
                    {
                        supervisor.DniSupervisor = supervisor.DniSupervisor.PadLeft(8, '0');
                    }
                }
                foreach (var cliente in clientes.SupervisoresConClientes.SelectMany(s => s.Clientes))
                {
                    if (string.IsNullOrEmpty(cliente.Dni))
                    {
                        return (false, "Todos los clientes deben tener un DNI válido.", null);
                    }
                    if (cliente.Dni.Length > 8)
                    {
                        return (false, "El DNI de un cliente no puede tener más de 8 caracteres.", null);
                    }
                    if (cliente.Dni.Length != 8)
                    {
                        cliente.Dni = cliente.Dni.PadLeft(8, '0');
                    }
                }
                var cross = await _repositoryAsignaciones.CrossAssignments(clientes);
                if (!cross.IsSuccess)
                {
                    return (false, cross.Message, null);
                }
                foreach (var supervisor in clientes.SupervisoresConClientes)
                {
                    if (string.IsNullOrEmpty(supervisor.DniSupervisor))
                    {
                        continue;
                    }
                    var listed = await _repositoryAsignaciones.CreateListName(supervisor.DniSupervisor);
                    if (!listed.IsSuccess)
                    {
                        return (false, listed.Message, null);
                    }
                    supervisor.NombreLista = listed.NombreLista;
                }
                return (true, "Asignaciones cruzadas procesadas correctamente", clientes);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
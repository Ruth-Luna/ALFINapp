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
        public async Task<(bool IsSuccess, string Message, List<Cliente> Data)> Execute(DetallesAssignmentsMasive clientes)
        {
            try
            {
                if (clientes == null || clientes.SupervisoresConClientes.Count == 0)
                {
                    return (false, "No se proporcionaron asignaciones para procesar.", new List<Cliente>());
                }
                // Validar que los clientes tengan un DNI válido
                foreach (var cliente in clientes.SupervisoresConClientes.SelectMany(s => s.Clientes))
                {
                    if (string.IsNullOrEmpty(cliente.Dni))
                    {
                        return (false, "Todos los clientes deben tener un DNI válido.", new List<Cliente>());
                    }
                    if (cliente.Dni.Length > 8)
                    {
                        return (false, "El DNI de un cliente no puede tener más de 8 caracteres.", new List<Cliente>());
                    }
                    if (cliente.Dni.Length != 8)
                    {
                        cliente.Dni = cliente.Dni.PadLeft(8, '0');
                    }
                }
                var cross = await _repositoryAsignaciones.CrossAssignments(clientes);
                if (!cross.IsSuccess)
                {
                    return (false, cross.Message, new List<Cliente>());
                }
                return (true, "Asignaciones cruzadas procesadas correctamente", clientes.SupervisoresConClientes.SelectMany(s => s.Clientes).ToList());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new List<Cliente>());
            }
        }
    }
}
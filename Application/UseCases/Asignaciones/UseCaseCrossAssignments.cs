using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Asignaciones;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.UseCases.Asignaciones
{
    public class UseCaseCrossAssignments : IUseCaseCrossAssignments
    {
        public async Task<(bool IsSuccess, string Message, List<Cliente> Data)> Execute(DetallesAssignmentsMasive clientes)
        {
            try
            {
                
                return (true, "Asignaciones cruzadas procesadas correctamente", clientes.SupervisoresConClientes.SelectMany(s => s.Clientes).ToList());
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new List<Cliente>());
            }
        }
    }
}
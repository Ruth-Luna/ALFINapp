using ALFINapp.Application.Interfaces.Referidos;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.UseCases.Referidos
{
    public class UseCaseReferirCliente : IUseCaseReferirCliente
    {
        public async Task<(bool IsSuccess, string Message)> Execute(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                {
                    return (false, "Cliente no puede ser nulo");
                }

                // Aquí se podría agregar la lógica para referir al cliente, como guardarlo en una base de datos o enviar una notificación.
                // Por ahora, simplemente retornamos un mensaje de éxito.

                if (cliente.FuenteBase == "DBA365")
                {
                    
                }
                else if (cliente.FuenteBase == "DBALFIN")
                {
                    
                }
                else
                {
                    return (false, "Fuente de referencia no válida");
                }
                return (true, "Cliente referido correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al referir cliente: {ex.Message}");
            }
        }
    }
}
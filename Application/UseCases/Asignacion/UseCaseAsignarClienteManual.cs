using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace ALFINapp.Application.UseCases.Asignacion
{
    public class UseCaseAsignarClienteManual : IUseCaseAsignarClienteManual
    {
        private readonly IRepositoryAsignacion _repositoryAsignacion;
        public UseCaseAsignarClienteManual(IRepositoryAsignacion repositoryAsignacion)
        {
            _repositoryAsignacion = repositoryAsignacion;
        }
        public async Task<(bool success, string message)> exec(string dniCliente, int IdUsuarioV, string baseTipo)
        {
            try
            {
                var result = await _repositoryAsignacion.AsignarClienteManual(dniCliente, baseTipo, IdUsuarioV);
                if (!result.success)
                {
                    return (false, result.message);
                }
                return (true, "Asignacion exitosa");
            }
            catch (SqlException ex)
            {
                string sqlMessage = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                Console.WriteLine($"Error SQL en la asignación manual de cliente: {sqlMessage}");
                return (false, sqlMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general en la asignación manual de cliente: {ex.Message}");
                return (false, "Ocurrió un error inesperado en la asignación manual de cliente.");
            }
        }
    }
}
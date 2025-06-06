using System.Data;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryAsignaciones : IRepositoryAsignaciones
    {
        public async Task<(bool IsSuccess, string Message, string NombreLista)> CreateListName(string dni_supervisor)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (System.Exception ex)
            {
                
                throw;
            }
        }

        public async Task<(bool IsSuccess, string Message)> CrossAssignmentsAsync(DetallesAssignmentsMasive clientes)
        {
            try
            {
                if (clientes == null || clientes.SupervisoresConClientes.Count == 0)
                {
                    return (false, "No se proporcionaron asignaciones para procesar.");
                }

                var asignaciones = new DataTable();
                asignaciones.Columns.Add("DNI_CLIENTE", typeof(string));
                asignaciones.Columns.Add("DNI_SUPERVISOR", typeof(string));
                asignaciones.Columns.Add("CELULAR_1", typeof(string));
                asignaciones.Columns.Add("CELULAR_2", typeof(string));
                asignaciones.Columns.Add("CELULAR_3", typeof(string));
                asignaciones.Columns.Add("CELULAR_4", typeof(string));
                asignaciones.Columns.Add("CELULAR_5", typeof(string));
                asignaciones.Columns.Add("DETALLE_BASE", typeof(string));

                return (true, "Asignaciones cruzadas procesadas correctamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
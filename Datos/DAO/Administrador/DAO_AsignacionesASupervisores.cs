using System.Data;
using ALFINapp.Application.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Administrador
{
    public class DAO_AsignacionesASupervisores
    {
        private readonly MDbContext _context;
        private DA_Usuario _dausuario = new DA_Usuario();
        private DAO_CruzarAsignaciones _daoCruzarAsignaciones;
        public DAO_AsignacionesASupervisores(
            MDbContext context,
            DAO_CruzarAsignaciones daoCruzarAsignaciones)
        {
            _context = context;
            _daoCruzarAsignaciones = daoCruzarAsignaciones;
        }
        
        public async Task<(bool IsSuccess, string Message, int numAsignaciones)> AssignLeads(string dni_supervisor, string nombre_lista)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DniSup", dni_supervisor),
                    new SqlParameter("@NombreLista", nombre_lista)
                };
                _context.Database.SetCommandTimeout(600);
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.SP_GESTION_ASIGNACION_AUTO_REFACTORIZADA @DniSup, @NombreLista",
                    parameters
                );
                return (true, "Clientes asignados correctamente.", result);
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al asignar clientes: {ex.Message}", 0);
            }
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
                var asignaciones = 0;
                string mensajeAsignacion = string.Empty;
                foreach (var supervisor in clientes.SupervisoresConClientes)
                {
                    var assignLeadsResult = await AssignLeads(supervisor.DniSupervisor
                        ?? string.Empty, supervisor.NombreLista ?? string.Empty);
                    if (!assignLeadsResult.IsSuccess)
                    {
                        return (false, assignLeadsResult.Message);
                    }
                    mensajeAsignacion = "El Supervisor " +
                        $"{supervisor.DniSupervisor} ha sido asignado a la lista {supervisor.NombreLista} " +
                        $"con {assignLeadsResult.numAsignaciones} leads." + mensajeAsignacion;
                    asignaciones += assignLeadsResult.numAsignaciones;
                }
                return (true, $"Se han asignado {asignaciones} Leads. {mensajeAsignacion}");
            }
            catch (System.Exception ex)
            {
                return (false, $"Error al asignar clientes: {ex.Message}");
            }
        }
    }
}
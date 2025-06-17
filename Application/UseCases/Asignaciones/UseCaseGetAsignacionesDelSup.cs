using ALFINapp.Application.Interfaces.Asignaciones;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Models;

namespace ALFINapp.Application.UseCases.Asignaciones
{
    public class UseCaseGetAsignacionesDelSup : IUseCaseGetAsignacionesDelSup
    {
        private readonly IRepositoryAsignaciones repositoryAsignaciones;
        public UseCaseGetAsignacionesDelSup(IRepositoryAsignaciones repositoryAsignaciones)
        {
            this.repositoryAsignaciones = repositoryAsignaciones;
        }
        async Task<(bool success, string message, ViewVerAsignacionesDelSupervisor data)> IUseCaseGetAsignacionesDelSup.exec()
        {
            try
            {
                var data = new ViewVerAsignacionesDelSupervisor();
                var result = await repositoryAsignaciones.GetAllAssignmentsFromSupervisor();
                if (result.IsSuccess)
                {
                    foreach (var asignacion in result.asignaciones)
                    {
                        data.asignaciones.Add(asignacion.toViewListas());
                    }
                    return (true, "Asignaciones obtenidas correctamente.", data);
                }
                else
                {
                    return (false, result.Message, new ViewVerAsignacionesDelSupervisor());
                }
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewVerAsignacionesDelSupervisor());
            }
        }
    }
}
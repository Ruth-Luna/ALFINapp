using ALFINapp.Application.Interfaces.Reagendamiento;
using ALFINapp.Domain.Interfaces;
using ALFINapp.DTOs;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ALFINapp.Application.UseCases.Reagendamiento
{
    public class UseCaseReagendar : IUseCaseReagendar
    {
        private readonly IRepositoryDerivaciones _repositoryDerivaciones;
        private readonly IRepositoryReagendacion _repositoryReagendacion;

        public UseCaseReagendar(IRepositoryDerivaciones repositoryDerivaciones,
            IRepositoryReagendacion repositoryReagendacion)
        {
            _repositoryReagendacion = repositoryReagendacion;
            _repositoryDerivaciones = repositoryDerivaciones;
        }

        public async Task<(bool IsSuccess, string Message)> exec(
            int IdDerivacion,
            DateTime FechaReagendamiento, List<string>? urls = null)
        {
            try
            {
                urls ??= new List<string>();
                var checkDis = await _repositoryReagendacion.checkDisReagendacion(IdDerivacion, FechaReagendamiento);
                if (!checkDis.success)
                {
                    return (false, checkDis.message);
                }

                // if (evidencias != null && evidencias.Count > 0)
                // {
                //     var uploadFiles = await _repositoryDerivaciones.uploadReagendacionConEvidencias(evidencias, IdDerivacion, FechaReagendamiento);
                //     if (!uploadFiles.success)
                //     {
                //         return (false, uploadFiles.message);
                //     }
                //     else
                //     {
                //         return (true, "Reagendamiento con evidencias realizado con éxito.");
                //     }
                // }
                
                // If no files to upload, proceed with the regular reagendamiento
                var reagendar = await _repositoryDerivaciones.uploadReagendacion(IdDerivacion, FechaReagendamiento, string.Join(",", urls));
                if (reagendar.success)
                {
                    return (true, "Reagendamiento realizado con éxito.");
                }
                else
                {
                    return (false, reagendar.message);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return (false, "An error occurred while processing your request.");
            }
        }
    }
}
using ALFINapp.Application.Interfaces.Reagendamiento;
using ALFINapp.Domain.Interfaces;

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

        public async Task<(bool IsSuccess, string Message)> exec(int IdDerivacion, DateTime FechaReagendamiento)
        {
            try
            {
                var checkDis = await _repositoryReagendacion.checkDisReagendacion(IdDerivacion, FechaReagendamiento);
                if (!checkDis.success)
                {
                    return (false, checkDis.message);
                }
                
                var reagendar = await _repositoryDerivaciones.uploadReagendacion(IdDerivacion, FechaReagendamiento);
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
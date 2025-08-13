using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reagendamiento;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Reagendamiento
{
    public class UseCaseGetReagendamiento : IUseCaseGetReagendamiento
    {
        private readonly IRepositoryDerivaciones _repositoryDerivaciones;
        public UseCaseGetReagendamiento(
            IRepositoryDerivaciones repositoryDerivaciones
            )
        {
            _repositoryDerivaciones = repositoryDerivaciones;
        }
        public async Task<(bool IsSuccess, string Message, ViewClienteReagendado Data)> exec(int IdDerivacion)
        {
            try
            {
                var derivacion = await _repositoryDerivaciones.getDerivacion(IdDerivacion);
                if (derivacion == null)
                {
                    return (false, "No se encontraron datos de la derivación", new ViewClienteReagendado());
                }
                var view = derivacion.ToViewClienteReagendado();
                return (true, "Datos del Reagendamiento que se enviaran", view);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewClienteReagendado());
            }
        }
    }
}
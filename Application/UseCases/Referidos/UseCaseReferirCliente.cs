using ALFINapp.Application.Interfaces.Consulta;
using ALFINapp.Application.Interfaces.Referidos;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Referidos
{
    public class UseCaseReferirCliente : IUseCaseReferirCliente
    {
        private readonly IRepositoryReferidos _repositoryReferidos;
        private readonly IUseCaseConsultaClienteDni _useCaseConsultaClienteDni;
        public UseCaseReferirCliente(
            IRepositoryReferidos repositoryReferidos,
            IUseCaseConsultaClienteDni useCaseConsultaClienteDni)
        {
            _repositoryReferidos = repositoryReferidos;
            _useCaseConsultaClienteDni = useCaseConsultaClienteDni;
        }
        public async Task<(bool IsSuccess, string Message)> Execute(Cliente cliente, Vendedor asesor)
        {
            try
            {
                if (cliente == null)
                {
                    return (false, "Cliente no puede ser nulo");
                }

                if (asesor == null)
                {
                    return (false, "Asesor no puede ser nulo");
                }

                var getReferido = await _useCaseConsultaClienteDni.Execute(cliente.Dni ?? string.Empty);
                if (getReferido.IsSuccess == false || getReferido.Data == null)
                {
                    return (false, getReferido.Message);
                }
                
                if (cliente.FuenteBase == "BDA365" || cliente.FuenteBase == "BDALFIN")
                {
                    var result = await _repositoryReferidos.ReferirCliente(cliente, asesor);
                    if (result.IsSuccess == false)
                    {
                        return (false, result.Message);
                    }
                }
                else
                {
                    return (false, "Fuente de referencia no válida");
                }
                var emailResult = await _repositoryReferidos.EnviarCorreoReferido(cliente.Dni ?? string.Empty);
                if (emailResult.IsSuccess == false)
                {
                    return (false, emailResult.Message);
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
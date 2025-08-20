using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Entities;
using ALFINapp.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryDerivaciones
    {
        public Task<List<DerivacionesAsesores>?> getDerivaciones(int idCliente, string docAsesor);
        public Task<List<DetallesDerivacionesAsesoresDTO>> getDerivaciones(List<Vendedor> asesores);
        public Task<GESTIONDETALLE?> getGestionDerivacion(string docCliente, string docAsesor);
        public Task<(bool success, string message)> uploadNuevaDerivacion(
            DerivacionesAsesores derivacion,
            int idBase,
            int idUsuario);
        public Task<(bool success, string message)> verDerivacion(string Dni);
        public Task<(bool success, string message)> verDisponibilidad(int idBase);
        public Task<DetallesDerivacionesAsesoresDTO?> getDerivacion(int idDer);
        public Task<(bool success, string message)> uploadReagendacion(int idDer, DateTime fechaReagendamiento, string urls);
        public Task<(bool success, string message)> uploadReagendacion(string dniCliente, DateTime fechaReagendamiento, string urls);
        public Task<(bool success, string message)> uploadReagendacionConEvidencias(List<DtoVUploadFiles> dtovuploadfiles, int idDerivacion, DateTime fechaReagendamiento);
        public Task<(bool success, string message)> marcarEvidenciaDisponible(int idDerivacion);
    }
}
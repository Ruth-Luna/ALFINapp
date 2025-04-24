using ALFINapp.API.DTOs;

namespace ALFINapp.Application.Interfaces.Reagendamiento
{
    public interface IUseCaseReagendar
    {
        public Task<(bool IsSuccess, string Message)> Reagendar(
            int idCliente,
            DateTime nuevaFechaVisita,
            string motivoReagendamiento,
            string nuevaAgencia,
            string nuevaOferta,
            List<(string Filtro, string Dato)> filtros);
    }
}
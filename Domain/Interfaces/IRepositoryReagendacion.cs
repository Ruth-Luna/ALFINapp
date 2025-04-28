namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryReagendacion
    {
        public Task<(bool success, string message)> checkDisReagendacion(int IdDerivacion, DateTime FechaReagendamiento);
    }
}
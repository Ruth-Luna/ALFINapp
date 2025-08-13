namespace ALFINapp.Application.Interfaces.Email
{
    public interface IUseCaseRegisterEmail
    {
        public Task<(bool IsSuccess, string Message)> Execute(string? email, int idUsuario);
    }
}
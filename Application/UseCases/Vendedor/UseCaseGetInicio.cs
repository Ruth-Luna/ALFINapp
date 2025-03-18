using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Vendedor;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;

namespace Application.UseCases.Vendedor
{
    public class UseCaseGetInicio : IUseCaseGetInicio
    {
        private readonly IRepositoryVendedor _repositoryVendedor;
        public UseCaseGetInicio(IRepositoryVendedor repositoryVendedor)
        {
            _repositoryVendedor = repositoryVendedor;
        }
        public async Task<(bool IsSuccess, string Message, InicioVendedor? Data)> Execute(int idUsuario)
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            var usuario = await _repositoryVendedor.GetVendedor(idUsuario);
            if (usuario == null)
            {
                return (false, "No se encontró el usuario", null);
            }

            var clientesA365 = _repositoryVendedor.GetClientesFromVendedor(idUsuario);
            var clientesAlfin = _repositoryVendedor.GetClientesAlfinFromVendedor(idUsuario);

            var convertClientesA365 = clientesA365!=null?clientesA365.Select(c => c.DtoToCliente()).ToList(): new List<Cliente>();
            var convertClientesAlfin = clientesAlfin!=null?clientesAlfin.Select(c => c.DtoToCliente()).ToList(): new List<Cliente>();

            int clientesPendientes = convertClientesA365.Count(dc =>
                (!dc.FechaTipificacionDeMayorPeso.HasValue ||
                (dc.FechaTipificacionDeMayorPeso.Value.Year != currentYear &&
                 dc.FechaTipificacionDeMayorPeso.Value.Month != currentMonth))) +
                convertClientesAlfin.Count(da =>
                (!da.FechaTipificacionDeMayorPeso.HasValue ||
                (da.FechaTipificacionDeMayorPeso.Value.Year != currentYear &&
                 da.FechaTipificacionDeMayorPeso.Value.Month != currentMonth)));

            int clientesTipificados = convertClientesA365.Count(dc =>
                dc.FechaTipificacionDeMayorPeso.HasValue &&
                dc.FechaTipificacionDeMayorPeso.Value.Year == currentYear &&
                dc.FechaTipificacionDeMayorPeso.Value.Month == currentMonth) +
                convertClientesAlfin.Count(da =>
                da.FechaTipificacionDeMayorPeso.HasValue &&
                da.FechaTipificacionDeMayorPeso.Value.Year == currentYear &&
                da.FechaTipificacionDeMayorPeso.Value.Month == currentMonth);

            int totalClientes = convertClientesA365.Count + convertClientesAlfin.Count;

            var convertVarInicio = new InicioVendedor
            {
                ClientesA365 = convertClientesA365,
                Vendedor = usuario,
                ClientesAlfin = convertClientesAlfin,
                clientesPendientes = clientesPendientes,
                clientesTipificados = clientesTipificados,
                clientesTotal = totalClientes
            };

            return (true, "Clientes encontrados", convertVarInicio);
        }
    }
}
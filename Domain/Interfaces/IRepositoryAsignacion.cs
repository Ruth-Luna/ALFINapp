using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryAsignacion
    {
        public Task<bool> AsignarClientesAsesor(DetallesAsignacionesDTO nuevaAsignacion);
        public Task<bool> AsignarClientesMasivoAsesor(List<DetallesAsignacionesDTO> nuevasAsignaciones);
        public Task<bool> AsignarClientesSupervisor(int idSupervisor, int idVendedor, string destino, int numClientes);
        public Task<(bool success, string message)> AsignarClienteManual(string dniCliente, string baseTipo, int idVendedor);
    }
}
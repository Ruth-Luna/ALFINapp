using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryAsignacion
    {
        public Task<bool> AsignarClientesMasivoAsesor(List<DetallesAsignacionesDTO> nuevasAsignaciones);
    }
}
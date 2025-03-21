using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Mappers
{
    public class MapperDerivaciones
    {
        public static Domain.Entities.Derivacion ToEntity(Infrastructure.Persistence.Models.DerivacionesAsesores model)
        {
            return new Domain.Entities.Derivacion
            {
                IdDerivacion = model.IdDerivacion,
                FechaDerivacion = model.FechaDerivacion,
                DniAsesor = model.DniAsesor,
                DniCliente = model.DniCliente,
                IdCliente = model.IdCliente,
                NombreCliente = model.NombreCliente,
                TelefonoCliente = model.TelefonoCliente,
                NombreAgencia = model.NombreAgencia,
                NumAgencia = model.NumAgencia,
                FueProcesado = model.FueProcesado,
                FechaVisita = model.FechaVisita,
                EstadoDerivacion = model.EstadoDerivacion,
                IdAsignacion = model.IdAsignacion,
                ObservacionDerivacion = model.ObservacionDerivacion,
                FueEnviadoEmail = model.FueEnviadoEmail
            };                
        }
    }
}
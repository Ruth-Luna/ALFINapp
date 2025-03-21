using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Application.Mappers
{
    public class MapperDesembolso
    {
        public static Domain.Entities.Desembolso ToEntity(Infrastructure.Persistence.Models.Desembolsos? model)
        {
            if (model == null) return new Domain.Entities.Desembolso();
            return new Domain.Entities.Desembolso
            {
                IdDesembolsos = model.IdDesembolsos,
                DniDesembolso = model.DniDesembolso,
                CuentaBT = model.CuentaBT,
                NOper = model.NOper,
                Sucursal = model.Sucursal,
                MontoFinanciado = model.MontoFinanciado,
                FechaSol = model.FechaSol,
                FechaDesembolsos = model.FechaDesembolsos,
                FechaGest = model.FechaGest,
                Canal = model.Canal,
                TipoDesem = model.TipoDesem,
                FechaProporcion = model.FechaProporcion,
                Observacion = model.Observacion,
                IdNombreBase = model.IdNombreBase
            };
        }
    }
}
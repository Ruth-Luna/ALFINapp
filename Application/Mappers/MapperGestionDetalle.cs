using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.Mappers
{
    public class MapperGestionDetalle
    {
        public static GestionDetalle ToEntity(GESTIONDETALLE? model)
        {
            if (model == null) return new GestionDetalle();
            return new GestionDetalle
            {
                IdFeedback = model.IdFeedback,
                IdAsignacion = model.IdAsignacion,
                CodCanal = model.CodCanal,
                Canal = model.Canal,
                DocCliente = model.DocCliente,
                FechaEnvio = model.FechaEnvio,
                FechaGestion = model.FechaGestion,
                HoraGestion = model.HoraGestion,
                Telefono = model.Telefono,
                OrigenTelefono = model.OrigenTelefono,
                CodCampaña = model.CodCampaña,
                CodTip = model.CodTip,
                Oferta = model.Oferta,
                DocAsesor = model.DocAsesor,
                Origen = model.Origen,
                ArchivoOrigen = model.ArchivoOrigen,
                FechaCarga = model.FechaCarga,
                IdDerivacion = model.IdDerivacion,
                IdSupervisor = model.IdSupervisor,
                Supervisor = model.Supervisor,
                IdDesembolso = model.IdDesembolso
            };
        }
    }
}
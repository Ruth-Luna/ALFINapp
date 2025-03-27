using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesGestionDetalleDTO
    {
        public int IdFeedback { get; set; }
        public int? IdAsignacion { get; set; }
        public string? CodCanal { get; set; }
        public string? Canal { get; set; }
        public string? DocCliente { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaGestion { get; set; }
        public TimeSpan? HoraGestion { get; set; }
        public string? Telefono { get; set; }
        public string? OrigenTelefono { get; set; }
        public string? CodCampaña { get; set; }
        public int CodTip { get; set; }
        public decimal Oferta { get; set; }
        public string? DocAsesor { get; set; }
        public string? Origen { get; set; }
        public string? ArchivoOrigen { get; set; }
        public DateTime? FechaCarga { get; set; }
        public int? IdDerivacion { get; set; }
        public int? IdSupervisor { get; set; }
        public string? Supervisor { get; set; }
        public DetallesGestionDetalleDTO(GESTIONDETALLE model)
        {
            IdFeedback = model.IdFeedback;
            IdAsignacion = model.IdAsignacion;
            CodCanal = model.CodCanal;
            Canal = model.Canal;
            DocCliente = model.DocCliente;
            FechaEnvio = model.FechaEnvio;
            FechaGestion = model.FechaGestion;
            HoraGestion = model.HoraGestion;
            Telefono = model.Telefono;
            OrigenTelefono = model.OrigenTelefono;
            CodCampaña = model.CodCampaña;
            CodTip = model.CodTip;
            Oferta = model.Oferta;
            DocAsesor = model.DocAsesor;
            Origen = model.Origen;
            ArchivoOrigen = model.ArchivoOrigen;
            FechaCarga = model.FechaCarga;
            IdDerivacion = model.IdDerivacion;
            IdSupervisor = model.IdSupervisor;
            Supervisor = model.Supervisor;
        }
        public ViewGestionDetalle toView ()
        {
            return new ViewGestionDetalle
            {
                IdFeedback = IdFeedback,
                IdAsignacion = IdAsignacion,
                CodCanal = CodCanal,
                Canal = Canal,
                DocCliente = DocCliente,
                FechaEnvio = FechaEnvio,
                FechaGestion = FechaGestion,
                HoraGestion = HoraGestion,
                Telefono = Telefono,
                OrigenTelefono = OrigenTelefono,
                CodCampaña = CodCampaña,
                CodTip = CodTip,
                Oferta = Oferta,
                DocAsesor = DocAsesor,
                Origen = Origen,
                ArchivoOrigen = ArchivoOrigen,
                FechaCarga = FechaCarga,
                IdDerivacion = IdDerivacion,
                IdSupervisor = IdSupervisor,
                Supervisor = Supervisor
            };
        }
    }
}
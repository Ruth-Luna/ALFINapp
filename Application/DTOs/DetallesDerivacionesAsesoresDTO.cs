using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesDerivacionesAsesoresDTO
    {
        public int IdDerivacion { get; set; }
        public DateTime FechaDerivacion { get; set; }
        public string? DniAsesor { get; set; }
        public string? DniCliente { get; set; }
        public int? IdCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? TelefonoCliente { get; set; }
        public string? NombreAgencia { get; set; }
        public string? NumAgencia { get; set; }
        public bool? FueProcesado { get; set; }
        public DateTime? FechaVisita { get; set; }
        public string? EstadoDerivacion { get; set; }
        public int? IdAsignacion { get; set; }
        public string? ObservacionDerivacion { get; set; }
        public bool? FueEnviadoEmail { get; set; }
        public int? IdDesembolso { get; set; }
        public string? DocSupervisor { get; set; }
        public decimal? OfertaMax { get; set; }
        public string? Supervisor { get; set; }
        public decimal? MontoDesembolso { get; set; }
        public string? RealError { get; set; }
        public bool? PuedeSerReagendado { get; set; } = true;
        public DateTime? FechaEvidencia { get; set; } = null;
        public bool? HayEvidencia { get; set; } = false;
        public DetallesDerivacionesAsesoresDTO(DerivacionesAsesores model)
        {
            IdDerivacion = model.IdDerivacion;
            FechaDerivacion = model.FechaDerivacion;
            DniAsesor = model.DniAsesor;
            DniCliente = model.DniCliente;
            IdCliente = model.IdCliente;
            NombreCliente = model.NombreCliente;
            TelefonoCliente = model.TelefonoCliente;
            NombreAgencia = model.NombreAgencia;
            NumAgencia = model.NumAgencia;
            FueProcesado = model.FueProcesado;
            FechaVisita = model.FechaVisita;
            EstadoDerivacion = model.EstadoDerivacion;
            IdAsignacion = model.IdAsignacion;
            ObservacionDerivacion = model.ObservacionDerivacion;
            FueEnviadoEmail = model.FueEnviadoEmail;
            IdDesembolso = model.IdDesembolso;
            DocSupervisor = model.DocSupervisor;
            OfertaMax = model.OfertaMax;
            Supervisor = model.Supervisor;
            MontoDesembolso = model.MontoDesembolso;
            RealError = model.RealError;
        }

        public DetallesDerivacionesAsesoresDTO(DerivacionConsultaDerivacionesXAsesorPorDniConReagendacion model)
        {
            IdDerivacion = model.IdDerivacion;
            FechaDerivacion = model.FechaDerivacion;
            DniAsesor = model.DniAsesor;
            DniCliente = model.DniCliente;
            IdCliente = model.IdCliente;
            NombreCliente = model.NombreCliente;
            TelefonoCliente = model.TelefonoCliente;
            NombreAgencia = model.NombreAgencia;
            NumAgencia = model.NumAgencia;
            FueProcesado = model.FueProcesado;
            FechaVisita = model.FechaVisita;
            EstadoDerivacion = model.EstadoDerivacion;
            IdAsignacion = model.IdAsignacion;
            ObservacionDerivacion = model.ObservacionDerivacion;
            FueEnviadoEmail = model.FueEnviadoEmail;
            IdDesembolso = model.IdDesembolso;
            DocSupervisor = model.DocSupervisor;
            OfertaMax = model.OfertaMax;
            Supervisor = model.Supervisor;
            MontoDesembolso = model.MontoDesembolso;
            RealError = model.RealError;
            PuedeSerReagendado = model.PuedeSerReagendado;
            FechaEvidencia = model.FechaEvidencia;
            HayEvidencia = model.HayEvidencia;
        }
        public ViewDerivaciones ToViewDerivaciones ()
        {
            return new ViewDerivaciones
            {
                IdDerivacion = IdDerivacion,
                FechaDerivacion = FechaDerivacion,
                DniAsesor = DniAsesor ?? string.Empty,
                DniCliente = DniCliente ?? string.Empty,
                IdCliente = IdCliente ?? 0,
                NombreCliente = NombreCliente ?? string.Empty,
                TelefonoCliente = TelefonoCliente ?? string.Empty,
                NombreAgencia = NombreAgencia ?? string.Empty,
                NumAgencia = NumAgencia ?? string.Empty,
                FueProcesado = FueProcesado ?? false,
                FechaVisita = FechaVisita ?? DateTime.Now,
                EstadoDerivacion = EstadoDerivacion ?? string.Empty,
                IdAsignacion = IdAsignacion ?? 0,
                ObservacionDerivacion = ObservacionDerivacion ?? string.Empty,
                FueEnviadoEmail = FueEnviadoEmail ?? false,
                IdDesembolso = IdDesembolso ?? 0,
                DocSupervisor = DocSupervisor ?? string.Empty,
                OfertaMax = OfertaMax ?? 0,
                Supervisor = Supervisor ?? string.Empty,
                MontoDesembolso = MontoDesembolso ?? 0,
                RealError = RealError ?? string.Empty,
                PuedeSerReagendado = PuedeSerReagendado ?? true,
                FechaEvidencia = FechaEvidencia ?? null,
            };
        }
        public ViewClienteReagendado ToViewClienteReagendado()
        {
            return new ViewClienteReagendado
            {
                IdDerivacion = IdDerivacion,
                Dni = DniCliente,
                NombresCompletos = NombreCliente,
                Telefono = TelefonoCliente,
                OfertaMax = OfertaMax,
                AgenciaAsignada = NombreAgencia,
                FechaVisitaPrevia = FechaVisita,
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

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
        }
    }
}
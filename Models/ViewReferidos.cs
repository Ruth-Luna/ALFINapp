using ALFINapp.Datos.Persistence.Procedures;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ALFINapp.Models
{
    public class ViewReferidos
    {
        public int IdReferido { get; set; } = 0;
        public int? IdBaseClienteA365 { get; set; } = null;
        public int? IdBaseClienteBanco { get; set; } = null;
        public int? IdSupervisorReferido { get; set; } = null;
        public string? NombreCompletoAsesor { get; set; } = string.Empty;
        public string? NombreCompletoCliente { get; set; } = string.Empty;
        public string? DniAsesor { get; set; } = string.Empty;
        public string? DniCliente { get; set; } = string.Empty;
        public DateTime? FechaReferido { get; set; } = null;
        public string? TraidoDe { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty;
        public string? Agencia { get; set; } = string.Empty;
        public DateTime? FechaVisita { get; set; } = null;
        public decimal? OfertaEnviada { get; set; } = null;
        public bool? FueProcesado { get; set; } = null;
        public string? CelularAsesor { get; set; } = string.Empty;
        public string? CorreoAsesor { get; set; } = string.Empty;
        public string? CciAsesor { get; set; } = string.Empty;
        public string? DepartamentoAsesor { get; set; } = string.Empty;
        public string? UbigeoAsesor { get; set; } = string.Empty;
        public string? BancoAsesor { get; set; } = string.Empty;
        public string? EstadoReferencia { get; set; } = string.Empty;
        public string? EstadoDesembolso { get; set; } = string.Empty;
        public string? EstadoGeneral { get; set; } = string.Empty;
        public DateTime? FechaDesembolso { get; set; } = null;
        public ViewReferidos() { }
        public ViewReferidos(ReferidosGetReferidosPorDni model)
        {
            IdReferido = model.IdReferido;
            IdBaseClienteA365 = model.IdBaseClienteA365;
            IdBaseClienteBanco = model.IdBaseClienteBanco;
            IdSupervisorReferido = model.IdSupervisorReferido;
            NombreCompletoAsesor = model.NombreCompletoAsesor;
            NombreCompletoCliente = model.NombreCompletoCliente;
            DniAsesor = model.DniAsesor;
            DniCliente = model.DniCliente;
            FechaReferido = model.FechaReferido;
            TraidoDe = model.TraidoDe;
            Telefono = model.Telefono;
            Agencia = model.Agencia;
            FechaVisita = model.FechaVisita;
            OfertaEnviada = model.OfertaEnviada ?? 0;
            FueProcesado = model.FueProcesado;
            CelularAsesor = model.CelularAsesor;
            CorreoAsesor = model.CorreoAsesor;
            CciAsesor = model.CciAsesor;
            DepartamentoAsesor = model.DepartamentoAsesor;
            UbigeoAsesor = model.UbigeoAsesor;
            BancoAsesor = model.BancoAsesor;
            EstadoReferencia = model.EstadoReferencia;
            EstadoDesembolso = model.EstadoDesembolso;
            EstadoGeneral = model.EstadoReferencia;
            FechaDesembolso = model.FechaDesembolso;
        }
        public ViewReferidos(ClientesReferidos model)
        {
            IdReferido = model.IdReferido;
            IdBaseClienteA365 = model.IdBaseClienteA365;
            IdBaseClienteBanco = model.IdBaseClienteBanco;
            IdSupervisorReferido = model.IdSupervisorReferido;
            NombreCompletoAsesor = model.NombreCompletoAsesor;
            NombreCompletoCliente = model.NombreCompletoCliente;
            DniAsesor = model.DniAsesor;
            DniCliente = model.DniCliente;
            FechaReferido = model.FechaReferido;
            TraidoDe = model.TraidoDe;
            Telefono = model.Telefono;
            Agencia = model.Agencia;
            FechaVisita = model.FechaVisita;
            OfertaEnviada = model.OfertaEnviada ?? 0;
            FueProcesado = model.FueProcesado;
            CelularAsesor = model.CelularAsesor;
            CorreoAsesor = model.CorreoAsesor;
            CciAsesor = model.CciAsesor;
            DepartamentoAsesor = model.DepartamentoAsesor;
            UbigeoAsesor = model.UbigeoAsesor;
            BancoAsesor = model.BancoAsesor;
            EstadoReferencia = model.EstadoReferencia;
            EstadoGeneral = model.EstadoReferencia;
        }
    }
}
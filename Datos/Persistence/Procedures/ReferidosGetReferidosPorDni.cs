using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Datos.Persistence.Procedures
{
    public class ReferidosGetReferidosPorDni
    {
        [Column("IdReferido")]
        public int IdReferido { get; set; }

        [Column("IdBaseClienteA365")]
        public int? IdBaseClienteA365 { get; set; }

        [Column("IdBaseClienteBanco")]
        public int? IdBaseClienteBanco { get; set; }

        [Column("IdSupervisorReferido")]
        public int? IdSupervisorReferido { get; set; }

        [Column("NombreCompletoAsesor")]
        public string NombreCompletoAsesor { get; set; } = string.Empty;

        [Column("NombreCompletoCliente")]
        public string NombreCompletoCliente { get; set; } = string.Empty;

        [Column("DniAsesor")]
        public string DniAsesor { get; set; } = string.Empty;

        [Column("DniCliente")]
        public string DniCliente { get; set; } = string.Empty;

        [Column("FechaReferido")]
        public DateTime? FechaReferido { get; set; }

        [Column("TraidoDe")]
        public string TraidoDe { get; set; } = string.Empty;

        [Column("Telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Column("Agencia")]
        public string Agencia { get; set; } = string.Empty;

        [Column("FechaVisita")]
        public DateTime? FechaVisita { get; set; }

        [Column("OfertaEnviada")]
        public decimal? OfertaEnviada { get; set; }

        [Column("FueProcesado")]
        public bool? FueProcesado { get; set; }

        [Column("CelularAsesor")]
        public string CelularAsesor { get; set; } = string.Empty;

        [Column("CorreoAsesor")]
        public string CorreoAsesor { get; set; } = string.Empty;

        [Column("CciAsesor")]
        public string CciAsesor { get; set; } = string.Empty;

        [Column("DepartamentoAsesor")]
        public string DepartamentoAsesor { get; set; } = string.Empty;

        [Column("UbigeoAsesor")]
        public string UbigeoAsesor { get; set; } = string.Empty;

        [Column("BancoAsesor")]
        public string BancoAsesor { get; set; } = string.Empty;

        [Column("EstadoReferencia")]
        public string EstadoReferencia { get; set; } = string.Empty;

        [Column("EstadoDesembolso")]
        public string EstadoDesembolso { get; set; } = string.Empty;
        [Column("FECHA_DESEMBOLSOS")]
        public DateTime? FechaDesembolso { get; set; } = null;
    }
}

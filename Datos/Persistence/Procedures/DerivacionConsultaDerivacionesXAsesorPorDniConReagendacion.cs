using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class DerivacionConsultaDerivacionesXAsesorPorDniConReagendacion
    {
        [Column("id_derivacion")]
        public int IdDerivacion { get; set; }
        [Column("fecha_derivacion")]
        public DateTime FechaDerivacion { get; set; }
        [Column("dni_asesor")]
        public string? DniAsesor { get; set; }
        [Column("dni_cliente")]
        public string? DniCliente { get; set; }
        [Column("id_cliente")]
        public int? IdCliente { get; set; }
        [Column("nombre_cliente")]
        public string? NombreCliente { get; set; }
        [Column("telefono_cliente")]
        public string? TelefonoCliente { get; set; }
        [Column("nombre_agencia")]
        public string? NombreAgencia { get; set; }
        [Column("num_agencia")]
        public string? NumAgencia { get; set; }
        [Column("fue_procesado")]
        public bool? FueProcesado { get; set; }
        [Column("fecha_visita")]
        public DateTime? FechaVisita { get; set; }
        [Column("estado_derivacion")]
        public string? EstadoDerivacion { get; set; }
        [Column("id_asignacion")]
        public int? IdAsignacion { get; set; }
        [Column("observacion_derivacion")]
        public string? ObservacionDerivacion { get; set; }
        [Column("fue_enviado_email")]
        public bool? FueEnviadoEmail { get; set; }
        [Column("ID_DESEMBOLSO")]
        public int? IdDesembolso { get; set; }
        [Column("doc_supervisor")]
        public string? DocSupervisor { get; set; }
        [Column("oferta_max")]
        public decimal? OfertaMax { get; set; }
        [Column("supervisor")]
        public string? Supervisor { get; set; }
        [Column("monto_desembolso")]
        public decimal? MontoDesembolso { get; set; }
        [Column("real_error")]
        public string? RealError { get; set; }
        public bool? PuedeSerReagendado { get; set; }
        [Column("fecha_evidencia")]
        public DateTime? FechaEvidencia { get; set; } = null;
        [Column("hay_evidencias")]
        public bool? HayEvidencia { get; set; } = false;
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Datos.Persistence.Entities
{
    public class ReagendamientosGetReagendamientos
    {
        [Column("id_derivacion")]
        public int? IdDerivacion { get; set; }

        [Column("id_agendamientos_re")]
        public int? IdAgendamientosRe { get; set; }

        [Column("oferta")]
        public decimal Oferta { get; set; }

        [Column("fecha_visita")]
        public DateTime? FechaVisita { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("agencia")]
        public string? Agencia { get; set; }

        [Column("fecha_agendamiento")]
        public DateTime? FechaAgendamiento { get; set; }

        [Column("fecha_derivacion")]
        public DateTime? FechaDerivacion { get; set; }

        [Column("dni_asesor")]
        public string? DniAsesor { get; set; }

        [Column("dni_cliente")]
        public string? DniCliente { get; set; }

        [Column("puede_ser_reagendado")]
        public bool PuedeSerReagendado { get; set; }

        [Column("NOMBRE_ASESOR")]
        public string? NombreAsesor { get; set; }

        [Column("estado_derivacion")]
        public string? EstadoDerivacion { get; set; }

        [Column("fecha_derivacion_original")]
        public DateTime? FechaDerivacionOriginal { get; set; }

        [Column("doc_supervisor")]
        public string? DocSupervisor { get; set; }
        [Column("nombre_cliente")]
        public string? NombreCliente { get; set; }

        [Column("numero_reagendamiento")]
        public long? NumeroReagendamiento { get; set; }
        // NUEVAS COLUMNAS
        [Column("id_desembolsos")]
        public int? IdDesembolsos { get; set; }
        [Column("fue_desembolsado")]
        public bool FueDesembolsado { get; set; }
        [Column("fecha_desembolso")]
        public DateTime? FechaDesembolso { get; set; }
        [Column("monto_financiado")]
        public decimal? MontoFinanciado { get; set; }
        [Column("fue_enviado_email")]
        public bool FueEnviadoEmail { get; set; }
        [Column("fue_procesado_formulario")]
        public bool FueProcesadoFormulario { get; set; }
    }

    public class ReagendamientosGetReagendamientosHistorico
    {
        [Column("id_derivacion")]
        public int? IdDerivacion { get; set; }

        [Column("id_agendamientos_re")]
        public int? IdAgendamientosRe { get; set; }

        [Column("oferta")]
        public decimal Oferta { get; set; }

        [Column("fecha_visita")]
        public DateTime? FechaVisita { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("agencia")]
        public string? Agencia { get; set; }

        [Column("fecha_agendamiento")]
        public DateTime? FechaAgendamiento { get; set; }

        [Column("fecha_derivacion")]
        public DateTime? FechaDerivacion { get; set; }

        [Column("dni_asesor")]
        public string? DniAsesor { get; set; }

        [Column("dni_cliente")]
        public string? DniCliente { get; set; }
        [Column("total_reagendamientos")]
        public int? total_reagendamientos { get; set; }
        [Column("numero_reagendamiento")]
        public long? NumeroReagendamiento { get; set; }
        [Column("nombre_cliente")]
        public string? NombreCliente { get; set; }
        [Column("estado_derivacion")]
        public string? EstadoDerivacion { get; set; }
        [Column("nombre_asesor")]
        public string? NombreAsesor { get; set; }
        
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Datos.Persistence.Entities
{
    public class ReagendamientosGetReagendamientos
    {
        [Column("id_derivacion")]
        public int IdDerivacion { get; set; }

        [Column("id_agendamientos_re")]
        public int IdAgendamientosRe { get; set; }

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

        [Column("numero_reagendamiento")]
        public int NumeroReagendamiento { get; set; }
    }
}

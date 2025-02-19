using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Models
{
    public class DerivacionesAsesores
    {
        [Key]
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
        [Column("estado_derivacion")]
        public string? EstadoDerivacion { get; set; }
    }
}
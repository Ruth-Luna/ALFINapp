using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class ClientesReferidos
    {
        [Key]
        [Column("id_referido")]
        public int IdReferido { get; set; }
        [Column("id_base_cliente_a365")]
        public int? IdBaseClienteA365 { get; set; }
        [Column("id_base_cliente_banco")]
        public int? IdBaseClienteBanco { get; set; }
        [Column("id_supervisor_referido")]
        public int? IdSupervisorReferido { get; set; }
        [Column("nombre_completo_asesor")]
        public string? NombreCompletoAsesor { get; set; }
        [Column("nombre_completo_cliente")]
        public string? NombreCompletoCliente { get; set; }
        [Column("dni_asesor")]
        public string? DniAsesor { get; set; }
        [Column("dni_cliente")]
        public string? DniCliente { get; set; }
        [Column("fecha_referido")]
        public DateTime? FechaReferido { get; set; }
        [Column("traido_de")]
        public string? TraidoDe { get; set; }
        [Column("telefono_cliente")]
        public string? Telefono { get; set; }
        [Column("agencia_referido")]
        public string? Agencia { get; set; }
        [Column("fecha_visita_agencia")]
        public DateTime? FechaVisita { get; set; }
        [Column("oferta_enviada")]
        public decimal? OfertaEnviada { get; set; }
        [Column("fue_procesado")]
        public bool? FueProcesado { get; set; }
        [Column("celular_asesor")]
        public string? CelularAsesor { get; set; }
        [Column("correo_asesor")]
        public string? CorreoAsesor { get; set; }
        [Column("cci_asesor")]
        public string? CciAsesor { get; set; }
        [Column("departamento_asesor")]
        public string? DepartamentoAsesor { get; set; }
        [Column("ubigeo_asesor")]
        public string? UbigeoAsesor { get; set; }
        [Column("banco_asesor")]
        public string? BancoAsesor { get; set; }
    }
}
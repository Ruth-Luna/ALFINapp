using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class ClientesReferidosDTO
    {
        public int IdReferido { get; set; }
        public int? IdBaseClienteA365 { get; set; }
        public int? IdBaseClienteBanco { get; set; }
        public int? IdSupervisorReferido { get; set; }
        public string? NombreCompletoAsesor { get; set; }
        public string? NombreCompletoCliente { get; set; }
        public string? DniAsesor { get; set; }
        public string? DniCliente { get; set; }
        public DateTime? FechaReferido { get; set; }
        public string? TraidoDe { get; set; }
        public string? Telefono { get; set; }
        public string? Agencia { get; set; }
        public DateTime? FechaVisita { get; set; }
        public decimal? OfertaEnviada { get; set; }
        public bool? FueProcesado { get; set; }
        public string? CelularAsesor { get; set; }
        public string? CorreoAsesor { get; set; }
        public string? CciAsesor { get; set; }
        public string? DepartamentoAsesor { get; set; }
        public string? UbigeoAsesor { get; set; }
        public string? BancoAsesor { get; set; }
        public string? EstadoReferencia { get; set; }
        public string? EstadoDesembolso { get; set; }
        public string? EstadoGeneral { get; set; }
        public DateTime? FechaDesembolso { get; set; }
    }
}
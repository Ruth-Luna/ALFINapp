using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesClienteDTO
    {
        // Propiedades de la tabla clientes_asignados
        public int? IdAsignacion { get; set;}
        public int? IdCliente { get; set;}
        public int? idUsuarioV { get; set;}
        public DateTime? FechaAsignacionV { get;set;}
        public string? DniVendedor {get; set;}
        public string? Destino { get; set; }
        //Propiedades de la tabla base_clientes
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        //Propiedades de la tabla usuarios
        public string? NombresCompletos { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? UltimaTipificacion { get; set; }
        public string? TipificacionMasRelevante { get; set; }
        public DetallesClienteDTO (SupervisorGetInicioData model)
        {
            IdAsignacion = model.IdAsignacion;
            IdCliente = model.IdCliente;
            idUsuarioV = model.idUsuarioV;
            Destino = model.Destino;
            FechaAsignacionV = model.FechaAsignacionV;
            DniVendedor = model.DniVendedor;
            Dni = model.Dni;
            XAppaterno = model.XAppaterno;
            XApmaterno = model.XApmaterno;
            XNombre = model.XNombre;
            NombresCompletos = model.NombresCompletos;
            UltimaTipificacion = model.UltimaTipificacion;
            TipificacionMasRelevante = model.TipificacionMasRelevante;
        }
        public ViewClienteSupervisor toView ()
        {
            return new ViewClienteSupervisor
            {
                IdAsignacion = this.IdAsignacion,
                IdCliente = this.IdCliente,
                idUsuarioV = this.idUsuarioV,
                FechaAsignacionV = this.FechaAsignacionV,
                DniVendedor = this.DniVendedor,
                Dni = this.Dni,
                XAppaterno = this.XAppaterno ?? "",
                XApmaterno = this.XApmaterno ?? "",
                XNombre = this.XNombre ?? "",
                NombresCompletos = this.NombresCompletos ?? "",
                ApellidoPaterno = this.ApellidoPaterno,
                UltimaTipificacion = this.UltimaTipificacion,
                TipificacionMasRelevante = this.TipificacionMasRelevante
            };
        }
    }
}
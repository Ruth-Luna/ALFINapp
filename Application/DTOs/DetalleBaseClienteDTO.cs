using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetalleBaseClienteDTO
    {
        // Propiedades de la tabla BaseCliente
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public int? Edad { get; set; }
        //Propiedades de la tabla DetalleBase
        public decimal? OfertaMax { get; set; }
        public string? Campaña { get; set; }
        public decimal? Cuota { get; set; }
        public decimal? Oferta12m { get; set; }
        public decimal? Tasa12m { get; set; }
        public decimal? Tasa18m { get; set; }
        public decimal? Cuota18m { get; set; }
        public decimal? Oferta24m { get; set; }
        public decimal? Tasa24m { get; set; }
        public decimal? Cuota24m { get; set; }
        public decimal? Oferta36m { get; set; }
        public decimal? Tasa36m { get; set; }
        public decimal? Cuota36m { get; set; }
        //Detalles relevantes de Bse Cliente
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
        //Detalles relevantes de DetalleBase
        public string? Sucursal { get; set; }
        public string? AgenciaComercial { get; set; }
        public string? TipoCliente { get; set; }
        public string? ClienteNuevo { get; set; }
        public string? GrupoTasa { get; set; }
        public string? GrupoMonto { get; set; }
        public int? Propension { get; set; }
        public string? PrioridadSistema { get; set; }
        //Ids y demas NO MOSTRABLE
        public int? IdAsignacion { get; set; }
        public int? IdBase { get; set; }
        //Tabla clientes_asignados
        public DateTime? FechaAsignacionVendedor { get; set; }
        public bool FinalizarTipificacion { get; set; }
        public string? ComentarioGeneral { get; set; }
        public string? TipificacionDeMayorPeso { get; set; }
        public int? PesoTipificacionMayor { get; set; }
        public DateTime? FechaTipificacionDeMayorPeso { get; set; }
        public int? IdUsuarioV { get; set; }
        public string? DniVendedor { get; set; }
        public string? Destino { get; set; }
        public string? TraidoDe { get; set; }
        public string? UltimaTipificacion { get; set; }
        //Tabla Listas Asignacion
        public int? IdLista { get; set; }
        public string? NombreLista { get; set; }
        public DateTime? FechaCreacionLista { get; set; }
        public DetalleBaseClienteDTO() { }
        public DetalleBaseClienteDTO(SupervisorGetAsignacionLeads model)
        {
            Dni = model.Dni;
            XAppaterno = model.XAppaterno;
            XApmaterno = model.XApmaterno;
            XNombre = model.XNombre;
            IdBase = model.IdCliente;
            IdAsignacion = model.IdAsignacion;
            IdUsuarioV = model.idUsuarioV;
            FechaAsignacionVendedor = model.FechaAsignacionV;

            UltimaTipificacion = model.UltimaTipificacion;
            TipificacionDeMayorPeso = model.TipificacionMasRelevante;
            DniVendedor = model.DniVendedor;
            Destino = model.Destino;
            IdLista = model.IdLista;
            NombreLista = model.NombreLista;
            FechaCreacionLista = model.FechaCreacionLista;
        }
        public DetalleBaseClienteDTO(InicioDetallesClientesFromAsesor model)
        {
            Dni = model.Dni;
            XAppaterno = model.XAppaterno;
            XApmaterno = model.XApmaterno;
            XNombre = model.XNombre;
            OfertaMax = model.OfertaMax;
            Campaña = model.Campaña;
            IdBase = model.IdBase;
            IdAsignacion = model.IdAsignacion;
            FechaAsignacionVendedor = model.FechaAsignacionVendedor;

            ComentarioGeneral = model.ComentarioGeneral;
            TipificacionDeMayorPeso = model.TipificacionDeMayorPeso;
            PesoTipificacionMayor = model.PesoTipificacionMayor;
            FechaTipificacionDeMayorPeso = model.FechaTipificacionDeMayorPeso;
            PrioridadSistema = model.PrioridadSistema;
        }
        public DetalleBaseClienteDTO(LeadsGetClientesAsignadosGestionLeads model)
        {
            Dni = model.Dni;
            XAppaterno = model.XAppaterno;
            XApmaterno = model.XApmaterno;
            XNombre = model.XNombre;
            OfertaMax = model.OfertaMax;
            Campaña = model.Campaña;
            IdBase = model.IdBase;
            IdAsignacion = model.IdAsignacion;
            FechaAsignacionVendedor = model.FechaAsignacionVendedor;

            ComentarioGeneral = model.ComentarioGeneral;
            TipificacionDeMayorPeso = model.TipificacionDeMayorPeso;
            PesoTipificacionMayor = model.PesoTipificacionMayor;
            FechaTipificacionDeMayorPeso = model.FechaTipificacionDeMayorPeso;
            PrioridadSistema = model.PrioridadSistema;
            TraidoDe = model.TraidoDe;
        }
        public DetalleBaseClienteDTO(ClientesAsignado model)
        {
            IdAsignacion = model.IdAsignacion;
            IdBase = model.IdCliente;
            IdUsuarioV = model.IdUsuarioV;
            FechaAsignacionVendedor = model.FechaAsignacionVendedor;
            FinalizarTipificacion = model.FinalizarTipificacion;
            ComentarioGeneral = model.ComentarioGeneral;
            TipificacionDeMayorPeso = model.TipificacionMayorPeso;
            PesoTipificacionMayor = model.PesoTipificacionMayor;
            FechaTipificacionDeMayorPeso = model.FechaTipificacionMayorPeso;
        }
        public Cliente DtoToCliente ()
        {
            Cliente cliente = new Cliente();
            cliente.Dni = Dni;
            cliente.XAppaterno = XAppaterno;
            cliente.XApmaterno = XApmaterno;
            cliente.XNombre = XNombre;
            cliente.Edad = Edad;
            if (OfertaMax < 1000)
            {
                cliente.OfertaMax = OfertaMax * 100;
            }
            else
            {
                cliente.OfertaMax = OfertaMax;
            }
            cliente.Campaña = Campaña;
            cliente.Cuota = Cuota;
            cliente.Oferta12m = Oferta12m;
            cliente.Tasa12m = Tasa12m;
            cliente.Tasa18m = Tasa18m;
            cliente.Cuota18m = Cuota18m;
            cliente.Oferta24m = Oferta24m;
            cliente.Tasa24m = Tasa24m;
            cliente.Cuota24m = Cuota24m;
            cliente.Oferta36m = Oferta36m;
            cliente.Tasa36m = Tasa36m;
            cliente.Cuota36m = Cuota36m;
            cliente.Departamento = Departamento;
            cliente.Provincia = Provincia;
            cliente.Distrito = Distrito;
            cliente.Sucursal = Sucursal;
            cliente.AgenciaComercial = AgenciaComercial;
            cliente.TipoCliente = TipoCliente;
            cliente.ClienteNuevo = ClienteNuevo;
            cliente.GrupoTasa = GrupoTasa;
            cliente.GrupoMonto = GrupoMonto;
            cliente.Propension = Propension;
            cliente.PrioridadSistema = PrioridadSistema;
            cliente.IdAsignacion = IdAsignacion;
            cliente.IdBase = IdBase;
            cliente.FechaAsignacionVendedor = FechaAsignacionVendedor;
            cliente.FinalizarTipificacion = FinalizarTipificacion;
            cliente.ComentarioGeneral = ComentarioGeneral;
            cliente.TipificacionDeMayorPeso = TipificacionDeMayorPeso;
            cliente.PesoTipificacionMayor = PesoTipificacionMayor;
            cliente.FechaTipificacionDeMayorPeso = FechaTipificacionDeMayorPeso;
            cliente.TraidoDe = TraidoDe;
            cliente.IdLista = IdLista;
            cliente.NombreLista = NombreLista;
            cliente.FechaCreacionLista = FechaCreacionLista;
            return cliente;
        }
    }
}
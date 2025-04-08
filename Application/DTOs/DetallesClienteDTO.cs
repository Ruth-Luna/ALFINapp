using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Models;
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
        public string? ApellidoMaterno { get; set; }
        public string? Nombres { get; set; }
        public string? UltimaTipificacion { get; set; }
        public string? TipificacionMasRelevante { get; set; }

        //Propiedades de TABLAS PARA CONSULTA GENERAL
        public int? IdBase { get; set; }
        public string? ColorFinal { get; set; }
        public string? Color { get; set; }
        public string? Campaña { get; set; }
        public decimal? OfertaMax { get; set; }
        public int? Plazo { get; set; }
        public decimal? CapacidadMax { get; set; }
        public decimal? SaldoDiferencialReeng { get; set; }
        public string? ClienteNuevo { get; set; }
        public string? Deuda1 { get; set; }
        public string? Entidad1 { get; set; }
        public decimal? Tasa1 { get; set; } // Tasa1
        public decimal? Tasa2 { get; set; } // Tasa2
        public decimal? Tasa3 { get; set; }
        public decimal? Tasa4 { get; set; }
        public decimal? Tasa5 { get; set; }
        public decimal? Tasa6 { get; set; }
        public decimal? Tasa7 { get; set; } // Tasa7
        public string? GrupoTasa { get; set; }
        public string? Usuario { get; set; }
        public string? SegmentoUser { get; set; }
        public string? TraidoDe { get; set; }
        public string? UserV3 { get; set; }
        public int? FlagDeudaVOferta { get; set; }
        public string? PerfilRo { get; set; }
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
        public DetallesClienteDTO (DetalleBase model, BaseCliente model2)
        {
            Dni = model2.Dni;
            ApellidoPaterno = model2.XAppaterno;
            ApellidoMaterno = model2.XApmaterno;
            Nombres = model2.XNombre;

            ColorFinal = model.ColorFinal;
            Color = model.Color;
            Campaña = model.Campaña;
            OfertaMax = model.OfertaMax;
            Plazo = model.Plazo;
            CapacidadMax = model.CapacidadMax;
            SaldoDiferencialReeng = model.SaldoDiferencialReeng;
            ClienteNuevo = model.ClienteNuevo;
            Deuda1 = $"{model.Deuda1} - {model.Deuda2} - {model.Deuda3}";
            Entidad1 = $"{model.Entidad1} - {model.Entidad2} - {model.Entidad3}";
            Tasa1 = model.Tasa1;
            Tasa2 = model.Tasa2;
            Tasa3 = model.Tasa3;
            Tasa4 = model.Tasa4;
            Tasa5 = model.Tasa5;
            Tasa6 = model.Tasa6;
            Tasa7 = model.Tasa7;
            GrupoTasa = model.GrupoTasa;
            Usuario = model.Usuario;
            SegmentoUser = model.SegmentoUser;
            TraidoDe = "BDA365";
            IdBase = model.IdBase;
            UserV3 = model.UserV3;
            FlagDeudaVOferta = model.FlagDeudaVOferta;
            PerfilRo = model.PerfilRo;
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
        public ViewClienteDetalles toViewConsulta ()
        {
            return new ViewClienteDetalles
            {
                IdBase = this.IdBase,
                Dni = this.Dni,
                ColorFinal = this.ColorFinal,
                Color = this.Color,
                Campaña = this.Campaña,
                OfertaMax = this.OfertaMax,
                Plazo = this.Plazo,
                CapacidadMax = this.CapacidadMax,
                SaldoDiferencialReeng = this.SaldoDiferencialReeng,
                ClienteNuevo = this.ClienteNuevo,
                Deuda1 = this.Deuda1,
                Entidad1 = this.Entidad1,
                Tasa1 = this.Tasa1,
                Tasa2 = this.Tasa2,
                Tasa3 = this.Tasa3,
                Tasa4 = this.Tasa4,
                Tasa5 = this.Tasa5,
                Tasa6 = this.Tasa6,
                Tasa7 = this.Tasa7,
                GrupoTasa = this.GrupoTasa,
                Usuario = this.Usuario,
                SegmentoUser = this.SegmentoUser,
                TraidoDe = this.TraidoDe ?? "",
                ApellidoPaterno = this.ApellidoPaterno ?? "",
                ApellidoMaterno = this.ApellidoMaterno ?? "",
                Nombres = this.Nombres ?? "",
                UserV3 = this.UserV3 ?? "",
                FlagDeudaVOferta = this.FlagDeudaVOferta ?? 0,
                PerfilRo = this.PerfilRo ?? ""
            };
        }
        public DetallesClienteDTO (ConsultaObtenerCliente model)
        {
            Dni = model.Dni;
            ApellidoPaterno = model.ApellidoPaterno;
            ApellidoMaterno = model.ApellidoMaterno;
            Nombres = model.Nombres;

            ColorFinal = model.ColorFinal;
            Color = model.Color;
            Campaña = model.Campaña;
            OfertaMax = model.OfertaMax;
            Plazo = model.Plazo;
            CapacidadMax = model.CapacidadMax;
            SaldoDiferencialReeng = model.SaldoDiferencialReeng;
            ClienteNuevo = model.ClienteNuevo;
            Deuda1 = $"{model.Deuda1}";
            Entidad1 = $"{model.Entidad1}";
            Tasa1 = model.Tasa1;
            Tasa2 = model.Tasa2;
            Tasa3 = model.Tasa3;
            Tasa4 = model.Tasa4;
            Tasa5 = model.Tasa5;
            Tasa6 = model.Tasa6;
            Tasa7 = model.Tasa7;
            GrupoTasa = model.GrupoTasa;
            Usuario = model.Usuario;
            SegmentoUser = model.SegmentoUser;
            TraidoDe = TraidoDe;
            IdBase = model.IdBase;
            UserV3 = model.UserV3;
            FlagDeudaVOferta = model.FlagDeudaVOferta;
            PerfilRo = model.PerfilRo;
        }
    }
}
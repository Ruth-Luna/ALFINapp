using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;
namespace ALFINapp.API.Models
{
    public class ViewClienteDetalles
    {
        public int? IdBase { get; set; }
        public string? Dni { get; set; }
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
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Nombres { get; set; }
        public string? UserV3 { get; set; }
        public int? FlagDeudaVOferta { get; set; }
        public string? PerfilRo { get; set; }
        public int idrol { get; set; } = 0; // Default value for idrol
        public ViewClienteDetalles()
        { }
        public ViewClienteDetalles(ConsultaObtenerCliente model)
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
            TraidoDe = model.TraidoDe;
            IdBase = model.IdBase;
            UserV3 = model.UserV3;
            FlagDeudaVOferta = model.FlagDeudaVOferta;
            PerfilRo = model.PerfilRo;
        }
    }
}
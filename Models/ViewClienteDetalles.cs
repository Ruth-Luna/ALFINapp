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
            Dni = model.Dni ?? string.Empty;
            ApellidoPaterno = model.ApellidoPaterno ?? string.Empty;
            ApellidoMaterno = model.ApellidoMaterno ?? string.Empty;
            Nombres = model.Nombres ?? string.Empty;

            ColorFinal = model.ColorFinal ?? string.Empty;
            Color = model.Color ?? string.Empty;
            Campaña = model.Campaña ?? string.Empty;
            OfertaMax = model.OfertaMax ?? 0;
            Plazo = model.Plazo ?? 0;
            CapacidadMax = model.CapacidadMax ?? 0;
            SaldoDiferencialReeng = model.SaldoDiferencialReeng ?? 0;
            ClienteNuevo = model.ClienteNuevo ?? string.Empty;
            Deuda1 = $"{model.Deuda1}";
            Entidad1 = $"{model.Entidad1}";
            Tasa1 = model.Tasa1 ?? 0; // Tasa1
            Tasa2 = model.Tasa2 ?? 0; // Tasa2
            Tasa3 = model.Tasa3 ?? 0;
            Tasa4 = model.Tasa4 ?? 0;
            Tasa5 = model.Tasa5 ?? 0;
            Tasa6 = model.Tasa6 ?? 0;
            Tasa7 = model.Tasa7 ?? 0; // Tasa7
            GrupoTasa = model.GrupoTasa ?? string.Empty;
            Usuario = model.Usuario ?? string.Empty;
            SegmentoUser = model.SegmentoUser ?? string.Empty;
            TraidoDe = model.TraidoDe ?? string.Empty;
            IdBase = model.IdBase ?? 0;
            UserV3 = model.UserV3 ?? string.Empty;
            FlagDeudaVOferta = model.FlagDeudaVOferta ?? 0;
            PerfilRo = model.PerfilRo ?? string.Empty;
        }
    }
}
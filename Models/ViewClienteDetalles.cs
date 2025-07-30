using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace ALFINapp.API.Models
{
    public class ViewClienteDetalles
    {
        public int? IdBase { get; set; } = 0;
        public string? Dni { get; set; } = string.Empty;
        public string? ColorFinal { get; set; } = string.Empty;
        public string? Color { get; set; } = string.Empty;
        public string? Campaña { get; set; } = string.Empty;
        public decimal? OfertaMax { get; set; } = 0;
        public int? Plazo { get; set; } = 0;
        public decimal? CapacidadMax { get; set; } = 0;
        public decimal? SaldoDiferencialReeng { get; set; } = 0;
        public string? ClienteNuevo { get; set; } = string.Empty;
        public string? Deuda1 { get; set; } = string.Empty; // Deuda1
        public string? Entidad1 { get; set; } = string.Empty; // Entidad1
        public decimal? Tasa1 { get; set; } = 0; // Tasa1
        public decimal? Tasa2 { get; set; } = 0; // Tasa2
        public decimal? Tasa3 { get; set; } = 0;
        public decimal? Tasa4 { get; set; } = 0;
        public decimal? Tasa5 { get; set; } = 0;
        public decimal? Tasa6 { get; set; } = 0;
        public decimal? Tasa7 { get; set; } = 0; // Tasa7
        public string? GrupoTasa { get; set; } = string.Empty;
        public string? Usuario { get; set; } = string.Empty;
        public string? SegmentoUser { get; set; } = string.Empty;
        public string? TraidoDe { get; set; } = string.Empty;
        public string? ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; } = string.Empty;
        public string? Nombres { get; set; } = string.Empty;
        public string? UserV3 { get; set; } = string.Empty;
        public int? FlagDeudaVOferta { get; set; } = 0;
        public string? PerfilRo { get; set; } = string.Empty;
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
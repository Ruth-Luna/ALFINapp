using ALFINapp.API.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ALFINapp.API.Models
{
    public class ViewReagendamiento
    {
        public List<ViewClienteReagendado> Clientes { get; set; } = new List<ViewClienteReagendado>();
        public List<DtoVReagendarFiltros> Filtros { get; set; } = new List<DtoVReagendarFiltros>();
    }
    public class ViewClienteReagendado
    {
        public int IdDerivacion { get; set; }
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public string? MejorCampaña { get; set; }
        public string? CampañaPrevia { get; set; }
        public decimal? OfertaMax { get; set; }
        public decimal? OfertaPrevia { get; set; }
        public string? AgenciaAsignada { get; set; }
        public string? AgenciaPrevia { get; set; }
        public DateTime? FechaVisitaPrevia { get; set; }
        public List<ViewOfertasCampanasAgencia>? OfertasCampanasAgenciaDisponibles { get; set; } = new List<ViewOfertasCampanasAgencia>();
    }
    public class ViewOfertasCampanasAgencia
    {
        public decimal? Oferta { get; set; }
        public string? Campana { get; set; }
        public string? Agencia { get; set; }
    }
}
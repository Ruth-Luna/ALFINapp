namespace ALFINapp.API.DTOs
{
    public class DtoVReagendarClientes
    {
        public List<int> IdDerivacion { get; set; } = new List<int>();
        public DateTime NuevaFechaVisita { get; set; } = DateTime.Now;
        public string MotivoReagendamiento { get; set; } = string.Empty;
        public string NuevaAgencia { get; set; } = string.Empty;
        public string NuevaOferta { get; set; } = string.Empty;
        public List<DtoVReagendarFiltros> Filtros { get; set; } = new List<DtoVReagendarFiltros>();
    }

    public class DtoVReagendarFiltros
    {
        public string Filtro { get; set; } = string.Empty;
        public string Dato { get; set; } = string.Empty;
    }
}
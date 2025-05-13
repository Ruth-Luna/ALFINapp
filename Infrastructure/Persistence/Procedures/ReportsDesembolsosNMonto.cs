namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsDesembolsosNMonto
    {
        public int? desembolsado { get; set; } = 0;
        public decimal? Importe_Desembolsado { get; set; } = 0;
    }
    public class ReportsEtiquetaMetaImporte
    {
        public string nombre_meta { get; set; } = string.Empty;
        public string nombre_porcentaje_categoria { get; set; } = string.Empty;
        public decimal importe_meta { get; set; } = 0;
        public int cantidad_meta { get; set; } = 0;
        public decimal porcentaje_importe { get; set; } = 0;
    }
}
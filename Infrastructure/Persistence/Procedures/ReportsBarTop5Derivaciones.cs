namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsBarTop5Derivaciones
    {
        public string? DniAsesor { get; set; }
        public string? NombresCompletosAsesor { get; set; }
        public int? ContadorDerivaciones { get; set; }
    }
    public class ReportsTablasMetas
    {
        public string? dni { get; set; }
        public string? nombre_completo { get; set; }
        public int? total_derivaciones { get; set; }
        public decimal? total_importe { get; set; }
        public int? total_gestion { get; set; }
        public decimal? porcentaje_gestiones { get; set; }
        public decimal? porcentaje_importe { get; set; }
        public decimal? porcentaje_derivaciones { get; set; }
        public int? metas_gestiones { get; set; }
        public decimal? metas_importe { get; set; }
        public int? metas_derivaciones { get; set; }
    }
}
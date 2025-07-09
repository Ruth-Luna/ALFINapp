namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class ReportsGPiePorcentajeGestionadoDerivadoDesembolsado
    {
        public string PERIODO { get; set; } = string.Empty;
        public int TOTAL_GESTIONADOS { get; set; }
        public int TOTAL_DERIVADOS { get; set; }
        public int TOTAL_DESEMBOLSADOS { get; set; }
        public decimal PORCENTAJE_DERIVADOS { get; set; }
        public decimal PORCENTAJE_DESEMBOLSADOS { get; set; }
        public decimal PORCENTAJE_NO_DERIVADO { get; set; }
    }
}
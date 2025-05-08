namespace ALFINapp.API.Models
{
    public class ViewReportesMetas
    {
        public List<ViewMetas> metas { get; set; } = new List<ViewMetas>();
        public int totalGestiones { get; set; } = 0;
        public decimal totalImporte { get; set; } = 0;
        public int totalDerivaciones { get; set; } = 0;
    }
    public class ViewMetas
    {
        public string nombresCompletos { get; set; } = string.Empty;
        public int totalDerivaciones { get; set; } = 0;
        public decimal totalImporte { get; set; } = 0;
        public int totalGestion { get; set; } = 0;
        public decimal porcentajeGestiones { get; set; } = 0;
        public decimal porcentajeImporte { get; set; } = 0;
        public decimal porcentajeDerivaciones { get; set; } = 0;
        public int metasGestiones { get; set; } = 0;
        public decimal metasImporte { get; set; } = 0;
        public decimal metasDerivaciones { get; set; } = 0;
    }
}
namespace ALFINapp.API.Models
{
    public class ViewReportesGeneral
    {
        public List<ViewUsuario>? Asesores { get; set; }
        public List<ViewUsuario>? Supervisores { get; set; }
        public List<ViewLineaGestionVsDerivacion>? lineaGestionVsDerivacion { get; set; }
        public List<ViewEtiquetas>? etiquetas { get; set; } = new List<ViewEtiquetas>();
        public ViewReportePieGeneral ProgresoGeneral { get; set; } = new ViewReportePieGeneral();
        public List<ViewReporteTablaGeneral>? reporteTablaGeneral { get; set; }
        public List<ViewReportePieGeneral>? pieContactabilidad { get; set; }
        public int? TotalDerivaciones { get; set; }
        public int? TotalDerivacionesDesembolsadas { get; set; }
        public int? TotalDerivacionesNoDesembolsadas { get; set; }
        public int? TotalDerivacionesNoProcesadas { get; set; }
        public int? TotalDerivacionesEnvioEmailAutomatico { get; set; }
        public int? TotalDerivacionesEnvioForm { get; set; }
        public List<DerivacionesFecha>? NumDerivacionesXFecha { get; set; }
        public List<DerivacionesFecha>? NumDesembolsosXFecha { get; set; }
        public List<ViewReporteBarGeneral>? top5asesores { get; set; }
        public bool? filtro_por_fechas { get; set; } = false;
        public FechaDelFiltro? fecha_filtro { get; set; } = new FechaDelFiltro();
    }
    public class FechaDelFiltro
    {
        public int? mes { get; set; } = null;
        public int? anio { get; set; } = null;
    }
    public class DerivacionesFecha
    {
        public string? Fecha { get; set; }
        public int Contador { get; set; }
    }
    public class ViewLineaGestionVsDerivacion
    {
        public DateOnly FECHA { get; set; }
        public int GESTIONES { get; set; }
        public int DERIVACIONES { get; set; }
    }
    public class ViewReportePieGeneral
    {
        public string? estado { get; set; }
        public string? PERIODO { get; set; }
        public int total { get; set; }
        public int TOTAL_ASIGNADOS { get; set; }
        public int TOTAL_GESTIONADOS { get; set; }
        public int TOTAL_DERIVADOS { get; set; }
        public int TOTAL_DESEMBOLSADOS { get; set; }
        public decimal total_importes { get; set; }
        public decimal porcentaje { get; set; }
        public decimal PORCENTAJE_GESTIONADOS { get; set; }
        public decimal PORCENTAJE_NO_GESTIONADOS { get; set; }
        public decimal PORCENTAJE_DERIVADOS { get; set; }
        public decimal PORCENTAJE_DESEMBOLSADOS { get; set; }
        public decimal PORCENTAJE_NO_DERIVADO { get; set; }
        public int metasGestiones { get; set; }
        public int metasDerivaciones { get; set; }
        public decimal metasDesembolsos { get; set; }
        public decimal metasImporte { get; set; }
    }
    public class ViewReporteBarGeneral
    {
        public string? dni { get; set; }
        public string? nombres_completos { get; set; }
        public int contador { get; set; }
    }
    public class ViewReporteTablaGeneral
    {
        public string? dni { get; set; }
        public string? nombres_asesor { get; set; }
        public string? nombres_supervisor { get; set; }
        public int? contador_asignados { get; set; } = 0;
        public int? contador_gestionado { get; set; } = 0;
        public int? contador_derivado { get; set; } = 0;
        public int? contador_desembolsado { get; set; } = 0;
        public decimal? importe_desembolsado { get; set; }
        public decimal? porcentaje_desembolsos_derivados { get; set; } = 0;
        public decimal? porcentaje_derivados_asignados { get; set; } = 0;
        public decimal? porcentaje_derivados_gestionados { get; set; } = 0;
        public decimal? porcentaje_gestionados_asignados { get; set; } = 0;
    }
    public class ViewReporteTablaMeses
    {
        public string? periodo { get; set; } = string.Empty;
        public int? total_asignados { get; set; } = 0;
        public int? total_gestionados { get; set; } = 0;
        public int? total_desembolsados { get; set; } = 0;
        public decimal? porcentaje_derivados { get; set; } = 0;
        public decimal? porcentaje_desembolsados { get; set; } = 0;
        public decimal? porcentaje_no_derivado { get; set; } = 0;
    }
    public class ViewEtiquetas
    {
        public string? nombreEtiqueta { get; set; } = string.Empty;
        public string? nombrePorcentaje { get; set; } = string.Empty;
        public string? nombreCategoria { get; set; } = string.Empty;
        public int cantidadEtiqueta { get; set; } = 0;
        public decimal importeEtiquetas { get; set; } = 0;
        public decimal porcentajeEtiqueta { get; set; } = 0;
    }
}
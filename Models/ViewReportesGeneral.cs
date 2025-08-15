using ALFINapp.Infrastructure.Persistence.Procedures;

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
        public ViewLineaGestionVsDerivacion(DateOnly fecha, int gestiones, int derivaciones)
        {
            FECHA = fecha;
            GESTIONES = gestiones;
            DERIVACIONES = derivaciones;
        }
        public ViewLineaGestionVsDerivacion(ReportsGLineasGestionVsDerivacionDiaria item)
        {
            FECHA = item.FECHA;
            GESTIONES = item.GESTIONES;
            DERIVACIONES = item.DERIVACIONES;
        }
        public ViewLineaGestionVsDerivacion() { }
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
        public ViewReportePieGeneral() { }
        public ViewReportePieGeneral(ReportsGPiePorcentajeGestionadosSobreAsignados? model,
            ReportsGPiePorcentajeGestionadoDerivadoDesembolsado? model2)
        {
            if (model != null)
            {
                estado = "GENERAL";
                PERIODO = model.PERIODO;
                TOTAL_ASIGNADOS = model.TOTAL_ASIGNADOS;
                TOTAL_GESTIONADOS = model.TOTAL_GESTIONADOS;
                TOTAL_DERIVADOS = model2?.TOTAL_DERIVADOS ?? 0;
                TOTAL_DESEMBOLSADOS = model2?.TOTAL_DESEMBOLSADOS ?? 0;
                total_importes = model2?.TOTAL_DESEMBOLSADOS ?? 0;
                PORCENTAJE_GESTIONADOS = model.PORCENTAJE_GESTIONADOS;
                PORCENTAJE_NO_GESTIONADOS = model.PORCENTAJE_NO_GESTIONADOS;
                PORCENTAJE_DERIVADOS = model2?.PORCENTAJE_DERIVADOS ?? 0;
                PORCENTAJE_DESEMBOLSADOS = model2?.PORCENTAJE_DESEMBOLSADOS ?? 0;
                PORCENTAJE_NO_DERIVADO = model2?.PORCENTAJE_NO_DERIVADO ?? 0;
            }
        }
        public ViewReportePieGeneral(ReportsPieContactabilidadCliente item)
        {
            estado = item.estado;
            total = item.cantidad;
            porcentaje = item.porcentaje;
        }
    }
    public class ViewReporteBarGeneral
    {
        public string? dni { get; set; }
        public string? nombres_completos { get; set; }
        public int contador { get; set; }
        public ViewReporteBarGeneral() { }
        public ViewReporteBarGeneral(string? dni, string? nombres_completos, int contador)
        {
            this.dni = dni;
            this.nombres_completos = nombres_completos;
            this.contador = contador;
        }
        public ViewReporteBarGeneral(ReportsBarTop5Derivaciones item)
        {
            dni = item.DniAsesor;
            nombres_completos = item.NombresCompletosAsesor;
            contador = item.ContadorDerivaciones ?? 0;
        }
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
        public ViewReporteTablaGeneral() { }
        public ViewReporteTablaGeneral(ReportsTablaGestionadoDerivadoDesembolsadoImporte item)
        {
            dni = item.dni_asesor;
            nombres_asesor = item.asesor;
            nombres_supervisor = item.supervisor;
            contador_asignados = item.asignados;
            contador_gestionado = item.gestionado;
            contador_derivado = item.derivado;
            contador_desembolsado = item.desembolsado;
            importe_desembolsado = item.Importe_Desembolsado;
            porcentaje_desembolsos_derivados = item.desembolsado / (item.derivado == 0 ? 1 : item.derivado) * 100;
            porcentaje_derivados_asignados = item.derivado / (item.asignados == 0 ? 1 : item.asignados) * 100;
            porcentaje_derivados_gestionados = item.derivado / (item.gestionado == 0 ? 1 : item.gestionado) * 100;
            porcentaje_gestionados_asignados = item.gestionado / (item.asignados == 0 ? 1 : item.asignados) * 100;
        }
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
        public ViewEtiquetas() { }
        public ViewEtiquetas(ReportsDesembolsosNMonto item)
        {
            nombreEtiqueta = "DESEMBOLSOS";
            nombreCategoria = "DESEMBOLSOS";
            nombrePorcentaje = "IMPORTES";
            cantidadEtiqueta = item.desembolsado ?? 0;
            importeEtiquetas = item.Importe_Desembolsado ?? 0;
            porcentajeEtiqueta = 0;
        }
        public ViewEtiquetas(ReportsEtiquetaMetaImporte item)
        {
            nombreEtiqueta = item.nombre_meta;
            nombreCategoria = "PORCENTAJE DE META ALCANZADA";
            nombrePorcentaje = item.nombre_porcentaje_categoria;
            cantidadEtiqueta = item.cantidad_meta ?? 0;
            importeEtiquetas = item.importe_meta ?? 0;
            porcentajeEtiqueta = item.porcentaje_importe ?? 0;
        }
    }
}
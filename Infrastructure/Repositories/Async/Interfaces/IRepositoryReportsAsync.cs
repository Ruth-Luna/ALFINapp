using ALFINapp.Application.DTOs;

namespace ALFINapp.Infrastructure.Repositories.Async.Interfaces
{
    public interface IRepositoryReportsAsync
    {
        public Task<(
            DetallesReportesLineaGestionVsDerivacionDTO linea, 
            DetallesReportesGpieDTO pie, 
            DetallesReportesBarDTO bar,
            DetallesReportesTablasDTO tabla,
            DetallesReportesGpieDTO pie2,
            DetallesReportesEtiquetasDTO etiquetas)> GetReportesAsync(
                int idUsuario,
                int? anio = null,
                int? mes = null);
    }
}
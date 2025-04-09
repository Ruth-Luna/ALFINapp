using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;
namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesLineaGestionVsDerivacionDTO
    {
        public List<ReportsGLineasGestionVsDerivacionDiaria> Reportes { get; set; } = new List<ReportsGLineasGestionVsDerivacionDiaria>();
        public DetallesReportesLineaGestionVsDerivacionDTO(List<ReportsGLineasGestionVsDerivacionDiaria> model)
        {
            Reportes = model;
        }
        public DetallesReportesLineaGestionVsDerivacionDTO()
        {
            Reportes = new List<ReportsGLineasGestionVsDerivacionDiaria>();
        }
        public List<ViewLineaGestionVsDerivacion> toViewLineaGestionVsDerivacion()
        {
            var result = new List<ViewLineaGestionVsDerivacion>();
            foreach (var item in Reportes)
            {
                var view = new ViewLineaGestionVsDerivacion
                {
                    FECHA = item.FECHA,
                    GESTIONES = item.GESTIONES,
                    DERIVACIONES = item.DERIVACIONES,
                };
                result.Add(view);
            }
            return result;
        }
    }
}
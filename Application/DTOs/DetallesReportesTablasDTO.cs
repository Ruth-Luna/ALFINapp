using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesTablasDTO
    {
        public List <ReportsTablaGestionadoDerivadoDesembolsadoImporte> Reportes { get; set; } = new List<ReportsTablaGestionadoDerivadoDesembolsadoImporte>();
        public DetallesReportesTablasDTO(List<ReportsTablaGestionadoDerivadoDesembolsadoImporte> model)
        {
            Reportes = model;
        }
        public DetallesReportesTablasDTO()
        {
            Reportes = new List<ReportsTablaGestionadoDerivadoDesembolsadoImporte>();
        }
        public List<ViewReporteTablaGeneral> toViewTabla ()
        {
            var reportes = new List<ViewReporteTablaGeneral>();
            foreach (var item in Reportes)
            {
                var reporte = new ViewReporteTablaGeneral();
                reporte.dni = item.dni_asesor;
                reporte.nombres_asesor = item.asesor;
                reporte.nombres_supervisor = item.supervisor;
                reporte.contador_gestionado = item.gestionado;
                reporte.contador_derivado = item.derivado;
                reporte.contador_desembolsado = item.desembolsado;
                reporte.importe_desembolsado = item.Importe_Desembolsado;
                reportes.Add(reporte);
            }
            return reportes;
        }
    }
}
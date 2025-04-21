using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesEtiquetasDTO
    {
        public List<DetalleEtiqueta> etiquetas { get; set; } = new List<DetalleEtiqueta>();
        public DetallesReportesEtiquetasDTO (ReportsDesembolsosNMonto model)
        {
            if (model != null)
            {
                var detalleEtiqueta = new DetalleEtiqueta();
                detalleEtiqueta.nombreEtiqueta = "Desembolsos e Importes";
                detalleEtiqueta.cantidadEtiqueta = model.desembolsado;
                detalleEtiqueta.importeEtiquetas = model.Importe_Desembolsado;
                etiquetas.Add(detalleEtiqueta);
            }
        }
        public DetallesReportesEtiquetasDTO ()
        {
            etiquetas = new List<DetalleEtiqueta>();
        }
        public List<ViewEtiquetas> toViewEtiquetas ()
        {
            var etiquetasView = new List<ViewEtiquetas>();
            foreach (var item in etiquetas)
            {
                var etiquetaView = new ViewEtiquetas();
                etiquetaView.nombreEtiqueta = item.nombreEtiqueta;
                etiquetaView.cantidadEtiqueta = item.cantidadEtiqueta;
                etiquetaView.importeEtiquetas = item.importeEtiquetas;
                etiquetasView.Add(etiquetaView);
            }
            return etiquetasView;
        }
    }

    public class DetalleEtiqueta 
    {
        public string nombreEtiqueta { get; set; } = string.Empty;
        public int cantidadEtiqueta { get; set; } = 0;
        public decimal importeEtiquetas { get; set; } = 0;
    }
}
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
                detalleEtiqueta.cantidadEtiqueta = model.desembolsado ?? 0;
                detalleEtiqueta.importeEtiquetas = model.Importe_Desembolsado ?? 0;
                etiquetas.Add(detalleEtiqueta);
            }
        }
        public DetallesReportesEtiquetasDTO ()
        {
            etiquetas = new List<DetalleEtiqueta>();
        }

        public DetallesReportesEtiquetasDTO (List<ReportsEtiquetaMetaImporte> model)
        {
            if (model != null)
            {
                foreach (var item in model)
                {
                    var detalleEtiqueta = new DetalleEtiqueta();
                    detalleEtiqueta.nombreEtiqueta = item.nombre_meta;
                    detalleEtiqueta.cantidadEtiqueta = item.cantidad_meta;
                    detalleEtiqueta.importeEtiquetas = item.importe_meta;
                    etiquetas.Add(detalleEtiqueta);
                }
            }
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

        public static implicit operator List<object>(DetallesReportesEtiquetasDTO v)
        {
            throw new NotImplementedException();
        }
    }

    public class DetalleEtiqueta 
    {
        public string nombreEtiqueta { get; set; } = string.Empty;
        public int cantidadEtiqueta { get; set; } = 0;
        public decimal importeEtiquetas { get; set; } = 0;
        public decimal porcentajeEtiqueta { get; set; } = 0;
    }
}
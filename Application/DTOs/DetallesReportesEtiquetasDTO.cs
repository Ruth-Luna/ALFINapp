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
                detalleEtiqueta.nombreEtiqueta = "Desembolsos";
                detalleEtiqueta.nombreEtiquetaCategoria = "Desembolsos";
                detalleEtiqueta.nombrePorcentajeCategoria = "Importes";
                detalleEtiqueta.cantidadEtiqueta = model.desembolsado ?? 0;
                detalleEtiqueta.importeEtiquetas = model.Importe_Desembolsado ?? 0;
                etiquetas.Add(detalleEtiqueta);
            }
        }
        public DetallesReportesEtiquetasDTO ()
        {
            var listaetiquetas = new List<DetalleEtiqueta>();
            var etiqueta = new DetalleEtiqueta();
            etiqueta.nombreEtiqueta = "PORCENTAJE DE META ALCANZADA";
            etiqueta.nombreEtiquetaCategoria = "PORCENTAJE DE META ALCANZADA";
            etiqueta.nombrePorcentajeCategoria = "META DE IMPORTES";
            etiqueta.cantidadEtiqueta = 0;
            etiqueta.importeEtiquetas = 0;
            listaetiquetas.Add(etiqueta);
            etiquetas = listaetiquetas;
        }

        public DetallesReportesEtiquetasDTO (List<ReportsEtiquetaMetaImporte> model)
        {
            if (model != null)
            {
                foreach (var item in model)
                {
                    var detalleEtiqueta = new DetalleEtiqueta();
                    detalleEtiqueta.nombreEtiqueta = item.nombre_meta ?? string.Empty;
                    detalleEtiqueta.nombrePorcentajeCategoria = item.nombre_porcentaje_categoria ?? string.Empty;
                    detalleEtiqueta.nombreEtiquetaCategoria = "Porcentaje de Meta Alcanzada";
                    detalleEtiqueta.cantidadEtiqueta = item.cantidad_meta ?? 0;
                    detalleEtiqueta.importeEtiquetas = item.importe_meta ?? 0;
                    detalleEtiqueta.porcentajeEtiqueta = item.porcentaje_importe ?? 0;
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
                etiquetaView.nombreEtiqueta = item.nombreEtiqueta.ToUpper();
                etiquetaView.nombreCategoria = item.nombreEtiquetaCategoria.ToUpper();
                etiquetaView.nombrePorcentaje = item.nombrePorcentajeCategoria.ToUpper();
                etiquetaView.cantidadEtiqueta = item.cantidadEtiqueta;
                etiquetaView.importeEtiquetas = item.importeEtiquetas;
                etiquetaView.porcentajeEtiqueta = item.porcentajeEtiqueta;
                if (item.porcentajeEtiqueta > 0)
                {
                    etiquetaView.porcentajeEtiqueta = Math.Round(item.porcentajeEtiqueta, 2);
                }
                else
                {
                    etiquetaView.porcentajeEtiqueta = 0;
                }
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
        public string nombreEtiquetaCategoria { get; set; } = string.Empty;
        public string nombrePorcentajeCategoria { get; set; } = string.Empty;
        public int cantidadEtiqueta { get; set; } = 0;
        public decimal importeEtiquetas { get; set; } = 0;
        public decimal porcentajeEtiqueta { get; set; } = 0;
    }
}
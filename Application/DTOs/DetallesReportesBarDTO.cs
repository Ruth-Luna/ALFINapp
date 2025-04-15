using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.Application.DTOs
{
    public class DetallesReportesBarDTO
    {
        public List<int> Contador { get; set; } = new List<int>();
        public List<string> Nombres { get; set; } = new List<string>();
        public List<string> DNIs { get; set; } = new List<string>();
        public List<DateTime> Fechas { get; set; } = new List<DateTime>();
        public DetallesReportesBarDTO (List<ReportsBarTop5Derivaciones> model)
        {
            foreach (var item in model)
            {
                Contador.Add(item.ContadorDerivaciones ?? 0);
                Nombres.Add(item.NombresCompletosAsesor ?? string.Empty);
                DNIs.Add(item.DniAsesor ?? string.Empty);
            }
        }
        public DetallesReportesBarDTO ()
        {
            Contador = new List<int>();
            Nombres = new List<string>();
            DNIs = new List<string>();
            Fechas = new List<DateTime>();
        }
        public ViewReporteBarGeneral toViewReporteBarGeneral()
        {
            return new ViewReporteBarGeneral
            {
                dni = DNIs.FirstOrDefault(),
                nombres_completos = Nombres.FirstOrDefault(),
                contador = Contador.FirstOrDefault()
            };
        }
        public List<ViewReporteBarGeneral> toViewListReporteBarGeneral()
        {
            var list = new List<ViewReporteBarGeneral>();
            for (int i = 0; i < Contador.Count; i++)
            {
                list.Add(new ViewReporteBarGeneral
                {
                    dni = DNIs[i],
                    nombres_completos = Nombres[i],
                    contador = Contador[i]
                });
            }
            return list;
        }
    }
}
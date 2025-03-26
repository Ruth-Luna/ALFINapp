using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Services
{
    public class ServiceReportes
    {
        public async Task<List<(string dniAsesor, int totalDer, int totalDerProc, int totalDesem)>> GroupDerivacionesXAsesor(
            List<DerivacionesAsesores> derivaciones,
            List<Desembolsos> desembolsos
            )
        {
            try
            {
                var derivacionesGroup = derivaciones.GroupBy(x => x.DniAsesor).Select(x => new
                {
                    DniAsesor = x.Key,
                    TotalDerivaciones = x.Count(),
                    TotalDerivacionesProcesadas = x.Count(y => y.FueProcesado == true),
                }).ToList();
                var desembolsosGroup = desembolsos.GroupBy(x => x.DocAsesor).Select(x => new
                {
                    DniAsesor = x.Key,
                    TotalDesembolsos = x.Count(),
                }).ToList();
                var result = derivacionesGroup.Join(desembolsosGroup,
                    x => x.DniAsesor,
                    y => y.DniAsesor,
                    (x, y) => new { x.DniAsesor, x.TotalDerivaciones, x.TotalDerivacionesProcesadas, y.TotalDesembolsos })
                    .Select(x => (x.DniAsesor ?? "", x.TotalDerivaciones, x.TotalDerivacionesProcesadas, x.TotalDesembolsos)).ToList();
                if (result == null)
                {
                    return new List<(string dniAsesor, int totalDer, int totalDerProc, int totalDesem)>();
                }
                return result;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error al procesar las derivaciones generales: " + ex.Message);
                return new List<(string dniAsesor, int totalDer, int totalDerProc, int totalDesem)>();
            }
        }
        public async Task<int> GroupAsignacionesXTipificacion()
        {
            try
            {
                return 0;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error al procesar las derivaciones por asesor: " + ex.Message);
                return 0;
            }
        }
    }
}
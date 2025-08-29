using ALFINapp.API.DTOs;
using ALFINapp.API.Models;
using ALFINapp.Datos.DAO.Derivaciones;
using ALFINapp.Datos.DAO.Reagendacion;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    public class ReagendamientoController : Controller
    {
        private readonly DAO_SubirReagendacion _dao_SubirReagendacion;
        private readonly DAO_Derivaciones _dao_Derivaciones;
        public ReagendamientoController(
            DAO_SubirReagendacion dao_SubirReagendacion,
            DAO_Derivaciones dao_Derivaciones)
        {
            _dao_SubirReagendacion = dao_SubirReagendacion;
            _dao_Derivaciones = dao_Derivaciones;
        }
        public async Task<IActionResult> Reagendar([FromBody] DtoVReagendar dtovreagendar)
        {
            if (dtovreagendar.FechaReagendamiento == null || dtovreagendar.FechaReagendamiento == DateTime.MinValue)
            {
                return Json(new { success = false, message = "La fecha de reagendamiento es obligatoria." });
            }
            if (dtovreagendar.IdDerivacion == null || dtovreagendar.IdDerivacion == 0)
            {
                return Json(new { success = false, message = "El id de derivación es obligatorio." });
            }
            var exec = await _dao_SubirReagendacion.reagendarCliente(
                dtovreagendar.IdDerivacion.Value,
                dtovreagendar.FechaReagendamiento.Value,
                dtovreagendar.urlEvidencias);
            if (!exec.success)
            {
                return Json(new { success = false, message = exec.message });
            }
            return Json(new { success = true, message = exec.message });
        }

        public async Task<IActionResult> Reagendamiento(int id)
        {
            var reagendamiento = await _dao_Derivaciones.GetDerivacionAsync(id);
            if (reagendamiento == null)
            {
                return Json(new { success = false, message = "No se encontraron datos de la derivación." });
            }
            return Json(new
            {
                success = true,
                message = "Datos de la derivación obtenidos correctamente.",
                data = new ViewClienteReagendado(reagendamiento)
            });
        }
    }
}
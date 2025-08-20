using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Reagendamiento;
using ALFINapp.Datos.DAO.Operaciones;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    public class OperacionesController : Controller
    {
        private readonly DAO_Derivaciones _dao_derivaciones;
        private readonly IUseCaseReagendar _useCaseReagendar;
        public OperacionesController(
            DAO_Derivaciones dao_derivaciones,
            IUseCaseReagendar useCaseReagendar)
        {
            _dao_derivaciones = dao_derivaciones;
            _useCaseReagendar = useCaseReagendar;
        }
        public IActionResult Operaciones()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDerivaciones()
        {
            var idusuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idusuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }
            var idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }
            var usuarioId = idusuario.Value;
            var rolUsuario = idRol.Value;
            var result = await _dao_derivaciones.GetAllDerivaciones(usuarioId, rolUsuario);
            if (!result.success)
            {
                return Json(new { success = false, message = result.message });
            }
            if (result.data == null)
            {
                // If no derivations are found, return an empty list this is to avoid null reference exceptions
                // the dao method should handle this case
                return Json(new { success = true, message = "No se encontraron derivaciones.", data = result.data });
            }
            return Json(new { success = true, message = result.message, data = result.data });
        }
        public async Task<IActionResult> Reagendar(
            [FromBody] DtoVReagendar dtovreagendar)
        {
            if (dtovreagendar.FechaReagendamiento == null || dtovreagendar.FechaReagendamiento == DateTime.MinValue)
            {
                return Json(new { success = false, message = "La fecha de reagendamiento es obligatoria." });
            }
            if (dtovreagendar.IdDerivacion == null || dtovreagendar.IdDerivacion == 0)
            {
                return Json(new { success = false, message = "El id de derivación es obligatorio." });
            }
            var exec = await _useCaseReagendar.exec(
                dtovreagendar.IdDerivacion.Value,
                dtovreagendar.FechaReagendamiento.Value,
                dtovreagendar.urlEvidencias);
            if (!exec.IsSuccess)
            {
                return Json(new { success = false, message = exec.Message });
            }
            return Json(new { success = true, message = exec.Message });
        }
    }
}
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
        private readonly DAO_Reagendamientos _dao_reagendamientos;
        public OperacionesController(
            DAO_Derivaciones dao_derivaciones,
            IUseCaseReagendar useCaseReagendar,
            DAO_Reagendamientos dao_reagendamientos)
        {
            _dao_derivaciones = dao_derivaciones;
            _useCaseReagendar = useCaseReagendar;
            _dao_reagendamientos = dao_reagendamientos;
        }
        [Route("operaciones")]
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
        [HttpGet]
        public async Task<IActionResult> GetAllReagendamientos()
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }
            var idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }
            var usuarioId = idUsuario.Value;
            var rolUsuario = idRol.Value;
            var result = await _dao_reagendamientos.GetAllReagendamientos(usuarioId, rolUsuario);
            if (!result.issuccess)
            {
                return Json(new { success = false, message = result.message });
            }
            if (result.data == null)
            {
                // If no reschedulings are found, return an empty list this is to avoid null reference exceptions
                // the dao method should handle this case
                return Json(new { success = true, message = "No se encontraron reagendamientos.", data = new List<ViewReagendamientos>() });
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
        public async Task<IActionResult> GetHistoricoReagendamientos(int idDerivacion)
        {
            if (idDerivacion <= 0)
            {
                return Json(new { success = false, message = "El id de derivación es obligatorio." });
            }
            var result = await _dao_reagendamientos.GetHistoricoReagendamientos(idDerivacion);
            if (!result.issuccess)
            {
                return Json(new { success = false, message = result.message });
            }
            if (result.data == null)
            {
                // If no reschedulings are found, return an empty list this is to avoid null reference exceptions
                // the dao method should handle this case
                return Json(new { success = true, message = "No se encontraron reagendamientos.", data = new List<ViewReagendamientos>() });
            }
            return Json(new { success = true, message = result.message, historico = result.data });
        }
    }
}
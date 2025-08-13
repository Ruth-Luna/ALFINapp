using ALFINapp.API.Filters;
using ALFINapp.Datos.DAO.Derivaciones;
using ALFINapp.Datos.DAO.Evidencias;
using ALFINapp.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class DerivacionController : Controller
    {
        
        private readonly DAO_DerivacionesVista _dao_derivacionesVista;
        private readonly DAO_SubirEvidencia _dao_subirEvidencia;
        public DerivacionController(
            DAO_SubirEvidencia dao_subirEvidencia,
            DAO_DerivacionesVista dao_derivacionesVista)
        {
            _dao_subirEvidencia = dao_subirEvidencia;
            _dao_derivacionesVista = dao_derivacionesVista;
        }

        [HttpGet]
        [PermissionAuthorization("Derivacion", "Derivacion")]
        public async Task<IActionResult> Derivacion()
        {
            var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
            if (UsuarioIdSupervisor == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var rolUsuario = HttpContext.Session.GetInt32("RolUser");
            if (rolUsuario == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var execute = await _dao_derivacionesVista.getDerivacionesVista((int)UsuarioIdSupervisor, (int)rolUsuario);
            if (!execute.success)
            {
                TempData["MessageError"] = execute.message;
                return RedirectToAction("Redireccionar", "Error");
            }
            var viewDerivaciones = execute.data;
            return View("Derivacion", viewDerivaciones);
        }
        [HttpPost]
        [PermissionAuthorization("Derivacion", "Derivacion")]
        public async Task<IActionResult> UploadEvidencia([FromBody] DtoVDerivacionEvidencia evidencia)
        {
            try
            {
                var result = await _dao_subirEvidencia.marcarEvidenciaDisponible(evidencia);
                if (!result.success)
                {
                    return Json(new { success = false, message = result.message });
                }
                return Json(new { success = true, message = result.message });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Error al subir los archivos." });
            }
        }
    }
}
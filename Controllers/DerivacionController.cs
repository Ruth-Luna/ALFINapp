using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Derivacion;
using ALFINapp.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class DerivacionController : Controller
    {
        
        private readonly IUseCaseGetDerivacion _useCaseGetDerivacion;
        private readonly IUseCaseUploadEvidencias _useCaseUploadEvidencias;
        public DerivacionController(
            IUseCaseGetDerivacion useCaseGetDerivacion,
            IUseCaseUploadEvidencias useCaseUploadEvidencias)
        {
            _useCaseGetDerivacion = useCaseGetDerivacion;
            _useCaseUploadEvidencias = useCaseUploadEvidencias;
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
            var execute = await _useCaseGetDerivacion.Execute(UsuarioIdSupervisor.Value, rolUsuario.Value);
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
                var result = await _useCaseUploadEvidencias.Execute(evidencia);
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
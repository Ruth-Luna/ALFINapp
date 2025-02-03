using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class ConsultaController : Controller
    {
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsesores;
        private readonly DBServicesGeneral _dbServicesGeneral;
        public ConsultaController(DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores , DBServicesGeneral dbServicesGeneral)
        {
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
        }

        [HttpGet]
        public IActionResult Consultas()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["Message"] = "No ha iniciado sesion, por favor inicie sesion.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["RolUser"] = HttpContext.Session.GetInt32("RolUser");
            Console.WriteLine("Rol del usuario: " + HttpContext.Session.GetInt32("RolUser"));
            return View("Consultas");
        }

        [HttpPost]
        public async Task<IActionResult> ReAsignarClienteAUsuario(string DniAReasignar, string BaseTipo)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Debe de iniciar la sesion." });
                }
                var baseClienteReasignar = await _dbServicesAsignacionesAsesores.GuardarReAsignacionCliente(DniAReasignar, BaseTipo, usuarioId.Value);

                if (baseClienteReasignar.IsSuccess == false)
                {
                    return Json(new { success = false, message = $"{baseClienteReasignar.message}" });
                }

                return Json(new { success = true, message = $"{baseClienteReasignar.message}" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VistaDerivacionManual()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Debe Iniciar Sesion" });
                }
                var getAgencias = await _dbServicesGeneral.GetUAgenciasConNumeros();
                if (getAgencias.IsSuccess == false)
                {
                    return Json(new { success = false, message = $"{getAgencias.Message}" });
                }
                ViewData["Agencias"] = getAgencias.data;
                return PartialView("_VistaDerivacionManual");
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
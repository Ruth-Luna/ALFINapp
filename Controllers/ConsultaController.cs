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
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;

        public ConsultaController(DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores, DBServicesGeneral dbServicesGeneral, DBServicesConsultasClientes dbServicesConsultasClientes)
        {
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
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

        [HttpGet]
        public async Task<IActionResult> VerificarDNIenBDoBanco(string dni)
        {
            try
            {
                var GetUserRol = HttpContext.Session.GetInt32("RolUser");
                if (GetUserRol == null)
                {
                    return Json(new { existe = false, error = true, message = "No ha iniciado sesion" });
                }
                Console.WriteLine($"DNI recibido: {dni}");

                // Buscar el cliente por DNI
                var GetClienteExistente = await _dbServicesConsultasClientes.GetClientsFromDBandBank(dni);

                if (GetClienteExistente.IsSuccess == false || GetClienteExistente.Data == null)
                {
                    return Json(new { existe = false, error = true, message = GetClienteExistente.message });
                }
                ViewData["RolUser"] = GetUserRol;
                if (GetClienteExistente.Data.TraidoDe == "BDA365")
                {
                        return PartialView("_DatosConsulta", GetClienteExistente.Data);
                }
                if (GetClienteExistente.Data.TraidoDe == "BDALFIN")
                {
                    return PartialView("_DatosConsulta", GetClienteExistente.Data);
                }
                else
                {
                    return Json(new { existe = false, error = true, message = "La Base fue conseguida, pero no se le permite ver los datos de este cliente" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar el DNI: {ex.Message}");
                return Json(new { existe = false, error = true, message = "Ocurrió un error interno. Por favor, intente nuevamente." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerificarTelefono(string telefono)
        {
            try
            {
                var GetUserRol = HttpContext.Session.GetInt32("RolUser");
                if (GetUserRol == null)
                {
                    return Json(new { existe = false, error = true, message = "No ha iniciado sesion" });
                }

                var getCliente = await _dbServicesConsultasClientes.GetClienteByTelefono(telefono);
                if (getCliente.IsSuccess == false || getCliente.Data == null)
                {
                    return Json(new { success = false, message = $"{getCliente.message}" });
                }
                ViewData["RolUser"] = GetUserRol;
                return PartialView("_DatosConsulta", getCliente.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
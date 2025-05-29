using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.Application.Interfaces.Consulta;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ConsultaController : Controller
    {
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsesores;
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;
        private readonly ILogger<ConsultaController> _logger;
        private readonly IUseCaseConsultaClienteDni _useCaseConsultaClienteDni;
        private readonly IUseCaseConsultaClienteTelefono _useCaseConsultaClienteTelefono;
        private readonly IUseCaseAsignarClienteManual _useCaseAsignarClienteManual;
        public ConsultaController(
            DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores, 
            DBServicesGeneral dbServicesGeneral, 
            DBServicesConsultasClientes dbServicesConsultasClientes,
            ILogger<ConsultaController> logger,
            IUseCaseConsultaClienteDni useCaseConsultaClienteDni,
            IUseCaseConsultaClienteTelefono useCaseConsultaClienteTelefono,
            IUseCaseAsignarClienteManual useCaseAsignarClienteManual)
        {
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
            _logger = logger;
            _useCaseConsultaClienteDni = useCaseConsultaClienteDni;
            _useCaseConsultaClienteTelefono = useCaseConsultaClienteTelefono;
            _useCaseAsignarClienteManual = useCaseAsignarClienteManual;
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
                // var baseClienteReasignar = await _dbServicesAsignacionesAsesores.GuardarReAsignacionCliente(DniAReasignar, BaseTipo, usuarioId.Value);
                var asignar = await _useCaseAsignarClienteManual.exec(DniAReasignar, usuarioId.Value, BaseTipo);
                if (asignar.success == false)
                {
                    return Json(new { success = false, message = $"{asignar.message}" });
                }
                if (asignar.success == false)
                {
                    return Json(new { success = false, message = $"{asignar.message}" });
                }

                return Json(new { success = true, message = $"{asignar.message}" });
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
                    return Json(new { existe = false, error = true, message = "No se ha encontrado una sesion activa, vuelva a iniciar sesion. " });
                }
                var exec = await _useCaseConsultaClienteDni.Execute(dni);
                if (exec.IsSuccess == false || exec.Data == null)
                {
                    return Json(new { existe = false, error = true, message = exec.Message });
                }
                ViewData["RolUser"] = GetUserRol;
                return PartialView("_DatosConsulta", exec.Data);
            }
            catch (Exception ex)
            {
                return Json(new { existe = false, error = true, message = ex.Message });
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

                var execute = await _useCaseConsultaClienteTelefono.exec(telefono);
                if (execute.IsSuccess == false || execute.Data == null)
                {
                    return Json(new { existe = false, error = true, message = execute.Message });
                }
                ViewData["RolUser"] = GetUserRol;
                return PartialView("_DatosConsulta", execute.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
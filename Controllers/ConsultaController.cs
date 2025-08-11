using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.Application.Interfaces.Consulta;
using ALFINapp.Datos.DAO;
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
        // DAO for client consultations
        // This DAO is used to interact with the database for client consultation operations

        private readonly DAO_ClientesAsignaciones _dao_ClientesAsignaciones;
        private readonly DAO_ClientesConsultas _dao_ClientesConsultas;
        public ConsultaController(
            DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores,
            DBServicesGeneral dbServicesGeneral,
            DBServicesConsultasClientes dbServicesConsultasClientes,
            ILogger<ConsultaController> logger,
            IUseCaseConsultaClienteDni useCaseConsultaClienteDni,
            IUseCaseConsultaClienteTelefono useCaseConsultaClienteTelefono,
            IUseCaseAsignarClienteManual useCaseAsignarClienteManual,
            DAO_ClientesConsultas dao_ClientesConsultas,
            DAO_ClientesAsignaciones dAO_ClientesAsignaciones)
        {
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
            _logger = logger;
            _useCaseConsultaClienteDni = useCaseConsultaClienteDni;
            _useCaseConsultaClienteTelefono = useCaseConsultaClienteTelefono;
            _useCaseAsignarClienteManual = useCaseAsignarClienteManual;
            _dao_ClientesConsultas = dao_ClientesConsultas;
            _dao_ClientesAsignaciones = dAO_ClientesAsignaciones;
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
                var asignar = await _dao_ClientesAsignaciones.AsignarClienteManual(DniAReasignar, BaseTipo, usuarioId.Value);
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
                var exec = await _dao_ClientesConsultas.GetClienteByDniAsync(dni);
                if (exec.IsSuccess == false || exec.Data == null)
                {
                    return Json(new { existe = false, error = true, message = exec.Message });
                }
                exec.Data.idrol = GetUserRol.Value;
                return Json(new { existe = true, error = false, data = exec.Data });
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

                var execute = await _dao_ClientesConsultas.GetClienteByTelefonoAsync(telefono);
                if (execute.IsSuccess == false || execute.Data == null)
                {
                    return Json(new { existe = false, error = true, message = execute.Message });
                }
                execute.Data.idrol = GetUserRol.Value;
                return Json(new { existe = true, error = false, data = execute.Data });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
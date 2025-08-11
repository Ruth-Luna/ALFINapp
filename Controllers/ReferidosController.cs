using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Infrastructure.Services;
using ALFINapp.Datos.DAO.Referidos;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ReferidosController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        public DBServicesDerivacion _dBServicesDerivacion;
        private readonly DAO_ClientesReferidosVista _dao_clientesReferidosVista;
        private readonly DAO_DerivarClienteReferido _dao_DerivarClienteReferido;
        public ReferidosController(
            DBServicesGeneral dbServicesGeneral,
            DBServicesReferido dbServicesReferido,
            DBServicesDerivacion dBServicesDerivacion,
            DAO_ClientesReferidosVista dao_clientesReferidosVista,
            DAO_DerivarClienteReferido dao_DerivarClienteReferido)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;
            _dBServicesDerivacion = dBServicesDerivacion;
            _dao_clientesReferidosVista = dao_clientesReferidosVista;
            _dao_DerivarClienteReferido = dao_DerivarClienteReferido;
        }
        [HttpGet]
        [PermissionAuthorization("Referidos", "Referidos")]
        public async Task<IActionResult> Referidos()
        {
            try
            {
                var idUsuarioSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (idUsuarioSupervisor == null)
                {
                    TempData["MessageError"] = "No se ha podido obtener el id del usuario supervisor";
                    return RedirectToAction("Index", "Home");
                }

                var getReferidos = await _dao_clientesReferidosVista.GetClientesReferidos();
                if (getReferidos.IsSuccess == false || getReferidos.Data == null)
                {
                    TempData["MessageError"] = getReferidos.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                return View("Referidos", getReferidos.Data);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DatosEnviarDerivacion(int IdReferido)
        {
            try
            {
                var referido = await _dao_clientesReferidosVista.GetClienteReferidoPorId(IdReferido);
                if (referido.IsSuccess == false || referido.Data == null)
                {
                    return Json(new { success = false, message = referido.Message });
                }
                return PartialView("_DatosEnviarDerivacion", referido.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EnviarDerivacionPorReferencia(int IdReferido)
        {
            try
            {
                var result = await _dao_DerivarClienteReferido.DerivarClienteReferidoAsync(IdReferido);
                if (!result.IsSuccess)
                {
                    return Json(new { success = false, message = result.Message });
                }
                return Json(new { success = true, message = "Derivacion enviada correctamente, se ha enviado automaticamente el correo de la derivacion a todos los destinatarios correspondientes" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
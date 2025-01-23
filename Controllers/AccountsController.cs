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
    public class AccountsController : Controller
    {
        private readonly DBServicesGeneral _dbservicesgeneral; // Add this line

        public AccountsController(DBServicesGeneral dbservicesgeneral)
        {
            _dbservicesgeneral = dbservicesgeneral;
        }

        [HttpGet]
        public async Task<IActionResult> GetInformationForAccounts()
        {
            int? RolUser = HttpContext.Session.GetInt32("RolUser");
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                    return RedirectToAction("Index", "Home");
                }
                var userInformation = await _dbservicesgeneral.GetUserInformation(usuarioId.Value);
                if (userInformation.IsSuccess == false)
                {
                    TempData["MessageError"] = userInformation.Message;
                    if (RolUser == 1)
                    {
                        return RedirectToAction("Ventas", "Vendedor");
                    }
                    else if (RolUser == 2)
                    {
                        return RedirectToAction("VistaMainSupervisor", "Supervisor");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewData["RolUser"] = RolUser;
                return View("AccountInformation", userInformation.Data);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                if (RolUser == 1)
                {
                    return RedirectToAction("Ventas", "Vendedor");
                }
                else if (RolUser == 2)
                {
                    return RedirectToAction("VistaMainSupervisor", "Supervisor");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNewPassword(string newPassword)
        {
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesion" });
                }
                var changePasswordResult = await _dbservicesgeneral.UpdatePasswordGeneralFunction(usuarioId.Value, newPassword);
                return Json(new { success = true, message = "Contraseña cambiada con exito" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
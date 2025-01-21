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
            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "Usted no ha iniciado sesion"}); 
                }
                var userInformation = await _dbservicesgeneral.GetUserInformation(usuarioId.Value);
                if (userInformation.IsSuccess == false)
                {
                    return Json(new { success = false, message = "No se pudo obtener la informacion de su cuenta"});
                }
                return PartialView("AccountInformation", userInformation.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message}); 
                throw;
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
                    return Json(new { success = false, message = "Usted no ha iniciado sesion"});
                }
                var changePasswordResult = await _dbservicesgeneral.UpdatePasswordGeneralFunction(usuarioId.Value, newPassword);
                return Json(new { success = true, message = "Contraseña cambiada con exito"});
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message}); 
            }
        }
    }
}
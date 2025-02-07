using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DBServicesConsultasAdministrador _DBServicesConsultasAdministrador;
        private readonly DBServicesUsuarios _DBServicesUsuarios;
        public UsuariosController(DBServicesConsultasAdministrador DBServicesConsultasAdministrador, DBServicesUsuarios DBServicesUsuarios)
        {
            _DBServicesConsultasAdministrador = DBServicesConsultasAdministrador;
            _DBServicesUsuarios = DBServicesUsuarios;
        }
        public async Task<IActionResult> Administracion()
        {
            var getUsuarios = await _DBServicesConsultasAdministrador.ConseguirTodosLosUsuarios();
            return View("Administracion", getUsuarios.Data);
        }
        [HttpPost]
        public async Task<IActionResult> ModificarUsuario([FromBody] Usuario usuario)
        {
            try
            {
            var result = await _DBServicesUsuarios.ModificarUsuario(usuario);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Datos actualizados correctamente" });
            }
            else
            {
                return Json(new { success = false, message = result.Message });
            }
            }
            catch (System.Exception ex)
            {
            return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
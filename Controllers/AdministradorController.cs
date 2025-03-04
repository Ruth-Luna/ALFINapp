using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
using ALFINapp.Models.DTOs;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class AdministradorController : Controller
    {
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesRoles _dBServicesRoles;
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;
        private readonly DBServicesConsultasAdministrador _dBServicesConsultasAdministrador;
        public AdministradorController(
            DBServicesGeneral dbServicesGeneral, 
            DBServicesRoles dBServicesRoles, 
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores,
            DBServicesConsultasAdministrador dBServicesConsultasAdministrador)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dBServicesRoles = dBServicesRoles;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
            _dBServicesConsultasAdministrador = dBServicesConsultasAdministrador;
        }

        [HttpGet]
        [PermissionAuthorization("Administrador", "Inicio")]        
        public async Task<IActionResult> Inicio()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? IdRol = HttpContext.Session.GetInt32("RolUser");
            if (IdRol == null || usuarioId == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }

            var allSupervisor = await _dBServicesConsultasAdministrador.ConseguirTodosLosSupervisores();
            if (!allSupervisor.IsSuccess || allSupervisor.Data == null)
            {
                TempData["MessageError"] = allSupervisor.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var consultasSupervisorEnriquecidas = new List<EnriquecerConsultasSupervisorDTO>();

            foreach (var supervisor in allSupervisor.Data)
            {
            }
            
            return View("Administrador");
        }
    }
}
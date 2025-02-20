using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
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
        public AdministradorController(DBServicesGeneral dbServicesGeneral, DBServicesRoles dBServicesRoles)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dBServicesRoles = dBServicesRoles;
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
            
            var supervisorData = new List<SupervisorDTO>
            {
                new SupervisorDTO
                {
                    IdAsignacion = 1,
                    IdCliente = 2,
                    idUsuarioV = 3,
                    FechaAsignacionV = DateTime.Now,
                    Dni = "23333333",
                    XAppaterno = "TEST1",
                    XApmaterno = "TEST1",
                    XNombre = "TEST1",
                    NombresCompletos = "TEST1",
                    DniVendedor = "TEST1",
                },
                new SupervisorDTO
                {
                    IdAsignacion = 2,
                    IdCliente = 3,
                    idUsuarioV = 4,
                    FechaAsignacionV = DateTime.Now.AddDays(-1),
                    Dni = "23333334",
                    XAppaterno = "TEST2",
                    XApmaterno = "TEST2",
                    XNombre = "TEST2",
                    NombresCompletos = "TEST2",
                    DniVendedor = "TEST2",
                },
                new SupervisorDTO
                {
                    IdAsignacion = 3,
                    IdCliente = 4,
                    idUsuarioV = 5,
                    FechaAsignacionV = DateTime.Now.AddDays(-2),
                    Dni = "23333335",
                    XAppaterno = "TEST3",
                    XApmaterno = "TEST3",
                    XNombre = "TEST3",
                    NombresCompletos = "TEST3",
                    DniVendedor = "TEST3",
                }
            };
            if (supervisorData == null)
            {
                return NotFound("El presente Usuario Supervisor no tiene clientes Asignados");
            }
            // Contar los clientes pendientes (idUsuarioV es null)
            int clientesPendientesSupervisor = 1;
            // Contar todos los clientes
            int totalClientes = 1;
            // Contar los clientes asignados (idUsuarioV no es null o 0)
            int clientesAsignadosSupervisor = 0;

            var getusuario = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            var usuario = getusuario.Data;
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            ViewData["ClientesPendientesSupervisor"] = clientesPendientesSupervisor;
            ViewData["clientesAsignadosSupervisor"] = clientesAsignadosSupervisor;
            ViewData["totalClientes"] = totalClientes;
            return View("Administrador", supervisorData);
        }
    }
}
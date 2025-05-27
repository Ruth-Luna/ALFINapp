using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsesoresController : Controller
    {
        private readonly MDbContext _context;
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsesores;
        private readonly DBServicesConsultasSupervisores _dbServicesConsultasSupervisores;
        private readonly DBServicesGeneral _dbServicesGeneral;
        public AsesoresController(DBServicesAsignacionesAsesores dbServicesAsignacionesAsesores,
                                                DBServicesGeneral dbServicesGeneral,
                                                DBServicesConsultasSupervisores dbServicesConsultasSupervisores,
                                                MDbContext context)
        {
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
            _dbServicesAsignacionesAsesores = dbServicesAsignacionesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _context = context;
        }
        [HttpGet]
        [PermissionAuthorization("Asesores", "Usuarios")]
        public IActionResult Usuarios()
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            return View("Usuarios");
        }
        [HttpGet]
        [PermissionAuthorization("Asesores", "Estado")]
        public async Task<IActionResult> Estado()
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            var asesoresAsignadosaSupervisor = await _dbServicesConsultasSupervisores.GetAsesorsFromSupervisor(idSupervisorActual.Value);
            if (asesoresAsignadosaSupervisor.IsSuccess == false || asesoresAsignadosaSupervisor.Data == null)
            {
                TempData["Error"] = "No se encontraron asesores asignados a su cuenta";
                return RedirectToAction("Supervisor", "Inicio");
            }
            return View("Estado", asesoresAsignadosaSupervisor.Data);
        }

        [HttpGet]
        public IActionResult AgregarNuevoAsesorView()
        {
            int? rolSupervisorActual = HttpContext.Session.GetInt32("RolUser");
            if (rolSupervisorActual == null)
            {
                return Json(new { success = false, message = "Usted no ha iniciado sesion" });
            }

            ViewData["RolActual"] = rolSupervisorActual.Value;
            return PartialView("_AgregarNuevoAsesor");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerClientesPorTipificacion(string tipificacion)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return Json(new { success = false, message = "Usted no ha iniciado sesion" });
            }
            // Consulta para las tipificaciones generales
            var clientesNumDB = (from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 where ca.IdUsuarioS == idSupervisorActual
                                    && ca.FechaAsignacionSup.HasValue
                                    && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                    && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                 select new
                                 {
                                     IdCliente = ce.IdCliente,
                                     TipificacionMayorPeso = ca.TipificacionMayorPeso
                                 }).ToList();

            // Filtrar por la tipificación más relevante
            var clientesFiltrados = clientesNumDB
                                    .Where(cndb => cndb.TipificacionMayorPeso == tipificacion)
                                    .ToList();

            var viewModel = new ResultadoTipificacionViewModelDTO
            {
                DetalleTipificacion = tipificacion,
                NumeroClientes = clientesFiltrados.Count
            };
            // Consulta para obtener los asesores asignados al supervisor junto al numero de clientes asignados, procesados, y demas
            var AsesoresDelSupervisor = await _dbServicesConsultasSupervisores.ConsultaAsesoresDelSupervisor(idSupervisorActual.Value);
            if (AsesoresDelSupervisor.IsSuccess == false || AsesoresDelSupervisor.Data == null)
            {
                TempData["Error"] = "No se encontraron asesores asignados a su cuenta";
                return RedirectToAction("Supervisor", "Inicio");
            }

            ViewData["AsesoresDelSupervisor"] = AsesoresDelSupervisor.Data;
            return PartialView("_ResultadoTipificacion", viewModel);
        }
        
    }
}
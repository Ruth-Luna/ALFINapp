using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
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
        public IActionResult Usuarios()
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return RedirectToAction("Home", "Index");
            }
            return View("Usuarios");
        }
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
        public IActionResult ObtenerClientesPorTipificacion(string tipificacion)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");

            // Consulta para las tipificaciones generales
            var clientesNumDB = (from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 where ca.IdUsuarioS == idSupervisorActual
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

            var AsesoresDelSupervisor = (from u in _context.usuarios
                                         where u.Rol == "VENDEDOR" && u.IDUSUARIOSUP == idSupervisorActual
                                         join ca in _context.clientes_asignados on u.IdUsuario equals ca.IdUsuarioV into caGroup
                                         from ca in caGroup.DefaultIfEmpty()  // Realizamos un left join
                                         group new { u, ca }
                                         by new
                                         {
                                             u.IdUsuario,
                                             u.NombresCompletos,
                                             u.Dni,
                                             u.Telefono,
                                             u.Departamento,
                                             u.Provincia,
                                             u.Distrito,
                                             u.Estado,
                                             u.Rol
                                         } into grouped
                                         select new UsuarioAsesorDTO
                                         {
                                             IdUsuario = grouped.Key.IdUsuario,
                                             Dni = grouped.Key.Dni,
                                             NombresCompletos = grouped.Key.NombresCompletos,
                                             Telefono = grouped.Key.Telefono,
                                             Departamento = grouped.Key.Departamento,
                                             Provincia = grouped.Key.Provincia,
                                             Distrito = grouped.Key.Distrito,
                                             Estado = grouped.Key.Estado,
                                             Rol = grouped.Key.Rol,
                                             TotalClientesAsignados = grouped.Count(g => g.ca != null
                                                                                     && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                     && g.ca.IdUsuarioS == idSupervisorActual
                                                                                     && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                     && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes asignados
                                             ClientesTrabajando = grouped.Count(g => g.ca != null
                                                                                     && g.ca.TipificacionMayorPeso != null
                                                                                     && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                     && g.ca.IdUsuarioS == idSupervisorActual
                                                                                     && g.ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                                     && g.ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month), // Clientes trabajados
                                             ClientesSinTrabajar = grouped.Count(g => g.ca != null
                                                                                     && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                     && g.ca.IdUsuarioS == idSupervisorActual)
                                                                                      - grouped.Count(g => g.ca != null
                                                                                     && g.ca.TipificacionMayorPeso != null
                                                                                     && g.ca.IdUsuarioV == grouped.Key.IdUsuario
                                                                                     && g.ca.IdUsuarioS == idSupervisorActual)
                                         }).ToList();

            ViewData["AsesoresDelSupervisor"] = AsesoresDelSupervisor;
            return PartialView("_ResultadoTipificacion", viewModel);
        }
    }
}
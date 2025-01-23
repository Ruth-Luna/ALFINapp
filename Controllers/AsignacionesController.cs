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
    public class AsignacionesController : Controller
    {
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly MDbContext _context;
        public AsignacionesController(DBServicesGeneral dbServicesGeneral, MDbContext context)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _context = context;
        }
        public IActionResult CargarActualizarAsignacion(int idUsuario)
        {
            // TODO: Implementación de la lógica para actualizar la asignación
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            var asesorBusqueda = (from u in _context.usuarios
                                  where u.IdUsuario == idUsuario
                                  select new UsuarioAsesorDTO
                                  {
                                      IdUsuario = u.IdUsuario,
                                      Dni = u.Dni,
                                      NombresCompletos = u.NombresCompletos,
                                      Telefono = u.Telefono,
                                      Departamento = u.Departamento,
                                      Provincia = u.Provincia,
                                      Distrito = u.Distrito,
                                      Region = u.REGION,
                                      Estado = u.Estado,
                                      Rol = u.Rol,
                                      TotalClientesAsignados = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario),
                                      ClientesTrabajando = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario
                                                                                            && ca.TipificacionMayorPeso != null),
                                      ClientesSinTrabajar = _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario) -
                                                            _context.clientes_asignados.Count(ca => ca.IdUsuarioV == idUsuario &&
                                                                                            ca.TipificacionMayorPeso != null)
                                  }).FirstOrDefault();
            Console.WriteLine($"El Asesor {asesorBusqueda.NombresCompletos} ha sido encontrado");
            if (asesorBusqueda == null)
            {
                Console.WriteLine("El Asesor no ha sido encontrado");
                return Json(new { success = false, message = "La entrada no ha sido ocurrido ha ocurrido un error" });
            }
            Console.WriteLine("Retornando la vista parcial");
            return PartialView("ActualizarAsignacion", asesorBusqueda); // Retorna una vista parcial
        }
    }
}
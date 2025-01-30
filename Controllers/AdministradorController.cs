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
        private readonly MDbContext _context;
        public AdministradorController(MDbContext context, DBServicesGeneral dbServicesGeneral)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Inicio(int page = 1, int pageSize = 20)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            var supervisorData = from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                 join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                 join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario into usuarioJoin
                                 from u in usuarioJoin.DefaultIfEmpty()
                                 where ca.IdUsuarioS == usuarioId
                                        && ca.ClienteDesembolso != true
                                        && ca.ClienteRetirado != true
                                        && ca.FechaAsignacionSup.HasValue
                                        && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                        && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                        && db.TipoBase == ca.FuenteBase
                                 select new SupervisorDTO
                                 {
                                     IdAsignacion = ca.IdAsignacion,
                                     IdCliente = ca.IdCliente,
                                     idUsuarioV = ca.IdUsuarioV.HasValue ? ca.IdUsuarioV.Value : 0,
                                     FechaAsignacionV = ca.FechaAsignacionVendedor,

                                     Dni = bc.Dni,
                                     XAppaterno = bc.XAppaterno,
                                     XApmaterno = bc.XApmaterno,
                                     XNombre = bc.XNombre,

                                     NombresCompletos = u != null ? u.NombresCompletos : "Asesor no Asignado",
                                     DniVendedor = u != null ? u.Dni : " ",
                                 };
            if (supervisorData == null)
            {
                return NotFound("El presente Usuario Supervisor no tiene clientes Asignados");
            }
            // Contar los clientes pendientes (idUsuarioV es null)
            int clientesPendientesSupervisor = supervisorData.Count(cliente => cliente.idUsuarioV == 0);
            // Contar todos los clientes
            int totalClientes = supervisorData.Count();
            // Contar los clientes asignados (idUsuarioV no es null o 0)
            int clientesAsignadosSupervisor = supervisorData.Count(cliente => cliente.idUsuarioV != 0);

            var paginatedData = supervisorData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);
            // Filtrado de las bases
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            ViewData["ClientesPendientesSupervisor"] = clientesPendientesSupervisor;
            ViewData["clientesAsignadosSupervisor"] = clientesAsignadosSupervisor;
            ViewData["totalClientes"] = totalClientes;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalClientes / (double)pageSize);

            return View("Administrador");
        }
        public async Task<IActionResult> Inicio()
        {
            return await Inicio(1, 20);
        }
    }

}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Filters;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class AsignacionController : Controller
    {
        private MDbContext _context;
        private DBServicesConsultasSupervisores _dbServicesConsultasSupervisores;
        public AsignacionController(MDbContext context , DBServicesConsultasSupervisores dbServicesConsultasSupervisores)
        {
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
        }

        [HttpGet]
        public async Task<IActionResult> Asignacion()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            var GetVendedoresAsignados = await _dbServicesConsultasSupervisores.GetAsesorsFromSupervisor(usuarioId);

            if (GetVendedoresAsignados.IsSuccess == false)
            {
                TempData["MessageError"] = GetVendedoresAsignados.Message;
                return RedirectToAction("Index", "Home");
            }
            var vendedoresAsignados = GetVendedoresAsignados.Data;

            // Inicializar la lista de VendedorConClientesDTO
            var vendedoresConClientes = new List<VendedorConClientesDTO>();

            foreach (var vendedorIndividual in GetVendedoresAsignados.Data)
            {
                // Llamada al servicio para obtener el número de clientes y el mapeo de datos
                var vendedorIndividualMapeado = await _dbServicesConsultasSupervisores.GetNumberTipificacionesPlotedOnDTO(vendedorIndividual, usuarioId.Value);

                if (vendedorIndividualMapeado.IsSuccess == false || vendedorIndividualMapeado.Data == null)
                {
                    TempData["MessageError"] = GetVendedoresAsignados.Message;
                    return RedirectToAction("Index", "Home");
                }

                // Agregar el VendedorConClientesDTO mapeado a la lista
                vendedoresConClientes.Add(vendedorIndividualMapeado.Data);
            }

            if (vendedoresConClientes == null)
            {
                TempData["MessageError"] = GetVendedoresAsignados.Message;
                return RedirectToAction("Index", "Home");
            }

            var DestinoBases = await _context.clientes_asignados
                                                            .Where(ca => ca.IdUsuarioS == usuarioId && ca.Destino != null) // Filtrar por usuarioId
                                                            .Select(ca => ca.Destino)                        // Seleccionar solo la columna destino
                                                            .Distinct()                              // Obtener solo valores distintos
                                                            .ToListAsync();                          // Convertir a lista

            if (DestinoBases == null)
            {
                TempData["MessageError"] = "No hay bases de destino disponibles para asignar.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["DestinoBases"] = DestinoBases;
            return View("Asignacion", vendedoresConClientes);
        }

    }
}
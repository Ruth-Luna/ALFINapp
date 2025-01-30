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
        private DBServicesGeneral _dbServicesGeneral;
        public AsignacionController(MDbContext context , DBServicesConsultasSupervisores dbServicesConsultasSupervisores, DBServicesGeneral dbServicesGeneral)
        {
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
            _dbServicesGeneral = dbServicesGeneral;
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

        public async Task<IActionResult> Supervisores()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                    return RedirectToAction("Index", "Home");
                }
    
                var GetUCampanas = await _dbServicesGeneral.GetUCampanas();
                if (GetUCampanas.IsSuccess == false || GetUCampanas.data == null)
                {
                    TempData["MessageError"] = GetUCampanas.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUColor = await _dbServicesGeneral.GetUColor();
                if (GetUColor.IsSuccess == false || GetUColor.data == null)
                {
                    TempData["MessageError"] = GetUColor.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUUsuario = await _dbServicesGeneral.GetUUsuario();
                if (GetUUsuario.IsSuccess == false || GetUUsuario.data == null)
                {
                    TempData["MessageError"] = GetUUsuario.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUTipoBase = await _dbServicesGeneral.GetUTipoBase();
                if (GetUTipoBase.IsSuccess == false || GetUTipoBase.data == null)
                {
                    TempData["MessageError"] = GetUTipoBase.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
    
                var GetDataLabels = new AsignacionSupervisoresDTO
                {
                    UCampanas = GetUCampanas.data,
                    UUsuario = GetUUsuario.data,
                    UTipoBase = GetUTipoBase.data
                };

                /*if (GetSupervisores.IsSuccess == false)
                {
                    TempData["MessageError"] = GetSupervisores.Message;
                    return RedirectToAction("Index", "Home");
                }
    
                var supervisores = GetSupervisores.Data;
    
                if (supervisores == null)
                {
                    TempData["MessageError"] = GetSupervisores.Message;
                    return RedirectToAction("Index", "Home");
                }*/
    
                return View("Supervisores", GetDataLabels);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Inicio", "Administrador");
            }
        }

    }
}
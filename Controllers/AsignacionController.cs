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

                var GetUClienteEstado = await _dbServicesGeneral.GetUClienteEstado();
                if (GetUClienteEstado.IsSuccess == false || GetUClienteEstado.data == null)
                {
                    TempData["MessageError"] = GetUClienteEstado.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUColor = await _dbServicesGeneral.GetUColor();
                if (GetUColor.IsSuccess == false || GetUColor.data == null)
                {
                    TempData["MessageError"] = GetUColor.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUColorFinal = await _dbServicesGeneral.GetUColorFinal();
                if (GetUColorFinal.IsSuccess == false || GetUColorFinal.data == null)
                {
                    TempData["MessageError"] = GetUColorFinal.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUFrescura = await _dbServicesGeneral.GetUFrescura();
                if (GetUFrescura.IsSuccess == false || GetUFrescura.data == null)
                {
                    TempData["MessageError"] = GetUFrescura.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUGrupoMonto = await _dbServicesGeneral.GetUGrupoMonto();
                if (GetUGrupoMonto.IsSuccess == false || GetUGrupoMonto.data == null)
                {
                    TempData["MessageError"] = GetUGrupoMonto.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUGrupoTasa = await _dbServicesGeneral.GetUGrupoTasa();
                if (GetUGrupoTasa.IsSuccess == false || GetUGrupoTasa.data == null)
                {
                    TempData["MessageError"] = GetUGrupoTasa.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUPropension = await _dbServicesGeneral.GetUPropension();
                if (GetUPropension.IsSuccess == false || GetUPropension.data == null)
                {
                    TempData["MessageError"] = GetUPropension.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetURangoEdad = await _dbServicesGeneral.GetURangoEdad();
                if (GetURangoEdad.IsSuccess == false || GetURangoEdad.data == null)
                {
                    TempData["MessageError"] = GetURangoEdad.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetURangoOferta = await _dbServicesGeneral.GetURangoOferta();
                if (GetURangoOferta.IsSuccess == false || GetURangoOferta.data == null)
                {
                    TempData["MessageError"] = GetURangoOferta.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetURangoTasas = await _dbServicesGeneral.GetURangoTasas();
                if (GetURangoTasas.IsSuccess == false || GetURangoTasas.data == null)
                {
                    TempData["MessageError"] = GetURangoTasas.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }

                var GetUTipoCliente = await _dbServicesGeneral.GetUTipoCliente();
                if (GetUTipoCliente.IsSuccess == false || GetUTipoCliente.data == null)
                {
                    TempData["MessageError"] = GetUTipoCliente.Message;
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
                    UClienteEstado = GetUClienteEstado.data,
                    UColor = GetUColor.data,
                    UColorFinal = GetUColorFinal.data,
                    UFrescura = GetUFrescura.data,
                    UGrupoMonto = GetUGrupoMonto.data,
                    UGrupoTasa = GetUGrupoTasa.data,
                    UPropension = GetUPropension.data,
                    URangoEdad = GetURangoEdad.data,
                    URangoOferta = GetURangoOferta.data,
                    URangoTasas = GetURangoTasas.data,
                    UTipoCliente = GetUTipoCliente.data,
                    UUsuario = GetUUsuario.data,
                    UTipoBase = GetUTipoBase.data,
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
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
        private DBServicesConsultasAdministrador _dbServicesConsultasAdministrador;
        private DBServicesAsignacionesAdministrador _dbServicesAsignacionesAdministrador;

        public AsignacionController(MDbContext context,
                        DBServicesConsultasSupervisores dbServicesConsultasSupervisores,
                        DBServicesGeneral dbServicesGeneral,
                        DBServicesConsultasAdministrador dbServicesConsultasAdministrador,
                        DBServicesAsignacionesAdministrador dbServicesAsignacionesAdministrador)
        {
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesConsultasAdministrador = dbServicesConsultasAdministrador;
            _dbServicesAsignacionesAdministrador = dbServicesAsignacionesAdministrador;
        }

        [HttpGet]
        [PermissionAuthorization("Asignacion", "Asignacion")]
        public async Task<IActionResult> Asignacion()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null )
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            var GetVendedoresAsignados = await _dbServicesConsultasSupervisores.GetAsesorsFromSupervisor(usuarioId.Value);

            if (GetVendedoresAsignados.IsSuccess == false || GetVendedoresAsignados.Data == null)
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
            var supervisorData = await _dbServicesConsultasSupervisores.ConsultaLeadsDelSupervisor(usuarioId.Value);

            if (supervisorData.IsSuccess == false)
            {
                TempData["MessageError"] = supervisorData.Message;
                return RedirectToAction("Inicio", "Supervisor");
            }

            int totalClientes = supervisorData.Data != null ? supervisorData.Data.Count() : 0;
            int clientesPendientesSupervisor = supervisorData.Data != null ? supervisorData.Data.Count(cliente => cliente.idUsuarioV == 0) : 0;
            int clientesAsignadosSupervisor = supervisorData.Data != null ? supervisorData.Data.Count(cliente => cliente.idUsuarioV != 0) : 0;

            var NumLeads = new List<int> { totalClientes, clientesPendientesSupervisor, clientesAsignadosSupervisor };
            ViewData["NumLeads"] = NumLeads;
            ViewData["DestinoBases"] = DestinoBases;
            return View("Asignacion", vendedoresConClientes);
        }

        [HttpGet]
        [PermissionAuthorization("Asignacion", "Supervisores")]
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
                var GetUClienteEstado = await _dbServicesGeneral.GetUClienteEstado();
                var GetUColor = await _dbServicesGeneral.GetUColor();
                var GetUColorFinal = await _dbServicesGeneral.GetUColorFinal();
                var GetUFrescura = await _dbServicesGeneral.GetUFrescura();
                var GetUGrupoMonto = await _dbServicesGeneral.GetUGrupoMonto();
                var GetUGrupoTasa = await _dbServicesGeneral.GetUGrupoTasa();
                var GetUPropension = await _dbServicesGeneral.GetUPropension();
                var GetURangoEdad = await _dbServicesGeneral.GetURangoEdad();
                var GetURangoOferta = await _dbServicesGeneral.GetURangoOferta();
                var GetURangoTasas = await _dbServicesGeneral.GetURangoTasas();
                var GetUTipoCliente = await _dbServicesGeneral.GetUTipoCliente();
                var GetUUsuario = await _dbServicesGeneral.GetUUsuario();
                var GetUTipoBase = await _dbServicesGeneral.GetUTipoBase();
                var GetUFlgDeudaPlus = await _dbServicesGeneral.GetUFlgDeudaPlus();

                if (GetUCampanas.IsSuccess == false || GetUCampanas.data == null)
                {
                    TempData["MessageError"] = GetUCampanas.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUClienteEstado.IsSuccess == false || GetUClienteEstado.data == null)
                {
                    TempData["MessageError"] = GetUClienteEstado.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUColor.IsSuccess == false || GetUColor.data == null)
                {
                    TempData["MessageError"] = GetUColor.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUColorFinal.IsSuccess == false || GetUColorFinal.data == null)
                {
                    TempData["MessageError"] = GetUColorFinal.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUFrescura.IsSuccess == false || GetUFrescura.data == null)
                {
                    TempData["MessageError"] = GetUFrescura.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUGrupoMonto.IsSuccess == false || GetUGrupoMonto.data == null)
                {
                    TempData["MessageError"] = GetUGrupoMonto.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUGrupoTasa.IsSuccess == false || GetUGrupoTasa.data == null)
                {
                    TempData["MessageError"] = GetUGrupoTasa.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUPropension.IsSuccess == false || GetUPropension.data == null)
                {
                    TempData["MessageError"] = GetUPropension.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetURangoEdad.IsSuccess == false || GetURangoEdad.data == null)
                {
                    TempData["MessageError"] = GetURangoEdad.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetURangoOferta.IsSuccess == false || GetURangoOferta.data == null)
                {
                    TempData["MessageError"] = GetURangoOferta.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetURangoTasas.IsSuccess == false || GetURangoTasas.data == null)
                {
                    TempData["MessageError"] = GetURangoTasas.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUTipoCliente.IsSuccess == false || GetUTipoCliente.data == null)
                {
                    TempData["MessageError"] = GetUTipoCliente.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUUsuario.IsSuccess == false || GetUUsuario.data == null)
                {
                    TempData["MessageError"] = GetUUsuario.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUTipoBase.IsSuccess == false || GetUTipoBase.data == null)
                {
                    TempData["MessageError"] = GetUTipoBase.Message;
                    return RedirectToAction("Inicio", "Administrador");
                }
                if (GetUFlgDeudaPlus.IsSuccess == false || GetUFlgDeudaPlus.data == null)
                {
                    TempData["MessageError"] = GetUFlgDeudaPlus.Message;
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
                    UFlgDeudaPlus = GetUFlgDeudaPlus.data
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
        [HttpGet]
        public async Task<IActionResult> BuscarAsignacionFiltrarBases(
                                string? base_busqueda = null,
                                string? rango_edad = null,
                                string? rango_tasas = null,
                                decimal? oferta = null,
                                string? tipo_cliente = null,
                                string? cliente_estado = null,
                                string? grupo_tasa = null,
                                string? grupo_monto = null,
                                int? deudas = null,
                                List<string>? campaña = null,
                                List<string>? usuarios = null,
                                List<string>? propension = null,
                                List<string>? color = null,
                                List<string>? color_final = null,
                                List<string>? frescura = null)
        {
            try
            {
                /*if (base_busqueda != null)
                {
                    base_busqueda = "'" + base_busqueda + "'";
                }*/

                var getAsignacionFiltrarBases = await _dbServicesConsultasAdministrador.AsignacionFiltrarBases(
                                                    base_busqueda,
                                                    rango_edad,
                                                    rango_tasas,
                                                    oferta,
                                                    tipo_cliente,
                                                    cliente_estado,
                                                    grupo_tasa,
                                                    grupo_monto,
                                                    deudas,
                                                    campaña,
                                                    usuarios,
                                                    propension,
                                                    color,
                                                    color_final,
                                                    frescura);

                if (getAsignacionFiltrarBases.IsSuccess == false)
                {
                    return Json(new { success = false, message = getAsignacionFiltrarBases.Message });
                }
                return PartialView("TablaAsignacion", getAsignacionFiltrarBases.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertarAsignacionASupervisores(
            [FromBody] InsertarAsignacionRequest request) // Recibe los datos desde el cuerpo de la solicitud
        {
            try
            {
                var ultimoIndiceGuardado = 0;

                // Accede a los datos desde el objeto request
                var idClientes = request.IdClientes;
                var SupervisoresData = request.SupervisoresData;
                var fuenteBase = request.FuenteBase;
                var destino = request.Destino;
                if (idClientes != null && idClientes.Count > 0 && idClientes.Any())
                {
                    var NumAsignaciones = 0;
                    if (SupervisoresData == null)
                    {
                        return Json(new { success = false, message = "La Data de supervisores se encuentra vacia." });
                    }

                    foreach (var dataSupervisor in SupervisoresData)
                    {
                        if (dataSupervisor.numeroClientesAsignados.HasValue)
                        {
                            NumAsignaciones = dataSupervisor.numeroClientesAsignados.Value + NumAsignaciones;
                        }
                    }

                    if (NumAsignaciones > idClientes.Count())
                    {
                        return Json(new { success = false, message = "El numero de clientes enviados para la asignacion es mayor que el numero total de la tabla consultada" });
                    }

                    foreach (var dataSupervisor in SupervisoresData)
                    {
                        if (dataSupervisor.numeroClientesAsignados.HasValue)
                        {
                            for (int i = 0; i < dataSupervisor.numeroClientesAsignados; i++)
                            {
                                var idCliente = idClientes[ultimoIndiceGuardado];
                                var GetInsercion = await _dbServicesAsignacionesAdministrador.InsertarAsignacionASupervisores(idCliente, dataSupervisor.idUsuario, fuenteBase, destino);
                                if (GetInsercion.IsSuccess == false)
                                {
                                    return Json(new { success = false, message = GetInsercion.Message });
                                }
                                ultimoIndiceGuardado++;
                            }
                        }
                        if (ultimoIndiceGuardado >= idClientes.Count)
                        {
                            return Json(new { success = false, message = "CORE DUMPED: THIS IS NOT C++" });
                        }
                    }
                    return Json(new { success = true, message = "Asignacion realizada correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "idClientes es null. Se mando una lista vacia" });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarSupervisoresYDestinos()
        {
            try
            {
                var GetSupervisores = await _dbServicesConsultasAdministrador.ConseguirTodosLosSupervisores();

                if (GetSupervisores.IsSuccess == false)
                {
                    return Json(new { success = false, message = GetSupervisores.Message });
                }

                var GetDestinos = await _context.clientes_asignados
                                                            .Where(ca => ca.Destino != null)
                                                            .Select(ca => ca.Destino)
                                                            .Distinct()
                                                            .ToListAsync();

                if (GetDestinos == null)
                {
                    return Json(new { success = false, message = "No hay destinos disponibles para asignar." });
                }

                ViewData["Destinos"] = GetDestinos;
                ViewData["Supervisores"] = GetSupervisores.Data;
                return PartialView("CamposSupervisoresDestino");
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerBaseDisponibleDelDestino(string destino)
        {
            try
            {
                var idSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisor == null)
                {
                    return Json(new { success = false, message = "No se ha iniciado sesión" });
                }
                var supervisorData = await _dbServicesConsultasSupervisores.ConsultaLeadsDelSupervisorDestino(idSupervisor.Value, destino);
                if (!supervisorData.IsSuccess)
                {
                    return Json(new { success = false, message = supervisorData.Message });
                }
                int clientesPendientesSupervisor = supervisorData.Data != null ? supervisorData.Data.Count(cliente => cliente.IdUsuarioV == null) : 0;
                int totalClientes = supervisorData.Data != null ? supervisorData.Data.Count() : 0;
                int clientesAsignadosSupervisor = supervisorData.Data != null ? supervisorData.Data.Count(cliente => cliente.IdUsuarioV != null) : 0;
                return Json(new { success = true, message = "Busqueda de datos por base con exito", data = new { clientesPendientesSupervisor, totalClientes, clientesAsignadosSupervisor } });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
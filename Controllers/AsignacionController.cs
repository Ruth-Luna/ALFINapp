using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Asignacion;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsignacionController : Controller
    {
        private MDbContext _context;
        private IUseCaseGetAsignacion _useCaseGetAsignacion;
        // private DBServicesConsultasSupervisores _dbServicesConsultasSupervisores; // Comentado porque está relacionado con supervisores
        private DBServicesGeneral _dbServicesGeneral;
        private DBServicesConsultasAdministrador _dbServicesConsultasAdministrador;
        private DBServicesAsignacionesAdministrador _dbServicesAsignacionesAdministrador;

        public AsignacionController(
            MDbContext context,
            // DBServicesConsultasSupervisores dbServicesConsultasSupervisores, // Comentado
            DBServicesGeneral dbServicesGeneral,
            DBServicesConsultasAdministrador dbServicesConsultasAdministrador,
            DBServicesAsignacionesAdministrador dbServicesAsignacionesAdministrador,
            IUseCaseGetAsignacion useCaseGetAsignacion)
        {
            _context = context;
            // _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores; // Comentado
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesConsultasAdministrador = dbServicesConsultasAdministrador;
            _dbServicesAsignacionesAdministrador = dbServicesAsignacionesAdministrador;
            _useCaseGetAsignacion = useCaseGetAsignacion;
        }

        [HttpGet]
        [PermissionAuthorization("Asignacion", "Asignacion")]
        public async Task<IActionResult> Asignacion()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (usuarioId == null || rol == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }

            var executeUseCase = await _useCaseGetAsignacion.Execute(usuarioId.Value);
            if (executeUseCase.IsSuccess == false)
            {
                TempData["MessageError"] = executeUseCase.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var NumLeads = new List<int> { executeUseCase.Data.TotalClientes, executeUseCase.Data.TotalClientesPendientes, executeUseCase.Data.TotalClientesAsignados };
            ViewData["NumLeads"] = NumLeads;
            ViewData["DestinoBases"] = executeUseCase.Data.Destinos;
            return View("Asignacion", executeUseCase.Data);
        }

        /*[HttpGet]
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
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUClienteEstado.IsSuccess == false || GetUClienteEstado.data == null)
                {
                    TempData["MessageError"] = GetUClienteEstado.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUColor.IsSuccess == false || GetUColor.data == null)
                {
                    TempData["MessageError"] = GetUColor.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUColorFinal.IsSuccess == false || GetUColorFinal.data == null)
                {
                    TempData["MessageError"] = GetUColorFinal.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUFrescura.IsSuccess == false || GetUFrescura.data == null)
                {
                    TempData["MessageError"] = GetUFrescura.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUGrupoMonto.IsSuccess == false || GetUGrupoMonto.data == null)
                {
                    TempData["MessageError"] = GetUGrupoMonto.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUGrupoTasa.IsSuccess == false || GetUGrupoTasa.data == null)
                {
                    TempData["MessageError"] = GetUGrupoTasa.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUPropension.IsSuccess == false || GetUPropension.data == null)
                {
                    TempData["MessageError"] = GetUPropension.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetURangoEdad.IsSuccess == false || GetURangoEdad.data == null)
                {
                    TempData["MessageError"] = GetURangoEdad.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetURangoOferta.IsSuccess == false || GetURangoOferta.data == null)
                {
                    TempData["MessageError"] = GetURangoOferta.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetURangoTasas.IsSuccess == false || GetURangoTasas.data == null)
                {
                    TempData["MessageError"] = GetURangoTasas.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUTipoCliente.IsSuccess == false || GetUTipoCliente.data == null)
                {
                    TempData["MessageError"] = GetUTipoCliente.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUUsuario.IsSuccess == false || GetUUsuario.data == null)
                {
                    TempData["MessageError"] = GetUUsuario.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUTipoBase.IsSuccess == false || GetUTipoBase.data == null)
                {
                    TempData["MessageError"] = GetUTipoBase.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (GetUFlgDeudaPlus.IsSuccess == false || GetUFlgDeudaPlus.data == null)
                {
                    TempData["MessageError"] = GetUFlgDeudaPlus.Message;
                    return RedirectToAction("Redireccionar", "Error");
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

                return View("Supervisores", GetDataLabels);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
        }*/

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

        /*[HttpPost]
        public async Task<IActionResult> InsertarAsignacionASupervisores(
            [FromBody] InsertarAsignacionRequest request)
        {
            try
            {
                var ultimoIndiceGuardado = 0;

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
        }*/

        /*[HttpGet]
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
        }*/

        /*[HttpGet]
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
        }*/

        [HttpGet]
        [PermissionAuthorization("Asignacion", "Supervisores")]
        public IActionResult Supervisores()
        {
            // Este retornará la vista ~/Views/Asignacion/Supervisores.cshtml
            return View("Supervisores");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Infrastructure.Services;
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.API.DTOs;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class AsignacionController : Controller
    {
        private MDbContext _context;
        private IUseCaseGetAsignacion _useCaseGetAsignacion;
        private IUseCaseAsignarClientes _useCaseAsignarClientes;
        private DBServicesConsultasSupervisores _dbServicesConsultasSupervisores; // Comentado porque está relacionado con supervisores
        private DBServicesGeneral _dbServicesGeneral;
        private DBServicesConsultasAdministrador _dbServicesConsultasAdministrador;
        private DBServicesAsignacionesAdministrador _dbServicesAsignacionesAdministrador;

        public AsignacionController(
            MDbContext context,
            DBServicesConsultasSupervisores dbServicesConsultasSupervisores, // Comentado
            DBServicesGeneral dbServicesGeneral,
            DBServicesConsultasAdministrador dbServicesConsultasAdministrador,
            DBServicesAsignacionesAdministrador dbServicesAsignacionesAdministrador,
            IUseCaseAsignarClientes useCaseAsignarClientes,
            IUseCaseGetAsignacion useCaseGetAsignacion)
        {
            _context = context;
            _dbServicesConsultasSupervisores = dbServicesConsultasSupervisores; // Comentado
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesConsultasAdministrador = dbServicesConsultasAdministrador;
            _dbServicesAsignacionesAdministrador = dbServicesAsignacionesAdministrador;
            _useCaseGetAsignacion = useCaseGetAsignacion;
            _useCaseAsignarClientes = useCaseAsignarClientes;
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

            return View("Asignacion", executeUseCase.Data);
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

        [HttpGet]
        public async Task<IActionResult> ObtenerBaseDestino(string filtro)
        {
            try
            {
                var idSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisor == null)
                {
                    return Json(new { success = false, message = "No se ha iniciado sesión" });
                }
                var supervisorData = await _dbServicesConsultasSupervisores.ConsultaLeadsDelSupervisorDestino(idSupervisor.Value, filtro);
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

        [HttpGet]
        [PermissionAuthorization("Asignacion", "Supervisores")]
        public IActionResult Supervisores()
        {
            // Este retornará la vista ~/Views/Asignacion/Supervisores.cshtml
            return View("Supervisores");
        }

        /// <summary>
        /// Activates an advisor based on the provided DNI and user ID.
        /// </summary>
        /// <param name="DNI">The DNI of the advisor to be activated.</param>
        /// <param name="idUsuario">The ID of the user performing the activation.</param>
        /// <returns>A JSON result indicating the success or failure of the activation process.</returns>
        /// <remarks>
        /// This method checks if the user is authenticated and verifies the provided DNI and user ID.
        /// If the advisor is already active, it returns a message indicating so.
        /// Otherwise, it attempts to activate the advisor and returns the result.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// Se capturan todas las excepciones y se devuelven como un mensaje de error en el JSON de respuesta.
        /// </exception>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var asignaciones = new List<AsignarAsesorDTO> {
        ///     new AsignarAsesorDTO { IdVendedor = 1, NumClientes = 5 },
        ///     new AsignarAsesorDTO { IdVendedor = 2, NumClientes = 3 }
        /// };
        /// var resultado = await AsignarClientesAAsesores(asignaciones, "BaseClientes2024");
        /// </code>
        /// </example>
        [HttpPost]
        public async Task<IActionResult> AsignarClientesAAsesores(List<DtoVAsignarClientes> asignacionasesor, string filter, string type_filter = "lista")
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisorActual == null)
                {
                    return Json(new { success = false, message = "No se pudo obtener el ID del supervisor actual recuerde Iniciar Sesion." });
                }
                var execute = await _useCaseAsignarClientes.exec(asignacionasesor, filter, type_filter, idSupervisorActual.Value);
                if (!execute.success)
                {
                    return Json(new { success = false, message = $"{execute.message}" });
                }
                else
                {
                    return Json(new { success = true, message = $"{execute.message}" });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = $"Ha ocurrido un error inesperado al modificar las asignaciones. {ex.Message}" });
            }
        }

    }
}
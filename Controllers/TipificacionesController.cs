using Microsoft.AspNetCore.Mvc;
using ALFINapp.Infrastructure.Services;
using ALFINapp.API.Filters;
using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Tipificacion;
using ALFINapp.Application.Interfaces.Derivacion; // Replace with the correct namespace where DBServicesGeneral is defined

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class TipificacionesController : Controller
    {

        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesTipificaciones _dbServicesTipificaciones;
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;
        private readonly MDbContext _context;
        private readonly IUseCaseUploadTipificaciones _useCaseUploadTipificaciones;
        private readonly IUseCaseUploadDerivacion _useCaseUploadDerivacion;
        public TipificacionesController(DBServicesGeneral dbServicesGeneral,
            DBServicesTipificaciones dbServicesTipificaciones,
            DBServicesDerivacion dBServicesDerivacion,
            MDbContext context,
            IUseCaseUploadTipificaciones useCaseUploadTipificaciones,
            IUseCaseUploadDerivacion useCaseUploadDerivacion,
            DBServicesConsultasClientes dbServicesConsultasClientes)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dBServicesDerivacion = dBServicesDerivacion;
            _context = context;
            _useCaseUploadTipificaciones = useCaseUploadTipificaciones;
            _useCaseUploadDerivacion = useCaseUploadDerivacion;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
        }

        [HttpPost]
        public async Task<IActionResult> GenerarDerivacion(
            string agenciaComercial,
            DateTime FechaVisita,
            string Telefono,
            int idBase,
            int type,
            int idAsignacion,
            string? NombresCompletos)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return Json(new { success = false, message = "No se consiguio el id de la sesion, inicie sesion nuevamente." });
            }
            if (agenciaComercial == null || FechaVisita == DateTime.MinValue || Telefono == null)
            {
                return Json(new { success = false, message = "Debe llenar todos los campos" });
            }

            var executeUseCase = await _useCaseUploadDerivacion.Execute(
                agenciaComercial,
                FechaVisita,
                Telefono,
                idBase,
                usuarioId.Value,
                idAsignacion,
                type,
                NombresCompletos);
            if (!executeUseCase.success)
            {
                return Json(new { success = false, message = executeUseCase.message });
            }
            return Json(new { success = true, message = executeUseCase.message });
        }

        [HttpPost]
        public async Task<IActionResult> TipificarMotivo(List<DtoVTipificarCliente> tipificaciones, int IdAsignacion)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            var getUseCase = await _useCaseUploadTipificaciones.execute(usuarioId.Value, tipificaciones, IdAsignacion, 2);
            if (!getUseCase.success)
            {
                TempData["MessageError"] = getUseCase.message;
                return RedirectToAction("Redireccionar", "Error");
            }
            TempData["Message"] = getUseCase.message;
            return RedirectToAction("Redireccionar", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> ViewGeneralTipificacion(int id_base, string traido_de = "A365")
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }
                return Json(new { success = true, data = "En progreso..." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TipificarClienteView(int id_base)
        {
            try
            {
                var IdUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (IdUsuario == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }

                var detallesClientes = await _dbServicesConsultasClientes.GetDataParaTipificarClienteA365(id_base, IdUsuario.Value);
                if (!detallesClientes.IsSuccess || detallesClientes.Data == null)
                {
                    return Json(new { success = false, message = detallesClientes.message });
                }
                var tipificaciones = await _dbServicesTipificaciones.ObtenerTipificaciones();
                if (!tipificaciones.IsSuccess || tipificaciones.Data == null)
                {
                    return Json(new { success = false, message = tipificaciones.Message });
                }
                var telefonosManuales = await _dbServicesConsultasClientes.GetTelefonosTraidosManualmente(detallesClientes.Data.IdCliente != null ? detallesClientes.Data.IdCliente.Value : 0);
                if (!telefonosManuales.IsSuccess)
                {
                    return Json(new { success = false, message = telefonosManuales.message });
                }
                var getAgenciasDisponibles = await _dbServicesGeneral.GetUAgenciasConNumeros();
                if (getAgenciasDisponibles.IsSuccess == false || getAgenciasDisponibles.data == null)
                {
                    return Json(new { success = false, message = getAgenciasDisponibles.Message });
                }

                ViewData["Tipificaciones"] = tipificaciones.Data;
                ViewData["AgenciasDisponibles"] = getAgenciasDisponibles.data;
                ViewData["numerosCreadosPorElUsuario"] = telefonosManuales.Data != null ? telefonosManuales.Data : null;
                ViewData["DNIcliente"] = detallesClientes.Data.Dni;
                ViewData["ID_asignacion"] = detallesClientes.Data.IdAsignacion ?? 0;
                ViewData["Fuente_BD"] = detallesClientes.Data.FuenteBase ?? "Fuente no disponible";
                ViewData["ID_cliente"] = detallesClientes.Data.IdCliente ?? 0;
                return PartialView("_Tipificarcliente", detallesClientes.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TipificarClienteDBALFINView(int id_base)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }

                var detallesClientes = await _dbServicesConsultasClientes.GetDataParaTipificarClienteAlfin(id_base, usuarioId.Value);
                if (!detallesClientes.IsSuccess || detallesClientes.Data == null)
                {
                    return Json(new { success = false, message = detallesClientes.message });
                }

                var tipificaciones = await _dbServicesTipificaciones.ObtenerTipificaciones();
                if (!tipificaciones.IsSuccess || tipificaciones.Data == null)
                {
                    return Json(new { success = false, message = tipificaciones.Message });
                }
                var telefonosManuales = await _dbServicesConsultasClientes.GetTelefonosTraidosManualmente(detallesClientes.Data.IdCliente != null ? detallesClientes.Data.IdCliente.Value : 0);
                if (!telefonosManuales.IsSuccess)
                {
                    return Json(new { success = false, message = telefonosManuales.message });
                }

                var getAgenciasDisponibles = await _dbServicesGeneral.GetUAgenciasConNumeros();
                if (getAgenciasDisponibles.IsSuccess == false || getAgenciasDisponibles.data == null)
                {
                    return Json(new { success = false, message = getAgenciasDisponibles.Message });
                }

                ViewData["Tipificaciones"] = tipificaciones.Data;
                ViewData["AgenciasDisponibles"] = getAgenciasDisponibles.data;
                ViewData["numerosCreadosPorElUsuario"] = telefonosManuales.Data != null ? telefonosManuales.Data : null;
                ViewData["DNIcliente"] = detallesClientes.Data.Dni;
                ViewData["ID_asignacion"] = detallesClientes.Data.IdAsignacion ?? 0;
                ViewData["Fuente_BD"] = "BASE_ASESORES";
                ViewData["ID_cliente"] = detallesClientes.Data.IdCliente ?? 0;

                return PartialView("_Tipificarcliente", detallesClientes.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
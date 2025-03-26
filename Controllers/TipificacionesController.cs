using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ALFINapp.Infrastructure.Persistence.Models;
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
        private readonly DBServicesConsultasAsesores _dbServicesAsesores;
        private readonly MDbContext _context;
        private readonly IUseCaseUploadTipificaciones _useCaseUploadTipificaciones;
        private readonly IUseCaseUploadDerivacion _useCaseUploadDerivacion;
        public TipificacionesController(DBServicesGeneral dbServicesGeneral,
            DBServicesTipificaciones dbServicesTipificaciones,
            DBServicesDerivacion dBServicesDerivacion,
            MDbContext context,
            DBServicesConsultasAsesores dbServicesAsesores,
            IUseCaseUploadTipificaciones useCaseUploadTipificaciones,
            IUseCaseUploadDerivacion useCaseUploadDerivacion)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dBServicesDerivacion = dBServicesDerivacion;
            _context = context;
            _dbServicesAsesores = dbServicesAsesores;
            _useCaseUploadTipificaciones = useCaseUploadTipificaciones;
            _useCaseUploadDerivacion = useCaseUploadDerivacion;
        }

        [HttpPost]
        public async Task<IActionResult> GenerarDerivacion(
            string agenciaComercial, 
            DateTime FechaVisita, 
            string Telefono, 
            int idBase,
            int type,
            int idAsignacion)
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
                type);
            if (!executeUseCase.success)
            {
                return Json(new { success = false, message = executeUseCase.message });
            }
            return Json(new { success = true, message = executeUseCase.message});
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
    }
}
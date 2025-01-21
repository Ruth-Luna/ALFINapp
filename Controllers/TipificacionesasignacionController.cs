using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class TipificacionesasignacionController : Controller
    {
        private readonly MDbContext _context;
        public TipificacionesasignacionController(MDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult ModificarAsignacionPorTipificacionView()
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");

                var tipificacionesGenerales = (from t in _context.tipificaciones
                                               select t).ToList();

                Console.WriteLine("Retornando la vista parcial");
                ViewData["tipificacionesGenerales"] = tipificacionesGenerales;
                return PartialView("_ModificarPorTipificacion");
            }
            catch (System.Exception)
            {
                return Json(new { error = true, message = "Ha ocurrido un Error al mandar los Datos" });
                throw;
            }
        }
    }
}
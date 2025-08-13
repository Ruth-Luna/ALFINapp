using ALFINapp.API.Filters;
using ALFINapp.Datos.DAO.Gestion;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class LeadsController : Controller
    {
        private readonly ILogger<LeadsController> _logger;
        private readonly DAO_GestionDeLeads _dao_gestionDeLeads;
        public LeadsController(
            ILogger<LeadsController> logger,
            DAO_GestionDeLeads dao_gestionDeLeads)
        {
            _logger = logger;
            _dao_gestionDeLeads = dao_gestionDeLeads;
        }
        [HttpGet]
        public async Task<IActionResult> Gestion(
            int paginaInicio = 0,
            int paginaFinal = 1,
            string filter = "",
            string searchfield = "",
            string order = "tipificacion",
            bool orderAsc = true)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            int? rol = HttpContext.Session.GetInt32("RolUser") ?? 0;
            if (rol == 0)
            {
                TempData["MessageError"] = "No se ha encontrado el rol del usuario";
                return RedirectToAction("Index", "Home");
            }
            if (paginaInicio < 0)
            {
                paginaInicio = 0;
            }
            if (paginaFinal < 1)
            {
                paginaFinal = 1;
            }
            if (paginaFinal < paginaInicio)
            {
                TempData["MessageError"] = "El rango de páginas no es válido";
                return RedirectToAction("Redireccionar", "Error");
            }

            if (rol.Value == 3)
            {
                var executeInicio = await _dao_gestionDeLeads
                    .GetLeadsAsignados(
                        usuarioId: usuarioId.Value,
                        intervaloInicio: paginaInicio,
                        intervaloFin: paginaFinal,
                        filter: filter,
                        search: searchfield,
                        order: order,
                        orderAsc: orderAsc);
                if (!executeInicio.IsSuccess || executeInicio.Data == null)
                {
                    TempData["MessageError"] = executeInicio.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }

                var dataInicio = executeInicio.Data;
                dataInicio.PaginaActual = paginaInicio;
                return View("Gestion", dataInicio);
            }
            else if (rol == 2)
            {
                var executeInicio = await _dao_gestionDeLeads
                    .GetLeadsAsignados(
                        usuarioId: usuarioId.Value,
                        paginaInicio,
                        paginaFinal,
                        filter: filter,
                        search: searchfield);
                if (!executeInicio.IsSuccess || executeInicio.Data == null)
                {
                    TempData["MessageError"] = executeInicio.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }

                var dataInicio = executeInicio.Data;
                dataInicio.PaginaActual = paginaInicio;
                return View("GestionS", dataInicio);
            }
            else
            {
                TempData["MessageError"] = "El rol del usuario no es válido";
                return RedirectToAction("Redireccionar", "Error");
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Leads;
using ALFINapp.Application.Interfaces.Vendedor;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    public class LeadsController : Controller
    {
        private readonly ILogger<LeadsController> _logger;
        private readonly IUseCaseGetAsignacionLeads _useCaseGetAsignacionLeads;
        public LeadsController(
            ILogger<LeadsController> logger,
            IUseCaseGetAsignacionLeads useCaseGetAsignacionLeads)
        {
            _logger = logger;
            _useCaseGetAsignacionLeads = useCaseGetAsignacionLeads;
        }
        [HttpGet]
        public async Task<IActionResult> Gestion(int paginaInicio = 0, int paginaFinal = 1)
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

            var executeInicio = await _useCaseGetAsignacionLeads.Execute(usuarioId.Value, rol.Value, paginaInicio, paginaFinal);
            if (!executeInicio.IsSuccess || executeInicio.Data == null)
            {
                TempData["MessageError"] = executeInicio.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            if (rol.Value == 3)
            {
                var dataInicio = executeInicio.Data;
                ViewData["TotalClientes"] = dataInicio.clientesTotal;
                ViewData["ClientesPendientes"] = dataInicio.clientesPendientes;
                ViewData["ClientesTipificados"] = dataInicio.clientesTipificados;
                ViewData["UsuarioNombre"] = dataInicio.Vendedor != null ? dataInicio.Vendedor.NombresCompletos : "Usuario No Encontrado";
                ViewData["ClientesTraidosDBALFIN"] = dataInicio.ClientesAlfin;
                ViewData["PaginaActual"] = paginaInicio;
                return View("Gestion", dataInicio.ClientesA365);
            }
            else if (rol == 2)
            {
                var dataInicio = executeInicio.Data;
                return View("GestionS", dataInicio);
            }
            else
            {
                TempData["MessageError"] = "El rol del usuario no es válido";
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GestionPag(int paginaInicio = 0, int paginaFinal = 1)
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

            var executeInicio = await _useCaseGetAsignacionLeads.Execute(usuarioId.Value, rol.Value, paginaInicio, paginaFinal);
            if (!executeInicio.IsSuccess || executeInicio.Data == null)
            {
                TempData["MessageError"] = executeInicio.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            if (rol.Value == 3)
            {
                var dataInicio = executeInicio.Data;
                ViewData["TotalClientes"] = dataInicio.clientesTotal;
                ViewData["ClientesPendientes"] = dataInicio.clientesPendientes;
                ViewData["ClientesTipificados"] = dataInicio.clientesTipificados;
                ViewData["UsuarioNombre"] = dataInicio.Vendedor != null ? dataInicio.Vendedor.NombresCompletos : "Usuario No Encontrado";
                ViewData["ClientesTraidosDBALFIN"] = dataInicio.ClientesAlfin;
                return Json (new
                {
                    data = dataInicio.ClientesA365
                });
            }
            else if (rol == 2)
            {
                var dataInicio = executeInicio.Data;
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
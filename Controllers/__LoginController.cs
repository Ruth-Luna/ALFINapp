using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ALFINapp.Datos;

namespace ALFINapp.API.Controllers;

public class __LoginController : Controller
{
    DA_Usuario _daUsuario = new DA_Usuario();

    private readonly ILogger<__LoginController> _logger;
    private readonly MDbContext _context;
    private readonly DBServicesUsuarios _dBServicesUsuarios;
    private readonly DBServicesGeneral _dBServicesGeneral;

    public __LoginController(ILogger<__LoginController> logger,
        MDbContext context,
        DBServicesUsuarios dBServicesUsuarios,
        DBServicesGeneral dBServicesGeneral)
    {
        _logger = logger;
        _context = context;
        _dBServicesUsuarios = dBServicesUsuarios;
        _dBServicesGeneral = dBServicesGeneral;
    }

    public IActionResult Index()
    {
        return View("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Error404()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string usuario, string password)
    {
        var usuarioValido = _daUsuario.ValidarUsuario(usuario, password);

        if (usuarioValido == null || usuarioValido.Resultado == 0)
        {
            ViewData["mensaje"] = "Credenciales incorrectas";
            return RedirectToAction("Index", "Home");
        }

        if (usuarioValido.Resultado == -1 || usuarioValido.Estado?.ToUpper() == "INACTIVO")
        {
            ViewData["mensaje"] = "El usuario se encuentra inactivo. Por favor contacte con el administrador.";
            return RedirectToAction("Index", "Home");
        }

        HttpContext.Session.SetInt32("UsuarioId", usuarioValido.IdUsuario);
        HttpContext.Session.SetInt32("RolUser", usuarioValido.IdRol.Value);
        HttpContext.Session.SetInt32("ActivarCambio", 1);
        HttpContext.Session.SetInt32("UsuarioId", usuario != null ? usuarioValido.IdUsuario : throw new Exception("El usuario original no está definido. Comuníquese con su Supervisor."));
        HttpContext.Session.SetInt32("RolUser", usuario != null ? usuarioValido.IdRol.Value : throw new Exception("El rol del usuario original no está definido. Comuníquese con su Supervisor."));
        switch (usuarioValido.IdRol)
        {
            case 1:
                return RedirectToAction("Inicio", "Administrador");
            case 2:
                return RedirectToAction("Inicio", "Supervisor");
            case 3:
                return RedirectToAction("Inicio", "Vendedor");
            case 4:
                return RedirectToAction("Inicio", "GerenteZonal");
            default:
                ViewData["mensaje"] = "Rol no reconocido. Contacte al administrador.";
                return RedirectToAction("Index", "Home");
        }
    }
}

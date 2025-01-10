using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Models;

namespace ALFINapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MDbContext _context;


    public HomeController(ILogger<HomeController> logger, MDbContext context)
    {
        _logger = logger;
        _context = context;
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

    public IActionResult Addingclient()
    {
        return View("Addingclient");
    }

    public IActionResult Error404()
    {
        return View();
    }

    // Acción para verificar si el DNI existe
    [HttpPost]
    public IActionResult VerificarUsuario(string dni)
    {
        if (!dni.All(char.IsDigit))
        {
            TempData["MessageError"] = "Ingrese Solo Numeros en el DNI";
            return RedirectToAction("Index", "Home");
        }
        dni = dni.Trim();
        var usuario = _context.usuarios.FirstOrDefault(u => u.Dni.Trim() == dni);

        if (usuario == null)
        {
            TempData["MessageError"] = "El Usuario a Buscar no se encuentra Registrado comunicarse con su Supervisor.";
            return RedirectToAction("Index", "Home");
        }

        if (usuario.Estado == "INACTIVO")
        {
            TempData["MessageError"] = "El Usuario se fue marcado como Inactivo, Comunicarse con su Supervisor.";
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrEmpty(usuario.Rol))
        {
            TempData["MessageError"] = "El rol del usuario no está definido. Comuníquese con su Supervisor.";
            return RedirectToAction("Index", "Home");
        }
        if (usuario.Rol == "VENDEDOR")
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            return RedirectToAction("Ventas", "User");
        }
        if (usuario.Rol == "SUPERVISOR")
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            return RedirectToAction("VistaMainSupervisor", "Supervisor");
        }
        if (usuario.Rol == "ADMINISTRADOR")
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            return RedirectToAction("VistaMainSupervisor", "Supervisor");
        }

        TempData["MessageError"] = "Algo salio Mal en la Autenticacion";
        return RedirectToAction("Index", "Home");
    }

}

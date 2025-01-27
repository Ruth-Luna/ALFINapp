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

    public IActionResult Error404()
    {
        return View();
    }

    // Acción para verificar si el DNI existe
    [HttpPost]
    public IActionResult VerificarUsuario(string dni, string password)
    {
        if (string.IsNullOrWhiteSpace(dni) ||!long.TryParse(dni, out _))
        {
            TempData["MessageError"] = "El DNI no puede estar vacio.";
            return RedirectToAction("Index", "Home");
        }
        if (!dni.All(char.IsDigit))
        {
            TempData["MessageError"] = "Ingrese Solo Numeros en el DNI";
            return RedirectToAction("Index", "Home");
        }

        if (password == null)
        {
            TempData["MessageError"] = "El campo de contraseña no puede estar vacio.";
            return RedirectToAction("Index", "Home");
        }
        dni = dni.Trim();
        var usuario = _context.usuarios.FirstOrDefault(u => u.Dni.Trim() == dni);
        if (usuario == null)
        {
            TempData["MessageError"] = "El Usuario a Buscar no se encuentra Registrado comunicarse con su Supervisor.";
            return RedirectToAction("Index", "Home");
        }

        if (usuario.contraseña == null)
        {
            TempData["MessageError"] = "El usuario tuvo una eliminacion manual de su contrase;a, comunicarse con servicio tecnico";
            return RedirectToAction("Index", "Home");
        }

        var passwordUsuario = _context.usuarios.FirstOrDefault(u => u.contraseña == password && u.Dni == dni);

        if (passwordUsuario == null)
        {
            TempData["MessageError"] = "La contraseña que ha ingresado es incorrecta. La contraseña por defecto es su DNI luego agreguele la cadena $clave123";
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
        
        HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
        if (usuario.Rol == "VENDEDOR")
        {
            HttpContext.Session.SetInt32("RolUser", 1);
            return RedirectToAction("Inicio", "Vendedor");
        }
        if (usuario.Rol == "SUPERVISOR")
        {
            HttpContext.Session.SetInt32("RolUser", 2);
            return RedirectToAction("Inicio", "Supervisor");
        }
        if (usuario.Rol == "ADMINISTRADOR")
        {
            HttpContext.Session.SetInt32("RolUser", 3);
            return RedirectToAction("VistaMainAdministrador", "Administrador");
        }

        TempData["MessageError"] = "Algo salio Mal en la Autenticacion";
        return RedirectToAction("Index", "Home");
    }

}

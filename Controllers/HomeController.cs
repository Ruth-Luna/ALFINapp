using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Models;
using ALFINapp.Services;
using System.Threading.Tasks;

namespace ALFINapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MDbContext _context;
    private readonly DBServicesUsuarios _dBServicesUsuarios;
    private readonly DBServicesGeneral _dBServicesGeneral;

    public HomeController(ILogger<HomeController> logger,
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
    // Acción para verificar si el DNI existe
    [HttpPost]
    public async Task<IActionResult> VerificarUsuario(string dni, string password)
    {
        if (string.IsNullOrWhiteSpace(dni) || !long.TryParse(dni, out _))
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
        var usuario = _context.usuarios.FirstOrDefault(u => u.Dni == dni);
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

        if (usuario.IdRol == null)
        {
            TempData["MessageError"] = "El rol del usuario no está definido. Comuníquese con su Supervisor.";
            return RedirectToAction("Index", "Home");
        }
        var usuarioOculto = await _dBServicesUsuarios.GetUsuarioOculto(dni);
        if (!usuarioOculto.IsSuccess)
        {
            TempData["MessageError"] = usuarioOculto.Message;
            return RedirectToAction("Index", "Home");
        }
        if (usuarioOculto.Data != null)
        {
            HttpContext.Session.SetInt32("ActivarCambio", 1);
            HttpContext.Session.SetInt32("UsuarioId", usuario != null ? usuario.IdUsuario : throw new Exception("El usuario original no está definido. Comuníquese con su Supervisor."));
            HttpContext.Session.SetInt32("RolUser", usuario != null ? usuario.IdRol.Value : throw new Exception("El rol del usuario original no está definido. Comuníquese con su Supervisor."));
            TempData["ActivarCambio"] = 1;
            TempData["DniCambio"] = usuarioOculto.Data.DniAlBanco + " - " + usuarioOculto.Data.NombreCambio;
            return RedirectToAction("Inicio", "Vendedor");
        }
        HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
        if (usuario.IdRol == 3)
        {
            HttpContext.Session.SetInt32("RolUser", usuario.IdRol.Value);
            return RedirectToAction("Inicio", "Vendedor");
        }
        if (usuario.IdRol == 2)
        {
            HttpContext.Session.SetInt32("RolUser", usuario.IdRol.Value);
            return RedirectToAction("Inicio", "Supervisor");
        }
        if (usuario.IdRol == 1)
        {
            HttpContext.Session.SetInt32("RolUser", usuario.IdRol.Value);
            return RedirectToAction("Inicio", "Administrador");
        }
        if (usuario.IdRol == 4)
        {
            HttpContext.Session.SetInt32("RolUser", usuario.IdRol.Value);
            return RedirectToAction("Inicio", "GerenteZonal");
        }

        TempData["MessageError"] = "Algo salio Mal en la Autenticacion";
        return RedirectToAction("Index", "Home");
    }
}

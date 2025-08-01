using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ALFINapp.Datos;
using ALFINapp.API.Models;
using System.Net.Mail;
using System.Net;
using ALFINapp.Datos.DAO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ALFINapp.API.Controllers;

public class HomeController : Controller
{
    DA_Login _daLogin = new DA_Login();

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

        if (usuario.Correo == null)
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            HttpContext.Session.SetInt32("RolUser", usuario.IdRol.Value);
            return RedirectToAction("Email", "Email");
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


    [HttpPost]
    public async Task<IActionResult> Login(string usuario, string password)
    {
        var usuarioValido = _daLogin.ValidarUsuario(usuario, password);

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

    [HttpPost]
    public JsonResult ValidarCorreoYUsuario(string usuario, string correo)
    {
        try
        {
            var usuarioValido = _daLogin.ValidarCorreo_Usuario(usuario, correo);

            if (usuarioValido.Resultado == 0)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "No se ha encontrado el usuario ni el correo correspondiente en nuestra Base de Datos. Intentalo nuevamente."
                });
            }

            return Json(new
            {
                success = true,
                mensaje = "Verificación exitosa. Se ha enviado un código de recuperación a tu correo."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                mensaje = "Ha ocurrido un error inesperado al validar tus datos.",
                detalle = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult InsertarSolicitudRecuperacionContrasenia([FromBody] ViewCorreoRecuperacion data)
    {
        try
        {
            var resultado = _daLogin.InsertarSolicitudYObtenerCodigo(data.Correo, data.Usuario);
            Console.WriteLine("id", resultado.IdSolicitud);
            if (resultado != null && !string.IsNullOrEmpty(resultado.Codigo))
            {
                bool correoEnviado = EnviarCorreoRecuperacion(resultado.Correo, resultado.Codigo);
                if (correoEnviado)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = "El código de verificación fue enviado correctamente.",
                        idUsuario = resultado.IdUsuario,
                        idSolicitud = resultado.IdSolicitud
                    });
                }
                else
                {
                    return Json(new { success = false, mensaje = "Error al enviar el correo." });
                }
            }

            return Json(new { success = false, mensaje = "No se pudo generar la solicitud." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, mensaje = "Error del servidor.", detalle = ex.Message });
        }
    }

    [HttpPost]
    public JsonResult VerificarCodigoCorreo(int idUsuario, string codigo)
    {
        int resultado = _daLogin.VerificarCodigoCorreo(idUsuario, codigo);
        Console.WriteLine(resultado);
        switch (resultado)
        {
            case 0:
                return Json(new { success = true, mensaje = "El código es válido." });
            case 1:
                return Json(new { success = false, mensaje = "El código ya ha sido utilizado." });
            case 2:
                return Json(new { success = false, mensaje = "El código ya ha expirado. Por favor solicita uno nuevamente" });
            case 3:
                return Json(new { success = false, mensaje = "El código ingresado es incorrecto." });

            default:
                return Json(new { success = false, mensaje = "Ocurrió un error al verificar el código." });
        }
    }


    [HttpGet]
    public JsonResult ObtenerEstadoCodigoCorreo(int idSolicitud)
    {
        var respuesta = _daLogin.VerificarEstadoCodigo(idSolicitud);
        return Json(respuesta);
    }

    private bool EnviarCorreoRecuperacion(string correoDestino, string codigo)
    {
        try
        {
            string asunto = "Código de recuperación de contraseña";
            string cuerpo = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2 style='color: #2c3e50;'>Recuperación de contraseña</h2>
                <p>Hola,</p>
                <p>Hemos recibido una solicitud para restablecer tu contraseña. Usa el siguiente código de verificación:</p>
                <div style='font-size: 28px; font-weight: bold; letter-spacing: 4px; padding: 12px 20px; background-color: #f0f0f0; width: fit-content; border-radius: 6px; margin: 20px auto; text-align: center;'>
                    {codigo}
                </div>
                <p>Este código tiene una validez de <strong>15 minutos</strong>.</p>
                <p>Si no realizaste esta solicitud, puedes ignorar este mensaje.</p>
                <br>
                <p style='font-size: 12px; color: #888;'>Este es un mensaje automático. Por favor, no respondas a este correo.</p>
            </div>";

            MailMessage mensaje = new MailMessage
            {
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true,
                From = new MailAddress("renzo.tinajeros960@gmail.com", "Nombre de tu sistema")
            };
            mensaje.To.Add(correoDestino);

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential("renzo.tinajeros960@gmail.com", "iehs umzt rqgl upqx");
                smtp.EnableSsl = true;
                smtp.Send(mensaje);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar correo: " + ex.Message);
            return false;
        }
    }
}

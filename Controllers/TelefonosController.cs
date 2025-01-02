using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class TelefonosController : Controller
    {
        private readonly MDbContext _context;

        public TelefonosController(MDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult AgregarTelefonoPorAsesor(string numeroTelefono, int idClienteTelefono)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string telefono = numeroTelefono;
                int idCliente = idClienteTelefono;

                // Validar que el idCliente es un número entero
                // Validar que el teléfono no esté vacío
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    return Json(new { success = false, message = "El número de teléfono no puede estar vacío" });
                }

                // Validar que el teléfono tenga exactamente 9 dígitos y comience con un número entre 1 y 9
                if (!System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^[1-9]\d{8}$"))
                {
                    return Json(new { success = false, message = "El número de teléfono debe tener exactamente 9 dígitos, comenzando con un número entre 1 y 9" });
                }

                // Crear nuevo teléfono
                var NuevoTelefono = new TelefonosAgregados
                {
                    IdCliente = idCliente,
                    Telefono = telefono,
                    AgregadoPor = "VENDEDOR" // Ajusta según sea necesario
                };

                _context.telefonos_agregados.Add(NuevoTelefono);
                _context.SaveChanges();

                return Json(new { success = true, message = "Se Ha agregado con exito el Numero de Telefono a la Base de Datos." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
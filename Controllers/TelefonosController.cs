using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller responsible for managing client phone number operations.
/// </summary>
  
/// <summary>
/// Initializes a new instance of the <see cref="TelefonosController"/> class with the specified database context.
/// </summary>
/// <param name="context">The database context used for data operations.</param>
  
namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class TelefonosController : Controller
    {
        private readonly MDbContext _context;

        public TelefonosController(MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a phone number for a client on behalf of an advisor. This method validates that the provided phone number is non-empty
        /// and exactly 9 digits long (starting with a digit between 1 and 9) before adding it to the database.
        /// </summary>
        /// <param name="numeroTelefono">The phone number to be added. It must be non-empty and match the required format.</param>
        /// <param name="idClienteTelefono">The identifier of the client associated with the phone number.</param>
        /// <returns>
        /// A JSON response indicating whether the operation was successful. If successful, the response contains a success flag and a confirmation message;
        /// if not, it contains an error message explaining the failure.
        /// </returns>
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

                if (string.IsNullOrWhiteSpace(telefono))
                {
                    return Json(new { success = false, message = "El número de teléfono no puede estar vacío" });
                }

                telefono = telefono.Trim().Replace(" ", "")
                                        .Replace("\t", "")
                                        .Replace("\n", "")
                                        .Replace("\r", "");

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